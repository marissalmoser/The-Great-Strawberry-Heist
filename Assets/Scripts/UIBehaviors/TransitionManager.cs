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

    /// <summary>
    /// Sets up singleton
    /// </summary>
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

    /// <summary>
    /// Fades from white to transparent
    /// </summary>
    /// <param name="seconds"></param>
    /// <returns></returns>
    private IEnumerator FadeIn(float seconds)
    {
        //if (fade.color.a == 0) { yield break; }
        _fade.color = new Color(_fade.color.r, _fade.color.g, _fade.color.b, 1);
        yield return null;
        yield return null;
        float alpha = 1;
        float t = 0;
        Vector3 color = new Vector3(_fade.color.r, _fade.color.g, _fade.color.b);
        while (alpha > 0)
        {
            t += Time.deltaTime / seconds;
            alpha = 1 - Mathf.Lerp(0, 1, t);
            _fade.color = new Color(color.x, color.y, color.z, alpha);
            yield return null;
        }
    }

    /// <summary>
    /// Whiteboard comes in like a projector screen, loads next scene
    /// </summary>
    /// <param name="scene"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Fades to white and loads scene
    /// </summary>
    /// <param name="seconds"></param>
    /// <param name="scene"></param>
    /// <returns></returns>
    public IEnumerator FadeOut(float seconds, string scene)
    {
        float alpha = 0;
        float t = 0;
        while (alpha < 1)
        {
            t += Time.deltaTime / seconds;
            alpha = Mathf.Lerp(0, 1, t);
            _fade.color = new Color(_fade.color.r, _fade.color.g, _fade.color.b, alpha);
            yield return null;
        }
        fadeIn = true;
        SceneManager.LoadScene(scene);
    }

    /// <summary>
    /// Circle exit animation for game scene
    /// </summary>
    /// <param name="scene"></param>
    public void CircleOut(string scene)
    {
        _circle.gameObject.SetActive(true);
        nextSceneToLoad = scene;
        fadeIn = true;
    }

    /// <summary>
    /// Called by circle animation event
    /// </summary>
    public void LoadScene()
    {
        SceneManager.LoadScene(nextSceneToLoad);
    }

    /// <summary>
    /// This function is called by Unity when a new scene finishes loading
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode lsm)
    {
        if (Instance == this)
        {
            SceneHasBeenLoaded();
        }
    }

    /// <summary>
    /// Runs when scene loads
    /// </summary>
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
