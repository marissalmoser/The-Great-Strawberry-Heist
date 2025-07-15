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
    private InputAction pause;

    bool isPaused;
    public static Action<bool> PauseGame;

    private void Awake()
    {
        MenuActions = GetComponent<PlayerInput>().currentActionMap;
        MenuActions.Enable();

        pause = MenuActions.FindAction("PauseGame");
        //quit = MenuActions.FindAction("Quit");
        //restart = MenuActions.FindAction("Restart");

        pause.performed += TogglePause;
        //restart.performed += RestartGame;
        //quit.performed += ConfirmQuit;

        //quit.Disable();
        //restart.Disable();
    }

    /// <summary>
    /// toggles the pause menu on and off. Invokes an action when the pause menu is
    /// toggled, and passes if it is being turned on or off.
    /// </summary>
    private void TogglePause(InputAction.CallbackContext context)
    {
        TogglePauseFunctionality();
    }

    public void TogglePauseFunctionality()
    {
        isPaused = !isPaused;
        PauseGame?.Invoke(isPaused);

        //pausing game
        if (isPaused)
        {
            pauseCanvas.SetActive(true);
            //restart.Enable();
            //quit.Enable();
            SfxManager.Instance.PauseAllSFX();
            Time.timeScale = 0;
        }
        //unpausing game
        else
        {
            //restart.Disable();
            //quit.Disable();
            Time.timeScale = 1;
            SfxManager.Instance.ResumeAllSFX();
            pauseCanvas.SetActive(false);
        }
    }

    /// <summary>
    /// Goes back to the main menu
    /// </summary>
    /// <param name="context"></param>
    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("GameScene");
    }

    /// <summary>
    /// Quits the game application;
    /// </summary>
    /// <param name="context"></param>
    public void ConfirmQuit()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
        //Application.Quit();
    }
    /// <summary>
    /// Loads the HowToPlay scene 
    /// </summary>
    public void GoToHowToPlay() 
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("HowToPlay");
    }

    public bool IsPaused()
    {
        return isPaused;
    }

    private void OnDisable()
    {
        pause.performed -= TogglePause;
        //restart.performed -= RestartGame;
        //quit.performed -= ConfirmQuit;
    }
}
