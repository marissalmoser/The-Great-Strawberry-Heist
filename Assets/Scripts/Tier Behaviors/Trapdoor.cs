/*****************************************************************************
// File Name :         Tier.cs
// Author :            Marissa Moser
// Creation Date :     01/30/2025
// Brief Description : The script on the trapdooe object to detext when the player 
    moves to the next tier, and invkokes that action
*****************************************************************************/

using UnityEngine;

public class Trapdoor : MonoBehaviour
{

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //can use pb to reference player behavior, to move them to the right position?
        if (collision.collider.TryGetComponent(out PlayerBehaviour pb))
        {
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (rb.velocity.y <= 0.1f)
            {
                TimerSystem.DoMovePlayer = false;
                TierManager.NextTierAction?.Invoke();
                Destroy(this);
            }
        }     
    }

    public void DisableDoor()
    {
        Destroy(this);
    }
}
