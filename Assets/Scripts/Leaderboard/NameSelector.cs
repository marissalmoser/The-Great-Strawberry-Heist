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
    InputActionMap actionMap;
    InputAction navigate, select;

    bool navigating = false;

    int nameIndex, charIndex;
    char[] letters = new char[] {'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J',
    'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'};

    [SerializeField] private float navDelay;
    [SerializeField] private List<TMP_Text> playerLetters = new List<TMP_Text>();

    private void Awake()
    {
        actionMap = GetComponent<PlayerInput>().currentActionMap;
        actionMap.Enable();

        navigate = actionMap.FindAction("Navigate");
        select = actionMap.FindAction("Select");

        navigate.performed += Navigate_performed;
        select.started += Select_started;

        nameIndex = 0; charIndex = 0;

        ClearName();

        DisplayName();
    }

    private void Select_started(InputAction.CallbackContext obj)
    {
        //LeaderboardManager.Instance.UpdateName(letters[charIndex]);
        //++nameIndex;
        if(nameIndex < 3)
        {
            LeaderboardManager.Instance.UpdateName(letters[charIndex]);
            ++nameIndex;
            //nameIndex = 0;
            //int i = Random.Range(1, 11);
            //LeaderboardManager.Instance.AddScore(i * 100);
            //ClearName();
            DisplayName();
            if(nameIndex == 3)
            {
                int i = Random.Range(1, 11);
                LeaderboardManager.Instance.AddScore(i * 100);
            }
        }
        //DisplayName();
    }

    private void Navigate_performed(InputAction.CallbackContext obj)
    {
        if (!navigating)
        {
            navigating = true;
            StartCoroutine(FindLetter());
        }
    }

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

    private void ClearName()
    {
        foreach(TMP_Text i in playerLetters)
        {
            i.text = "";
        }
    }

    private void DisplayName()
    {
        if (nameIndex < 3)
        {
            playerLetters[nameIndex].text = letters[charIndex].ToString();
        }
    }

    private void OnDisable()
    {
        actionMap.Disable();
        navigate.performed -= Navigate_performed;
        select.started -= Select_started;
    }
}
