/*****************************************************************************
 * Author: Marissa Moser
 * Creation Date: 2/7/2025
 * File Name: RollingFruit.cs
 * Brief: Rolling fruit object. Movement is done in the animator.
 * ***************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class RollingFruit : MonoBehaviour
{
    [Tooltip("Value affects hamster knockback, not movement of orange")]
    [SerializeField] private bool _movesLeft;
    [SerializeField] private GameObject _particlePrefab;

    [Tooltip("Score and Vitality for the Orange")]
    [SerializeField] private int score = 500;
    [SerializeField] private int vitality = 0;

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
                ScoreManager.Instance.AddScore(score, vitality, transform.position, false);
                SfxManager.Instance.PlaySFX("OrangeExplode");
                Instantiate(_particlePrefab, transform.position, Quaternion.identity);
                DestroyFruit();
            }
            
        }
    }
    private void Awake()
    {
        score = 500;
        vitality = 0;
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
