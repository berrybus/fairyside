using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class VNManager : MonoBehaviour {

    ScriptLine[] script;

    public TMP_Text display;
    public TMP_Text nameDisplay;
    private int lineIndex = 1;
    private int scriptIndex = 0;
    public SpriteRenderer continueArrowRenderer;
    public SpriteRenderer avatarRenderer;
    public SpriteRenderer bg;
    public SpriteRenderer namebox;
    public Sprite[] bgList;

    public Image intro;
    public TMP_Text introText;

    public AudioClip whimsical;
    public AudioClip somber;
    public AudioClip writerTheme;
    public AudioClip creation;
    public AudioClip arcadia;
    public AudioClip gretchen;
    public AudioClip nacht;
    public AudioClip mainTheme;

    public AudioClip click;
    public AudioClip enter;

    // Start is called before the first frame update
    void Start() {
        display.text = "";
        bg.sprite = bgList[GameManager.instance.currentMemory];
        script = ScriptRepository.scripts[GameManager.instance.currentMemory];
        SetupStaticElements();
        if (GameManager.instance.currentMemory > 0
            && GameManager.instance.currentMemory < GameManager.maxMemory  -1) {
            StartCoroutine(ShowIntro());
        } else {
            HideIntro();
            StartCoroutine(Typewriter());
        }
        StartMusic();
    }

    void StartMusic() {
        switch (GameManager.instance.currentMemory) {
            case 0:
                GameManager.instance.PlayMusic(mainTheme);
                break;
            case 1:
                GameManager.instance.PlayMusic(whimsical);
                break;
            case 2: case 3:
                GameManager.instance.PlayMusic(somber);
                break;
            case 4: case 5:
                GameManager.instance.PlayMusic(creation);
                break;
            case 6:
                GameManager.instance.PlayMusic(writerTheme);
                break;
            case 7:
                GameManager.instance.PlayMusic(gretchen);
                break;
            case 8:
                GameManager.instance.PlayMusic(arcadia);
                break;
            case 9:
                GameManager.instance.PlayMusic(nacht);
                break;
            case 10: case 11:
                GameManager.instance.PlayMusic(mainTheme);
                break;

        }
    }

    IEnumerator ShowIntro() {
        intro.enabled = true;
        introText.text = "Memory " + GameManager.instance.currentMemory;
        yield return new WaitForSeconds(1.0f);
        HideIntro();
        StartCoroutine(Typewriter());
    }

    IEnumerator Typewriter() {
        while (scriptIndex < script.Length) {
            string currentLine = script[scriptIndex].line;
            while (lineIndex <= currentLine.Length) {
                display.text = currentLine[..lineIndex];
                lineIndex += 1;
                if (lineIndex % 2 == 0) {
                    GameManager.instance.PlaySFX(click);
                }
                yield return new WaitForSeconds(0.0625f);
                if (scriptIndex < script.Length) {
                    currentLine = script[scriptIndex].line;
                }
            }
            yield return null;
        }
    }

    private void FixedUpdate() {
        if (scriptIndex < script.Length) {
            string currentLine = script[scriptIndex].line;
            continueArrowRenderer.enabled = lineIndex > currentLine.Length;
        }
    }

    public void Continue(InputAction.CallbackContext ctx) {
        if (intro.enabled || !ctx.performed || scriptIndex >= script.Length) {
            return;
        }
        string currentLine = script[scriptIndex].line;
        if (lineIndex <= currentLine.Length) {
            display.text = currentLine;
            lineIndex = currentLine.Length + 1;
        } else {
            scriptIndex += 1;
            lineIndex = 1;
            if (scriptIndex >= script.Length) {
                GameManager.instance.FinishedPlayingMemory();
            } else {
                SetupStaticElements();
            }
            GameManager.instance.PlaySFX(enter);
        }
    }

    void SetupStaticElements() {
        nameDisplay.text = script[scriptIndex].name;
        namebox.enabled = script[scriptIndex].name != "";
        Sprite avatar = Resources.Load<Sprite>(script[scriptIndex].avatar);
        avatarRenderer.sprite = avatar;
    }

    private void HideIntro() {
        intro.enabled = false;
        introText.text = "";
    }
}
