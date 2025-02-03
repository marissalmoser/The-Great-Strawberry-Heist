/*****************************************************************************
// File Name :         Tier.cs
// Author :            Marissa Moser
// Creation Date :     01/30/2025
//
// Brief Description : Derived from Sigleton Monobehavior. Reference using TierManager.Instance
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class TierManager : Singleton<TierManager>
{
    [SerializeField] List<GameObject> cakeTiers;
    List<Tier> tiers = new List<Tier>();
    private int currentTier = 0;    //0 is bottom tier

    protected override void Awake()
    {
        base.Awake();

        //get tier behaviors from game objects and save in a list
        foreach (GameObject tier in cakeTiers)
        {
            tiers.Add(tier.GetComponent<Tier>());
        }

        //starting tier count?
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            SwipeTier();
        }
    }

    /// <summary>
    /// Called when the cat swipes the bottom cake tier
    /// </summary>
    public void SwipeTier()
    {
        //move player to next tier
        if(currentTier == 0)
        {
            //TODO: call move player somehow
            tiers[0].DiableCam();

        }
        else
        {
            currentTier--;
        }

        tiers[0].Swipe();

        cakeTiers.RemoveAt(0);
        tiers.RemoveAt(0);

        //TODO: Camera shake
    }

    /// <summary>
    /// Called when the payer passes into the next cake tier
    /// </summary>
    public void NextTier()
    {
        tiers[currentTier].DiableCam();
        currentTier++;
        print(currentTier);
    }
}
