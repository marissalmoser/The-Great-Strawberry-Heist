using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerSystem : MonoBehaviour
{
    [SerializeField] int startDelayTime;

    [SerializeField] List<int> tierTimes = new List<int>();

    [Tooltip("Edit this to change how long the camera shakes before a tier is swiped.")]
    [SerializeField] int tierCamShakeDuration;

    [Tooltip("Edit this to change how long into each tier the falling icing is triggered.")]
    [SerializeField] int triggerFallingIcingTime;

    private int currentTime;
    private int currentMaxTime;

    private Coroutine currentTimer;

    private void Start()
    {
        TierManager.NextTierAction += NextTier;

        //update time values 
        for (int i = 0; i < tierTimes.Count; i++)
        {
            tierTimes[i] -= tierCamShakeDuration;
        }
        currentTime = 0;
        currentMaxTime = tierTimes[0];

        UpdateTimerUI();

        //start delay coroutine
        currentTimer = StartCoroutine(StartDelay());
    }

    /// <summary>
    /// Triggered when player makes it to the next tier
    /// </summary>
    private void NextTier()
    {
        tierTimes.RemoveAt(0);
        currentMaxTime = tierTimes[0] + currentTime;
        currentTime = 0;
        
        UpdateTimerUI();

        StopCoroutine(currentTimer);
        currentTimer = StartCoroutine(TierTimer());
    }

    private void UpdateTimerUI()
    {
        //max time - current time / max time + shake delay
    }

    IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(startDelayTime);

        currentTimer = StartCoroutine(TierTimer());
    }

    /// <summary>
    /// Coroutine used for each tier
    /// </summary>
    /// <returns></returns>
    IEnumerator TierTimer()
    {
        //count until swipe shaking should start
        while(currentTime < currentMaxTime)
        {
            yield return new WaitForSeconds(1);

            //check for falling icing time
            if (currentTime == triggerFallingIcingTime)
            {
                //TODO: trigger falling icing for this tier
                print("ICING");
            }

            currentTime++;
            UpdateTimerUI();
        }

        //start tier swipe sequence
        TierManager.SwipeTierAction?.Invoke(tierCamShakeDuration);
        for(int i = 0; i < tierCamShakeDuration; i++)
        {
            yield return new WaitForSeconds(1);
            currentTime++;
            UpdateTimerUI();
        }

        //tier is swiped
        currentTime = 0;
        tierTimes.RemoveAt(0);
        currentMaxTime = tierTimes[0];

        UpdateTimerUI();

        //start next tier's timer;
        currentTimer = StartCoroutine(TierTimer());
    }

    private void OnDisable()
    {
        TierManager.NextTierAction -= NextTier;
    }
}
