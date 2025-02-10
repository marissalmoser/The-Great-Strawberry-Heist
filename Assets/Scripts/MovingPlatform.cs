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

    bool isMoving;

    private void Start()
    {
        isMoving = true;
        StartCoroutine(MovePlatform());
    }

    IEnumerator MovePlatform()
    {
        Vector3 startPos = startPosGO.transform.position;
        Vector3 endPos = endPosGO.transform.position;
        transform.position = startPos;

        yield return new WaitForSeconds(delayStartTime);

        while(isMoving)
        {
            //move to end pos
            float t = 0;
            while (t <= 1.0f)
            {
                t += Time.deltaTime / moveTime;
                transform.position = Vector3.Lerp(startPos, endPos, t);
                yield return new WaitForFixedUpdate();
            }
            transform.position = endPos;

            //wait
            yield return new WaitForSeconds(waitTime);

            //move to start pos
            t = 0;
            while (t <= 1.0f)
            {
                t += Time.deltaTime / moveTime;
                transform.position = Vector3.Lerp(endPos, startPos, t);
                yield return new WaitForFixedUpdate();
            }
            transform.position = startPos;

            //wait
            yield return new WaitForSeconds(waitTime);
        }

    }
}
