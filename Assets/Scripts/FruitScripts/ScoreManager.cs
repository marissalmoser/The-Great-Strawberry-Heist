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

    public void Start()
    {
        ScoreText.text = "Score: " + Totalscore.ToString();
    }
   
    /// <summary>
    /// Static method to change the score multiplier
    /// </summary>
    public void ChangeMultiplier(float newMultiplier)
    {
        multiplier = newMultiplier;
        Debug.Log("Multiplier changed to: " + multiplier);
    }

    /// <summary>
    /// Adds to total score when a fruit is collected
    /// </summary>
    public void AddScore(int scoreAmt, int vitalityAmt)
    {
        Totalscore += Mathf.RoundToInt(scoreAmt * multiplier);
        Vitalitymeter = Mathf.Max(vitalityAmt + Vitalitymeter, maxVitalityMeter);
        //Debug.Log("Updated Score: " + Totalscore);
        //Debug.Log("Amount: " + amount
        //Debug.Log("Vitality Meter: " + Vitalitymeter + " | Vitality Progress: " + vitalityProgress);

        ScoreText.text = "Score: " + Totalscore.ToString();
        ChangeVitality();
    }

    private void ChangeVitality() 
    {
        //Vitality Logic Change
        int breakpointMultiplierIndex = Mathf.FloorToInt(Vitalitymeter / vitalityBreakpointInterval);
        ChangeMultiplier(breakpoints[breakpointMultiplierIndex]);

        //Vitality UI change
        BarFillImage.fillAmount = (float)Vitalitymeter / maxVitalityMeter;
        MultiplierText.text = multiplier + "x";
    }

    [ContextMenu("Player Hit")]
    public void PlayerHit() 
    {
        Vitalitymeter = Mathf.Max(Vitalitymeter - vitalityDecreasePoints, 0);
        ChangeVitality();
    }
}
