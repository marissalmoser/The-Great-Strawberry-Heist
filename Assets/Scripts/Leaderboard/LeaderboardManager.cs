using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Leaderboards;
using Unity.Services.Authentication;
using Unity.Services.Core;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance;

    string playerName = "";

    //ID of the leaderboard
    private const string LeaderboardID = "TGSHLeaderboard";

    /// <summary>
    /// Enables the Leaderboad SDK and allows the player to submit scores anonymously
    /// </summary>
    private async void Awake()
    {
        await UnityServices.InitializeAsync(); //Waits for the initialize to
                                               //finish before continuing
        NameSelector.good = true;

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    /// <summary>
    /// Adds a letter to the character's name
    /// </summary>
    /// <param name="letter"></param>
    public void UpdateName(char letter)
    {
        playerName += letter;
    }

    public async void AddScore(int score)
    {
        // await SignInAnonymously();
        await AddPlayer(playerName);

        if(ScoreManager.highScore > 0)
        {
            score = ScoreManager.highScore;
        }

        var scoreResponse = await LeaderboardsService.Instance.AddPlayerScoreAsync(LeaderboardID, score);
        //Debug.Log(JsonConvert.SerializeObject(scoreResponse));

        AuthenticationService.Instance.SignOut(true);

        playerName = "";

        //UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        StartCoroutine(TransitionManager.Instance.FadeOut(0.25f, false, "MainMenu"));
    }

    async Task AddPlayer(string name)
    {
        AuthenticationService.Instance.SignedIn += () =>
        {
           // Debug.Log("Signed in as: " + AuthenticationService.Instance.PlayerId);
        };
        AuthenticationService.Instance.SignInFailed += s =>
        {
            // Take some action here...
           // Debug.Log(s);
        };

        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        catch
        {
            Debug.Log("Player is already signed in");
        }
        await AuthenticationService.Instance.UpdatePlayerNameAsync(name);
        Debug.Log("Done");
    }

    public async void GetScores()
    {
        var scoresResponse = await LeaderboardsService.Instance.GetScoresAsync(
            LeaderboardID,
            new GetScoresOptions { Limit = 5 } //Limits to top 5 scores
        );
      //  Debug.Log(JsonConvert.SerializeObject(scoresResponse));
    }
}
