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
    [SerializeField] private int scoreToAdd;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //can use pb to reference player behavior, to move them to the right position?
        if (collision.collider.TryGetComponent(out PlayerBehaviour pb))
        {
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (rb.velocity.y <= 0.5f && !TimerSystem.TimeUp)
            {
                CollisionLogic();
            }
        }     
    }

    public void CollisionLogic()
    {
        print("trapdoor");
        TimerSystem.DoMovePlayer = false;
        TierManager.NextTierAction?.Invoke();
        ScoreManager.Instance.AddScore(scoreToAdd, 0);
        Destroy(this);
    }

    public void DisableDoor()
    {
        Destroy(this);
    }
}
