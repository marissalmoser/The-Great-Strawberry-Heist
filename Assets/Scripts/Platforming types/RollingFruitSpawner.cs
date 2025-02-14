/*****************************************************************************
 * Author: Marissa Moser
 * Creation Date: 2/7/2025
 * File Name: RollingFruitSpawner.cs
 * Brief: Spawns in rolling fruit after a random range of timeat the game object's
 *      position
 * ***************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingFruitSpawner : MonoBehaviour
{
    [Tooltip("The specific fruit prefab you want to spawn at this point, with the animator set up")]
    [SerializeField] private GameObject RollingFruitPrefab;

    [Tooltip("Minimum time for another fruit to spawn")]
    [SerializeField] private int minWaitTime;
    [Tooltip("Maximum time for another fruit to spawn")]
    [SerializeField] private int maxWaitTime;

    bool isSpawning;

    void Start()
    {
        isSpawning = true;
        StartCoroutine(RollingFruit() );
    }

    IEnumerator RollingFruit()
    {
        while( isSpawning )
        {
            Instantiate(RollingFruitPrefab);

            yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime + 1));
        }
    }
}
