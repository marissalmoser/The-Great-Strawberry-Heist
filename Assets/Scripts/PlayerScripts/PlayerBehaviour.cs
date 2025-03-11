/*****************************************************************************
 * Author: Brad Dixon
 * Creation Date: 1/28/2025
 * File Name: PlayerBehaviour.cs
 * Brief: Controls the movement and actions of the player
 * ***************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBehaviour : MonoBehaviour
{
    private InputActionMap actions;
    private InputAction playerMove, playerJump;

    private BoxCollider2D hitbox;

    [Header("Layer Masks")]
    [SerializeField] private LayerMask ground;
    [SerializeField] private LayerMask slow;

    [Header("Player Movement")]

    [Tooltip("How fast the player's base speed is")]
    [SerializeField] private float playerSpeed;

    [Tooltip("A multipler for slowing the player down")]
    [SerializeField] private float slowMultipler;

    private float speedMultiplier;

    [Tooltip("How far the player jumps into the air")]
    [SerializeField] private float jumpHeight;

    [Tooltip("How long the player is slowed after being hit by icing")]
    [SerializeField] private float fallingIcingSlowTime; 

    //A base value for returning the player's speed to normal
    private const float BASE_MULTIPLER = 1;
    [SerializeField] private Rigidbody2D rb2d;
    private float moveValue;

    private float isGrounded;
    private bool canMove;
    private bool facingLeft;

    private SpriteRenderer sr;
    private Animator animator;

    [Header("Collision with obstacles")]

    [SerializeField] private float _invincibilityFramesInSeconds;
    private float invincibilitySecondsRemaining;
    private bool inKnockback;
    [SerializeField] private float _invincibilityFlashingSpeed;

    [Tooltip("Value between 0 and 255, smaller numbers meaning less opaque at minimum")]
    [SerializeField] private float _invincibilityFlashingMinOpacity;
    [SerializeField] private Vector2 _knockbackVelocity;

    [Tooltip("Prefabs")]
    [SerializeField] private GameObject multiplierChangePrefab;

    /// <summary>
    /// Enables the action map and inputs for the rest of the code
    /// </summary>
    private void Awake()
    {
        actions = GetComponent<PlayerInput>().currentActionMap;
        actions.Enable();

        playerMove = actions.FindAction("Move");
        playerJump = actions.FindAction("Jump");

        playerJump.performed += PlayerJump_performed;

        speedMultiplier = BASE_MULTIPLER;

        hitbox = GetComponent<BoxCollider2D>();

        TierManager.SwipeTierAction += MoveToNextTier;
        canMove = true;

        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Executes when the jump button is pressed
    /// </summary>
    /// <param name="obj"></param>
    private void PlayerJump_performed(InputAction.CallbackContext obj)
    {
        if (canMove)
        {
            PlayerJump();
        }
    }

    /// <summary>
    /// Executes player code at a fixed rate
    /// </summary>
    private void FixedUpdate()
    {
        RotatePlayer();

        animator.SetFloat("yVelocity", rb2d.velocity.y);

        if (canMove)
        {
            MovePlayer();
        }
    }

    /// <summary>
    /// Animation event to trigger a change in the jump animation
    /// </summary>
    public void StopJumpAnim()
    {
        animator.SetBool("Jump", false);
    }

    /// <summary>
    /// Rotates the player to look left or right
    /// </summary>
    private void RotatePlayer()
    {
        if(moveValue > 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            facingLeft = false;
        }
        else if(moveValue < 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
            facingLeft = true;
        }
    }

    /// <summary>
    /// Moves the player left and right
    /// </summary>
    private void MovePlayer()
    {
        moveValue = playerMove.ReadValue<float>();
        moveValue = moveValue * playerSpeed * speedMultiplier;

        animator.SetFloat("Speed", Mathf.Abs(moveValue));

        rb2d.velocity = new Vector2(moveValue, rb2d.velocity.y);
    }

    /// <summary>
    /// Causes the player to jump when the button is pressed
    /// </summary>
    private void PlayerJump()
    {
        if(CanJump())
        {
            SfxManager.Instance.PlaySFX("HamsterJump");
            animator.SetBool("Jump", true);
            rb2d.AddForce(new Vector2(rb2d.velocity.x, jumpHeight), ForceMode2D.Impulse);
        }
    }

    /// <summary>
    /// Checks if the player is grounded to see if they can jump
    /// </summary>
    /// <returns></returns>
    public bool CanJump()
    {
        var hit = Physics2D.BoxCast(hitbox.bounds.center, hitbox.bounds.size * .95f, 0, Vector2.down, 0.1f, ground);
        if (hit.collider != null)
        {
            // Bounds check: hamster is entirely above the surface it is interacting with (rather than within)
            // Velocity check: hamster is not currently jumping (needed because there are some platforms you can
            //                 stand inside of and should be able to jump inside of)
            return (hitbox.bounds.min.y > hit.collider.bounds.max.y - 0.1f) || (Mathf.Abs(rb2d.velocity.y) < 0.01f);
        }
        return false;
    }
    
    public bool PlayerPlatformCheck()
    {
        return rb2d.velocity.y <= 0.01f;
    }

    /// <summary>
    /// Slows the player down when they enter the slow batter
    /// </summary>
    public void SlowPlayer()
    {
        speedMultiplier = slowMultipler;
    }

    /// <summary>
    /// Sets the player's speed back to normal
    /// </summary>
    public void NormalSpeed()
    {
        speedMultiplier = BASE_MULTIPLER;
    }

    /// <summary>
    /// Called when the cat swipes the bottom cake tier. Duration is the pause while
    /// the camera shakes before the tier is swiped.
    /// </summary>
    private void MoveToNextTier(float shakeDuration, float playerMoveDuration)
    {
        if(TierManager.Instance.IsInBottomTier())
        {
            StartCoroutine(MovePlayerToNextTier(shakeDuration, playerMoveDuration));
        }
    }

    /// <summary>
    /// Moves the player to the next tier when they are swiped out of their current one.
    /// </summary>
    private IEnumerator MovePlayerToNextTier(float delay, float playerMoveDuration)
    {
        //wait for camera shake
        yield return new WaitForSeconds(delay);

        //check if player should actually move
        if(!TimerSystem.DoMovePlayer)
        {
            yield break;
        }

        rb2d.velocity = Vector2.zero;

        //TODO: play swipe animation :D

        //Stop player movement and collisions
        canMove = false;
        hitbox.enabled = false;
        rb2d.isKinematic = true;

        //move player
        Vector3 startPos = transform.position;
        Vector3 endPos = TierManager.Instance.GetNextSpawn();
        float t = 0;
        while (t <= 1.0f)
        {
            t += Time.deltaTime / playerMoveDuration; 
            transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return new WaitForFixedUpdate();         
        }
        transform.position = endPos;

        //resume player movement and collisions
        canMove = true;
        hitbox.enabled = true;
        rb2d.isKinematic = false;
    }

    /// <summary>
    /// Disables the map and jump callback function when the script is disabled
    /// </summary>
    private void OnDisable()
    {
        playerJump.performed -= PlayerJump_performed;
        TierManager.SwipeTierAction -= MoveToNextTier;
        actions.Disable();
    }

    public void GotHitByIcing()
    {
        if (invincibilitySecondsRemaining <= 0)
        {
            // Knocks hamster back in opposite of the direction it's facing
            KnockBack(!facingLeft);
        }
        StartCoroutine(FallingIcingCooldown());
    }

    private IEnumerator FallingIcingCooldown()
    {
        SlowPlayer();
        yield return new WaitForSeconds(fallingIcingSlowTime);
        NormalSpeed();
    }

    public void GotHitByOrange(bool direction)
    {
        if (invincibilitySecondsRemaining <= 0)
        {
            // Knocks hamster back in the direction the orange is moving
            KnockBack(direction);
        }
    }

    /// <summary>
    /// Syncs timing of hamster idle with fruit bounce animation
    /// Called on frame 0 of hamster idle
    /// Does not run if no fruits are left in the scene to sync to
    /// </summary>
    public void SyncIdle()
    {
        var fruit = FindObjectOfType<FruitCollect>();
        if (fruit != null)
        {
            Animator reference = fruit.GetComponent<Animator>();
            if (reference.GetCurrentAnimatorStateInfo(0).normalizedTime != animator.GetCurrentAnimatorStateInfo(0).normalizedTime)
            {
                animator.Play("PlayerIdle", 0, reference.GetCurrentAnimatorStateInfo(0).normalizedTime);
                //print("Synced idle from " + animator.GetCurrentAnimatorStateInfo(0).normalizedTime + "to " + reference.GetCurrentAnimatorStateInfo(0).normalizedTime);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Becoming grounded while in knockback re-enables movement
        if (inKnockback && CanJump())
        {
            inKnockback = false;
            canMove = true;
        }
    }

    /// <summary>
    /// Applies knockback velocity to hamster, disables controls, sets I-frames
    /// </summary>
    private void KnockBack(bool knockHamsterLeft)
    {
        // TO DO: play the animation for getting knocked back
        rb2d.velocity = new Vector2(_knockbackVelocity.x * (knockHamsterLeft ? -1 : 1), _knockbackVelocity.y);
        Singleton<ScoreManager>.Instance.PlayerHit();
        inKnockback = true;
        canMove = false;
        StartCoroutine(InvincibilityFrames());
    }

    /// <summary>
    /// Tracks I-frames and visually indicates your invincibility
    /// </summary>
    private IEnumerator InvincibilityFrames()
    {
        bool opacityGoingDown = true;
        invincibilitySecondsRemaining = _invincibilityFramesInSeconds;
        while (invincibilitySecondsRemaining > 0)
        {
            yield return null;
            invincibilitySecondsRemaining -= Time.deltaTime;

            if (opacityGoingDown)
            {
                sr.color = sr.color - new Color(0, 0, 0, _invincibilityFlashingSpeed / 255f * Time.deltaTime);
                if (sr.color.a < _invincibilityFlashingMinOpacity / 255f)
                {
                    opacityGoingDown = false;
                }
            }
            else
            {
                sr.color = sr.color + new Color(0, 0, 0, _invincibilityFlashingSpeed / 255f * Time.deltaTime);
                if (sr.color.a >= 1)
                {
                    opacityGoingDown = true;
                }
            }
        }
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1);
    }
    /// <summary>
    /// Displays text related to multiplier changes through a prefab on the player
    /// </summary>
    /// <param name="newMultiplier"></param>
    public void DisplayMultiplierChange(float newMultiplier) 
    {
        var text = Instantiate(multiplierChangePrefab, transform.position, Quaternion.identity).GetComponent<TextRise>();
        text.SetRisingText(newMultiplier + "x");
    }
}
