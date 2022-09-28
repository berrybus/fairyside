using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour {
    public TMP_Text hp;
    public TMP_Text mp;
    public TMP_Text gems;
    public TMP_Text levelName;
    [SerializeField]
    private Transform hpBar;
    [SerializeField]
    private Transform mpBar;

    private void Start() {
        levelName.text = GameManager.levelNames[GameManager.instance.currentLevel];
        UpdateText();
    }

    private void FixedUpdate() {
        UpdateText();
    }

    private void UpdateText() {
        gems.text = PlayerManager.instance.gemCount.ToString();
        hp.text = PlayerManager.instance.hp.ToString() + "/" + PlayerManager.instance.maxHp.ToString();
        mp.text = ((int)PlayerManager.instance.mp).ToString() + "/" + PlayerManager.instance.maxMp.ToString();
        float hpPercentage = (float) PlayerManager.instance.hp / (float) PlayerManager.instance.maxHp;
        float mpPercentage = (float) PlayerManager.instance.mp / (float) PlayerManager.instance.maxMp;
        hpBar.localScale = new Vector3(hpPercentage * 16.0f, 16, 16);
        mpBar.localScale = new Vector3(mpPercentage * 16.0f, 16, 16);
    }
}
