/*****************************************************************************
// File Name :         FallingBatter.cs
// Author :            Kadin Harris
// Creation Date :     02/14/2025
//
// Brief Description :Behavior for the falling batter
*****************************************************************************/
using UnityEngine;

public class FallingBatter : MonoBehaviour
{
    public GameObject objectToSpawn; 
    public Transform spawnPoint;
    
    [SerializeField] LayerMask layerToHit;
    [SerializeField] GameObject alienBar;

    private void Start()
    {
        GetComponent<Rigidbody2D>().gravityScale = 0;
    }
    //private void Update()
    //{
    //   if(Input.GetKey(KeyCode.E))
    //   {
    //        TriggerFall();
    //   }
    //}

    /// <summary>
    /// Makes the icing fall
    /// </summary>
    public void TriggerFall()
    {
        GetComponent<Rigidbody2D>().gravityScale = 1;
        alienBar.SetActive(true);
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
