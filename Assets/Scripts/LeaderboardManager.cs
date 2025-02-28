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
    //List<string> names = new List<string>();

    //ID of the leaderboard
    private const string LeaderboardID = "TGSHLeaderboard";

    /// <summary>
    /// Enables the Leaderboad SDK and allows the player to submit scores anonymously
    /// </summary>
    private async void Awake()
    {
        await UnityServices.InitializeAsync(); //Waits for the initialize to
                                               //finish before continuing
        //await SignInAnonymously();
    }

    async Task SignInAnonymously()
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
        Debug.Log("Done");
    }

    public async void AddScore(int score, string name)
    {
        // await SignInAnonymously();
        await AddPlayer(name);

        var scoreResponse = await LeaderboardsService.Instance.AddPlayerScoreAsync(LeaderboardID, score);
        Debug.Log(JsonConvert.SerializeObject(scoreResponse));

        AuthenticationService.Instance.SignOut(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            int i = Random.Range(1, 11);
        string name;

        switch(i)
        {
            case 1:
                name = "AAA";
                break;
            case 2:
                name = "BBB";
                break;
            case 3:
                name = "CCC";
                break;
            case 4:
                name = "DDD";
                break;
            case 5:
                name = "EEE";
                break;
            case 6:
                name = "FFF";
                break;
            case 7:
                name = "GGG";
                break;
            case 8:
                name = "HHH";
                break;
            case 9:
                name = "III";
                break;
            case 10:
                name = "JJJ";
                break;
            default:
                name = "MORG";
                break;
        }
        //if (names.Count <= 0)
        //{
            //await AddPlayer(name);
        //}
        //else
        //{
        //    //foreach(string n in names)
        //    //{
        //    //    if(n.Equals(name))
        //    //    {
        //    //        newPlayer = false;
        //    //    }
        //    //}
        //}

        //if(newPlayer)
        //{
        //    await AddPlayer(name);
        //}
            AddScore(i * 100, name);
        }
    }

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
        Debug.Log("Done");
    }
}
