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


    private void TogglePause(InputAction.CallbackContext context)
    {
        isPaused = !isPaused;
        PauseGame?.Invoke(isPaused);
        print("pause game");
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

    private void RestartGame(InputAction.CallbackContext context)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void ConfirmQuit(InputAction.CallbackContext context)
    {
        Time.timeScale = 1;
        print("quit game");
        Application.Quit();
    }

    private void OnDisable()
    {
        pause.performed -= TogglePause;
        restart.performed -= RestartGame;
        quit.performed -= ConfirmQuit;
    }
}
