/******************************************************************
 *    Author: Marissa 
 *    Contributors: 
 *    Date Created: 9/12/24
 *    Description: Sxf class with fields to customize each sfx
 *******************************************************************/
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

[System.Serializable]
public class SFX
{
    public string name;

    public AudioClip[] clips;

    [HideInInspector]
    public AudioClip clip;

    public AudioMixerGroup mixer;

    [Range(0f, 1f)]
    public float maxVolume;

    [Range(0f, 3f)]
    public float pitch;

    public bool doLoop;

    [HideInInspector]
    public AudioSource source;
}