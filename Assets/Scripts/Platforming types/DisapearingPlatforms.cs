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
    [Tooltip("True means the player has to touch the top for the platform to disappear")]
    [SerializeField] private bool CoroutineStarted;
    Rigidbody2D rb;


    private Vector3 defaultPosition;
    [SerializeField] private float disappearTime = 2f;
    [SerializeField] private float reappearTime = 5f;
    
    private SpriteRenderer spriteRenderer;
    [SerializeField] private SpriteRenderer _shadow;
    [SerializeField] private GameObject _particles;
    [SerializeField] private GameObject _particleShadow;
    [SerializeField] private GameObject _particleSmoke;
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

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!CoroutineStarted && collision.collider.TryGetComponent<PlayerBehaviour>(out PlayerBehaviour pb))
        {
           if (pb.PlayerPlatformCheck())
           {
                StartCoroutine(DisappearAndReappear());
                CoroutineStarted = true;
           }
            
        }
    }

    public void ForceDestruction()
    {
        if (!CoroutineStarted)
        {
            StartCoroutine(DisappearAndReappear());
            CoroutineStarted = true;
        }
    }

    /// <summary>
    /// Disables and then enables components based on the disappear and reappear Time
    /// </summary>
    IEnumerator DisappearAndReappear()
    {
        SfxManager.Instance.PlaySFX("DisappearingPlatform");
        float t = 0;
        float direction = 1;
        int shakeCount = 1;
        float timeSinceLastShake = 0;
        _particles.SetActive(true);
        _particleShadow.SetActive(true);
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
        if (_shadow != null) _shadow.enabled = false;
        _particleSmoke.SetActive(true);
        platformCollider.enabled = false;

        yield return new WaitForSeconds(reappearTime);
        SfxManager.Instance.StopSFX("DisappearingPlatform");
        spriteRenderer.enabled = true;
        if (_shadow != null) _shadow.enabled = true;
        _particles.SetActive(false);
        _particleShadow.SetActive(false);
        _particleSmoke.SetActive(false);
        platformCollider.enabled = true;

        CoroutineStarted = false;
    }
}
