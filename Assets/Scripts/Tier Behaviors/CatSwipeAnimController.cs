using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatSwipeAnimController : MonoBehaviour
{
    Animator anim;
    [SerializeField] List<float> catTransformAmts = new();

    void OnEnable()
    {
        TimerSystem.CatSwipeAnim += CatSwipe;
        anim = GetComponent<Animator>();
    }

    private void CatSwipe()
    {
        transform.localPosition = transform.localPosition + new Vector3(catTransformAmts[0], 0, 0);
        catTransformAmts.RemoveAt(0);
        anim.SetTrigger("Swipe");
    }

    void OnDisable()
    {
        TimerSystem.CatSwipeAnim -= CatSwipe;
    }
}
