/*****************************************************************************
// File Name :         PlayerTransportable.cs
// Author :            Dalsten Yan
// Creation Date :     02/24/2025
//
// Brief Description : Helps move the player on a moving platform, parenting
    it to a child of the moving platform so that it shares it's position only
    while the player rides the platform. 
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTransportable : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) 
        {
            //Optional just for more movement accuracy,
            //if the player is jumping UP above the platform, this piece of code
            //ensures that the platform doesn't parent the Player and move him a little while he's jumping.
            //Can be removed if unecessary.
            if (collision.gameObject.GetComponent<Rigidbody2D>().velocity.y > 0)
                return;
            collision.transform.parent = transform;
        }
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        collision.transform.parent = null;
    }
}
