/*****************************************************************************
// File Name :         Tier.cs
// Author :            Marissa Moser
// Creation Date :     01/30/2025
//
// Brief Description : 
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class Tier : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera tierCam;
    [SerializeField] private GameObject tierSpawnPt;
    [SerializeField] private AnimationCurve _turnEaseCurve;


    void Awake()
    {
        
    }

    public void DiableCam()
    {
        tierCam.gameObject.SetActive(false);
    }

    Transform GetTierSpawn()
    {
        return tierSpawnPt.transform;
    }

    public void Swipe()
    {
        StartCoroutine(MoveTier());
    }

    private IEnumerator MoveTier()
    {
        float timeElapsed = 0f;
        float totalDuration = _turnEaseCurve.keys[_turnEaseCurve.length - 1].time;

        float startPositionX = transform.position.x;

        //randomply decides a direction
        float targetPositionX = Random.Range(0,2) > 1 ? startPositionX - 40f : startPositionX + 40f;

        while (timeElapsed < totalDuration)
        {
            float t = _turnEaseCurve.Evaluate(timeElapsed);

            startPositionX = Mathf.Lerp(startPositionX, targetPositionX, t);

            transform.position = new Vector3(startPositionX, transform.position.y, 0);

            timeElapsed += Time.deltaTime;

            yield return null;
        }
        transform.position = new Vector3(startPositionX, transform.position.y, 0);
    }
}
