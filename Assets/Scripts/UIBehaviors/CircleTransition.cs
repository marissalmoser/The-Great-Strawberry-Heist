using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CircleTransition : MonoBehaviour
{
    [SerializeField] private TransitionManager tm;

    public void FadeOutMainMenu()
    {
        tm.FadeOut(0.25f, true);
    }

    public void DisableWhiteboard()
    {
        tm.WhiteboardOut();
    }

    public void LoadScene()
    {
        tm.LoadScene();
    }
}
