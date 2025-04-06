/******************************************************************************
 * Author: Brad Dixon
 * Creation Date: 3/13/2025
 * File Name: NameSelector.cs
 * Brief: Lets player submit their name to the leaderboard
 * ***************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class NameSelector : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    InputActionMap actionMap;
    InputAction navigate, select;

    bool navigating = false;

    int nameIndex, charIndex;
    char[] letters = new char[] {'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J',
    'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'};

    [SerializeField] private float navDelay;
    [SerializeField] private List<TMP_Text> playerLetters = new List<TMP_Text>();

    [SerializeField] Color startingColor = Color.white;
    [SerializeField] Color newColor = Color.black;

    /// <summary>
    /// Enables player input
    /// </summary>
    private void Awake()
    {
        actionMap = GetComponent<PlayerInput>().currentActionMap;
        actionMap.Enable();

        navigate = actionMap.FindAction("Navigate");
        select = actionMap.FindAction("Select");

        navigate.performed += Navigate_performed;
        select.started += Select_started;

        nameIndex = 0; charIndex = 0;

        scoreText.text = ScoreManager.highScore.ToString();

        ClearName();

        DisplayName();

        StartCoroutine(FlashLetter());
    }

    /// <summary>
    /// Player input for selecting a letter
    /// </summary>
    /// <param name="obj"></param>
    private void Select_started(InputAction.CallbackContext obj)
    {
        if(nameIndex < 3)
        {
            LeaderboardManager.Instance.UpdateName(letters[charIndex]);

            playerLetters[nameIndex].faceColor = Color.white;
            StopAllCoroutines();

            ++nameIndex;

            DisplayName();
            StartCoroutine(FlashLetter());

            if (nameIndex == 3)
            {
                int i = Random.Range(1, 11);
                LeaderboardManager.Instance.AddScore(i * 100);
            }
        }
    }

    /// <summary>
    /// Player input for switching through the letters
    /// </summary>
    /// <param name="obj"></param>
    private void Navigate_performed(InputAction.CallbackContext obj)
    {
        if (!navigating)
        {
            navigating = true;
            StartCoroutine(FindLetter());
        }
    }

    /// <summary>
    /// Has the letter flash colors so it's easier to tell what the current letter is
    /// </summary>
    /// <returns></returns>
    IEnumerator FlashLetter()
    {
        int i = nameIndex;

        Debug.Log("i = " + i);

        bool doWait = false; ;

        while (i == nameIndex)
        {
            float duration = 1;
            float elapsedTime = 0f;
            yield return new WaitForSeconds(0.03f);
            elapsedTime = 0f;
            while (elapsedTime <= duration)
            {
                playerLetters[i].faceColor = Color.Lerp(startingColor, newColor, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            Color c = startingColor;
            startingColor = newColor;
            newColor = c;

            if(doWait)
                yield return new WaitForSeconds(0.3f);

            doWait = !doWait;
        }

    }

    /// <summary>
    /// Creates a delay between navigating through letters so it's harder to skip past an intended letter
    /// </summary>
    /// <returns></returns>
    IEnumerator FindLetter()
    {
        while(navigate.ReadValue<float>() != 0)
        {
            if(navigate.ReadValue<float>() > 0)
            {
                --charIndex;
                if(charIndex < 0)
                {
                    charIndex = letters.Length - 1;
                }
            }
            else if(navigate.ReadValue<float>() < 0)
            {
                ++charIndex;
                if (charIndex >= letters.Length)
                {
                    charIndex = 0;
                }
            }

            DisplayName();

            yield return new WaitForSeconds(navDelay);
        }
        navigating = false;
    }

    /// <summary>
    /// Clears the name
    /// </summary>
    private void ClearName()
    {
        foreach(TMP_Text i in playerLetters)
        {
            i.text = "";
        }
    }

    /// <summary>
    /// Displays the letters for a person's name
    /// </summary>
    private void DisplayName()
    {
        if (nameIndex < 3)
        {
            playerLetters[nameIndex].text = letters[charIndex].ToString();
        }
    }

    /// <summary>
    /// Disables the player input
    /// </summary>
    private void OnDisable()
    {
        actionMap.Disable();
        navigate.performed -= Navigate_performed;
        select.started -= Select_started;
    }
}
