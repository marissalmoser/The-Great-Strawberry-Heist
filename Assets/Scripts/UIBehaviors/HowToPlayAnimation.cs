/******************************************************************************
 * File Name: HowToPlayAnimation.cs
 * Author: Adam Blumhardt
 * Creation Date: 3/9/2025
 * Brief Description: Handles the animations in the how to play scene
 * ***************************************************************************/
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class HowToPlayAnimation : MonoBehaviour
{
    [SerializeField] private string _gameScene;
    [SerializeField] private Animator _animator;
    private enum State { Undefined, TutorialFadeIn, OnTutorial, Controls1FadeIn, OnControls1, ControlsFadeOut,
    Controls2FadeIn, OnControls2 }
    private State state;
    private AsyncOperation asyncOperation;

    private InputActionMap actionMap;
    private InputAction jump;

    private bool canClick;

    private void Start()
    {
        actionMap = GetComponent<PlayerInput>().currentActionMap;
        jump = actionMap.FindAction("Jump");

        jump.started += Jump_started;
        state = State.TutorialFadeIn;

        // Loading game scene in the background because, might as well
        //asyncOperation = SceneManager.LoadSceneAsync(_gameScene);
        //asyncOperation.allowSceneActivation = false;
    }

    /// <summary>
    /// Continues scene progression when you press button after animation has finished
    /// </summary>
    /// <param name="obj"></param>
    private void Jump_started(InputAction.CallbackContext obj)
    {
        if (state == State.OnTutorial)
        {
            _animator.SetBool("clicked", true);
            state = State.Controls1FadeIn;
        }
        if (state == State.OnControls1)
        {
            _animator.SetBool("clicked", false);
            state = State.Controls2FadeIn;
        }
        if (state == State.OnControls2)
        {
            _animator.SetBool("clicked", true);
            state = State.ControlsFadeOut;
        }
    }

    /// <summary>
    /// Uses hamster input map so that all valid jump buttons work to continue
    /// Continues scene progression when you press button after animation has finished
    /// </summary>
    //private void OnJump(InputAction.CallbackContext obj)
    //{
    //    if (state == State.OnTutorial)
    //    {
    //        _animator.SetBool("clicked", true);
    //        state = State.Controls1FadeIn;
    //    }
    //    if (state == State.OnControls1)
    //    {
    //        _animator.SetBool("clicked", true);
    //        state = State.Controls2FadeIn;
    //    }
    //    if (state == State.OnControls2)
    //    {
    //        _animator.SetBool("clicked", true);
    //        state = State.ControlsFadeOut;
    //    }

    //    _animator.SetBool("clicked", false);
    //}

    /// <summary>
    /// Called by animation events at the end of the animations
    /// </summary>
    public void AnimationFinished()
    {
        if (state == State.TutorialFadeIn)
        {
            state = State.OnTutorial;
        }
        if (state == State.Controls1FadeIn)
        {
            state = State.OnControls1;
        }
        if(state == State.Controls2FadeIn)
        {
            state = State.OnControls2;
        }
        if (state == State.ControlsFadeOut)
        {
            TransitionManager.Instance.CutOutWhiteboard(_gameScene);
            //asyncOperation.allowSceneActivation = true;
        }
    }

    /// <summary>
    /// Disables the action map
    /// </summary>
    private void OnDisable()
    {
        actionMap.Disable();
        jump.started -= Jump_started;
    }
}
