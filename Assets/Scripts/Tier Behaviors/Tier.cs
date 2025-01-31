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
    [SerializeField] private GameObject tierSpawnPt;

    void Awake()
    {
        
    }

    public void DiableCam()
    {
        tierCam.gameObject.SetActive(false);
    }

    Transform GetTierSpawn()
    {
        return tierSpawnPt.transform;
    }
}
