/******************************************************************
 *    Author: Marissa 
 *    Contributors: 
 *    Date Created: 9/12/24
 *    Description: Sfx manager singleton. Call to start a sound effect.
 *******************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SfxManager : Singleton<SfxManager>
{
    [SerializeField] private List<SFX> _SFXs = new List<SFX>();
    [SerializeField] private AudioMixer _masterMixer;

    protected override void Awake()
    {
        base.Awake();

        //create audio components and set fields
        for (int i = 0; i < _SFXs.Count; i++)
        {
            //need to be different game objects so sxfs can overlap and not cut off
            GameObject go = Instantiate(new GameObject(), transform);

            _SFXs[i].source = go.AddComponent<AudioSource>();
            _SFXs[i].source.outputAudioMixerGroup = _SFXs[i].mixer;
            _SFXs[i].source.volume = _SFXs[i].maxVolume;
            _SFXs[i].source.pitch = _SFXs[i].pitch;
            _SFXs[i].source.playOnAwake = false;
            _SFXs[i].source.loop = _SFXs[i].doLoop;
        }
    }

    #region playing sfx functions
    /// <summary>
    /// Plays the given sound effect. Finds the index of the specific sound effect,
    /// sets the audio clip based on the avaliable clips, and then plays the clip
    /// </summary>
    /// <param name="name"></param>
    public void PlaySFX(string name)
    {
        SFX sfx = _SFXs[_SFXs.FindIndex(i => i.name == name)];
        sfx.source.clip = sfx.clips[UnityEngine.Random.Range(0, sfx.clips.Length)];
        sfx.source.volume = sfx.maxVolume;
        sfx.source.Play();
    }

    /// <summary>
    /// Stops the given sound effect from playing. Finds the index of the specific
    /// sound effect, and then stops the clip.
    /// </summary>
    /// <param name="name"></param>
    public void StopSFX(string name)
    {
        SFX sfx = _SFXs[_SFXs.FindIndex(i => i.name == name)];
        sfx.source.Stop();
    }

    /// <summary>
    /// Fades in the given sound effect. Finds the index of the specific sound effect,
    /// sets the audio clip based on the avaliable clips, and then plays the clip
    /// </summary>
    public void FadeInSFX(string name, float duration)
    {
        SFX sfx = _SFXs[_SFXs.FindIndex(i => i.name == name)];
        sfx.source.clip = sfx.clips[UnityEngine.Random.Range(0, sfx.clips.Length)];
        sfx.source.volume = 0;
        sfx.source.Play();

        StartCoroutine(StartFade(sfx.source, sfx.maxVolume, duration));
    }

    /// <summary>
    /// Fades the given sound effect's volume to 0 over the duration.
    /// </summary>
    public void FadeOutSFX(string name, float duration)
    {
        SFX sfx = _SFXs[_SFXs.FindIndex(i => i.name == name)];

        StartCoroutine(StartFade(sfx.source, 0, duration));
    }

    /// <summary>
    /// coroutine used by the fade in and fade out functions to fade a sfx to a
    /// specific volume over the duration.
    /// </summary>
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

    #endregion

    #region mixer functions

    /// <summary>
    /// Sets the volume of the SFX mixer input 0 thru -80
    /// </summary>
    /// <param name="volume"></param>
    public void SetMixerVolume(float volume, string mixerName)
    {
        _masterMixer.SetFloat(mixerName,  Mathf.Log(volume) * 20);
    }

    #endregion

}