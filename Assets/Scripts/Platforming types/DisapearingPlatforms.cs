/*****************************************************************************
// File Name :         DisapearingPlatforms.cs
// Author :            Kadin Harris
// Creation Date :     02/12/2025
// Brief Description : The script on disappearing platforms that makes them disappear and then reappear.
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisapearingPlatforms : MonoBehaviour
{
    [SerializeField] private float disappearTime = 2f;
    [SerializeField] private float reappearTime = 5f;
    
    private SpriteRenderer spriteRenderer;
    private Collider2D platformCollider;

    /// <summary>
    /// Grabs Components
    /// </summary>
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        platformCollider = GetComponent<Collider2D>();
    }


    /// <summary>
    /// Checks if it is the player
    /// </summary>
    void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerBehaviour playerBehaviour = collision.gameObject.GetComponent<PlayerBehaviour>();

        if (playerBehaviour != null)
        {
            StartCoroutine(DisappearAndReappear());
        }
    }

    /// <summary>
    /// Disables and then enables components based on the disappear and reappear Time
    /// </summary>
    IEnumerator DisappearAndReappear()
    {
        yield return new WaitForSeconds(disappearTime);
        spriteRenderer.enabled = false;
        platformCollider.enabled = false;

        yield return new WaitForSeconds(reappearTime);
        spriteRenderer.enabled = true;
        platformCollider.enabled = true;
    }
}
