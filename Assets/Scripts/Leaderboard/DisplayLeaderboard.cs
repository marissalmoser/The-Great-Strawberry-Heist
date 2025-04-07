using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Leaderboards;
using Unity.Services.Authentication;
using Unity.Services.Core;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TMPro;
using System.IO;

public class DisplayLeaderboard : MonoBehaviour
{
    private const string LeaderboardID = "TGSHLeaderboard";

    [SerializeField] List<TMP_Text> names = new List<TMP_Text>();
    [SerializeField] List<TMP_Text> scores = new List<TMP_Text>();

    private string filePath;

    private async void Awake()
    {
        await UnityServices.InitializeAsync();

        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        filePath = Application.dataPath + "/Blacklist.txt";

        InvokeRepeating("GetScores", 0, 5);
    }

    public async void GetScores()
    {
        if (LeaderboardsService.Instance != null)
        {
            for (int i = 0; i < 5; ++i)
            {
                var scoresResponse = await LeaderboardsService.Instance.GetScoresAsync(
                    LeaderboardID,
                    new GetScoresOptions { Offset = i, Limit = i + 1 } //Limits to top 5 scores
                );
                Debug.Log(JsonConvert.SerializeObject(scoresResponse));

                string s = JsonConvert.SerializeObject(scoresResponse);

                //Debug.Log(s.IndexOf("playerName"));
                if (s.IndexOf("playerName") != -1)
                {
                    int nameStart = s.IndexOf("playerName") + 13;
                    names[i].text = s.Substring(nameStart, 3);
                    if (NeedsCensored(s.Substring(nameStart, 3)))
                    {
                        names[i].text = "???";
                    }
                    else
                    {
                        names[i].text = s.Substring(nameStart, 3);
                    }

                    int scoreStart = s.IndexOf("score") + 7;
                    // scores[i].text = s.Substring(scoreStart, (s.IndexOf("}]}") - scoreStart - 2));
                    string test = s.Substring(scoreStart, 6);
                    if (test.Contains("."))
                    {
                        test = test.Substring(0, test.IndexOf("."));
                    }
                    scores[i].text = test;
                    //string score = s.Substring(scoreStart, (s.IndexOf("}]}") - scoreStart - 2));

                    //Debug.Log(int.Parse(score));
                }
                else
                {
                    names[i].text = "";
                    scores[i].text = "";
                }
            }
        }
    }

    /// <summary>
    /// Checks if the name is one of the blacklisted names
    /// </summary>
    /// <param name="playerName"></param>
    /// <returns></returns>
    private bool NeedsCensored(string playerName)
    {
        string[] badNames = File.ReadAllLines(filePath);

        foreach(string i in badNames)
        {
            if(playerName.Equals(i))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Signs out the player when the scene is left
    /// </summary>
    private void OnDisable()
    {
        AuthenticationService.Instance.SignOut(true);
    }
}
