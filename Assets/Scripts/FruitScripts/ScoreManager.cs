/*****************************************************************************
// File Name :         FruitCollect.cs
// Author :            Kadin Harris
// Contributors:       Dalsten Yan
// Creation Date :     01/30/2025
//
// Brief Description : Updates the score and multiplier for each fruit collected.
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class ScoreManager : Singleton<ScoreManager>
{
    public TMP_Text ScoreText, MultiplierText;
    [Tooltip("The Fill Image from the multiplier bar")]
    public Image BarFillImage;

    [SerializeField]
    [Tooltip("The Multiplier magnitudes")]
    List<float> breakpoints;
    
    //The total score integer
    private int Totalscore;

    //total vitality amount
    private int Vitalitymeter = 0;

    [SerializeField]
    [Tooltip("Maximum Vitality Meter")]
    private int maxVitalityMeter = 100;
    [SerializeField]
    [Tooltip("The Mutiplier will change every ___ vitality points")]
    private int vitalityBreakpointInterval = 30;
    [SerializeField]
    [Tooltip("How much vitality points decrease on player being hit")]
    private int vitalityDecreasePoints = 10;
    
    // Multiplier that can be changed in the Inspector
    [SerializeField]
    private float multiplier = 1f;
    private PlayerBehaviour player;

    private int recentlyAddedScore;

    public int RecentlyAddedScore { get => recentlyAddedScore; private set => recentlyAddedScore = value; }

    public void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>();
        ScoreText.text = "Score: " + Totalscore.ToString();
    }
   
    /// <summary>
    /// Static method to change the score multiplier
    /// </summary>
    public void ChangeMultiplier(float newMultiplier)
    {
        //if the multiplier changed at all, display the new multiplier at the player's recent location
        if (newMultiplier != multiplier) 
        {
            player.DisplayMultiplierChange(newMultiplier);
        }
        multiplier = newMultiplier;
    }

    /// <summary>
    /// Adds to total score when a fruit is collected
    /// </summary>
    public void AddScore(int scoreAmt, int vitalityAmt)
    {
        recentlyAddedScore = Mathf.RoundToInt(scoreAmt * multiplier);
        Totalscore += recentlyAddedScore;
        Vitalitymeter = Mathf.Min(vitalityAmt + Vitalitymeter, maxVitalityMeter);

        ScoreText.text = "Score: " + Totalscore.ToString();
        ChangeVitality();
    }


    /// <summary>
    /// UI and Logical code for changing Vitality
    /// </summary>
    private void ChangeVitality() 
    {
        //--Vitality Logic Change--
        //The the floor int function allows the multiplier to be chosen based on the interval of the breakpoint
        int breakpointMultiplierIndex = Mathf.FloorToInt(Vitalitymeter / vitalityBreakpointInterval);

        //The min function ensures that the index is constrained to the max indices of the breakpoints, so even if the max cap of vitality is raised, it only ever stops at the lastmost multiplier
        breakpointMultiplierIndex = Mathf.Min(breakpoints.Count - 1 , breakpointMultiplierIndex);

        ChangeMultiplier(breakpoints[breakpointMultiplierIndex]);

        //--Vitality UI change--
        BarFillImage.fillAmount = (float)Vitalitymeter / maxVitalityMeter;
        MultiplierText.text = multiplier + "x";
    }

    [ContextMenu("Player Hit")]
    /// <summary>
    /// public method that can be called which decreases the player's multiplier by the set amount (vitalityDecreasePoints) in the inspector. 
    /// </summary>
    public void PlayerHit()
    {
        Vitalitymeter = Mathf.Max(Vitalitymeter - vitalityDecreasePoints, 0);
        ChangeVitality();
    }


    [ContextMenu("Layer Swipe")]
    /// <summary>
    /// public method thats called when the layer is swiped and vitality is reset
    /// </summary>
    public void LayerSwipeVitalityChange() 
    {
        Vitalitymeter = 0;
        ChangeVitality();
    }
}
