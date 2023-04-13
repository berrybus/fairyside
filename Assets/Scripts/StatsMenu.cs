using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class StatsMenu : UIScreen {
    public TMP_Text currentStreak;
    public TMP_Text highestStreak;
    public TMP_Text totalWins;
    public TMP_Text totalDeaths;
    public TMP_Text maxRepeats;
    public TMP_Text fastestTime;

    public void Update() {
        currentStreak.text = GameManager.instance.currentStreak.ToString();
        highestStreak.text = GameManager.instance.highestStreak.ToString();
        totalWins.text = GameManager.instance.totalWins.ToString();
        totalDeaths.text = GameManager.instance.totalDeaths.ToString();
        maxRepeats.text = GameManager.instance.maxRepeats.ToString();
        if (GameManager.instance.fastestTime > 0) {
            System.TimeSpan time = System.TimeSpan.FromSeconds(GameManager.instance.fastestTime);
            string timeString;
            if (time.Hours > 0) {
                timeString = time.ToString("hh':'mm':'ss");
            } else {
                timeString = time.ToString("mm':'ss");
            }
            fastestTime.text = timeString;
        }
    }

    public override void MoveUp(InputAction.CallbackContext ctx) { }

    public override void MoveDown(InputAction.CallbackContext ctx) { }

    public override void Confirm(InputAction.CallbackContext ctx) {
        if (!ctx.performed) {
            return;
        }
        if (manager) {
            manager.Cancel(ctx);
        } else {
            Cancel(ctx);
        }
    }
}
