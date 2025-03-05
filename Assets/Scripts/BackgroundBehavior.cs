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
using UnityEngine;

public class BackgroundBehavior : MonoBehaviour
{
    public static Action MoveBackgroundAction;
    private Vector3 defaultPos;

    [Tooltip("How long the tier should float for before falling")]
    [SerializeField] private float coyoteTime;
    [Tooltip("How long the tier should take to fall")]
    [SerializeField] private float fallTime;

void OnEnable()
    {
        MoveBackgroundAction += CallMoveBackgroundCoroutine;
        defaultPos = transform.position;
    }

    /// <summary>
    /// Called by the MoveBackground Action to start the coroutine that mves the 
    /// baclground.
    /// </summary>
    private void CallMoveBackgroundCoroutine()
    {
        defaultPos += new Vector3(0, 20, 0);
        StartCoroutine(MoveBackground());
    }

    void OnDisable()
    {
        MoveBackgroundAction -= CallMoveBackgroundCoroutine;
    }

    /// <summary>
    /// Moves the background after coyote time has passed for as long as fall time is
    /// set to.
    /// </summary>
    /// <returns></returns>
    private IEnumerator MoveBackground()
    {
        yield return new WaitForSeconds(coyoteTime);

        Vector3 startPos = transform.position;
        Vector3 endPos = defaultPos;
        float t = 0;
        while (t <= 1.0f)
        {
            t += Time.deltaTime / fallTime;
            transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return new WaitForFixedUpdate();
        }
        transform.position = endPos;
    }
}
