using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinMusic : MonoBehaviour
{
    public static WinMusic Instance;

    [SerializeField] float fadeOutDuration = 0.75f;
    float winMusicVol;

    [SerializeField] AudioSource WinTriggerSFX;
    [SerializeField] AudioSource IntroSecquenceSFX;
    [SerializeField] AudioSource WinMusicLoop;

    //fades out bg music and starts win music loop
    public static Action TriggerWinMusic;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            StartSetup();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void StartSetup()
    {
        SceneManager.sceneLoaded += SwitchMusic;
        TriggerWinMusic += StartWinMusic;
        winMusicVol = WinMusicLoop.volume;
        WinMusicLoop.Play();
    }


    /// <summary>
    /// Coroutine to fade in or out the provided track based on the target volume parameter and timed by the fadeDuration
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartFade(AudioSource audioSource, float targetVolume, float duration)
    {
        float currentTime = 0;
        float start = audioSource.volume;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }

        if (targetVolume <= 0)
        {
            audioSource.Stop(); //doesnt work cus its pass by value parem
            audioSource.volume = start;
        }

        yield break;
    }

    private void SwitchMusic(Scene scene, LoadSceneMode arg1)
    {
        if (scene.name == "GameScene")
        {
            //fade out music
            StartCoroutine(StartFade(WinMusicLoop, 0, fadeOutDuration));
            Invoke("StopLoopingMusic", fadeOutDuration + 1);

            //start intro track
            IntroSecquenceSFX.Play();
        }

        if(scene.name == "HowToPlay")
        {
            StartCoroutine(StartFade(WinMusicLoop, 0, 1));
        }

        if (scene.name == "MainMenu" && !WinMusicLoop.isPlaying)
        {
            WinMusicLoop.volume = winMusicVol;
            WinMusicLoop.Play();
        }
    }

    private void StartWinMusic()
    {
        StartCoroutine(WinMusicTriggerAndLoop());
    }

    private IEnumerator WinMusicTriggerAndLoop()
    {
        WinMusicLoop.volume = winMusicVol;
        WinTriggerSFX.volume = winMusicVol;

        WinTriggerSFX.Play();

        yield return new WaitForSeconds(WinTriggerSFX.clip.length - 0.3f);

        WinMusicLoop.Play();
    }

    private void StopLoopingMusic()
    {
        WinMusicLoop.Stop();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= SwitchMusic;
        TriggerWinMusic -= StartWinMusic;
    }
}
