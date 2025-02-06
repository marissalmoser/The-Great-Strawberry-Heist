/*****************************************************************************
// File Name :         FruitCollect.cs
// Author :            Kadin Harris
// Creation Date :     01/30/2025
//
// Brief Description : Updates the score and multiplier for each fruit collected.
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class ScoreManager : Singleton<ScoreManager>
{
    public TMP_Text ScoreText;
    
    //The total score integer
    private int Totalscore;
    
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
    public void AddScore(int amount)
    {
        Totalscore += Mathf.RoundToInt(amount * multiplier);
        Debug.Log("Updated Score: " + Totalscore);
        Debug.Log("Amount: " + amount);
        Debug.Log("Multiplier: " + multiplier);

        ScoreText.text = "Score: " + Totalscore.ToString();
    }
}
