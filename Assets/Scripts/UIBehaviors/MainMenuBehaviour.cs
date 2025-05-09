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
    [SerializeField] private GameObject creditButton;
    [SerializeField] private GameObject backButton;

    bool hasSelected = false;

    /// <summary>
    /// Enables action map
    /// </summary>
    private void Awake()
    {
        actionMap = GetComponent<PlayerInput>().currentActionMap;

        actionMap.Enable();
        select = actionMap.FindAction("Select");
        select.started += Select_started;
    }

    /// <summary>
    /// Starts the game
    /// </summary>
    /// <param name="obj"></param>
    private void Select_started(InputAction.CallbackContext obj)
    {
        if(!hasSelected)
        {
            hasSelected = true;
            SfxManager.Instance.PlaySFX("Menuing");
            TransitionManager.Instance.WhiteboardIn(nextScene);
        }
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
