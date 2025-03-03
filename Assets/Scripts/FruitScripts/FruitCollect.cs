/*****************************************************************************
// File Name :         FruitCollect.cs
// Author :            Kadin Harris
// Creation Date :     01/30/2025
//
// Brief Description : Added to fruit to add how much score said fruit is worth 
Derived from Sigleton Monobehavior. Reference using ScoreManage.Instance
*****************************************************************************/


using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FruitCollect : MonoBehaviour
{
    // This variable can be modified in the Inspector
    [SerializeField]
    private int score;



    ///<summary>
    /// Method to collect fruit 
    ///<summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
      
        if (collision.TryGetComponent(out PlayerBehaviour pb))
        {
            ScoreManager.Instance.AddScore(score);
            SfxManager.Instance.PlaySFX("FruitPickup");
            Destroy(gameObject);
        }
    }




}
