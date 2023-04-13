using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InGameTimer: MonoBehaviour {
    public TMP_Text timer;
    private int countdown = 0;
    private void Update() {
        if (Time.deltaTime == 0) {
            countdown = 120;
        }

        if (countdown > 0) {
            System.TimeSpan time = System.TimeSpan.FromSeconds(PlayerManager.instance.gameTime);
            string timeString;
            if (time.Hours > 0) {
                timeString = time.ToString("hh':'mm':'ss");
            } else {
                timeString = time.ToString("mm':'ss");
            }
            timer.text = timeString;
            timer.alpha = Mathf.Min(countdown, 45f) / 45f;
        } else {
            timer.text = "";
        }
    }

    private void FixedUpdate() {
        countdown -= 1;
        countdown = Mathf.Max(0, countdown);
    }
}
