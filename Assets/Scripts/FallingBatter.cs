using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class FallingBatter : MonoBehaviour
{
    public GameObject objectToSpawn; 
    public Transform spawnPoint;

  //Turn of rigid body in start function and the write a PUBLIC function to turn it on. Add alien Beam

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.TryGetComponent(out PlayerBehaviour pb))
        {
            pb.GotHitByIcing();
        }

        if (collision.gameObject.CompareTag("Ground"))  
        {
            Instantiate(objectToSpawn, spawnPoint.position, spawnPoint.rotation);
            Destroy(gameObject);
        }
    }
}
