/*****************************************************************************
// File Name :         Tier.cs
// Author :            Marissa Moser
// Creation Date :     01/30/2025
//
// Brief Description : 
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class Tier : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera tierCam;

    void Awake()
    {
        
    }

    public void DiableCam()
    {
        tierCam.gameObject.SetActive(false);
    }
}
