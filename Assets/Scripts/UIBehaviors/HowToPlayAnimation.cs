/******************************************************************************
 * File Name: HowToPlayAnimation.cs
 * Author: Adam Blumhardt
 * Creation Date: 3/9/2025
 * Brief Description: Handles the animations in the how to play scene
 * ***************************************************************************/
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HowToPlayAnimation : MonoBehaviour
{
    [SerializeField] private string _gameScene;
    [SerializeField] private Animator _animator;
    private enum State { Undefined, TutorialFadeIn, OnTutorial, ControlsFadeIn, OnControls }
    private State state;
    private AsyncOperation asyncOperation;

    private void Start()
    {
        state = State.TutorialFadeIn;

        // Loading game scene in the background because, might as well
        asyncOperation = SceneManager.LoadSceneAsync(_gameScene);
        asyncOperation.allowSceneActivation = false;
    }

    /// <summary>
    /// Uses hamster input map so that all valid jump buttons work to continue
    /// Continues scene progression when you press button after animation has finished
    /// </summary>
    private void OnJump()
    {
        if (state == State.OnTutorial)
        {
            _animator.Play("ControlsFadeIn");
            state = State.ControlsFadeIn;
        }
        if (state == State.OnControls)
        {
            asyncOperation.allowSceneActivation = true;
        }
    }

    /// <summary>
    /// Called by animation events at the end of the animations
    /// </summary>
    public void AnimationFinished()
    {
        if (state == State.TutorialFadeIn)
        {
            state = State.OnTutorial;
        }
        if (state == State.ControlsFadeIn)
        {
            state = State.OnControls;
        }
    }
}
