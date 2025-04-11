/*****************************************************************************
// File Name :         TimerUIAnimEvents.cs
// Author :            Marissa Moser
// Creation Date :     03/13/2025
//
// Brief Description : Manages the animations of the timer UI element. Invoke static
    actions to call anims from timer script.
*****************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerUIAnimEvents : MonoBehaviour
{
    Animator anim;
    public static Action PlayTimerStart, PlayTimerMidpoint, PlayTimerAlarm;
    public static Action<bool> CancelAnim;

    void Start()
    {
        anim = GetComponent<Animator>();

        //set images to hidden
        Image[] sr = GetComponentsInChildren<Image>();
        foreach (Image image in sr)
        {
            image.enabled = false;
        }

        PlayTimerStart += TimerStart;
        PlayTimerMidpoint += TimerMidpoint;
        PlayTimerAlarm += TimerAlarm;
        CancelAnim += StopAnims;
    }

    /// <summary>
    /// Returns Timer to Idle Anim state
    /// </summary>
    private void StopAnims(bool input)
    {
        anim.SetBool("Cancel", input);
    }

    /// <summary>
    /// Calls timer fade in animation
    /// </summary>
    private void TimerStart()
    {
        anim.SetTrigger("Start");

        //set images to visible
        Image[] sr = GetComponentsInChildren<Image>();
        foreach (Image image in sr)
        {
            image.enabled = true;
        }
    }

    /// <summary>
    /// Calls timer midpoint animation
    /// </summary>
    private void TimerMidpoint()
    {
        anim.SetTrigger("Midpoint");
    }

    /// <summary>
    /// Calls timer alarm animation
    /// </summary>
    private void TimerAlarm()
    {
        anim.SetTrigger("Alarm");
    }

    private void OnDisable()
    {
        PlayTimerStart -= TimerStart;
        PlayTimerMidpoint -= TimerMidpoint;
        PlayTimerAlarm -= TimerAlarm;
        CancelAnim -= StopAnims;
    }
}
