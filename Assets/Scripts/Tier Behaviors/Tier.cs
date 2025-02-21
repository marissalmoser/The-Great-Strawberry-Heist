/*****************************************************************************
// File Name :         Tier.cs
// Author :            Marissa Moser
// Creation Date :     01/30/2025
//
// Brief Description : Contains functionality that is specific per tier. Such
    functinoality includes camera functions and the swipe behavior
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class Tier : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera tierCam;
    [SerializeField] private GameObject tierSpawnPt;
    [Tooltip("The trapdoor that leads out of this tier")]
    [SerializeField] private GameObject Trapdoor;
    [SerializeField] private AnimationCurve _swipeEaseCurve;
    private CinemachineBasicMultiChannelPerlin _perlinNoise;

    void Start()
    {
        _perlinNoise = tierCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    /// <summary>
    /// Disables the tier camera by seting it active and removing it's parent from
    /// the tier so that its position isn't swiped.
    /// </summary>
    public void DisableCam()
    {
        tierCam.gameObject.SetActive(false);
        tierCam.gameObject.transform.SetParent(null, true);
    }

    /// <summary>
    /// Call to shake the camera in this tier for the duration
    /// </summary>
    public void ShakeCam(float amplitude, float frequency, float duration)
    {
        StartCoroutine(Shake(amplitude, frequency, duration));
    }

    /// <summary>
    /// gets the transform of the spawn point in this tier
    /// </summary>
    public Transform GetTierSpawn()
    {
        return tierSpawnPt.transform;
    }

    /// <summary>
    /// Starts swipe coroutine
    /// </summary>
    public void Swipe()
    {
        StartCoroutine(MoveTier());
        if (Trapdoor.GetComponent<Trapdoor>() != null)
        {
            Trapdoor.GetComponent<Trapdoor>().DisableDoor();
        }
    }

   
    public IEnumerator SwipeCanceled(float timeRemaining)
    { 
        yield return new WaitForSeconds(timeRemaining);
        Swipe();
    }

    /// <summary>
    /// Moves the tier off screen based on the swipe ease curve
    /// </summary>
    private IEnumerator MoveTier()
    {
        DisableCam();
        float timeElapsed = 0f;
        float totalDuration = _swipeEaseCurve.keys[_swipeEaseCurve.length - 1].time;

        float startPositionX = transform.position.x;

        //randomply decides a direction
        float targetPositionX = Random.Range(0,2) >= 1 ? startPositionX - 40f : startPositionX + 40f;

        while (timeElapsed < totalDuration)
        {
            float t = _swipeEaseCurve.Evaluate(timeElapsed);

            startPositionX = Mathf.Lerp(startPositionX, targetPositionX, t);

            transform.position = new Vector3(startPositionX, transform.position.y, 0);

            timeElapsed += Time.deltaTime;

            yield return null;
        }
        transform.position = new Vector3(startPositionX, transform.position.y, 0);
    }

    /// <summary>
    /// Call coroutine to shake a camera's noise component for the duration at the 
    /// specified amplitude and frequency.
    /// </summary>
    private IEnumerator Shake(float amplitude, float frequency, float duration)
    {
        // Set the noise parameters
        _perlinNoise.m_AmplitudeGain = amplitude;
        _perlinNoise.m_FrequencyGain = frequency;

        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Reset noise parameters
        _perlinNoise.m_AmplitudeGain = 0f;
        _perlinNoise.m_FrequencyGain = 0f;
    }
}
