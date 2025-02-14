/*****************************************************************************
// File Name :         Tier.cs
// Author :            Marissa Moser
// Creation Date :     01/30/2025
// Brief Description : The script on the trapdooe object to detext when the player 
    moves to the next tier, and invkokes that action
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class Trapdoor : MonoBehaviour
{
    [SerializeField] private GameObject door;

    private void OnTriggerExit2D(Collider2D collision)
    {
        //can use pb to reference player behavior, to move them to the right position?
        if(collision.TryGetComponent(out PlayerBehaviour pb))
        {
            TierManager.NextTierAction?.Invoke();
            door.SetActive(true);   //delay this?
            Destroy(this);
        }
    }
}
