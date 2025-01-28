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

    //A base value for returning the player's speed to normal
    private const float BASE_MULTIPLER = 1;
    [SerializeField] private Rigidbody2D rb2d;
    private float moveValue;

    private float isGrounded;

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
    }

    /// <summary>
    /// Executes when the jump button is pressed
    /// </summary>
    /// <param name="obj"></param>
    private void PlayerJump_performed(InputAction.CallbackContext obj)
    {
        PlayerJump();
    }

    /// <summary>
    /// Executes player code at a fixed rate
    /// </summary>
    private void FixedUpdate()
    {
        MovePlayer();
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
            rb2d.AddForce(new Vector2(rb2d.velocity.x, jumpHeight), ForceMode2D.Impulse);
        }
    }

    /// <summary>
    /// Checks if the player is grounded to see if they can jump
    /// </summary>
    /// <returns></returns>
    private bool CanJump()
    {
        return Physics2D.BoxCast(hitbox.bounds.center, hitbox.bounds.size * .95f, 0, Vector2.down, 0.1f, ground);
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
}
