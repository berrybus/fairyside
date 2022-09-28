using UnityEngine;
using UnityEngine.SceneManagement;

public enum SpellColor {
    // default
    White,
    // more shots
    Green,
    // piercing
    Blue,
    // damage mult
    Red,
    // bouncing
    Yellow,
    // homing
    Pink,
    // phasing
    Orange
}

public class PlayerManager: MonoBehaviour
{
    public static PlayerManager instance;
    [System.NonSerialized]
    public int hp = 5;
    [System.NonSerialized]
    public int maxHp = 5;
    [System.NonSerialized]
    public int startHp = 5;
    [System.NonSerialized]
    public float mp = 100;
    [System.NonSerialized]
    public int maxMp = 100;
    [System.NonSerialized]
    public int lvl = 1;
    [System.NonSerialized]
    public static int maxLvl = 10;
    [System.NonSerialized]
    public int atk = 3;
    [System.NonSerialized]
    public int regen = 1;
    [System.NonSerialized]
    public int castSpeed = 2;
    [System.NonSerialized]
    public int castSpeedCap = 10;
    [System.NonSerialized]
    public int luk = 0;
    [System.NonSerialized]
    public int speed = 2;
    [System.NonSerialized]
    public int numShots = 1;
    [System.NonSerialized]
    public float damageMult = 1f;
    [System.NonSerialized]
    public int shotSpeed = 6;
    [System.NonSerialized]
    public float knockbackMult = 1f;
    [System.NonSerialized]
    public float knockbackCap = 6f;
    [System.NonSerialized]
    public int range = 1;
    [System.NonSerialized]
    public int bulletSize = 1;
    [System.NonSerialized]
    public bool spellCanHone = false;
    [System.NonSerialized]
    public bool spellCanBounce = false;
    [System.NonSerialized]
    public bool spellCanPierce = false;
    [System.NonSerialized]
    public bool spellCanPhase = false;

    [System.NonSerialized]
    public int maxHPInc;
    [System.NonSerialized]
    public int mpRegenInc;
    [System.NonSerialized]
    public int speedInc;
    [System.NonSerialized]
    public int attackInc;
    [System.NonSerialized]
    public int luckInc;
    [System.NonSerialized]
    public int castSpdInc;
    [System.NonSerialized]
    public int shotSpdInc;
    [System.NonSerialized]
    public int rangeInc;
    [System.NonSerialized]
    public int knockbackInc;

    [System.NonSerialized]
    public int gemCount = 0;

    [System.NonSerialized]
    public int exp = 0;

    [System.NonSerialized]
    public SpellColor spellColor = SpellColor.White;

    public DamageText damageText;

    [System.NonSerialized]
    public float gameTime = 0;

    public void StartGame() {
        gemCount = 0;
        gameTime = 0;
        spellColor = SpellColor.White;
        SetupStartStats();
    }

    private void Awake() {
        if (instance == null) {
            DontDestroyOnLoad(gameObject);
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public int ManaCost() {
        return 5 + (int)(((numShots - 1) / 2f) * 5f);
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
    
    public void EnemyDie() {
        mp += Random.Range(10, 20);
        mp = Mathf.Min(mp, maxMp);
    }

    public (int, bool) EnemyHit() {
        float damage = Random.Range((int)(atk / 2), (int)(atk * 1.5));
        damage *= damageMult;
        bool didCrit = Random.Range(0.0f, 1.0f) <= luk * 0.07 + 0.05;
        if (didCrit) {
            int critMult = luk >= 3 ? 3 : 2;
            damage *= critMult;
        }
        int damageRet = (int) Mathf.Max(1, damage);
        return (damageRet, didCrit);
    }

    public int ExpToLevel() {
        return (int) (20 * Mathf.Exp(lvl / 3));
    }

    public bool AtMaxLvl() {
        return lvl >= maxLvl;
    }

    private void SetupStartStats() {
        hp = startHp;
        maxHp = startHp;
        mp = maxMp;
        atk = 3;
        regen = 1;
        castSpeed = 2;
        shotSpeed = 6;
        luk = 0;
        speed = 2;
        damageMult = 1f;
        knockbackMult = 1f;
        range = 1;
        bulletSize = 1;
        numShots = 1;
        spellCanBounce = false;
        spellCanHone = false;
        spellCanPierce = false;
        spellCanPhase = false;
        spellColor = SpellColor.White;
    }

    private void FixedUpdate() {
        mp += Mathf.Exp((1f + 0.01f * regen) * mp / (float)maxMp) * (0.1f + (0.02f * regen));
        mp = Mathf.Min(mp, maxMp);

        if (SceneManager.GetActiveScene().name == "Game") {
            gameTime += Time.deltaTime;
        }
    }

    public Color ParticleColor() {
        switch (spellColor) {
            case SpellColor.White:
                return Color.white;
            case SpellColor.Blue:
                return Color.cyan;
            case SpellColor.Red:
                return Color.red;
            case SpellColor.Green:
                return Color.green;
            case SpellColor.Pink:
                return Color.magenta;
            case SpellColor.Yellow:
                return Color.yellow;
            case SpellColor.Orange:
                return new Color(1.0f, 0.5f, 0.2f);
            default:
                return Color.white;
        }
    }

}