/******************************************************************
 *    Author: Marissa 
 *    Contributors: 
 *    Date Created: 9/12/24
 *    Description: Sfx manager singleton. Call to start a sound effect.
 *******************************************************************/
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class SfxManager : Singleton<SfxManager>
{
    [SerializeField] private List<SFX> _SFXs = new List<SFX>();
    [SerializeField] private AudioMixer _masterMixer;
    [Header("Fruit Sound Effects Lists")]
    [SerializeField] private AudioClip[] T1FruitSFXs;
    [SerializeField] private AudioClip[] T2FruitSFXs;
    [SerializeField] private AudioClip[] T3FruitSFXs;
    [SerializeField] private AudioClip[] T4FruitSFXs;
    [SerializeField] private AudioClip[] T5FruitSFXs;
    private List<AudioClip[]> AllFruitSFXs = new();

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
            _SFXs[i].source.clip = _SFXs[i].clips[0];
        }
        FruitSFXSetup();
    }

    /// <summary>
    /// Adds all the Fruit sound effect arrays to the master sound effect array.
    /// </summary>
    private void FruitSFXSetup() 
    {
        AllFruitSFXs.Add(T1FruitSFXs);
        AllFruitSFXs.Add(T2FruitSFXs);
        AllFruitSFXs.Add(T3FruitSFXs);
        AllFruitSFXs.Add(T4FruitSFXs);
        AllFruitSFXs.Add(T5FruitSFXs);
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
        sfx.source.PlayOneShot(sfx.source.clip, 1);
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

    /// <summary>
    /// Only changes the pickup sounds for the fruits when the tier is swiped. Can be expanded upon to include changing of other SFX as well
    /// </summary>
    /// <param name="SFXindex"></param>
    public void SFXTierChange(int SFXindex) 
    {
        _SFXs[0].clips = AllFruitSFXs[SFXindex];
        string debugMessage = "Fruit SFX are now set to: ";
        foreach (var sfx in _SFXs[0].clips) 
        {
            debugMessage += sfx.name + ", ";
        }
        Debug.Log(debugMessage);
    }

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