using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinMusic : Singleton<WinMusic>
{
    bool isDestroying;
    [SerializeField] float fadeOutDuration = 0.75f;
    float winMusicVol;

    [SerializeField] AudioSource WinTriggerSFX;
    [SerializeField] AudioSource IntroSecquenceSFX;
    [SerializeField] AudioSource WinMusicLoop;

    //fades out bg music and starts win music loop
    public static Action TriggerWinMusic;

    protected override void Awake()
    {
        base.Awake();
        SceneManager.sceneLoaded += StopMusic;
        TriggerWinMusic += StartWinMusic;
        winMusicVol = WinMusicLoop.volume;
        WinMusicLoop.Play();

        if (Instance == this)
        {
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
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
            audioSource.Stop();
        }

        yield break;
    }

    private void StopMusic(Scene scene, LoadSceneMode arg1)
    {
        if (!isDestroying && scene.name == "GameScene")
        {
            isDestroying = true;

            //fade out music
            StartCoroutine(StartFade(WinMusicLoop, 0, fadeOutDuration));
            Invoke("StopLoopingMusic", fadeOutDuration + 1);

            //start intro track
            IntroSecquenceSFX.Play();
        }

        if(scene.name == "HowToPlay")
        {
            StartCoroutine(StartFade(WinMusicLoop, winMusicVol * 0.75f, 2));
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

        yield return new WaitForSeconds(WinTriggerSFX.clip.length - 0.1f);

        print(WinTriggerSFX.clip.length - 0.1f);

        WinMusicLoop.Play();
    }

    private void StopLoopingMusic()
    {
        WinMusicLoop.Stop();
    }
}
