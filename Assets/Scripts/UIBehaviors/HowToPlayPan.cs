/******************************************************************************
 * Author: Brad Dixon
 * File Name: HowToPlayPan.cs
 * Creation Date: 3/4/2025
 * Brief: Pans the camera throughout the how to play screen
 * ***************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HowToPlayPan : MonoBehaviour
{
    [SerializeField] private GameObject mainCamera;
    private Vector3 newPos;
    [SerializeField] private string gameScene;

    [Header("Panning variables")]
    [Tooltip("The panning speed")]
    [SerializeField] private float panSpeed;

    [Tooltip("How far down it goes before loading the scene")]
    [SerializeField] private float minYValue;

    private void Start()
    {
        newPos = mainCamera.transform.position;
        newPos.y = minYValue;
    }

    private void Update()
    {
        float move = panSpeed * Time.deltaTime;

        mainCamera.transform.position = Vector3.MoveTowards(mainCamera.transform.position, newPos, move);

        if(mainCamera.transform.position.y <= minYValue)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(gameScene);
        }
    }
}
