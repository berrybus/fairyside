using System;

[System.Serializable]
public class RunSave {
    public int runLevel;
    public int maxHp;
    public int hp;
    public int atk = 3;
    public int regen = 1;
    public int castSpeed = 2;
    public int castSpeedCap = 10;
    public int luk = 0;
    public int speed = 2;
    public int numShots = 1;
    public float damageMult = 1f;
    public int shotSpeed = 6;
    public float knockbackMult = 1f;
    public int range = 1;
    public int bulletSize = 1;
    public bool spellCanHone;
    public bool spellCanBounce;
    public bool spellCanPierce;
    public bool spellCanPhase;
    public bool itemsAreMagnetic;
    public int gemCount = 0;
    public SpellColor spellColor;
    public float gameTime = 0;

    public RunSave(int runLevel, int maxHp, int hp, int atk, int regen, int castSpeed, int castSpeedCap, int luk, int speed, int numShots, float damageMult, int shotSpeed, float knockbackMult, int range, int bulletSize, bool spellCanHone, bool spellCanBounce, bool spellCanPierce, bool spellCanPhase, bool itemsAreMagnetic, int gemCount, SpellColor spellColor, float gameTime) {
        this.runLevel = runLevel;
        this.maxHp = maxHp;
        this.hp = hp;
        this.atk = atk;
        this.regen = regen;
        this.castSpeed = castSpeed;
        this.castSpeedCap = castSpeedCap;
        this.luk = luk;
        this.speed = speed;
        this.numShots = numShots;
        this.damageMult = damageMult;
        this.shotSpeed = shotSpeed;
        this.knockbackMult = knockbackMult;
        this.range = range;
        this.bulletSize = bulletSize;
        this.spellCanHone = spellCanHone;
        this.spellCanBounce = spellCanBounce;
        this.spellCanPierce = spellCanPierce;
        this.spellCanPhase = spellCanPhase;
        this.itemsAreMagnetic = itemsAreMagnetic;
        this.gemCount = gemCount;
        this.spellColor = spellColor;
        this.gameTime = gameTime;
    }
}
