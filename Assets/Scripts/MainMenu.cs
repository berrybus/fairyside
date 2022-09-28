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
            PlayerManager.instance.StartGame();
            manager.UpdateScreen(MenuScreen.Pregame);
        } else if (currentSelect == 1) {
            manager.UpdateScreen(MenuScreen.Memories);
        } else if (currentSelect == 2) {
            manager.UpdateScreen(MenuScreen.Notes);
        } else if (currentSelect == 4) {
            manager.UpdateScreen(MenuScreen.Settings);
        } else if (currentSelect == options.Length - 1) {
            Application.Quit();
        }
    }

    public override void Cancel(InputAction.CallbackContext ctx) {
        // Do nothing, we can't cancel from main menu
    }

}
