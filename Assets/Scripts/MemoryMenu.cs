using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class MemoryMenu : UIScreen {
    protected override void Start() {
        base.Start();
        setMemoryText();
    }

    protected override void OnEnable() {
        base.OnEnable();
        if (GameManager.instance) {
            setMemoryText();
        }
    }
    private void setMemoryText() {
        if (GameManager.instance == null) {
            return;
        }
        for (int i = GameManager.maxMemory; i < options.Length - 2; i ++) {
            options[i].text = "Locked";
        }
        options[options.Length - 2].text = GameManager.instance.finishedGame ? "Credits" : "Locked";
    }

    public override void Confirm(InputAction.CallbackContext ctx) {
        if (!ctx.performed) {
            return;
        }
        if (currentSelect < GameManager.maxMemory) {
            GameManager.instance.PlaySingleMemory(currentSelect);
        } else if (currentSelect == options.Length - 1) {
            if (manager) {
                manager.Cancel(ctx);
            } else {
                Cancel(ctx);
            }
        }
    }

}
