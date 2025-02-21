/*****************************************************************************
// File Name :         TimerSystem.cs
// Author :            Marissa Moser
// Creation Date :     02/14/2025
//
// Brief Description : Manages the game's timer. Contains customizable times for
    designers to edit. Triggers the tier swipe action and falling icing. Triggers
    the game's end and win conditions.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerSystem : MonoBehaviour
{
    [Tooltip("Reference to the timer object on the canvas.")]
    [SerializeField] Image timerImage;

    [Header("Timers")]
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

    [Header("Falling Batter")]
    [Tooltip("Add all the falling batter in each tier to a new list in this list.")]
    [SerializeField] List<GameObject> fallingIcingTier1 = new List<GameObject>();
    [SerializeField] List<GameObject> fallingIcingTier2 = new List<GameObject>();
    [SerializeField] List<GameObject> fallingIcingTier3 = new List<GameObject>();
    [SerializeField] List<GameObject> fallingIcingTier4 = new List<GameObject>();
    [SerializeField] List<GameObject> fallingIcingTier5 = new List<GameObject>();
    List<List<GameObject>> fallingIcing = new List<List<GameObject>>();


    private float currentTime;
    private float currentMaxTime;
    private bool isShaking;
    private bool triggeredIcing;
    public static bool DoMovePlayer;

    private Coroutine currentTimer;

    private void Start()
    {
        TierManager.NextTierAction += NextTier;

        //add falling icing to main list
        fallingIcing.Add(fallingIcingTier1);
        fallingIcing.Add(fallingIcingTier2);
        fallingIcing.Add(fallingIcingTier3);
        fallingIcing.Add(fallingIcingTier4);
        fallingIcing.Add(fallingIcingTier5);

        //set startting times
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
        SfxManager.Instance.StopSFX("CatHiss");
        SfxManager.Instance.PlaySFX("CatSad");
        TierManager.SwipeCanceledAction?.Invoke(currentMaxTime - currentTime);

        //check for win condition
        if (tierTimes.Count <= 0)
        {
            StopAllCoroutines();
            print("player made it to the strawberry");
            //TODO: trigger win condition
            return;
        }

        float time = currentMaxTime - currentTime;
        currentMaxTime = tierTimes[0] + time;
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
        float time = (currentTime) / (currentMaxTime);
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
        triggeredIcing = false;
        DoMovePlayer = true;

        //count until swipe shaking should start
        while(currentTime < (currentMaxTime - tierCamShakeDuration))
        {
            yield return new WaitForSeconds(0.1f);

            //check for falling icing time
            if (!triggeredIcing && currentTime >= triggerFallingIcingTime)
            {
                StartCoroutine(TriggerFallingIcing());
                triggeredIcing = true;
            }

            currentTime += 0.1f;
            UpdateTimerUI();
        }

        //start tier swipe sequence
        SfxManager.Instance.PlaySFX("CatHiss");
        TierManager.SwipeTierAction?.Invoke(tierCamShakeDuration, playerMoveAfterSwipeTransitionTime);
        isShaking = true;
        while (currentTime < currentMaxTime)
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
        isShaking = false;

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
        while (fallingIcing[0].Count > 0)
        {
            //trigger next in order splotch, then delete
            fallingIcing[0][0].GetComponent<FallingBatter>().TriggerFall();
            fallingIcing[0].RemoveAt(0);

            yield return new WaitForSeconds(delayFallingIcingTime);
        }

        //remove empty list of icing
        fallingIcing.RemoveAt(0);
    }

    private void OnDisable()
    {
        TierManager.NextTierAction -= NextTier;
    }
}
