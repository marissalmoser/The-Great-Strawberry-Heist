using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance;
    [SerializeField] private SpriteRenderer fade;

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Fades from current screen color to transparent
    private IEnumerator FadeIn(float seconds)
    {
        if (fade.color.a == 0) { yield break; }
        yield return null;
        float alpha = 1;
        float t = 0;
        float color = fade.color.r;
        while (alpha > 0)
        {
            t += Time.deltaTime / seconds;
            alpha = 1 - Mathf.Lerp(0, 1, t);
            fade.color = new Color(color, color, color, alpha);
            yield return null;
        }
    }

    // Fades to black (or other color value) and loads scene
    public IEnumerator FadeOut(float seconds, float color, string scene)
    {
        float alpha = 0;
        float t = 0;
        while (alpha < 1)
        {
            t += Time.deltaTime / seconds;
            alpha = Mathf.Lerp(0, 1, t);
            fade.color = new Color(color, color, color, alpha);
            yield return null;
        }
        SceneManager.LoadScene(scene);
    }

    // Cuts to black and then loads scene, if seconds != 0
    // Cuts directly to scene if seconds == 0 (used for title screen play button)
    public IEnumerator CutOut(float seconds, float color, string scene)
    {
        if (seconds != 0)
        {
            fade.color = Color.black;
        }
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / seconds;
            yield return null;
        }
        SceneManager.LoadScene(scene);
    }

    /// <summary>
    /// This function is auto-called by Unity when a new scene finishes loading
    /// It tells the StageManager to set up the level from the current checkpoint
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode lsm)
    {
        if (Instance == this)
        {
            SceneHasBeenLoaded();
        }
    }

    // Runs whenever a scene loads, a new scene or the same one
    private void SceneHasBeenLoaded()
    {
        StartCoroutine(FadeIn(0.5f));
    }
}
