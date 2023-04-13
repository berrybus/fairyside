using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Linq;

public class MainMenu: UIScreen
{
    public TMP_Text[] originalOptions;

    private void Awake() {
        originalOptions = options;
    }

    protected override void OnEnable() {
        base.OnEnable();
        if (!GameManager.SavedRunAvailable()) {
            originalOptions[0].color = deselect;
            originalOptions[0].fontSize = 16;
            options = originalOptions.Skip(1).ToArray();
        } else {
            options = originalOptions;
        }
    }

    public override void Confirm(InputAction.CallbackContext ctx) {
        if (!ctx.performed) {
            return;
        }
        if (options[currentSelect].text == "Continue") {
            GameManager.instance.LoadRunAndStart();
        } else if (options[currentSelect].text == "Start") {
            PlayerManager.instance.StartGame();
            manager.UpdateScreen(MenuScreen.Pregame);
        } else if (options[currentSelect].text == "Memories") {
            manager.UpdateScreen(MenuScreen.Memories);
        } else if (options[currentSelect].text == "Notes") {
            manager.UpdateScreen(MenuScreen.Notes);
        } else if (options[currentSelect].text == "Stats") {
            manager.UpdateScreen(MenuScreen.Stats);
        } else if (options[currentSelect].text == "Settings") {
            manager.UpdateScreen(MenuScreen.Settings);
        } else if (options[currentSelect].text == "Quit") {
            GameManager.instance.StoreAchievements();
            Application.Quit();
        }
    }

    public override void Cancel(InputAction.CallbackContext ctx) {
        // Do nothing, we can't cancel from main menu
    }

}
