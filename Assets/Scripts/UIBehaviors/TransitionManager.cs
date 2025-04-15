using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance;
    private bool fadeIn;
    [SerializeField] private Image _fade;
    [SerializeField] private Image _whiteboard;
    [SerializeField] private Image _whiteboardB;
    [SerializeField] private float _whiteboardDropdownSpeed;
    [SerializeField] private float _whiteboardSlowdownMult;
    [SerializeField] private float _whiteboardFadeSpeed;
    [SerializeField] private Image _circle;
    private float whiteboardSpeedCurrent;
    private string nextSceneToLoad;

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneHasBeenLoaded();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Fades from current screen color to transparent
    private IEnumerator FadeIn(float seconds)
    {
        //if (fade.color.a == 0) { yield break; }
        _fade.color = new Color(1, 1, 1, 1);
        yield return null;
        yield return null;
        float alpha = 1;
        float t = 0;
        float color = _fade.color.r;
        while (alpha > 0)
        {
            print("aaaa" + Time.deltaTime / seconds);
            t += Time.deltaTime / seconds;
            alpha = 1 - Mathf.Lerp(0, 1, t);
            _fade.color = new Color(color, color, color, alpha);
            yield return null;
        }
    }

    public IEnumerator WhiteboardIn(string scene)
    {
        whiteboardSpeedCurrent = _whiteboardDropdownSpeed;
        _whiteboard.transform.localPosition = new Vector3(0, 1080);
        _whiteboard.gameObject.SetActive(true);
        _whiteboardB.gameObject.SetActive(false);

        while (_whiteboard.transform.localPosition.y > 0)
        {
            _whiteboard.transform.position = _whiteboard.transform.position - new Vector3(0, whiteboardSpeedCurrent * Time.deltaTime);
            yield return null;
        }

        while (whiteboardSpeedCurrent > 0.05f)
        {
            whiteboardSpeedCurrent *= _whiteboardSlowdownMult;
            _whiteboard.transform.position = _whiteboard.transform.position - new Vector3(0, whiteboardSpeedCurrent * Time.deltaTime);
            yield return null;
        }

        whiteboardSpeedCurrent = _whiteboardDropdownSpeed;
        while (_whiteboard.transform.localPosition.y < 0)
        {
            _whiteboard.transform.position = _whiteboard.transform.position + new Vector3(0, whiteboardSpeedCurrent * Time.deltaTime);
            yield return null;
        }
        _whiteboard.transform.localPosition = Vector2.zero;

        _whiteboardB.gameObject.SetActive(true);
        float alpha = 0;
        float t = 0;
        while (alpha < 1)
        {
            t += Time.deltaTime / _whiteboardFadeSpeed;
            alpha = Mathf.Lerp(0, 1, t);
            _whiteboardB.color = new Color(1, 1, 1, alpha);
            yield return null;
        }

        SceneManager.LoadScene(scene);
    }

    // Fades to black (or other color value) and loads scene
    public IEnumerator FadeOut(float seconds, string scene)
    {
        float alpha = 0;
        float t = 0;
        while (alpha < 1)
        {
            t += Time.deltaTime / seconds;
            alpha = Mathf.Lerp(0, 1, t);
            _fade.color = new Color(1, 1, 1, alpha);
            yield return null;
        }
        fadeIn = true;
        SceneManager.LoadScene(scene);
    }

    // Cuts to black and then loads scene, if seconds != 0
    // Cuts directly to scene if seconds == 0 (used for title screen play button)
    public IEnumerator CutOut(float seconds, float color, string scene)
    {
        if (seconds != 0)
        {
            _fade.color = Color.black;
        }
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / seconds;
            yield return null;
        }
        SceneManager.LoadScene(scene);
    }

    public void CircleOut(string scene)
    {
        _circle.gameObject.SetActive(true);
        nextSceneToLoad = scene;
        fadeIn = true;
    }

    public void LoadScene()
    {
        SceneManager.LoadScene(nextSceneToLoad);
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
        _whiteboard.gameObject.SetActive(false);
        _whiteboardB.gameObject.SetActive(false);
        _circle.gameObject.SetActive(false);
        if (fadeIn)
        {
            StartCoroutine(FadeIn(0.25f));
            fadeIn = false;
        }
    }
}
