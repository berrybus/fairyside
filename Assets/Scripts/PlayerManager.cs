using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerGun {
    Basic
}
public class PlayerManager: MonoBehaviour
{
    public static PlayerManager instance;
    public int hp = 5;
    public int maxHp = 5;
    public float mp = 100;
    public int maxMp = 100;
    public int lvl = 1;
    public int atk = 4;

    public int gemCount = 0;

    public PlayerGun curGun;

    public DamageText damageText;

    public void StartGame() {
        hp = maxHp;
        mp = maxMp;
        gemCount = 0;
    }

    private void Awake() {
        if (instance == null) {
            DontDestroyOnLoad(gameObject);
            instance = this;
        } else {
            Destroy(gameObject);
        }
        curGun = PlayerGun.Basic;
    }

    public int ManaCost() {
        switch (curGun) {
            case PlayerGun.Basic:
                return 5;
            default:
                return 5;
        }
    }

    public bool PlayerHit() {
        int oldHP = hp;
        hp -= 1;
        if (hp <= 0 && oldHP > 0) {
            MenuScreenManager.instance.NotifyOfDeath();
            return true;
        } else {
            return false;
        }
    }

    public void EnemyHit(int damage, int healthLeft, Vector3 pos) {
        var text = Instantiate(damageText, pos + new Vector3(0, 0.75f, 0), Quaternion.identity);
        text.damage.text = damage.ToString();
        if (healthLeft <= 0) {
            mp += Random.Range(10, 20);
            mp = Mathf.Min(mp, maxMp);
        }
    }

    public int PlayerAttackVal() {
        return Random.Range(atk / 2, (int) (atk * 1.5));
    }

    private void FixedUpdate() {
        mp += Mathf.Exp(1 * mp / (float)maxMp) * 0.1f;
        mp = Mathf.Min(mp, maxMp);
    }

}
