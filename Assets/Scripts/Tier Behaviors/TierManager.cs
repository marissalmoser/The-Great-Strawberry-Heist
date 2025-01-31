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

public class TierManager : Singleton<TierManager>
{
    [SerializeField] List<GameObject> cakeTiers;
    List<Tier> tiers = new List<Tier>();
    private int currentTier;    //0 is bottom tier

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

    void Start()
    {
        
    }

    /// <summary>
    /// Called when the cat swipes the bottom cake tier
    /// </summary>
    public void SwipeTier()
    {
        //update current tier
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
