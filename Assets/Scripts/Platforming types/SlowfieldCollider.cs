/*****************************************************************************
// File Name :         SlowfieldCollider.cs
// Author :            Marissa Moser
// Creation Date :     03/13/2025
//
// Brief Description : Detects when the slow field hits the ground and then 
    disables the parents rigid body and deletes the collider. GO only collides
    with ground layers.
*****************************************************************************/
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
