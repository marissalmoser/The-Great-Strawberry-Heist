using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FruitCollect : MonoBehaviour
{
    // This variable can be modified in the Inspector
    public int score;

    // Multiplier that can be changed in the Inspector
    [SerializeField]
    private float multiplier = 1f;

    // Static Action for adding score
    public static Action<int> OnAddScore;

    void Awake()
    {
        // Check if the method is already subscribed
        if (OnAddScore == null || !OnAddScore.GetInvocationList().Contains((Delegate)(Action<int>)AddScore))
        {
            OnAddScore += AddScore;
        }
        Debug.Log("Awake called");
    }

    void OnDestroy()
    {
        // Unsubscribe from the OnAddScore action when the object is destroyed
        OnAddScore -= AddScore;
    }

    // Static method to change the score multiplier
    // Static method to change the score multiplier
    public void ChangeMultiplier(float newMultiplier)
    {
        multiplier = newMultiplier;
        Debug.Log("Multiplier changed to: " + multiplier);
    }

    // Method to add score with the multiplier
    private void AddScore(int amount)
    {
        score += Mathf.RoundToInt(amount * multiplier);
        Debug.Log("Updated Score: " + score);
        Debug.Log("Amount: " + amount);
        Debug.Log("Multiplier: " + multiplier);
    }

    //Method to collect fruit 

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            if (!(OnAddScore is null))
                OnAddScore(score);
           
            Destroy(gameObject);
        }
    }




}
