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
    [SerializeField] GameObject explodingParticlePos;
    [SerializeField] GameObject explodingParticlePrefab;

    [Tooltip("How long the falling icing should appear for before falling")]
    [SerializeField] float icingWaitTime;
    SpriteRenderer sr;

    private void Start()
    {
        GetComponent<Rigidbody2D>().gravityScale = 0;
        sr = GetComponent<SpriteRenderer>();
        sr.enabled = false;
    }
    //private void Update()
    //{
    //    if (Input.GetKey(KeyCode.E))
    //    {
    //        TriggerFall();
    //    }
    //}

    /// <summary>
    /// Makes the icing fall
    /// </summary>
    public void TriggerFall()
    {
        StartCoroutine(IcingFall());
    }

    private IEnumerator IcingFall()
    {
        sr.enabled = true;
        alienBar.SetActive(true);

        yield return new WaitForSeconds(icingWaitTime);

        GetComponent<BoxCollider2D>().enabled = true;
        GetComponent<Animator>().SetTrigger("Fall");
        GetComponent<Rigidbody2D>().gravityScale = 1;

        SfxManager.Instance.PlaySFX("IcingFalling");
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
            Instantiate(objectToSpawn, spawnPoint.position, spawnPoint.rotation);
            Destroy(gameObject);
        }
    } 
}
