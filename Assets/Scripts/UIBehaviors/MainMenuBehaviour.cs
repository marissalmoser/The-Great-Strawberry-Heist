/******************************************************************************
 * Author: Brad Dixon
 * File Name: MainMenuBehaviour.cs
 * Creation Date: 3/4/2025
 * Brief: Button controls for the main menu
 * ***************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuBehaviour : MonoBehaviour
{
    InputActionMap actionMap;
    InputAction select;

    [SerializeField] private string nextScene;
    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject htpButton;
    [SerializeField] private GameObject leaderButton;
    [SerializeField] private GameObject creditButton;
    [SerializeField] private bool pressAnyForMainMenuFade;
    [SerializeField] private bool pressAnyForMainMenuWhiteboard;

    bool hasSelected = false;
    public static int lastPosition = 0;

    /// <summary>
    /// Enables action map
    /// </summary>
    private void Awake()
    {
        actionMap = GetComponent<PlayerInput>().currentActionMap;

        actionMap.Enable();
        select = actionMap.FindAction("Select");
        select.started += Select_started;

        // Set cursor position to where it last was upon entering scene
        if (startButton != null)
        {
            switch (lastPosition)
            {
                case 0:
                    EventSystem.current.SetSelectedGameObject(startButton); break;
                case 1:
                    EventSystem.current.SetSelectedGameObject(htpButton); break;
                case 2:
                    EventSystem.current.SetSelectedGameObject(leaderButton); break;
                case 3:
                    EventSystem.current.SetSelectedGameObject(creditButton); break;
            }
        }
    }

    /// <summary>
    /// Returns to main menu if bool is set, ignored otherwise
    /// </summary>
    /// <param name="obj"></param>
    private void Select_started(InputAction.CallbackContext obj)
    {
        if (!hasSelected)
        {
            if (pressAnyForMainMenuWhiteboard) // used for credits
            {
                SfxManager.Instance.PlaySFX("Menuing");
                TransitionManager.Instance.CutOutWhiteboard("MainMenu");
            }
            else if (pressAnyForMainMenuFade) // used for leaderboard
            {
                SfxManager.Instance.PlaySFX("Menuing");
                StartCoroutine(TransitionManager.Instance.FadeOut(0.25f, false, "MainMenu"));
            }
            else
            {
                return;
            }
            hasSelected = true;
        }
    }

    public void PressStart()
    {
        if (!hasSelected)
        {
            lastPosition = 0;
            hasSelected = true;
            SfxManager.Instance.PlaySFX("Menuing");
            TransitionManager.Instance.StopAllCoroutines();
            StartCoroutine(TransitionManager.Instance.FadeOut(0.25f, false, "GameScene"));
        }
    }

    public void PressHowToPlay()
    {
        if (!hasSelected)
        {
            lastPosition = 1;
            hasSelected = true;
            SfxManager.Instance.PlaySFX("Menuing");
            TransitionManager.Instance.StopAllCoroutines();
            TransitionManager.Instance.WhiteboardIn("HowToPlay");
        }
    }

    public void PressLeaderboard()
    {
        if (!hasSelected)
        {
            lastPosition = 2;
            hasSelected = true;
            SfxManager.Instance.PlaySFX("Menuing");
            TransitionManager.Instance.StopAllCoroutines();
            StartCoroutine(TransitionManager.Instance.FadeOut(0.25f, false, "HighScoreLeaderboard"));
        }
    }

    public void PressCredits()
    {
        if (!hasSelected)
        {
            lastPosition = 3;
            hasSelected = true;
            SfxManager.Instance.PlaySFX("Menuing");
            TransitionManager.Instance.StopAllCoroutines();
            TransitionManager.Instance.WhiteboardIn("Credits");
        }
    }

    /// <summary>
    /// Quits out of the game
    /// </summary>
    public void PressQuit()
    {
        Application.Quit();
    }

    /// <summary>
    /// Disables the action map
    /// </summary>
    private void OnDisable()
    {
        actionMap.Disable();
        select.started -= Select_started;
    }
}
