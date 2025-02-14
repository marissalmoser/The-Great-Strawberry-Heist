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
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.TryGetComponent(out PlayerBehaviour pb))
        {
            //TODO: Call function on player for hit functionality? IDK talk to design
            Debug.Log("Rolling Fruit Hit Player");
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
