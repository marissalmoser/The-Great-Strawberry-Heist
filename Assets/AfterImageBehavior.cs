using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImageBehavior : MonoBehaviour
{
    SpriteRenderer sr;
    [SerializeField] private float fadeTime = 2f;

    private float startingAlpha;
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        startingAlpha = sr.color.a;
    }
    public GameObject Setup(Sprite afterImageSprite, Color afterImageColor) 
    {
        sr.sprite = afterImageSprite;
        afterImageColor.a = startingAlpha;
        sr.color = afterImageColor;
        StartCoroutine(DecreaseOpacity());
        return gameObject;
    }
    /// <summary>
    /// Sets the alpha of the spriteRenderer, reducing it all the way down to 0
    /// </summary>
    /// <param name="alpha"></param>
    public void SetAlpha(float alpha) 
    {
        Color color = sr.color;
        color.a = Mathf.Max(0, color.a - alpha);
        sr.color = color;
    }
    /// <summary>
    /// Coroutine to decrease opacity of the sprite over a period of time specified by fadeTime
    /// </summary>
    /// <returns></returns>
    IEnumerator DecreaseOpacity() 
    {
        float elapsedTime = 0;
        float decrements = startingAlpha / fadeTime;
        while (elapsedTime < fadeTime) 
        {
            SetAlpha(Time.deltaTime * decrements); 
            yield return null;
            elapsedTime += Time.deltaTime;
        }
        yield return null;
        Destroy(gameObject);
        
    }
}
