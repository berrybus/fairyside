using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class IngameMenu : UIScreen {
    public override void Confirm(InputAction.CallbackContext ctx) {
        if (!ctx.performed) {
            return;
        }
        if (currentSelect == 0) {
            gameObject.SetActive(false);
            manager.CloseMenu();
        } else if (currentSelect == 1) {
            manager.UpdateScreen(MenuScreen.Card);
        } else if (currentSelect == 2) {
            manager.UpdateScreen(MenuScreen.Notes);
        } else if (currentSelect == 3) {
            manager.UpdateScreen(MenuScreen.Settings);
        } else if (currentSelect == 4) {
            Time.timeScale = 1;
            GameManager.instance.GoToMenu();
        }
    }
}
