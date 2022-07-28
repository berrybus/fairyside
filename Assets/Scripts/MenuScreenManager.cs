using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public enum MenuScreen {
    Main,
    Memories,
    Stats,
    Settings,
    Death,
    None
}

public class MenuScreenManager : MonoBehaviour {
    private UIScreen currentScreen;
    public UIScreen[] screens;
    public static MenuScreenManager instance;
    public PlayerInput playerInput;
    public bool ingame;
    public string returnActionMap = "Player";

    private void Awake() {
        currentScreen = screens[0];
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        foreach (UIScreen screen in screens) {
            screen.manager = this;
            screen.gameObject.SetActive(false);
        }
        currentScreen.gameObject.SetActive(true);
        if (ingame) {
            gameObject.SetActive(false);
        }
    }

    public void UpdateScreen(MenuScreen newScreen) {
        currentScreen.gameObject.SetActive(false);
        foreach (UIScreen screen in screens) {
            if (screen.screenType == newScreen) {
                currentScreen = screen;
            }
        }
        currentScreen.gameObject.SetActive(true);
    }

    public void MoveUp(InputAction.CallbackContext ctx) {
        currentScreen.MoveUp(ctx);
    }

    public void MoveDown(InputAction.CallbackContext ctx) {
        currentScreen.MoveDown(ctx);
    }

    public void MoveLeft(InputAction.CallbackContext ctx) {
        currentScreen.MoveLeft(ctx);
    }

    public void MoveRight(InputAction.CallbackContext ctx) {
        currentScreen.MoveRight(ctx);
    }

    public void Confirm(InputAction.CallbackContext ctx) {
        currentScreen.Confirm(ctx);
    }

    public void Cancel(InputAction.CallbackContext ctx) {

        currentScreen.Cancel(ctx);

        if (!ctx.performed) {
            return;
        }

        if (ingame && currentScreen.parentScreenType == MenuScreen.None) {
            CloseMenu();
        } else {
            UpdateScreen(currentScreen.parentScreenType);
        }
    }

    public void OpenMenu() {
        foreach (UIScreen screen in screens) {
            screen.gameObject.SetActive(false);
        }

        gameObject.SetActive(true);
        currentScreen.gameObject.SetActive(true);
        playerInput.SwitchCurrentActionMap("UI");
    }

    public void CloseMenu() {
        gameObject.SetActive(false);
        playerInput.SwitchCurrentActionMap(returnActionMap);
    }

    public void NotifyOfDeath() {
        foreach (UIScreen screen in screens) {
            if (screen.screenType == MenuScreen.Death) {
                currentScreen = screen;
                OpenMenu();
                return;
            }
        }
    }

    public void PlayerMenu(InputAction.CallbackContext ctx) {
        if (!ctx.performed) {
            return;
        }
        OpenMenu();
    }

}
