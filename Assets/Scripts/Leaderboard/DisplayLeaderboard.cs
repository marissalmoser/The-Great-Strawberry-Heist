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
using UnityEngine.InputSystem;

public class DisplayLeaderboard : MonoBehaviour
{
    private const string LeaderboardID = "TGSHLeaderboard";

    [SerializeField] List<TMP_Text> names = new List<TMP_Text>();
    [SerializeField] List<TMP_Text> scores = new List<TMP_Text>();
    [SerializeField] List<TMP_Text> nums = new List<TMP_Text>();

    [SerializeField] float timeBeforeSwap;

    InputActionMap actionMap;
    InputAction scroll;
    //private bool top5;

    private string filePath;

    private int scrollIndex; //Used to keep track of how far up or down the leaderboard is
    private bool canScroll;

    private async void Awake()
    {
        actionMap = GetComponent<PlayerInput>().currentActionMap;
        actionMap.Enable();
        scroll = actionMap.FindAction("Scroll");
        scroll.performed += Scroll_performed;

        canScroll = true;

        await UnityServices.InitializeAsync();

        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        filePath = Application.dataPath + "/Blacklist.txt";

       // top5 = true;
        scrollIndex = 0;

        InvokeRepeating("GetScores", 0, 1);
        //InvokeRepeating("SwapBetween", timeBeforeSwap, timeBeforeSwap);
    }

    private void Scroll_performed(InputAction.CallbackContext obj)
    {
        if (canScroll)
        {
            float scrollValue = scroll.ReadValue<float>();

            if (scrollValue > 0 && scrollIndex > 0)
            {
                --scrollIndex;
            }
            else if (scrollValue < 0 && scrollIndex < 5)
            {
                ++scrollIndex;
            }

            canScroll = false;

            StartCoroutine(SlightDelay());
        }
    }

    /// <summary>
    /// Switches between 1-5 and 6-10
    /// </summary>
    //private void SwapBetween()
    //{
    //    LeaderboardAnimController lac = FindObjectOfType<LeaderboardAnimController>();
    //    lac.StartAnim();
    //    StartCoroutine(SlightDelay());
    //}

    IEnumerator SlightDelay()
    {
        yield return new WaitForSeconds(.02f);
        canScroll = true;
    }

    /// <summary>
    /// Displays the scores
    /// </summary>
    public async void GetScores()
    {
        if (LeaderboardsService.Instance != null)
        {
            //if (top5)
            //{
                for (int i = 0; i < + 5; ++i)
                {
                    var scoresResponse = await LeaderboardsService.Instance.GetScoresAsync(
                        LeaderboardID,
                        new GetScoresOptions { Offset = scrollIndex + i, Limit = scrollIndex + i + 1 } //Limits to top 5 scores
                    );
                //Debug.Log(JsonConvert.SerializeObject(scoresResponse));

                    nums[i].text = (scrollIndex + i + 1).ToString();
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
                //}
            }
            //else
            //{
            //    for (int i = 5; i < 10; ++i)
            //    {
            //        var scoresResponse = await LeaderboardsService.Instance.GetScoresAsync(
            //            LeaderboardID,
            //            new GetScoresOptions { Offset = i, Limit = i + 1 } //Limits to top 5 scores
            //        );
            //        //Debug.Log(JsonConvert.SerializeObject(scoresResponse));

            //        string s = JsonConvert.SerializeObject(scoresResponse);

            //        //Debug.Log(s.IndexOf("playerName"));
            //        if (s.IndexOf("playerName") != -1)
            //        {
            //            int nameStart = s.IndexOf("playerName") + 13;
            //            names[i - 5].text = s.Substring(nameStart, 3);
            //            if (NeedsCensored(s.Substring(nameStart, 3)))
            //            {
            //                names[i - 5].text = "???";
            //            }
            //            else
            //            {
            //                names[i - 5].text = s.Substring(nameStart, 3);
            //            }

            //            int scoreStart = s.IndexOf("score") + 7;
            //            // scores[i].text = s.Substring(scoreStart, (s.IndexOf("}]}") - scoreStart - 2));
            //            string test = s.Substring(scoreStart, 6);
            //            if (test.Contains("."))
            //            {
            //                test = test.Substring(0, test.IndexOf("."));
            //            }
            //            scores[i - 5].text = test;
            //            //string score = s.Substring(scoreStart, (s.IndexOf("}]}") - scoreStart - 2));

            //            //Debug.Log(int.Parse(score));
            //        }
            //        else
            //        {
            //            names[i - 5].text = "";
            //            scores[i - 5].text = "";
            //        }
            //    }
            //}
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
        actionMap.Disable();
        scroll.performed -= Scroll_performed;
    }
}
