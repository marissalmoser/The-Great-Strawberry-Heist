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
using TMPro;
using UnityEngine;

public class FruitCollect : MonoBehaviour
{
    // This variable can be modified in the Inspector
    [SerializeField]
    private int score;
    [SerializeField]
    private int vitality;
    [SerializeField]
    private GameObject textScorePrefab;



    ///<summary>
    /// Method to collect fruit 
    ///<summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
      
        if (collision.TryGetComponent(out PlayerBehaviour pb))
        {
            ScoreManager.Instance.AddScore(score, vitality);
            SfxManager.Instance.PlaySFX("FruitPickup");
            var textObj = Instantiate(textScorePrefab, transform.position, Quaternion.identity).GetComponent<TextRise>();
            textObj.SetRisingText("+" + ScoreManager.Instance.RecentlyAddedScore);
            Destroy(gameObject);
        }
    }




}
