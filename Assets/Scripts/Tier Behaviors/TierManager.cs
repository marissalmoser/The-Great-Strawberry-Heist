/*****************************************************************************
// File Name :         Tier.cs
// Author :            Marissa Moser
// Creation Date :     01/30/2025
//
// Brief Description : Derived from Sigleton Monobehavior. Reference using TierManager.Instance
    contains functionality to control the tiers.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class TierManager : Singleton<TierManager>
{
    [Tooltip("Every tier game object should be added to this list i norder, with the" +
        "bottom tier as index 0.")]
    [SerializeField] List<GameObject> cakeTiers;
    List<Tier> tiers = new List<Tier>();

    private int currentTier = 0;    //0 is bottom tier

    [Tooltip("Edit this to change how big each shake is when a tier is swiped")]
    [SerializeField] float tierCamShakeAmplitude;
    [Tooltip("Edit this to change how often the camera shakes when a tier is swiped")]
    [SerializeField] float tierCamShakeFrequency;
    [Tooltip("Edit this to change how long the camera shakes when a tier is swiped")]
    [SerializeField] float tierCamShakeDuration;

    protected override void Awake()
    {
        base.Awake();

        //get tier behaviors from game objects and save in a list
        foreach (GameObject tier in cakeTiers)
        {
            tiers.Add(tier.GetComponent<Tier>());
        }
    }

    /// <summary>
    /// Will be reomoved after testing is sufficient
    /// </summary>
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            SwipeTier(tierCamShakeDuration);
        }
    }

    /// <summary>
    /// Called when the cat swipes the bottom cake tier
    /// </summary>
    public void SwipeTier(float duration)
    {
        StartCoroutine(SwipeCoroutine(duration));
    }

    /// <summary>
    /// Called when the payer passes into the next cake tier
    /// </summary>
    public void NextTier()
    {
        tiers[currentTier].DisableCam();
        currentTier++;
        print(currentTier);
    }

    /// <summary>
    /// Swipes the tier from the cake. Camera with shake for the duration before the
    /// sjake is performed.
    /// </summary>
    private IEnumerator SwipeCoroutine(float duration)
    {
        tiers[0].ShakeCam(tierCamShakeAmplitude, tierCamShakeFrequency, duration);

        yield return new WaitForSeconds(duration);

        //move player to next tier
        if (currentTier == 0)
        {
            //TODO: call move player somehow, get spawn transition from tier 1
            tiers[0].DisableCam();
        }
        else
        {
            currentTier--;
        }

        tiers[0].Swipe();

        cakeTiers.RemoveAt(0);
        tiers.RemoveAt(0);
    }
}
