/*****************************************************************************
// File Name :         PlayerAnimFunctions.cs
// Author :            Marissa Moser
// Creation Date :     01/30/2025
// Brief Description : Functionality for some anim events to help declutter the 
    player behavior script.
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimFunctions : MonoBehaviour
{
    bool isGrowing;
    bool isShrinking;

    public void TriggerScaleInc()
    {
        if(isGrowing)
        {
            return;
        }

        isGrowing = true;
        StopAllCoroutines();
        StartCoroutine(ChangeScale(new Vector3(8, 8, 8), 1.0f));
    }

    public void TriggerScaleDec()
    {
        if (isShrinking)
        {
            return;
        }

        isShrinking = true;
        StopAllCoroutines();
        StartCoroutine(ChangeScale(new Vector3(5, 5, 5), 0.5f));
    }

    public IEnumerator ChangeScale(Vector3 targetScale, float duration)
    {
        Vector3 initialScale = transform.localScale; 
        float timeElapsed = 0f; 

        // Gradually change the scale over time
        while (timeElapsed < duration)
        {
            transform.localScale = Vector3.Lerp(initialScale, targetScale, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;

        isGrowing = false;
        isShrinking = false;
    }

}
