using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class tempHTP : MonoBehaviour
{
    [SerializeField] private GameObject screen;
    void Start()
    {

    }

    private void OnJump()
    {
        if(screen.activeInHierarchy)
        {
            SceneManager.LoadScene(2);
        }
        else
        {
            screen.SetActive(true);
        }
    }
}
