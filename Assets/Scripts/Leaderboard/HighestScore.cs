/******************************************************************************
 * Author: Brad Dixon
 * File Name: HighestScore.cs
 * Creation Date: 4/5/2025
 * Brief: Displays the highest leaderboard score on the main menu
 * ***************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Leaderboards;
using Unity.Services.Authentication;
using Unity.Services.Core;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TMPro;

public class HighestScore : MonoBehaviour
{
    private const string LeaderboardID = "TGSHLeaderboard";

    [SerializeField] TMP_Text highestScore;
    [SerializeField] TMP_Text noHighestScore;

    /// <summary>
    /// Enables the leaderboard and authentication for the cloud data
    /// </summary>
    private async void Awake()
    {
        highestScore.gameObject.SetActive(true);
        noHighestScore.gameObject.SetActive(false);

        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        InvokeRepeating("DisplayHighestScore", 0, 1);
    }

    /// <summary>
    /// Displays the highest score of the leaderboard
    /// </summary>
    private async void DisplayHighestScore()
    {
        var scoresResponse = await LeaderboardsService.Instance.GetScoresAsync(
            LeaderboardID,
            new GetScoresOptions { Limit = 1 } //Limits to top 5 scores
        );

        Debug.Log(JsonConvert.SerializeObject(scoresResponse));

        string s = JsonConvert.SerializeObject(scoresResponse);

        if (s.IndexOf("playerName") != -1)
        {
            int scoreStart = s.IndexOf("score") + 7;
            string test = s.Substring(scoreStart, 6);
            if (test.Contains("."))
            {
                test = test.Substring(0, test.IndexOf("."));
            }
            highestScore.text = test;
            try
            {
                highestScore.gameObject.SetActive(true);
                noHighestScore.gameObject.SetActive(false);
            } catch { }
        }
        else
        {
            try
            {
                highestScore.gameObject.SetActive(false);
                noHighestScore.gameObject.SetActive(true);
            }
            catch { }
        }
    }

    /// <summary>
    /// Signs the player out
    /// </summary>
    private void OnDisable()
    {
        AuthenticationService.Instance.SignOut(true);
    }
}
