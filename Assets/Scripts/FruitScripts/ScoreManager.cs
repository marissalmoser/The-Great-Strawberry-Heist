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
using UnityEngine.SceneManagement;

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
    private float Vitalitymeter = 0;
    private bool isInStarMode;

    public static int highScore;

    [SerializeField]
    [Tooltip("Maximum Vitality Meter")]
    private int maxVitalityMeter = 100;
    [SerializeField]
    [Tooltip("The Mutiplier will change every ___ vitality points")]
    private int vitalityBreakpointInterval = 30;
    [SerializeField]
    [Tooltip("How much vitality points decrease on player being hit")]
    private int vitalityDecreasePoints = 10;
    [SerializeField]
    [Tooltip("How long star mode lasts and how many seconds for the vitality bar to deplete until its empty")]
    private float starModeDuration = 5;

    // Multiplier that can be changed in the Inspector
    [SerializeField]
    private float multiplier = 1f;
    private PlayerBehaviour player;
    private bool multiplierHit;

    private int recentlyAddedScore;

    [SerializeField]
    private GameObject textScorePrefab;

    public int RecentlyAddedScore { get => recentlyAddedScore; private set => recentlyAddedScore = value; }
    public bool IsInStarMode { get => isInStarMode; private set => isInStarMode = value; }

    public void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>();
        ScoreText.text = "Score: " + Totalscore.ToString();
        highScore = 0;
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

            //plays sound if multiplier went up, loss sound if it went down
            if (!multiplierHit)
            {
                switch (newMultiplier)
                {
                    case 1.25f:
                        SfxManager.Instance.PlaySFX("Multiplier1.25");
                        break;
                    case 1.5f:
                        SfxManager.Instance.PlaySFX("Multiplier1.5");
                        break;
                    case 2f:
                        SfxManager.Instance.PlaySFX("Multiplier2.0");
                        break;
                }
            }
            else
            {
                SfxManager.Instance.PlaySFX("MultiplierLoss");
            }
            multiplierHit = false;
        }
        multiplier = newMultiplier;
    }

    /// <summary>
    /// Adds to total score when a fruit is collected
    /// </summary>
    public void AddScore(int scoreAmt, int vitalityAmt, Vector3 spawnPosition)
    {
        recentlyAddedScore = Mathf.RoundToInt(scoreAmt * multiplier);
        Totalscore += recentlyAddedScore;
        highScore = Totalscore;

        ScoreText.text = "Score: " + Totalscore.ToString();

        var textObj = Instantiate(textScorePrefab, spawnPosition, Quaternion.identity).GetComponent<TextRise>();
        textObj.SetRisingText("+" + recentlyAddedScore);

        if (!isInStarMode) 
        {
            Vitalitymeter = Mathf.Min(vitalityAmt + Vitalitymeter, maxVitalityMeter);
            ChangeVitality();
        }
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

        //When the player is in star mode, don't allow additional fruit pickups to add to vitality or change the multiplier (since it needs to be stuck at 2x briefly)

        ChangeMultiplier(breakpoints[breakpointMultiplierIndex]);
        //--Vitality UI change--
        BarFillImage.fillAmount = Vitalitymeter / maxVitalityMeter;
        MultiplierText.text = multiplier + "x";


        //When Vitality meter is full/has reached the max vitality amount, start star mode
        if (Vitalitymeter >= maxVitalityMeter)
        {
            StartCoroutine(ActivateStarMode());
        }
    }

    [ContextMenu("Player Hit")]
    /// <summary>
    /// public method that can be called which decreases the player's multiplier by the set amount (vitalityDecreasePoints) in the inspector. 
    /// </summary>
    public void PlayerHit()
    {
        Vitalitymeter = Mathf.Max(Vitalitymeter - vitalityDecreasePoints, 0);
        multiplierHit = true;
        ChangeVitality();
    }


    [ContextMenu("Layer Swipe")]
    /// <summary>
    /// public method thats called when the layer is swiped and vitality is reset
    /// </summary>
    public void LayerSwipeVitalityChange() 
    {
        if (!isInStarMode) 
        {
            Vitalitymeter = 0;
            ChangeVitality();
        }
    }
    /// <summary>
    /// Runs an Enumerator that procedurally decreases Vitality meter and then resets the player's vitality to zero
    /// </summary>
    /// <returns></returns>
    private IEnumerator ActivateStarMode() 
    {
        //Activation Logic
        isInStarMode = true;
        player.StartStarMode();
        StarModeVisualChange();
        yield return null;

        //Timer Logic
        float elapsedTime = 0;
        while (elapsedTime < starModeDuration) 
        {
            Vitalitymeter -= Time.deltaTime * (maxVitalityMeter / starModeDuration);
            BarFillImage.fillAmount = Vitalitymeter / maxVitalityMeter;
            yield return null;
            elapsedTime += Time.deltaTime;
        }

        //De-Activation Logic
        //Reused this method because it resets the Vitality to 0 and updates UI already
        EndStarMode();
        LayerSwipeVitalityChange();
    }
    /// <summary>
    /// Ends star mode
    /// </summary>
    public void EndStarMode()
    {
        player.StopStarMode();
        isInStarMode = false;
        Debug.Log("☆STAR MODE FINISHED!");
    }
    /// <summary>
    /// TODO: Activates any IMMEDIATE visual/UI changes such as the vitality bar changing sprites/color or any other visual changes
    /// </summary>
    private void StarModeVisualChange() 
    {
        //can be removed later
        Debug.Log("★STAR MODE ACTIVATED!");
    }

    [ContextMenu("Activate Star Mode")]
    public void DebugStarMode() 
    {
        StartCoroutine(ActivateStarMode());
    }
}
