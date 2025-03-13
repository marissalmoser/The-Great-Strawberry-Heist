using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Music
{
    public string name;

    public AudioClip MusicClip;

    [Range(0f, 1f)]
    public float maxVolume;

    [Range(0f, 3f)]
    public float pitch;

    public bool doLoop = true;
}
