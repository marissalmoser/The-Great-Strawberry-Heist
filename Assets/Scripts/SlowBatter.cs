/*****************************************************************************
 * Author: Brad Dixon
 * Creation Date: 1/28/2025
 * File Name: SlowBatter.cs
 * Brief: Slows the player when they are touching the batter.
 * ***************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowBatter : MonoBehaviour
{
    /// <summary>
    /// Slows the player
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerBehaviour>() && !ScoreManager.Instance.IsInStarMode)
        {
            //Debug.Log("slow");
            collision.GetComponent<PlayerBehaviour>().SlowPlayer();
        }
    }

    /// <summary>
    /// Player speed becomes normal
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerBehaviour>() && !ScoreManager.Instance.IsInStarMode)
        {
            //Debug.Log("normal");
            collision.GetComponent<PlayerBehaviour>().NormalSpeed();
        }
    }
}
