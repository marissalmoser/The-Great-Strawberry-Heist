/******************************************************************************
 * Author: Brad Dixon
 * File Name: LeaderboardAnimController.cs
 * Creation Date: 4/16/2025
 * Brief: Controls the animation events for the whiteboard
 * ****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardAnimController : MonoBehaviour
{
    private bool first;
    [SerializeField] private GameObject oneToFive, sixToTen;
    [SerializeField] Animator animator;
    [SerializeField] Canvas canvas;

    private void Start()
    {
        animator.SetBool("wipe", false);
        first = true;
        oneToFive.SetActive(true);
        sixToTen.SetActive(false);
        canvas.gameObject.SetActive(true);
    }

    public void SwitchScreens()
    {
        Debug.Log("reached");
        canvas.gameObject.SetActive(true);
        animator.SetBool("wipe", false);
        first = !first;
        oneToFive.SetActive(first);
        sixToTen.SetActive(!first);
    }

    public void StartAnim()
    {
        //oneToFive.SetActive(false);
        //sixToTen.SetActive(false);
        animator.SetBool("wipe", true);
    }

    public void DisableScreens()
    {
        canvas.gameObject.SetActive(false);
        oneToFive.SetActive(false);
        sixToTen.SetActive(false);
    }
}
