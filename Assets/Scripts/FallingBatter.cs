/****************************************************************************
// File Name :         FallingBatter.cs
// Author :            Kadin Harris
// Creation Date :     02/14/2025
//
// Brief Description :Behavior for the falling batter,
*****************************************************************************/
using UnityEngine;
using System.Collections; 

public class FallingBatter : MonoBehaviour
{
    public GameObject objectToSpawn; 
    public Transform spawnPoint;
    
    [SerializeField] LayerMask layerToHit;
    [SerializeField] GameObject alienBar;

    private void Start()
    {
        alienBar.SetActive(false);
        GetComponent<Rigidbody2D>().gravityScale = 0;
    }

    /// <summary>
    /// Makes the icing fall
    /// </summary>
    public void TriggerFall()
    {
        StartCoroutine(FlashAlienBar());
    }

    private IEnumerator FlashAlienBar()
    {
        Renderer barRenderer = alienBar.GetComponent<Renderer>();
        if (barRenderer == null) yield break;

        for (int i = 0; i < 6; i++) 
        {
            alienBar.SetActive(!alienBar.activeSelf);
            yield return new WaitForSeconds(0.3f); 
        }

        alienBar.SetActive(false); 
        GetComponent<Rigidbody2D>().gravityScale = 1; 
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.TryGetComponent(out PlayerBehaviour pb))
        {
            pb.GotHitByIcing();
            GetComponent<Collider2D>().enabled = false;
            Destroy(gameObject);
        }

        if ((layerToHit.value & (1 << collision.gameObject.layer)) > 0)
        {
            Instantiate(objectToSpawn, spawnPoint.position, spawnPoint.rotation);
            Destroy(gameObject);
        }
    } 
}
