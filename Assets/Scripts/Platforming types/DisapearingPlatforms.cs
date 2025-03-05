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
    private Vector3 defaultPosition;
    [SerializeField] private float disappearTime = 2f;
    [SerializeField] private float reappearTime = 5f;
    
    private SpriteRenderer spriteRenderer;
    private Collider2D platformCollider;

    [SerializeField] private float _shakeIntensity;
    [SerializeField] private float _timeToReachMaxShake;
    [SerializeField] private float _timeBetweenShakes;

    /// <summary>
    /// Grabs Components
    /// </summary>
    void Start()
    {
        defaultPosition = transform.localPosition;
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
        float t = 0;
        float direction = 1;
        int shakeCount = 1;
        float timeSinceLastShake = 0;
        while (t < disappearTime)
        {
            yield return null;
            t += Time.deltaTime;
            timeSinceLastShake += Time.deltaTime;
            if (timeSinceLastShake > _timeBetweenShakes)
            {
                transform.localPosition = defaultPosition +
                    new Vector3(Mathf.Min(t, _timeToReachMaxShake) * direction * _shakeIntensity, 0, 0);
                shakeCount++;
                direction = Mathf.Sin(shakeCount * (Mathf.PI / 2));
                timeSinceLastShake -= _timeBetweenShakes;
            }
        }
        spriteRenderer.enabled = false;
        platformCollider.enabled = false;

        yield return new WaitForSeconds(reappearTime);
        spriteRenderer.enabled = true;
        platformCollider.enabled = true;
    }
}
