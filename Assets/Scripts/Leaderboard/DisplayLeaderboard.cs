using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Leaderboards;
using Unity.Services.Authentication;
using Unity.Services.Core;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class DisplayLeaderboard : MonoBehaviour
{
    private const string LeaderboardID = "TGSHLeaderboard";

    private async void Awake()
    {
        await UnityServices.InitializeAsync();

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            GetScores();
        }
    }

    public async void GetScores()
    {
        var scoresResponse = await LeaderboardsService.Instance.GetScoresAsync(
            LeaderboardID,
            new GetScoresOptions { Limit = 5 } //Limits to top 5 scores
        );
        Debug.Log(JsonConvert.SerializeObject(scoresResponse));
    }
}
