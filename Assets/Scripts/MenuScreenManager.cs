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
    Card,
    Pregame,
    Death,
    None,
    Notes
}

public class MenuScreenManager : MonoBehaviour {
    private UIScreen currentScreen;
    public UIScreen[] screens;
    public static MenuScreenManager instance;
    public PlayerInput playerInput;
    public bool ingame;
    public string returnActionMap = "Player";

    public AudioClip confirmClip;
    public AudioClip moveClip;
    public AudioClip cancelClip;

    private bool canTakeAction = false;

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
        canTakeAction = false;
        if (gameObject.activeInHierarchy) {
            StartCoroutine(AllowAction());
        } else {
            canTakeAction = true;
        }
    }

    private IEnumerator AllowAction() {
        yield return new WaitForEndOfFrame();
        yield return new WaitForSecondsRealtime(0.5f);
        canTakeAction = true;
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

    public void OpenMenu() {
        foreach (UIScreen screen in screens) {
            screen.gameObject.SetActive(false);
        }

        gameObject.SetActive(true);
        currentScreen.gameObject.SetActive(true);
        playerInput.SwitchCurrentActionMap("UI");
    }

    public void CloseMenu() {
        Time.timeScale = 1;
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

    public void MoveUp(InputAction.CallbackContext ctx) {
        currentScreen.MoveUp(ctx);

        if (ctx.performed && currentScreen.options.Length > 0 && currentScreen.playClicks) {
            GameManager.instance.PlaySFX(moveClip);
        }
    }

    public void MoveDown(InputAction.CallbackContext ctx) {
        currentScreen.MoveDown(ctx);

        if (ctx.performed && currentScreen.options.Length > 0 && currentScreen.playClicks) {
            GameManager.instance.PlaySFX(moveClip);
        }
    }

    public void MoveLeft(InputAction.CallbackContext ctx) {
        currentScreen.MoveLeft(ctx);
    }

    public void MoveRight(InputAction.CallbackContext ctx) {
        currentScreen.MoveRight(ctx);
    }

    public void Confirm(InputAction.CallbackContext ctx) {
        if (!canTakeAction) {
            return;
        }

        currentScreen.Confirm(ctx);

        if (!ctx.performed) {
            return;
        }
        GameManager.instance.PlaySFX(confirmClip);
    }

    public void Cancel(InputAction.CallbackContext ctx) {
        currentScreen.Cancel(ctx);

        if (!ctx.performed) {
            return;
        }

        if (currentScreen.parentScreenType != MenuScreen.None || ingame) {
            GameManager.instance.PlaySFX(cancelClip);
        }

        if (ingame && currentScreen.parentScreenType == MenuScreen.None) {
            CloseMenu();
        } else {
            UpdateScreen(currentScreen.parentScreenType);
        }
    }

    public void PlayerMenu(InputAction.CallbackContext ctx) {
        if (!ctx.performed || GameManager.instance.isTransitioning) {
            return;
        }
        GameManager.instance.PlaySFX(confirmClip);
        Time.timeScale = 0;
        OpenMenu();
    }

}
