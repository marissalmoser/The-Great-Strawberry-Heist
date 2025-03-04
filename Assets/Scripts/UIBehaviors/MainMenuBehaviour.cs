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

public class MainMenuBehaviour : MonoBehaviour
{
    InputActionMap actionMap;

    [SerializeField] private string gameScene;

    private void Awake()
    {
        actionMap = GetComponent<PlayerInput>().currentActionMap;

        actionMap.Enable();
    }

    public void PressStart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(gameScene);
    }

    public void PressQuit()
    {
        if(UnityEditor.EditorApplication.isPlaying)
        {
            UnityEditor.EditorApplication.isPlaying = false;
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
