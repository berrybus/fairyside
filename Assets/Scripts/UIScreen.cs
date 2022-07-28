using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class UIScreen : MonoBehaviour {
    private Color deselect = new Color(138f / 255f, 143f / 255f, 158f / 255f, 1f);
    protected int currentSelect = 0;
    public TMP_Text[] options;
    public MenuScreen screenType;
    public MenuScreen parentScreenType;
    public MenuScreenManager manager;

    protected virtual void Start() {
        SetTextColors();
    }

    protected virtual void OnEnable() {
        SetTextColors();
    }

    protected virtual void SetTextColors() {
        for (int i = 0; i < options.Length; i++) {
            if (currentSelect == i) {
                options[i].color = Color.white;
                options[i].fontSize = 18;
            } else {
                options[i].color = deselect;
                options[i].fontSize = 16;
            }
        }
    }

    public virtual void MoveUp(InputAction.CallbackContext ctx) {
        if (!ctx.performed) {
            return;
        }
        currentSelect -= 1;
        currentSelect %= options.Length;
        if (currentSelect < 0) {
            currentSelect += options.Length;
        }
        SetTextColors();
    }

    public virtual void MoveDown(InputAction.CallbackContext ctx) {
        if (!ctx.performed) {
            return;
        }
        currentSelect += 1;
        currentSelect %= options.Length;

        SetTextColors();
    }

    public virtual void MoveLeft(InputAction.CallbackContext ctx) { }

    public virtual void MoveRight(InputAction.CallbackContext ctx) { }

    public virtual void Confirm(InputAction.CallbackContext ctx) { }

    public virtual void Cancel(InputAction.CallbackContext ctx) {
        if (!ctx.performed) {
            return;
        }

        gameObject.SetActive(false);
    }
}