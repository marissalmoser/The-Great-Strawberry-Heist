/******************************************************************************
 * Author: Brad Dixon
 * Creation Date: 2/11/2025
 * File Name: LaunchPad.cs
 * Brief: A platform that launches the player upwards
 * ***************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchPad : MonoBehaviour
{
    [Tooltip("True means the player has to touch the top to launch upwards")]
    [SerializeField] private bool hitTop;

    [Tooltip("The force in which the player gets launched upwards")]
    [SerializeField] private float launchForce;

    Rigidbody2D rb;

    /// <summary>
    /// Checks for when the player touches the launch pad
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerBehaviour>())
        {
            rb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (hitTop)
            {
                //Used to check that the player is falling onto the top of the pad
                if (rb.velocity.y < 0)
                {
                    LaunchPlayer();
                }
            }
            else
            {
                LaunchPlayer();
            }
        }
    }

    /// <summary>
    /// Launchs the player upwards by x amount
    /// </summary>
    private void LaunchPlayer()
    {
        //Makes the y velocity 0 so the launch force is consistent
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(Vector2.up * launchForce, ForceMode2D.Impulse);
    }
}
