using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CircleTransition : MonoBehaviour
{
    [SerializeField] private TransitionManager tm;

    public void LoadScene()
    {
        tm.LoadScene();
    }
}
