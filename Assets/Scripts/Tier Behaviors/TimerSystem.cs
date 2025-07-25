/*****************************************************************************
// File Name :         TimerSystem.cs
// Author :            Marissa Moser
// Creation Date :     02/14/2025
//
// Brief Description : Manages the game's timer. Contains customizable times for
    designers to edit. Triggers the tier swipe action and falling icing. Triggers
    the game's end and win conditions.
*****************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
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

    [Tooltip("Edit this to change the delay before the game starts (for the opening sequence")]
    [SerializeField] float startDelayTime;
    [Tooltip("Edit this to change when the ")]
    [SerializeField] float startTimerAnimTime = 8.5f;
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

    [Header("Timer start text")]
    [Tooltip("Game object to be enabled when timer begins.")]
    [SerializeField] private GameObject startText;
    [SerializeField] private int timeBonusMultiplier;

    [Header("Intro skip timings")]
    [SerializeField] private PlayableDirector introCutscene;
    [SerializeField] private GameObject skipGraphic;
    [SerializeField] private AudioSource timerSound;
    [SerializeField] private float startDelayTimeAfterSkip;
    [SerializeField] private float introSoundFadeOutDuration;

    private float currentTime;
    private float currentMaxTime;
    private bool triggeredIcing;
    private bool triggeredTimerSound;
    private bool triggeredTimerMidAnim;
    private bool triggeredCatAnim;
    public static bool DoMovePlayer;
    public static bool TimeUp;
    public static Action StartGame, SkipIntro, CatSwipeAnim;

    private Coroutine currentTimer;

    private void Start()
    {
        SkipIntro += SkipStartDelay;
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
        if (tierTimes.Count != 0)
        {
            tierTimes.RemoveAt(0);
        }
        SfxManager.Instance.StopSFX("TimerClick");
        SfxManager.Instance.StopSFX("CatHiss");
        SfxManager.Instance.PlaySFX("CatSad");
        TierManager.SwipeCanceledAction?.Invoke(currentMaxTime - currentTime);

        //check for win condition
        if (tierTimes.Count <= 0)
        {
            StopAllCoroutines();
            TierManager.EndSequence?.Invoke(false);
            int timeBonus = Mathf.CeilToInt(currentMaxTime - currentTime) * timeBonusMultiplier;
            ScoreManager.Instance.AddScore(timeBonus, 0, transform.position);
            MusicManager.StopBGMusic?.Invoke();
            return;
        }

        float time = currentMaxTime - currentTime;
        currentMaxTime = tierTimes[0] + time;
        currentTime = 0;

        UpdateTimerUI();
        TimerUIAnimEvents.CancelAnim?.Invoke(true);

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
        yield return new WaitForSeconds(startTimerAnimTime);

        TimerUIAnimEvents.PlayTimerStart?.Invoke();

        yield return new WaitForSeconds(startDelayTime - startTimerAnimTime);

        StartDelayFinished();
    }

    /// <summary>
    /// Code that sets up and starts the game
    /// </summary>
    private void StartDelayFinished()
    {
        currentTimer = StartCoroutine(TierTimer());
        StartGame?.Invoke();
        MusicManager.StartBGMusic?.Invoke();
    }

    /// <summary>
    /// Called to skip the intro cutscene
    /// </summary>
    private void SkipStartDelay()
    {
        // Detaches listener because multiple presses shouldn't keep trying to skip        
        SkipIntro -= SkipStartDelay;

        // Only skips if cutscene has not reached the point it would skip to
        // Skipping includes:
        // Manually hiding the skip graphic early
        // Stopping regular start delay coroutine and replacing it to correct the timing
        // Canceling timer anim and making sure it becomes visible
        // Preventing the timer sound from playing
        // Fading out the opening pan sound effect
        if (introCutscene.time < startDelayTime - startDelayTimeAfterSkip)
        {
            introCutscene.time = startDelayTime - startDelayTimeAfterSkip;
            skipGraphic.SetActive(false);
            StopCoroutine(currentTimer);
            TimerUIAnimEvents.CancelAnim.Invoke(true);
            TimerUIAnimEvents.TimerVisible.Invoke(true);
            timerSound.playOnAwake = false;
            if (WinMusic.Instance != null)
            {
                WinMusic.CutOffIntro.Invoke(introSoundFadeOutDuration);
            }
            StartCoroutine(StartDelayAfterSkip());
        }
    }

    /// <summary>
    /// Waits for proper time after a skip, then starts games
    /// </summary>
    private IEnumerator StartDelayAfterSkip()
    {
        yield return new WaitForSeconds(startDelayTimeAfterSkip);
        TimerUIAnimEvents.CancelAnim.Invoke(false);
        StartDelayFinished();
    }

    /// <summary>
    /// Coroutine used for each tier
    /// </summary>
    /// <returns></returns>
    IEnumerator TierTimer()
    {
        triggeredIcing = false;
        triggeredTimerSound = false;
        triggeredTimerMidAnim = false;
        triggeredCatAnim = false;
        TimeUp = false;

        TimerUIAnimEvents.CancelAnim?.Invoke(false);

        //count until swipe shaking should start
        while (currentTime < (currentMaxTime - tierCamShakeDuration))
        {
            yield return new WaitForSeconds(0.1f);

            //check for falling icing time
            if (!triggeredIcing && currentTime >= triggerFallingIcingTime)
            {
                StartCoroutine(TriggerFallingIcing());
                triggeredIcing = true;
            }
            //check to trigger sfx
            if(!triggeredTimerSound && (currentMaxTime - currentTime) <= 18)
            {
                SfxManager.Instance.FadeInSFX("TimerClick", 10);
                triggeredTimerSound = true;
            }
            //play timer midpoint anim
            if(!triggeredTimerMidAnim && ((currentMaxTime/2) - 1) <= currentTime)
            {
                triggeredTimerMidAnim = true;
                TimerUIAnimEvents.PlayTimerMidpoint?.Invoke();
            }

            currentTime += 0.1f;
            UpdateTimerUI();
        }

        //start tier swipe sequence
        //TODO: call timeline
        TimerUIAnimEvents.PlayTimerAlarm?.Invoke();
        SfxManager.Instance.PlaySFX("CatHiss");
        TierManager.SwipeTierAction?.Invoke(tierCamShakeDuration, playerMoveAfterSwipeTransitionTime); 
        
        while (currentTime < currentMaxTime)
        {
            yield return new WaitForSeconds(0.1f);
            currentTime += 0.1f;
            UpdateTimerUI();

            if (!triggeredCatAnim && (currentMaxTime - currentTime) <= 0.2f)
            {
                triggeredCatAnim = true;
                CatSwipeAnim?.Invoke();
            }
        }

        //CatSwipeAnim?.Invoke();

        //tier is swiped
        TimeUp = true;
        currentTime = 0;
        tierTimes.RemoveAt(0);

        //checks for the last tier, evaulates end condition
        if(tierTimes.Count <= 0)
        {
            print("time ran out in the last tier!");
            TierManager.EndSequence?.Invoke(true);
            MusicManager.StopBGMusic?.Invoke();
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
        while (fallingIcing[0].Count > 0 && fallingIcing[0][0].gameObject != null)
        {
            //trigger next in order splotch, then delete
            fallingIcing[0][0].GetComponent<FallingBatter>().TriggerFall(fallingIcing.Count);
            fallingIcing[0].RemoveAt(0);

            yield return new WaitForSeconds(delayFallingIcingTime);
        }

        //remove empty list of icing
        fallingIcing.RemoveAt(0);
    }

    private void OnDisable()
    {
        TierManager.NextTierAction -= NextTier;
        if (SkipIntro != null)
        {
            SkipIntro -= SkipStartDelay;
        }
    }
}
