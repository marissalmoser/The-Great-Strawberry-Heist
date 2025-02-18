using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class FallingBatter : MonoBehaviour
{
    public GameObject objectToSpawn; 
    public Transform spawnPoint;
    
    [SerializeField] LayerMask layerToHit;

    //Turn of rigid body in start function and the write a PUBLIC function to turn it on.2

    private void Start()
    {
        GetComponent<Rigidbody2D>().gravityScale = 0;
    }

    /// <summary>
    /// Makes the icing fall
    /// </summary>
    public void TriggerFall()
    {
        GetComponent<Rigidbody2D>().gravityScale = 1;
    }

    /// <summary>
    /// press L to make icing fall
    /// </summary>
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.L))
        {
            TriggerFall();
        }

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.TryGetComponent(out PlayerBehaviour pb))
        {
            pb.GotHitByIcing();
        }

        if ((layerToHit.value & (1 << collision.gameObject.layer)) > 0)
        {
            Instantiate(objectToSpawn, spawnPoint.position, spawnPoint.rotation);
            Destroy(gameObject);
        }
    }
}
