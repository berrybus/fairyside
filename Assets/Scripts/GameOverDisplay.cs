using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class GameOverDisplay : MonoBehaviour {
    public TMP_Text timeDisplay;
    public TMP_Text lvlDisplay;
    public TMP_Text xpDisplay;
    public TMP_Text gemDisplay;
    public SpriteRenderer arrowRenderer;
    public StatUpIndicator indicator;
    public Transform indicatorPosition;
    [SerializeField]
    private Transform expBar;
    private bool isAnimating = true;
    private int displayLvl;
    void Start() {
        System.TimeSpan time = System.TimeSpan.FromSeconds(PlayerManager.instance.gameTime);
        string timeString = "";
        if (time.Hours > 0) {
            timeString = time.ToString("hh':'mm':'ss");
        } else {
            timeString = time.ToString("mm':'ss");
        }
        timeDisplay.text = "Time: " + timeString;
        displayLvl = PlayerManager.instance.lvl;
        SetupAnimateText();
        StartCoroutine(AnimateXPIncrease());
    }

    private void SetupAnimateText() {
        lvlDisplay.text = "LVL " + displayLvl;
        if (PlayerManager.instance.AtMaxLvl()) {
            xpDisplay.text = "XP: 0/--";
        } else {
            xpDisplay.text = string.Format("XP: {0}/{1}", PlayerManager.instance.exp, PlayerManager.instance.ExpToLevel());
        }
        gemDisplay.text = PlayerManager.instance.gemCount.ToString();
        if (PlayerManager.instance.AtMaxLvl()) {
            expBar.localScale = new Vector3(1f, 1f, 1f);
        } else {
            float expPercentage = (float)PlayerManager.instance.exp / Mathf.Max(1f, PlayerManager.instance.ExpToLevel());
            expPercentage = Mathf.Min(1f, expPercentage);
            expBar.localScale = new Vector3(expPercentage, 1f, 1f);
        }
        arrowRenderer.enabled = !isAnimating;
    }

    IEnumerator AnimateXPIncrease() {
        yield return new WaitForSeconds(0.5f);
        while (PlayerManager.instance.gemCount > 0 && !PlayerManager.instance.AtMaxLvl()) {
            PlayerManager.instance.gemCount -= 1;
            PlayerManager.instance.exp += 1;
            if (PlayerManager.instance.exp >= PlayerManager.instance.ExpToLevel()) {
                displayLvl = PlayerManager.instance.lvl + 1;
                StatUpIndicator statUp = Instantiate(indicator, indicatorPosition.position, Quaternion.identity);
                statUp.newLvl = displayLvl;
                yield return new WaitForSeconds(0.5f);
                PlayerManager.instance.lvl += 1;
                PlayerManager.instance.exp = 0;
            }
            yield return new WaitForSeconds(0.03f);
        }
        isAnimating = false;
    }

    private void FixedUpdate() {
        SetupAnimateText();
    }

    public void Continue(InputAction.CallbackContext ctx) {
        if (!ctx.performed || isAnimating) {
            return;
        }
        GameManager.instance.GoToMenu();
    }
}
