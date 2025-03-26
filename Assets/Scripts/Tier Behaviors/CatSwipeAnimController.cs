using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatSwipeAnimController : MonoBehaviour
{
    Animator anim;

    void OnEnable()
    {
        TimerSystem.CatSwipeAnim += CatSwipe;
        anim = GetComponent<Animator>();
    }

    private void CatSwipe()
    {
        transform.localPosition = transform.localPosition + new Vector3(-0.32f, 0, 0);
        anim.SetTrigger("Swipe");
    }

    void OnDisable()
    {
        TimerSystem.CatSwipeAnim -= CatSwipe;
    }
}
