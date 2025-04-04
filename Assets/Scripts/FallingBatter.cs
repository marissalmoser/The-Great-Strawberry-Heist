/*****************************************************************************
// File Name :         FallingBatter.cs
// Author :            Kadin Harris
// Creation Date :     02/14/2025
//
// Brief Description :Behavior for the falling batter
*****************************************************************************/
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class FallingBatter : MonoBehaviour
{
    public GameObject objectToSpawn; 
    public Transform spawnPoint;
    
    [SerializeField] LayerMask layerToHit;
    [SerializeField] GameObject alienBar;
    [SerializeField] GameObject exclamationMark;
    [SerializeField] GameObject explodingParticlePos;
    [SerializeField] GameObject explodingParticlePrefab;

    [Tooltip("How long the falling icing should appear for before falling")]
    [SerializeField] float icingWaitTime;
    SpriteRenderer sr;

    int level = 0;

    private void Start()
    {
        alienBar.SetActive(false);
        exclamationMark.SetActive(false);
        GetComponent<Rigidbody2D>().gravityScale = 0;
        sr = GetComponent<SpriteRenderer>();
        sr.enabled = false;
    }
    /// <summary>
    /// Makes the icing fall
    /// </summary>
    public void TriggerFall(int icingLevel)
    {
        level = icingLevel;
        StartCoroutine(IcingFall());
    }

    private IEnumerator IcingFall()
    {
        {
            alienBar.SetActive(true);
            exclamationMark.SetActive(true);
            sr.enabled = true;
            Renderer barRenderer = alienBar.GetComponent<Renderer>();

            yield return new WaitForSeconds(icingWaitTime);
            
            if (barRenderer == null) yield break;

            // Flash effect using barRenderer
            for (int i = 0; i < 6; i++)
            {
                barRenderer.enabled = !barRenderer.enabled;
                yield return new WaitForSeconds(0.3f); // Wait for 0.3 seconds
            }

            GetComponent<BoxCollider2D>().enabled = true;
            exclamationMark.SetActive(false);
            GetComponent<Animator>().SetTrigger("Fall");
            GetComponent<Rigidbody2D>().gravityScale = 1;

            if(TierManager.Instance.GetTierCount() - 1 == level)
            {
                SfxManager.Instance.PlaySFX("IcingFalling");
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.TryGetComponent(out PlayerBehaviour pb))
        {
            if (!ScoreManager.Instance.IsInStarMode) 
            {
                pb.GotHitByIcing();
                GetComponent<Collider2D>().enabled = false;
            }
            Instantiate(explodingParticlePrefab, explodingParticlePos.transform.position, Quaternion.identity);

            SfxManager.Instance.StopSFX("IcingFalling");
            SfxManager.Instance.PlaySFX("IcingLand");

            Destroy(gameObject);
        }

        if ((layerToHit.value & (1 << collision.gameObject.layer)) > 0)
        {
            Instantiate(objectToSpawn, spawnPoint.position + new Vector3(0, 0.3f, 0), spawnPoint.rotation);

            if (TierManager.Instance.GetTierCount() - 1 == level)
            {
                SfxManager.Instance.PlaySFX("IcingLand");
            }
            SfxManager.Instance.StopSFX("IcingFalling");

            Destroy(gameObject);
        }
    }

    public void MoveBar()
    {
        alienBar.transform.localPosition = alienBar.transform.localPosition + new Vector3(0, -8, 0);
    }
}
