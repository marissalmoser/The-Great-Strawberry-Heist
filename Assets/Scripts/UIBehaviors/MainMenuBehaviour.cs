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

public class MainMenuBehaviour : MonoBehaviour
{
    InputActionMap actionMap;

    [SerializeField] private string nextScene;
    [SerializeField] private GameObject creditButton;
    [SerializeField] private GameObject backButton;

    /// <summary>
    /// Enables action map
    /// </summary>
    private void Awake()
    {
        actionMap = GetComponent<PlayerInput>().currentActionMap;

        actionMap.Enable();
    }

    /// <summary>
    /// Starts the game
    /// </summary>
    public void PressStart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(nextScene);
    }

    /// <summary>
    /// Selects the back button
    /// </summary>
    public void PressCredits()
    {
        //Sets the event system to select the back button
        EventSystem.current.SetSelectedGameObject(backButton);
    }

    /// <summary>
    /// Re-selects the credits button
    /// </summary>
    public void PressBack()
    {
        //Sets the event system to select the credit button
        EventSystem.current.SetSelectedGameObject(creditButton);
    }

    /// <summary>
    /// Quits out of the game
    /// </summary>
    public void PressQuit()
    {
        //Quits out of the editor instead
        if(EditorApplication.isPlaying)
        {
            EditorApplication.isPlaying = false;
        }
        else
        {
            Application.Quit();
        }
    }

    /// <summary>
    /// Disables the action map
    /// </summary>
    private void OnDisable()
    {
        actionMap.Disable();
    }
}
