/*****************************************************************************
 * Author: Marissa Moser
 * Creation Date: 2/7/2025
 * File Name: RollingFruit.cs
 * Brief: Rolling fruit object. Movement is done in the animator.
 * ***************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingFruit : MonoBehaviour
{
    [Tooltip("Value affects hamster knockback, not movement of orange")]
    [SerializeField] private bool _movesLeft;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out PlayerBehaviour pb))
        {
            if (!ScoreManager.Instance.IsInStarMode)
            {
                pb.GotHitByOrange(_movesLeft);
            }
            else 
            {
                DestroyFruit();
            }
            
        }
    }

    /// <summary>
    /// An event to call from an animation event when the rolling fruit object should
    /// be destroyed.
    /// </summary>
    public void DestroyFruit()
    {
        Destroy(gameObject);
    }
}
