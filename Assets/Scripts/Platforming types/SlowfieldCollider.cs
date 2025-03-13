using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowfieldCollider : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Rigidbody2D rb =  GetComponentInParent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Static;
        Destroy(gameObject);
    }
}
