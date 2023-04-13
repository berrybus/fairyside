using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class MemoryMenu : UIScreen {
    protected override void Start() {
        base.Start();
        SetMemoryText();
    }

    protected override void OnEnable() {
        base.OnEnable();
        if (GameManager.instance) {
            SetMemoryText();
        }
    }
    private void SetMemoryText() {
        if (GameManager.instance == null) {
            return;
        }
        for (int i = 1; i < GameManager.maxMemory - 1; i ++) {
            if (GameManager.instance.watchedMemory[i]) {
                options[i].text = "Memory" + i;
            } else {
                options[i].text = "Locked";
            }
        }
        options[0].text = GameManager.instance.watchedMemory[0] ? "Prologue" : "Locked";
        options[GameManager.maxMemory - 1].text = GameManager.instance.watchedMemory[^1] ? "Epilogue" : "Locked";
        options[^2].text = GameManager.instance.finishedGame ? "Credits" : "Locked";
    }

    public override void Confirm(InputAction.CallbackContext ctx) {
        if (!ctx.performed) {
            return;
        }
        if (currentSelect < GameManager.maxMemory) {
            if (GameManager.instance.watchedMemory[currentSelect]) {
                GameManager.instance.PlaySingleMemory(currentSelect);
            }
        } else if (currentSelect == options.Length - 2) {
            if (GameManager.instance.finishedGame) {
                GameManager.instance.PlayCredits();
            }
        } else if (currentSelect == options.Length - 1) {
            if (manager) {
                manager.Cancel(ctx);
            } else {
                Cancel(ctx);
            }
        }
    }

}
