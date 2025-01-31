using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class Trapdoor : MonoBehaviour
{
    [SerializeField] private GameObject door;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //can use pb to reference player behavior, to move them to the right position?
        if(collision.TryGetComponent(out PlayerBehaviour pb))
        {
            TierManager.Instance.NextTier();
            door.SetActive(true);   //delay this
            Destroy(this);
        }
    }
}
