using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public int maxLvl = 10;
    [System.NonSerialized]
    public int atk = 3;
    [System.NonSerialized]
    public int wis = 1;
    [System.NonSerialized]
    public int dex = 0;
    [System.NonSerialized]
    public int luk = 0;
    [System.NonSerialized]
    public int spd = 2;

    [System.NonSerialized]
    public int gemCount = 0;

    [System.NonSerialized]
    public int exp = 0;

    [System.NonSerialized]
    public PlayerGun curGun;

    public Sprite basicGunSprite;
    public Sprite grassGunSprite;
    public Sprite waterGunSprite;
    public Sprite fireGunSprite;

    public DamageText damageText;

    [System.NonSerialized]
    public float gameTime = 0;

    public void StartGame() {
        hp = maxHp;
        mp = maxMp;
        gemCount = 0;
        curGun = PlayerGun.Basic;
        gameTime = 0;
        setupStartStats();
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
                return 7;
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

    public int EnemyHit(int healthLeft, Vector3 pos, bool displayOnTop) {
        float damage = Random.Range((int)(atk / 2), (int)(atk * 1.5));
        damage *= GunDamageMultiplier();
        bool didCrit = Random.Range(0.0f, 1.0f) <= luk * 0.05 + 0.05;
        if (didCrit) {
            int critMult = luk >= 5 ? 3 : 2;
            damage *= critMult;
        }
        damage = (int) Mathf.Max(1, damage);
        float textOffset = displayOnTop ? 0.50f : 0.0f;
        var text = Instantiate(damageText, pos + new Vector3(0, 0.50f + textOffset, 0), Quaternion.identity);
        text.damage.text = damage.ToString();
        if (didCrit) {
            text.damage.fontSize = 10;
            text.damage.color = new Color(255f / 255f, 212f / 255f, 95f / 255f, 1.0f);
        }
        if (healthLeft <= 0) {
            mp += Random.Range(10, 20);
            mp = Mathf.Min(mp, maxMp);
        }
        return (int) damage;
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

    public int expToLevel() {
        return (int) (20 * Mathf.Exp(lvl / 4f));
    }

    public bool atMaxLvl() {
        return lvl >= maxLvl;
    }

    private void setupStartStats() {
        atk = 3 + (lvl + 1) / 5;
        wis = 1 + (lvl + 3) / 5;
        dex = 0 + lvl / 5;
        luk = 0 + (lvl - 1) / 5;
        spd = 2 + (lvl + 2) / 5;
    }

    private void FixedUpdate() {
        mp += Mathf.Exp((1f + 0.01f * wis) * mp / (float)maxMp) * (0.1f + (0.02f * wis));
        mp = Mathf.Min(mp, maxMp);

        if (System.Array.Exists(SceneSwitcher.levels, level => level == SceneManager.GetActiveScene().name)) {
            gameTime += Time.deltaTime;
        }
    }

}