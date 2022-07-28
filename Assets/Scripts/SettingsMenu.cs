using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class SettingsMenu : UIScreen {
    public TMP_Text resolutionDisplay;
    public TMP_Text fullscreenDisplay;

    private const int baseHeight = 180;
    private const int baseWidth = 320;

    private int currentResIdx = 0;

    private List<(int, int)> supportedResolutions = new List<(int, int)>();

    public SpriteRenderer[] leftArrows;
    public SpriteRenderer[] rightArrows;

    public Sprite selectedArrow;
    public Sprite deselectedArrow;

    private void Awake() {
        FindAvailableResolutions();
    }

    protected override void Start() {
        base.Start();
        GetClosestResolution();
    }

    protected override void OnEnable() {
        base.OnEnable();
        GetClosestResolution();
    }

    private void FindAvailableResolutions() {
        int height = baseHeight;
        int width = baseWidth;

        Resolution largestRes = Screen.resolutions[Screen.resolutions.Length - 1];

        while (height <= largestRes.height && width <= largestRes.width) {
            supportedResolutions.Add((width, height));
            height += baseHeight;
            width += baseWidth;
        }

        // Not sure how we would get to this case, but make sure we have at least one
        // support resolution
        if (supportedResolutions.Count == 0) {
            supportedResolutions.Add((baseWidth, baseHeight));
        }

    }

    private void GetClosestResolution() {
        int potentialIdx = 0;
        while (supportedResolutions[potentialIdx].Item1 < Screen.width &&
               supportedResolutions[potentialIdx].Item2 < Screen.height &&
               potentialIdx < supportedResolutions.Count - 1) {
            potentialIdx += 1;
        }
        currentResIdx = potentialIdx;
    }

    private void UpdateScreen(bool fullScreen) {
        int resIdx = fullScreen ? supportedResolutions.Count - 1 : currentResIdx;
        (int, int) res = supportedResolutions[resIdx];
        Screen.SetResolution(res.Item1, res.Item2, fullScreen);
    }

    private void Update() {
        resolutionDisplay.text = supportedResolutions[currentResIdx].Item2.ToString() + "p";
        fullscreenDisplay.text = Screen.fullScreen ? "Yes" : "No";
        for (int i = 0; i < leftArrows.Length; i++) {
            if (currentSelect == i) {
                leftArrows[i].sprite = selectedArrow;
                rightArrows[i].sprite = selectedArrow;
            } else {
                leftArrows[i].sprite = deselectedArrow;
                rightArrows[i].sprite = deselectedArrow;
            }
        }
    }

    public override void MoveRight(InputAction.CallbackContext ctx) {
        if (!ctx.performed) {
            return;
        }

        if (currentSelect == 0) {
            currentResIdx += 1;
            currentResIdx %= supportedResolutions.Count;
            UpdateScreen(false);
        } else if (currentSelect == 1) {
            UpdateScreen(!Screen.fullScreen);
        }
    }

    public override void MoveLeft(InputAction.CallbackContext ctx) {
        if (!ctx.performed) {
            return;
        }

        if (currentSelect == 0) {
            currentResIdx -= 1;
            currentResIdx %= supportedResolutions.Count;
            if (currentResIdx < 0) {
                currentResIdx += supportedResolutions.Count;
            }
            UpdateScreen(false);
        } else if (currentSelect == 1) {
            UpdateScreen(!Screen.fullScreen);
        }
    }

    public override void Confirm(InputAction.CallbackContext ctx) {
        if (!ctx.performed) {
            return;
        }
        if (currentSelect == 3) {
            if (manager) {
                manager.Cancel(ctx);
            } else {
                Cancel(ctx);
            }
        }
    }
}
