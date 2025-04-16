using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatSwipeAnimController : MonoBehaviour
{
    Animator anim;
    [SerializeField] bool isReactCat;
    [SerializeField] List<float> catTransformAmts = new();
    public static Action TriggerCatReact;

    void OnEnable()
    {
        TimerSystem.CatSwipeAnim += CatSwipe;
        TriggerCatReact += CatReact;
        anim = GetComponent<Animator>();
    }

    private void CatSwipe()
    {
        if (!isReactCat)
        {
            transform.localPosition = transform.localPosition + new Vector3(catTransformAmts[0], 0, 0);
            catTransformAmts.RemoveAt(0);
            anim.SetTrigger("Swipe");
        }
    }

    private void CatReact()
    {
        if(isReactCat)
        {
            anim.SetTrigger("react");
        }
    }

    void OnDisable()
    {
        TimerSystem.CatSwipeAnim -= CatSwipe;
        TriggerCatReact -= CatReact;
    }
}
