using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneSwitcher : MonoBehaviour {
    public static string currentLoadingScene;

    public static SceneSwitcher instance;

    public Image overlay;

    public static string[] levels = new string[] { "School1" };
    public static int currentLevel = -1;

    void Awake() {
        if (instance == null) {
            DontDestroyOnLoad(gameObject);
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (scene.name == "Menu" || scene.name == "Loading") {
            overlay.color = Color.clear;
        } else {
            overlay.color = Color.black;
            StartCoroutine(FadeIn());
        }
    }

    public void NextLevel() {
        if (ChangeInProgress()) {
            return;
        }
        StartCoroutine(StartNextLevel());
    }

    IEnumerator FadeIn() {
        while (overlay.color.a > 0.0) {
            float newAlpha = Mathf.Max(0, (float)(overlay.color.a - 4.0 * Time.deltaTime));
            overlay.color = new Color(0, 0, 0, newAlpha);
            yield return null;
        }
        yield return null;
    }

    IEnumerator FadeOut() {
        while (overlay.color.a < 1.0) {
            float newAlpha = Mathf.Min(1, (float)(overlay.color.a + 6.0 * Time.deltaTime));
            overlay.color = new Color(0, 0, 0, newAlpha);
            yield return null;
        }
        yield return null;
    }

    IEnumerator StartNextLevel() {
        yield return FadeOut();
        currentLevel += 1;
        currentLevel %= levels.Length;
        currentLoadingScene = levels[currentLevel];
        SceneManager.LoadScene("Loading");
        yield return null;
    }

    public void StartGame() {
        if (ChangeInProgress()) {
            return;
        }
        PlayerManager.instance.StartGame();
        SceneManager.LoadScene("Memory1");
    }

    IEnumerator ToMenu() {
        yield return FadeOut();
        SceneManager.LoadScene("Menu");
        yield return null;
    }

    public void GoToMenu() {
        if (ChangeInProgress()) {
            return;
        }
        StartCoroutine(ToMenu());
    }

    public bool ChangeInProgress() {
        return overlay.color.a > 0.0f && overlay.color.a < 1.0f;
    }
}
