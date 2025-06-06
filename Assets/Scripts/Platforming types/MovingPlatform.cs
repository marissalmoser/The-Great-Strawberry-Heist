/*****************************************************************************
// File Name :         MovingPlatform.cs
// Author :            Marissa Moser
// Creation Date :     02/10/2025
// Brief Description : The script on the moving platform objects. Contains customizable
    functinality for the platforms to move between two positions at a variable speed.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Tooltip("How long (in seconds) the platform should move for")]
    [SerializeField] float moveTime;
    [Tooltip("How long the platform should wait (in seconds) before moving")]
    [SerializeField] float delayStartTime;
    [Tooltip("How long the platform should wait (in seconds) before moving to its other position")]
    [SerializeField] float waitTime;

    [Tooltip("Game Object positioned at the same place as the platform")]
    [SerializeField] GameObject startPosGO;
    [Tooltip("Game Object positioned where the platform should move to")]
    [SerializeField] GameObject endPosGO;

    Coroutine coroutine;
    bool isMoving;

    private void Start()
    {
        isMoving = true;
        coroutine = StartCoroutine(MovePlatform());
        TimerSystem.StartGame += StopAndRestart;
    }

    /// <summary>
    /// Moves the platform from start pos to end pos, then back to start pos. Loops
    /// as long as the isMoving bool is true;
    /// </summary>
    /// <returns></returns>
    IEnumerator MovePlatform()
    {
        Transform startPos = startPosGO.transform;
        Transform endPos = endPosGO.transform;
        transform.position = startPos.position;

        // Make the positions siblings of the platform instead of children,
        // so that we can check their position every frame without
        // it being modified by the movement of the platform
        startPosGO.transform.parent = transform.parent;
        endPosGO.transform.parent = transform.parent;

        yield return new WaitForSeconds(delayStartTime);

        while(isMoving)
        {
            //move to end pos
            float t = 0;
            while (t <= 1.0f)
            {
                t += Time.deltaTime / moveTime;
                transform.position = Vector3.Lerp(startPos.position, endPos.position, t);
                yield return new WaitForFixedUpdate();
            }
            transform.position = endPos.position;

            //wait
            yield return new WaitForSeconds(waitTime);

            //move to start pos
            t = 0;
            while (t <= 1.0f)
            {
                t += Time.deltaTime / moveTime;
                transform.position = Vector3.Lerp(endPos.position, startPos.position, t);
                yield return new WaitForFixedUpdate();
            }
            transform.position = startPos.position;

            //wait
            yield return new WaitForSeconds(waitTime);
        }

    }

    private void StopAndRestart()
    {
        StopCoroutine(coroutine);
        coroutine = StartCoroutine(MovePlatform());
    }

    private void OnDestroy()
    {
        TimerSystem.StartGame -= StopAndRestart;
    }
}
