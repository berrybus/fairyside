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
        setMemoryText();
    }
    private void setMemoryText() {
        if (GameManager.instance == null) {
            return;
        }
        for (int i = GameManager.maxMemory; i < options.Length - 1; i ++) {
            options[i].text = "Locked";
        }
    }

    public override void Confirm(InputAction.CallbackContext ctx) {
        if (!ctx.performed) {
            return;
        }
        if (currentSelect < 14) {
            if (currentSelect < GameManager.maxMemory) {
                GameManager.instance.PlaySingleMemory(currentSelect);
            }
        }
        else if (currentSelect == 14) {
            if (manager) {
                manager.Cancel(ctx);
            } else {
                Cancel(ctx);
            }
        }
    }

}
