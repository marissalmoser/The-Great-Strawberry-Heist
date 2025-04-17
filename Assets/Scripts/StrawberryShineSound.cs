/*****************************************************************************
// File Name :         StrawberryShineSound.cs
// Author :            Marissa Moser
// Creation Date :     03/25/2025
//
// Brief Description : Contains functionalityfor the strawberry shine sfx.
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrawberryShineSound : MonoBehaviour
{
    [SerializeField] private AudioSource source;
    void Start()
    {
        Invoke("ChangeStrawberryRange", 7.5f);
        source.volume = source.volume / 4;
    }

    /// <summary>
    /// Changed the 3D sound range of the audio source after the intro scene has
    /// completed.
    /// </summary>
    private void ChangeStrawberryRange()
    {
        source.maxDistance = 30;
    }

    /// <summary>
    /// Plays the soud sfx at a random pitch.
    /// </summary>
    public void PlayShine()
    {
        float randomPitch = Random.Range(0.98f, 1.02f);
        source.pitch = randomPitch;
        source.Play();
    }
}
