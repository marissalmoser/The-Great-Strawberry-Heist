/*****************************************************************************
// File Name :         Tier.cs
// Author :            Marissa Moser
// Creation Date :     01/30/2025
//
// Brief Description : Derived from Sigleton Monobehavior. Reference using TierManager.Instance
    contains functionality to control the tiers.
*****************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Events;

public class TierManager : Singleton<TierManager>
{
    [Tooltip("Every tier game object should be added to this list i norder, with the" +
        "bottom tier as index 0.")]
    [SerializeField] List<GameObject> cakeTiers;
    List<Tier> tiers = new List<Tier>();
    private Vector3 nextSpawnPt;
    private bool canSwipe;

    private int currentTier = 0;    //0 is bottom tier

    [Tooltip("Edit this to change how big each shake is when a tier is swiped")]
    [SerializeField] float tierCamShakeAmplitude;
    [Tooltip("Edit this to change how often the camera shakes when a tier is swiped")]
    [SerializeField] float tierCamShakeFrequency;

    [Tooltip("Edit this to change when the game starts")]
    [SerializeField] private float startDelay = 20f;
    public static UnityAction StartGameAction; // Static action for starting the game
    public static bool DoMovePlayer; // Controls if the player can move (default is false)

    public static Action<float, float> SwipeTierAction;
    public static Action NextTierAction;
    public static Action<float> SwipeCanceledAction;

    protected override void Awake()
    {
        base.Awake();

        StartCoroutine(StartDelay());

        //get tier behaviors from game objects and save in a list
        foreach (GameObject tier in cakeTiers)
        {
            tiers.Add(tier.GetComponent<Tier>());
        }

        SwipeTierAction += SwipeTier;
        SwipeCanceledAction += NextTier;

        canSwipe = true;
    }
    
    /// <summary>
    /// Handles the delay at the start of the game and invokes the StartGameAction.
    /// </summary>
    private IEnumerator StartDelay()
    {
        Debug.Log("Starting delay...");

        yield return new WaitForSeconds(startDelay); // Wait for the specified start delay

        Debug.Log("Start game event invoked!");
        StartGameAction?.Invoke(); // Invoke the Start Game event

        DoMovePlayer = true; 
    }

    /// <summary>
    /// function that returns true if the player is in the bottom tier.
    /// </summary>
    public bool IsInBottomTier()
    {
        return currentTier == 0;
    }

    public Vector3 GetNextSpawn()
    {
        return nextSpawnPt;
    }

    /// <summary>
    /// Called when the cat swipes the bottom cake tier. Duration is the pause while
    /// the camera shakes before the tier is swiped.
    /// </summary>
    public void SwipeTier(float shakeDuration, float playerMoveDuration)
    {
        if(!canSwipe)
        {
            return;
        }

        if(IsInBottomTier())
        {
            nextSpawnPt = tiers[1].GetTierSpawn().position;
        }
        
        StartCoroutine(SwipeCoroutine(shakeDuration));
    }

    /// <summary>
    /// Called when the payer passes into the next cake tier
    /// </summary>
    public void NextTier(float duration)
    {
        tiers[currentTier].DisableCam();
        StartCoroutine(tiers[currentTier].SwipeCanceled(duration));
        cakeTiers.RemoveAt(0);
        tiers.RemoveAt(0);

        //prevents null error
        if(tiers.Count > currentTier + 1)
        {
            nextSpawnPt = tiers[currentTier + 1].GetTierSpawn().position;
        }
    }

    /// <summary>
    /// Swipes the tier from the cake. Camera will shake for the duration before the
    /// swipe is performed.
    /// </summary>
    private IEnumerator SwipeCoroutine(float duration)
    {
        tiers[0].ShakeCam(tierCamShakeAmplitude, tierCamShakeFrequency, duration);

        TimerSystem.DoMovePlayer = true;

        yield return new WaitForSeconds(duration);

        //check if tier should actually swipe
        if (!TimerSystem.DoMovePlayer)
        {
            yield break;
        }

        if (currentTier == 0)
        {
            tiers[0].DisableCam();
        }
        else
        {
            currentTier--;
        }

        tiers[0].Swipe();

        cakeTiers.RemoveAt(0);
        tiers.RemoveAt(0);

        if (tiers.Count < 2)
        {
            canSwipe = false;
            print("last tier swiped");
        }

        ScoreManager.Instance.LayerSwipeVitalityChange();
    }

    private void OnDisable()
    {
        SwipeTierAction -= SwipeTier;
        SwipeCanceledAction -= NextTier;
    }
}
