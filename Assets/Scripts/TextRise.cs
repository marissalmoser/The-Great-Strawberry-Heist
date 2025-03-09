/******************************************************************
 *    Author: Adam Blumhardt 
 *    Modified by: Dalsten Yan 
 *    Date Created: 3/7/25
 *    Description: Animation script that controls score text moving upwards and showing score.
 *******************************************************************/
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextRise : MonoBehaviour
{
    private RectTransform rt;
    private TMP_Text text;
    private float initialY;
    private Color initialColor;
    [SerializeField] private int riseDistance;
    [SerializeField] private float riseSeconds;
    [SerializeField] private bool destroy;
    [SerializeField] private bool fade;

    void Start()
    {
        rt = GetComponent<RectTransform>();
        text = GetComponentInChildren<TMP_Text>();
        initialY = rt.anchoredPosition.y;
        initialColor = text.color;
        text.text = "+" + ScoreManager.Instance.RecentlyAddedScore;
        StartCoroutine(Rise());
    }

    /// <summary>
    /// Coroutine that slowly makes the text rise over a specified number of seconds and distance. Can destroy or fade the text over time.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Rise()
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / riseSeconds;
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, Mathf.Lerp(initialY, initialY + riseDistance, t));
            if (fade && (t > 0.5f))
            {
                text.color = initialColor - new Color(0, 0, 0, Mathf.Lerp(0, 1, (t - 0.5f) * 2));
            }
            yield return null;
        }
        if (destroy)
        {
            Destroy(gameObject);
        }
    }
}
