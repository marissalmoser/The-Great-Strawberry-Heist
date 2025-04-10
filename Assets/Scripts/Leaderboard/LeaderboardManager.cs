using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Leaderboards;
using Unity.Services.Authentication;
using Unity.Services.Core;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models;
using Unity.Services.CloudSave.Models.Data.Player;
using SaveOptions = Unity.Services.CloudSave.Models.Data.Player.SaveOptions;

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance;

    string playerName = "";

    // Used to keep track of the most recent score
    private int scoreIndex;

    //ID of the leaderboard
    private const string LeaderboardID = "TGSHLeaderboard";

    private int score;

    /// <summary>
    /// Enables the authentication service to sign into the unity cloud.
    /// </summary>
    private async void Awake()
    {
        await UnityServices.InitializeAsync(); //Waits for the initialize to
                                               //finish before continuing

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        await GetRecentIndex();
        AuthenticationService.Instance.SignOut(true);
    }

    /// <summary>
    /// Adds a letter to the character's name
    /// </summary>
    /// <param name="letter"></param>
    public void UpdateName(char letter)
    {
        playerName += letter;
    }

    /// <summary>
    /// Adds a score to the leaderboard
    /// </summary>
    /// <param name="score"></param>
    public async void AddScore(int score)
    {
        // await SignInAnonymously();
        await AddPlayer(playerName);

        if(ScoreManager.highScore > 0)
        {
            score = ScoreManager.highScore;
        }

        this.score = score;

        var scoreResponse = await LeaderboardsService.Instance.AddPlayerScoreAsync(LeaderboardID, score);
        Debug.Log(JsonConvert.SerializeObject(scoreResponse));

        await SaveRecentScore();

        AuthenticationService.Instance.SignOut(true);

        playerName = "";

        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Signs into the unity cloud and adds the player's name
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    async Task AddPlayer(string name)
    {
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in as: " + AuthenticationService.Instance.PlayerId);
        };
        AuthenticationService.Instance.SignInFailed += s =>
        {
            // Take some action here...
            Debug.Log(s);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        await AuthenticationService.Instance.UpdatePlayerNameAsync(name);
    }

    /// <summary>
    /// Saves the index of the most recent score
    /// </summary>
    public async Task SaveRecentIndex()
    {
        await GetRecentIndex();

        var data = new Dictionary<string, object> { { "recentIndex", scoreIndex } };
        await CloudSaveService.Instance.Data.Player.SaveAsync(data, new SaveOptions(new PublicWriteAccessClassOptions()));
        ++scoreIndex;
    }

    public async Task SaveRecentScore()
    {
        await SaveRecentIndex();

        var data = new Dictionary<string, object> { { scoreIndex.ToString(), playerName + score.ToString() } };
        await CloudSaveService.Instance.Data.Player.SaveAsync(data, new SaveOptions(new PublicWriteAccessClassOptions()));

        Debug.Log("Reached!!");
    }


    public async Task GetRecentIndex()
    {
        var playerData = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { "recentIndex" });
        if (playerData.TryGetValue("recentIndex", out var keyName))
        {
            scoreIndex = keyName.Value.GetAs<int>();
            Debug.Log(scoreIndex);
        }
        else
        {
            scoreIndex = 0;
            Debug.Log("No current index so starting at 0");
        }
    }
}
