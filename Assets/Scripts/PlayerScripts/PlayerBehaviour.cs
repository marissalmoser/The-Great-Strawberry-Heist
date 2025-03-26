/*****************************************************************************
 * Author: Brad Dixon
 * Creation Date: 1/28/2025
 * File Name: PlayerBehaviour.cs
 * Brief: Controls the movement and actions of the player
 * ***************************************************************************/
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    private bool inEnd = false;
    [SerializeField]
    private bool canMove = false;
    private bool facingLeft;
    private bool isSpinning;

    private SpriteRenderer sr;
    private Animator animator;

    [Header("Collision with obstacles")]

    [Tooltip("Timer starts after hamster's dizziness has cleared")]
    [SerializeField] private float _invincibilityFramesInSeconds;
    private float invincibilitySecondsRemaining;
    private bool inKnockback;
    [SerializeField] private float _invincibilityFlashingSpeed;
    [SerializeField] private float _lengthOfDizzy;
    private bool dizzy;

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
        TimerSystem.StartGame += StartGameplay;

        speedMultiplier = BASE_MULTIPLER;

        hitbox = GetComponent<BoxCollider2D>();

        TierManager.SwipeTierAction += MoveToNextTier;
        TierManager.EndSequence += EndAnim;
        //canMove = true;

        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Executes when the jump button is pressed
    /// </summary>
    /// <param name="obj"></param>
    private void PlayerJump_performed(InputAction.CallbackContext obj)
    {
        if (canMove && !inEnd)
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

        animator.SetFloat("Speed", Mathf.Abs(rb2d.velocity.x));

        if (canMove && !inEnd)
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

    private void StartGameplay()
    {
        canMove = true;
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
    /// Plays the appropriate animation sequence based on whether the player 
    /// got swiped at the end
    /// </summary>
    /// <param name="wasSwiped"></param>
    private void EndAnim(bool wasSwiped)
    {
        inEnd = true;

        if(!wasSwiped)
        {
            print("run heree");
            StartCoroutine(RunToStrawberry());
        }
    }

    /// <summary>
    /// Moves the player right until they reach the strawberry
    /// </summary>
    public IEnumerator RunToStrawberry()
    {
        //makes player face right and disable their input
        transform.rotation = Quaternion.Euler(0, 0, 0);
        actions.Disable();
        Invoke("CallStrawberrySound", 0.5f);

        while (inEnd)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            rb2d.velocity = new Vector2(playerSpeed, 0);
            yield return null;
        }
    }

    /// <summary>
    /// Invoked in RunToStrawberry();
    /// </summary>
    void CallStrawberrySound()
    {
        SfxManager.Instance.PlaySFX("StrawberryPickup");
    }

    /// <summary>
    /// Returns the player to the main menu
    /// </summary>
    public void ReturnToMenu()
    {
        //TODO: go through high score secquence first
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
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

        //Stop player movement and collisions
        canMove = false;

        //play swipe animation
        //TODO: Hamster Sound Here
        animator.SetTrigger("Swipe");

        //wait for anim look around
        yield return new WaitForSeconds(1f);
        SfxManager.Instance.PlaySFX("CatSwipe");

        hitbox.enabled = false;
        rb2d.isKinematic = true;

        //move player
        Vector3 startPos = transform.position;
        Vector3 endPos = TierManager.Instance.GetNextSpawn();
        float t = 0;
        while (t <= 1.0f)
        {
            if (inEnd && !isSpinning)
            {
                //transition to dizzy swipe animation
                isSpinning = true;
                StartCoroutine(RotateForSeconds(playerMoveDuration - t, 400));
            }
            t += Time.deltaTime / (playerMoveDuration - 1); 
            transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return new WaitForFixedUpdate();         
        }
        transform.position = endPos;

        //resume player movement and collisions
        canMove = true;
        hitbox.enabled = true;
        rb2d.isKinematic = false;

        //checks if move player to strawberry
        if(inEnd)
        {
            StartCoroutine(DizzySwipeAnim());
            yield break;
        }

        animator.SetTrigger("EndSwipe");
    }

    private IEnumerator DizzySwipeAnim()
    {
        //transition to slide
        animator.SetBool("StartSlide", true);

        //move player for slide
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + new Vector3(-7, 0, 0);
        float t = 0;
        while (t <= 0.7f)
        {
            t += Time.deltaTime / 1;
            transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return new WaitForFixedUpdate();
        }
        //transform.position = endPos;

        //wait for stars
        yield return new WaitForSeconds(1);
        animator.SetTrigger("StopStars");

        //anim event calls run to strawberry
    }


    private IEnumerator RotateForSeconds(float duration, float speed)
    {
        float elapsedTime = 0f;
        animator.SetTrigger("DizzySwipe");

        //TODO: in this loop also move the player scale from 5 to 8 and back

        // Keep rotating the object while elapsed time is less than the duration
        while (elapsedTime < duration)
        {
            transform.Rotate(Vector3.forward * speed * Time.deltaTime);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // After rotation, return the object to 0 degrees rotation
        float currentRotation = transform.eulerAngles.z;
        float targetRotation = 0f;
        float timeToReset = 0.5f;
        float resetTimeElapsed = 0f;

        while (resetTimeElapsed < timeToReset)
        {
            // Interpolate back to 0 degrees using Mathf.LerpAngle
            float newRotation = Mathf.LerpAngle(currentRotation, targetRotation, resetTimeElapsed / timeToReset);
            transform.rotation = Quaternion.Euler(0f, 0f, newRotation);

            resetTimeElapsed += Time.deltaTime;

            yield return null;
        }

        // Ensure the final rotation is exactly 0 degrees
        transform.rotation = Quaternion.Euler(0f, 0f, targetRotation);
    }



    /// <summary>
    /// Disables the map and jump callback function when the script is disabled
    /// </summary>
    private void OnDisable()
    {
        playerJump.performed -= PlayerJump_performed;
        TierManager.SwipeTierAction -= MoveToNextTier;
        TierManager.EndSequence -= EndAnim;
        TimerSystem.StartGame -= StartGameplay;

        actions.Disable();
    }

    public void GotHitByIcing()
    {
        if (invincibilitySecondsRemaining <= 0)
        {
            // Knocks hamster back in opposite of the direction it's facing
            if(canMove)
            {
                animator.SetTrigger("Splat");
            }
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
            if (canMove)
            {
                animator.SetTrigger("Stun");
            }
            KnockBack(direction);
            SfxManager.Instance.PlaySFX("HitByOrange");
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
        if (inKnockback && CanJump() && !dizzy)
        {
            dizzy = true;
            inKnockback = false;
            StartCoroutine(Dizzy());
        }

        //Wall bump functionality
        if (collision.gameObject.name.Contains("Wall") && CanJump())
        {
            StartCoroutine(WallBump());
        }
    }

    /// <summary>
    /// Wall bump functionality
    /// </summary>
    /// <returns></returns>
    private IEnumerator WallBump()
    {
        //TODO: play animation?
        SfxManager.Instance.PlaySFX("HamsterWallBump");
        canMove = false;

        rb2d.velocity = new Vector2(facingLeft ? 8 : -8, 10);
        yield return new WaitForSeconds(0.1f);
        rb2d.velocity = new Vector2(facingLeft ? 7 : -7, rb2d.velocity.y);
        yield return new WaitForSeconds(0.1f);
        rb2d.velocity = new Vector2(facingLeft ? 5 : -5, rb2d.velocity.y);
        yield return new WaitForSeconds(0.1f);
        rb2d.velocity = new Vector2(facingLeft ? 3 : -3, rb2d.velocity.y);
        yield return new WaitForSeconds(0.1f);
        canMove = true;
        rb2d.velocity = new Vector2(facingLeft ? 1.5f : -1.5f, rb2d.velocity.y);
        yield return new WaitForSeconds(0.1f);
        rb2d.velocity = new Vector2(facingLeft ? .8f : -.8f, rb2d.velocity.y);
        yield return new WaitForSeconds(0.1f);
        rb2d.velocity = new Vector2(0, rb2d.velocity.y);
        yield return new WaitForSeconds(0.1f);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //Plays the strawberry collection anim
        if (collision.gameObject.name.Contains("Strawberry") &&
            (transform.position.x >= collision.transform.position.x))
        {
            inEnd = false;
            isSpinning = false;
            rb2d.velocity = Vector2.zero;
            rb2d.isKinematic = true;
            collision.gameObject.SetActive(false);
            transform.position = transform.position + new Vector3(0, 1.39f, 0);

            animator.SetBool("Collect", true);
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
        invincibilitySecondsRemaining = _invincibilityFramesInSeconds;
        yield return new WaitUntil(() => !inKnockback && !dizzy);

        bool opacityGoingDown = true;
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

    public IEnumerator Dizzy()
    {
        float t = 0;
        while (t < _lengthOfDizzy)
        {
            yield return null;
            t += Time.deltaTime;
            rb2d.velocity = new Vector2(0, rb2d.velocity.y);
        }
        dizzy = false;
        canMove = true;
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
