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

    [SerializeField] private string gameScene;
    [SerializeField] private GameObject creditButton;
    [SerializeField] private GameObject backButton;

    private void Awake()
    {
        actionMap = GetComponent<PlayerInput>().currentActionMap;

        actionMap.Enable();
    }

    public void PressStart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(gameScene);
    }

    public void PressCredits()
    {
        //Sets the event system to select the back button
        EventSystem.current.SetSelectedGameObject(backButton);
    }

    public void PressBack()
    {
        //Sets the event system to select the credit button
        EventSystem.current.SetSelectedGameObject(creditButton);
    }

    public void PressQuit()
    {
        if(EditorApplication.isPlaying)
        {
            EditorApplication.isPlaying = false;
        }
        else
        {
            Application.Quit();
        }
    }

    private void OnDisable()
    {
        actionMap.Disable();
    }
}
