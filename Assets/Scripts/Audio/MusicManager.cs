/******************************************************************
 *    Author: Dalsten 
 *    Contributors: 
 *    Date Created: 3/3/25
 *    Description: Music manager singleton. Call to start a tier's music.
 *******************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MusicManager : Singleton<MusicManager>
{

    [Tooltip("(Don't assign to this list) Keeps track of all of the AudioSource objects")]
    [SerializeField] private List<AudioSource> _BGMs = new();
    [SerializeField] [Range(0,1)] private float maxVolume = 0.45f;

    [Header("Time Variables/Settings")]
    [Tooltip("How much the volume change for fading in/out should change by. 0 Means it changes instantly ")]
    [SerializeField] private float timeIncrements = 100f;
    [Tooltip("Duration in seconds for the length of time that the new track fades in for")]
    [SerializeField] private float fadeInDuration = 3.0f;
    [Tooltip("Duration in seconds for the length of time that the old track fades out for")]
    [SerializeField] private float fadeOutDuration = 1.5f;
    private int musicIndex = 0;
    public static Action StartBGMusic;
    protected override void Awake()
    {
        base.Awake();
        _BGMs = GetComponentsInChildren<AudioSource>().ToList<AudioSource>();
        StartBGMusic += StartMusic;
        WinMusic.TriggerWinMusic += FadeOutTrack;
    }
    [ContextMenu("Increase Tier and Switch Soundtracks")]
    /// <summary>
    /// Method to change the currently playing track to the next Tier's track. Can be called from the script's context menu for debugging purposes
    /// </summary>
    public void PlayNextTrack() 
    {
        print("PLAY NEXT TRACK");
        StopAllCoroutines();
        FadeOutTrack();
        musicIndex = (musicIndex + 1) % _BGMs.Count;
        SfxManager.Instance.SFXTierChange(musicIndex);
        FadeInTrack();
    }
    [ContextMenu("Fade Current Track Out")]
    /// <summary>
    /// Method to fade a provided track's volume out.
    /// </summary>
    private void FadeOutTrack() 
    {
        StartCoroutine(FadeTrack(fadeOutDuration, _BGMs[musicIndex], 0));
    }

    [ContextMenu("Fade Current Track In")]
    /// <summary>
    /// Method to fade a provided track's volume in.
    /// </summary>
    private void FadeInTrack()
    {
        StartCoroutine(FadeTrack(fadeInDuration, _BGMs[musicIndex], maxVolume));
    }
    /// <summary>
    /// Coroutine to fade in or out the provided track based on the target volume parameter and timed by the fadeDuration
    /// </summary>
    /// <param name="fadeDuration"></param>
    /// <param name="currentTrack"></param>
    /// <param name="targetVolume"></param>
    /// <returns></returns>
    private IEnumerator FadeTrack(float fadeDuration, AudioSource currentTrack, float targetVolume) 
    {
        float incrementalWaitDuration = fadeDuration / timeIncrements;
        float incrementalVolumeChange = 
            (currentTrack.volume <= 0 ? targetVolume : -currentTrack.volume) / timeIncrements;
        //Debug.Log("Volume Change: " + incrementalVolumeChange);
        //Debug.Log("Wait Duration: " + incrementalWaitDuration);

        for (int i = 0; i < timeIncrements; i++) 
        {
            currentTrack.volume += incrementalVolumeChange;
            yield return new WaitForSeconds(incrementalWaitDuration);
        }
        currentTrack.volume = targetVolume;
    }

    private void StartMusic()
    {
        foreach(var track in _BGMs)
        {
            track.Play();
        }

        _BGMs[0].volume = maxVolume;
    }
}
