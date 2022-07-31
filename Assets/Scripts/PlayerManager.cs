using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerGun {
    Basic,
    Grass,
    Water,
    Fire
}

public class PlayerManager: MonoBehaviour
{
    public static PlayerManager instance;
    [System.NonSerialized]
    public int hp = 5;
    [System.NonSerialized]
    public int maxHp = 5;
    [System.NonSerialized]
    public float mp = 100;
    [System.NonSerialized]
    public int maxMp = 100;
    [System.NonSerialized]
    public int lvl = 1;
    [System.NonSerialized]
    public int atk = 3;
    [System.NonSerialized]
    public int wis = 1;
    [System.NonSerialized]
    public int dex = 1;
    [System.NonSerialized]
    public int luk = 0;
    [System.NonSerialized]
    public int spd = 6;

    [System.NonSerialized]
    public int gemCount = 0;

    [System.NonSerialized]
    public PlayerGun curGun;

    public Sprite basicGunSprite;
    public Sprite grassGunSprite;
    public Sprite waterGunSprite;
    public Sprite fireGunSprite;

    public DamageText damageText;

    public void StartGame() {
        hp = maxHp;
        mp = maxMp;
        gemCount = 0;
        curGun = PlayerGun.Basic;
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
                return 4;
            default:
                return 4;
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

    public void EnemyHit(int damage, int healthLeft, Vector3 pos, bool displayOnTop) {
        float textOffset = displayOnTop ? 0.75f : 0.0f;
        var text = Instantiate(damageText, pos + new Vector3(0, 0.75f + textOffset, 0), Quaternion.identity);
        text.damage.text = damage.ToString();
        if (Random.Range(0, 3) == 0) {
            text.damage.fontSize = 10;
            text.damage.color = new Color(255f / 255f, 212f / 255f, 95f / 255f, 1.0f);
        }
        if (healthLeft <= 0) {
            mp += Random.Range(10, 20);
            mp = Mathf.Min(mp, maxMp);
        }
    }

    public int PlayerAttackVal() {
        float damage = Random.Range((int) (atk / 2), (int) (atk * 1.5));
        damage *= GunDamageMultiplier();
        return (int)Mathf.Max(1, damage);
    }

    public float GunDamageMultiplier() {
        switch (curGun) {
            case PlayerGun.Basic:
                return 1f;
            case PlayerGun.Grass:
                return 1f;
            case PlayerGun.Water:
                return 0.75f;
            case PlayerGun.Fire:
                return 2f;
            default:
                return 1.0f;
        }
    }

    public float GunKnockbackMultiplier() {
        switch (curGun) {
            case PlayerGun.Basic:
                return 1f;
            case PlayerGun.Grass:
                return 1f;
            case PlayerGun.Water:
                return 0.0f;
            case PlayerGun.Fire:
                return 2f;
            default:
                return 1.0f;
        }
    }

    public Sprite CurrentGunSprite() {
        switch (curGun) {
            case PlayerGun.Basic:
                return basicGunSprite;
            case PlayerGun.Grass:
                return grassGunSprite;
            case PlayerGun.Water:
                return waterGunSprite;
            case PlayerGun.Fire:
                return fireGunSprite;
            default:
                return basicGunSprite;
        }
    }

    private void FixedUpdate() {
        mp += Mathf.Exp((1f + 0.01f * wis) * mp / (float)maxMp) * (0.1f + (0.02f * wis));
        mp = Mathf.Min(mp, maxMp);
    }

}