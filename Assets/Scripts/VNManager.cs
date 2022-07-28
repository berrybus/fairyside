using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public struct ScriptLine {
    public string line;
    public string name;
    public string avatar;

    public ScriptLine(string avatar, string name, string line) {
        this.avatar = avatar;
        this.name = name;
        this.line = line;
    }
}
public class VNManager : MonoBehaviour {

    ScriptLine[] script = new ScriptLine[] {
        new ScriptLine(
            "ShirleyAvatar",
            "Shirley",
            "The first day of school! I'm so excited!"),
        new ScriptLine(
            "ShirleyAvatar",
            "Shirley",
            "Ah, the bell!"),
        new ScriptLine(
            "ShirleyAvatar",
            "Shirley",
            "Let's see, where's my first class..."),
        new ScriptLine(
            "ShirleyAvatar",
            "Shirley",
            "Hmm, maybe I'll just follow those girls, they look like they know where they're going."),
    };

    public TMP_Text display;
    public TMP_Text nameDisplay;
    private int lineIndex = 1;
    private int scriptIndex = 0;
    public SpriteRenderer continueArrowRenderer;
    public SpriteRenderer avatarRenderer;

    // Start is called before the first frame update
    void Start() {
        display.text = "";
        SetupStaticElements();
        StartCoroutine(Typewriter());
    }

    IEnumerator Typewriter() {
        while (scriptIndex < script.Length) {
            string currentLine = script[scriptIndex].line;
            while (lineIndex <= currentLine.Length) {
                display.text = currentLine[..lineIndex];
                lineIndex += 1;
                yield return new WaitForSeconds(0.0625f);
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
        if (scriptIndex < script.Length && ctx.performed) {
            string currentLine = script[scriptIndex].line;
            if (lineIndex <= currentLine.Length) {
                display.text = currentLine;
                lineIndex = currentLine.Length + 1;
            } else {
                scriptIndex += 1;
                lineIndex = 1;
                if (scriptIndex >= script.Length) {
                    SceneSwitcher.instance.NextLevel();
                } else {
                    SetupStaticElements();
                }
            }
        }
    }

    void SetupStaticElements() {
        nameDisplay.text = script[scriptIndex].name;
        Sprite avatar = Resources.Load<Sprite>(script[scriptIndex].avatar);
        avatarRenderer.sprite = avatar;
    }

    // Update is called once per frame
    void Update() {

    }
}
