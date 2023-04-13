using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class GameOverDisplay : MonoBehaviour {
    public TMP_Text endDisplay;
    public TMP_Text timeDisplay;
    public TMP_Text rerunDisplay;
    public TMP_Text streakDisplay;
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

    public AudioClip greatJob;
    public AudioClip goodTry;

    void Start() {
        System.TimeSpan time = System.TimeSpan.FromSeconds(PlayerManager.instance.gameTime);
        string timeString;
        if (time.Hours > 0) {
            timeString = time.ToString("hh':'mm':'ss");
        } else {
            timeString = time.ToString("mm':'ss");
        }
        timeDisplay.text = "Time: " + timeString;
        displayLvl = PlayerManager.instance.lvl;
        rerunDisplay.text = "Reruns: " + GameManager.instance.numRepeats;
        SetupAnimateText();
        UpdateStats();
        streakDisplay.text = "Win streak: " + GameManager.instance.currentStreak;
        StartCoroutine(AnimateXPIncrease());
        if (GameManager.instance.currentLevel == GameManager.maxLevel) {
            GameManager.instance.PlaySFX(greatJob);
        } else {
            GameManager.instance.PlaySFX(goodTry);
        }
    }

    private void UpdateStats() {
        if (GameManager.instance.currentLevel == GameManager.maxLevel) {
            GameManager.instance.currentStreak += 1;
            GameManager.instance.highestStreak = Mathf.Max(GameManager.instance.highestStreak, GameManager.instance.currentStreak);
            if (GameManager.instance.fastestTime == 0) {
                GameManager.instance.fastestTime = PlayerManager.instance.gameTime;
            } else {
                GameManager.instance.fastestTime = Mathf.Min(GameManager.instance.fastestTime, PlayerManager.instance.gameTime);
            }
            GameManager.instance.CheckSpeedrunnerAchievement();
            GameManager.instance.totalWins += 1;
        } else {
            GameManager.instance.currentStreak = 0;
            GameManager.instance.totalDeaths += 1;
        }
        GameManager.instance.maxRepeats = Mathf.Max(GameManager.instance.maxRepeats, GameManager.instance.numRepeats);
    }

    private void SetupAnimateText() {
        if (GameManager.instance.currentLevel == GameManager.maxLevel) {
            endDisplay.text = "You Won!";
        } else {
            endDisplay.text = "Game Over";
        }
        lvlDisplay.text = "LVL " + displayLvl;
        if (PlayerManager.instance.AtMaxLvl()) {
            xpDisplay.text = "XP: 0/--";
            PlayerManager.instance.exp = 0;
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
        GameManager.instance.GoToMenuOrCredits();
        GameManager.DeleteSavedRun();
    }
}
