/*****************************************************************************
// File Name :         BackgroundBehavior.cs
// Author :            Marissa Moser
// Contributors:       
// Creation Date :     03/05/2025
//
// Brief Description : Moves the background when a tier is swiped.
*****************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class BackgroundBehavior : MonoBehaviour
{
    public static Action StartTimelineAnim;
    private Vector3 defaultPos;

    [SerializeField] GameObject background;
    [Tooltip("How long the tier should float for before falling")]
    [SerializeField] private float coyoteTime;
    [Tooltip("How long the tier should take to fall")]
    [SerializeField] private float fallTime;

    [SerializeField] List<PlayableDirector> tierSwipeAnims = new();

void OnEnable()
    {
        StartTimelineAnim += CallTierTransition;
        defaultPos = background.transform.position;
    }

    /// <summary>
    /// Called by the MoveBackground Action to start the coroutine that mves the 
    /// baclground.
    /// </summary>
    private void CallTierTransition()
    {
        //defaultPos += new Vector3(0, 20, 0);
        //StartCoroutine(MoveBackground());

        if (tierSwipeAnims.Count != 0)
        {
            tierSwipeAnims[0].Play();
            tierSwipeAnims.RemoveAt(0);
        }
    }

    void OnDisable()
    {
        StartTimelineAnim -= CallTierTransition;
    }

    /// <summary>
    /// Moves the background after coyote time has passed for as long as fall time is
    /// set to.
    /// </summary>
    /// <returns></returns>
    private IEnumerator MoveBackground()
    {
        yield return new WaitForSeconds(coyoteTime);

        Vector3 startPos = background.transform.position;
        Vector3 endPos = defaultPos;
        float t = 0;
        while (t <= 1.0f)
        {
            t += Time.deltaTime / fallTime;
            background.transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return new WaitForFixedUpdate();
        }
        background.transform.position = endPos;
    }
}
