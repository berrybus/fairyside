using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class DeathMenu : UIScreen {
    public override void Confirm(InputAction.CallbackContext ctx) {
        if (!ctx.performed) {
            return;
        }
        SceneSwitcher.instance.GoToMenu();
    }

    public override void Cancel(InputAction.CallbackContext ctx) {
        if (!ctx.performed) {
            return;
        }
        SceneSwitcher.instance.GoToMenu();
    }

}
