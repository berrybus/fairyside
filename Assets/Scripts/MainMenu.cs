using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class MainMenu: UIScreen
{
    public override void Confirm(InputAction.CallbackContext ctx) {
        if (!ctx.performed) {
            return;
        }
        if (currentSelect == 0) {
            SceneSwitcher.instance.StartGame();
        } else if (currentSelect == 3) {
            manager.UpdateScreen(MenuScreen.Settings);
        }
    }

    public override void Cancel(InputAction.CallbackContext ctx) {
        // Do nothing, we can't cancel from main menu
    }

}
