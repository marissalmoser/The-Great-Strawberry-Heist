using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerSystem : MonoBehaviour
{
    [Tooltip("Reference to the timer object on the canvas.")]
    [SerializeField] Image timerImage;

    [Tooltip("Enable this if you only want the to test the timer for one tier. Arrange" +
        "the tier manager so the tier you are testing is the first tier, and ensure the " +
        "first index of the timers below is set for your tier.")]
    [SerializeField] bool testOneTier;

    [Tooltip("Edit this to change the delay before the game starts (for the opening sequence)")]
    [SerializeField] float startDelayTime;
    [Tooltip("How long the timer for each tier should be. Assign a new index per tier. " +
        "You do not need a timer for the top strawberry 'tier'.")]
    [SerializeField] List<float> tierTimes = new List<float>();
    [Tooltip("Edit this to change how long the camera shakes before a tier is swiped.")]
    [SerializeField] float tierCamShakeDuration;
    [Tooltip("How many seconds the player takes to be moved from the bottom tier when" +
        "they get swiped")]
    [SerializeField] private float playerMoveAfterSwipeTransitionTime;
    [Tooltip("Edit this to change how long into each tier the falling icing is triggered.")]
    [SerializeField] float triggerFallingIcingTime; //should this be a range?
    [Tooltip("Edit this to change the time in between each falling icing splotch after" +
        "they have been triggered.")]
    [SerializeField] float delayFallingIcingTime;

    private float currentTime;
    private float currentMaxTime;

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
        currentMaxTime = tierTimes[0] + (currentMaxTime - currentTime); //TODO: add cam shake time if camera is not shaking

        print(currentTime + " and " + currentMaxTime);
        currentTime = 0;

        UpdateTimerUI();

        StopCoroutine(currentTimer);
        currentTimer = StartCoroutine(TierTimer());
    }

    /// <summary>
    /// update the time values and then cal this to update the timer UI
    /// </summary>
    private void UpdateTimerUI()
    {
        float time = (currentTime) / (currentMaxTime + tierCamShakeDuration);
        timerImage.fillAmount = (1 - time);
    }

    /// <summary>
    /// Plays a delay befor the gamae timer starts, then starts the first tier's timer
    /// </summary>
    /// <returns></returns>
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
        while(currentTime < (currentMaxTime))
        {
            yield return new WaitForSeconds(0.1f);

            //check for falling icing time
            if (currentTime == triggerFallingIcingTime) //maybe make <= and bool
            {
                //TODO: trigger falling icing for this tier
                print("ICING");
            }

            currentTime += 0.1f;
            UpdateTimerUI();
        }

        print("start shake");
        //start tier swipe sequence
        TierManager.SwipeTierAction?.Invoke(tierCamShakeDuration, playerMoveAfterSwipeTransitionTime);
        while (currentTime < (currentMaxTime + tierCamShakeDuration))
        {
            yield return new WaitForSeconds(0.1f);
            currentTime += 0.1f;
            UpdateTimerUI();
        }

        //tier is swiped
        currentTime = 0;
        tierTimes.RemoveAt(0);

        //checks for the last tier, evaulates end condition
        if(tierTimes.Count <= 0)
        {
            print("time ran out in the last tier!");
            //TODO: trigger end condition
            yield break;
        }

        currentMaxTime = tierTimes[0];

        UpdateTimerUI();

        //Wait for player to be moved
        yield return new WaitForSeconds(playerMoveAfterSwipeTransitionTime);

        //start next tier's timer if not testing
        if (!testOneTier)
        {
            currentTimer = StartCoroutine(TierTimer());
        }
    }

    /// <summary>
    /// Triggeres all the falling icing in this tier.
    /// </summary>
    /// <returns></returns>
    private IEnumerator TriggerFallingIcing()
    {
        //while there is icing left in this tier to fall
        while(true)
        {
            //TODO: trigger next in order splotch

            yield return new WaitForSeconds(delayFallingIcingTime);
        }
    }

    private void OnDisable()
    {
        TierManager.NextTierAction -= NextTier;
    }
}
