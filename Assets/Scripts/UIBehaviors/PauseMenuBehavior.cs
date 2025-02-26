/*****************************************************************************
// File Name :         PauseMenuBehavior.cs
// Author :            Marissa Moser
// Creation Date :     02/26/2025
//
// Brief Description : Manages the pause menu, and quit and restart inputs.
*****************************************************************************/
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenuBehavior : MonoBehaviour
{
    [SerializeField] private GameObject pauseCanvas;

    private InputActionMap MenuActions;
    private InputAction pause, quit, restart;

    bool isPaused;
    public static Action<bool> PauseGame;

    private void Awake()
    {
        MenuActions = GetComponent<PlayerInput>().currentActionMap;
        MenuActions.Enable();

        pause = MenuActions.FindAction("PauseGame");
        quit = MenuActions.FindAction("Quit");
        restart = MenuActions.FindAction("Restart");

        pause.performed += TogglePause;
        restart.performed += RestartGame;
        quit.performed += ConfirmQuit;

        quit.Disable();
        restart.Disable();
    }

    /// <summary>
    /// toggles the pause menu on and off. Invokes an action when the pause menu is
    /// toggled, and passes if it is being turned on or off.
    /// </summary>
    private void TogglePause(InputAction.CallbackContext context)
    {
        isPaused = !isPaused;
        PauseGame?.Invoke(isPaused);

        //pausing game
        if(isPaused)
        {
            pauseCanvas.SetActive(true);
            restart.Enable();
            quit.Enable();
            Time.timeScale = 0;
        }
        //unpausing game
        else
        {
            restart.Disable();
            quit.Disable();
            Time.timeScale = 1;
            pauseCanvas.SetActive(false);
        }
    }

    /// <summary>
    /// Restarts the game scene and sets the time scale back to 1.
    /// </summary>
    /// <param name="context"></param>
    private void RestartGame(InputAction.CallbackContext context)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Quits the game application;
    /// </summary>
    /// <param name="context"></param>
    private void ConfirmQuit(InputAction.CallbackContext context)
    {
        Time.timeScale = 1;
        Application.Quit();
    }

    private void OnDisable()
    {
        pause.performed -= TogglePause;
        restart.performed -= RestartGame;
        quit.performed -= ConfirmQuit;
    }
}
