using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour {
    public TMPro.TMP_Text hp;
    public TMPro.TMP_Text mp;
    public TMPro.TMP_Text gems;
    public TMPro.TMP_Text lvl;
    public TMPro.TMP_Text atk;
    [SerializeField]
    private Transform hpBar;
    [SerializeField]
    private Transform mpBar;

    private void Start() {
        UpdateText();
    }

    private void FixedUpdate() {
        UpdateText();
    }

    private void UpdateText() {
        gems.text = PlayerManager.instance.gemCount.ToString();
        hp.text = PlayerManager.instance.hp.ToString() + "/" + PlayerManager.instance.maxHp.ToString();
        mp.text = ((int)PlayerManager.instance.mp).ToString() + "/" + PlayerManager.instance.maxMp.ToString();
        lvl.text = PlayerManager.instance.lvl.ToString();
        atk.text = PlayerManager.instance.atk.ToString();
        float hpPercentage = (float) PlayerManager.instance.hp / (float) PlayerManager.instance.maxHp;
        float mpPercentage = (float) PlayerManager.instance.mp / (float) PlayerManager.instance.maxMp;
        hpBar.localScale = new Vector3(hpPercentage * 16.0f, 16, 16);
        mpBar.localScale = new Vector3(mpPercentage * 16.0f, 16, 16);
    }
}
