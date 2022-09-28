using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PregameMenu : UIScreen {

    public TMP_Text maxHP;
    public TMP_Text mpRegen;
    public TMP_Text speed;
    public TMP_Text dmgMult;
    public TMP_Text attack;
    public TMP_Text luck;
    public TMP_Text castSpd;
    public TMP_Text shotSpd;
    public TMP_Text range;
    public TMP_Text spellCount;
    public TMP_Text knockback;
    public TMP_Text level;

    public TMP_Text pointsLeft;

    public int maxHPInc;
    public int mpRegenInc;
    public int speedInc;
    public int attackInc;
    public int luckInc;
    public int castSpdInc;
    public int shotSpdInc;
    public int rangeInc;
    public int knockbackInc;

    public Sprite selectedArrow;
    public Sprite unSelectedArrow;

    public SpriteRenderer hpArrow;
    public SpriteRenderer regenArrow;
    public SpriteRenderer speedArrow;
    public SpriteRenderer attackArrow;
    public SpriteRenderer attackArrowRight;
    public SpriteRenderer luckArrow;
    public SpriteRenderer castSpdArrow;
    public SpriteRenderer castSpdArrowRight;
    public SpriteRenderer shotSpdArrow;
    public SpriteRenderer rangeArrow;
    public SpriteRenderer knockbackArrow;
    public SpriteRenderer knockbackArrowRight;

    private bool committedChanges = false;

    public AudioClip startClip;

    protected override void OnEnable() {
        base.OnEnable();
        if (PlayerManager.instance) {
            committedChanges = false;
            var info = PlayerManager.instance;
            maxHPInc = info.maxHPInc;
            mpRegenInc = info.mpRegenInc;
            speedInc = info.speedInc;
            attackInc = info.attackInc;
            luckInc = info.luckInc;
            castSpdInc = info.castSpdInc;
            shotSpdInc = info.shotSpdInc;
            rangeInc = info.rangeInc;
            knockbackInc = info.knockbackInc;
        }
    }
    
    private int PointsUsed() {
        return maxHPInc + mpRegenInc + speedInc + attackInc + luckInc + castSpdInc + shotSpdInc + rangeInc + knockbackInc;
    }

    protected override void SetTextColors() {
        for (int i = 0; i < options.Length; i++) {
            if (currentSelect == i) {
                options[i].color = Color.white;
                options[i].fontSize = 13;
            } else {
                options[i].color = deselect;
                options[i].fontSize = 12;
            }
        }
    }

    public override void MoveRight(InputAction.CallbackContext ctx) {
        if (!ctx.performed) {
            return;
        }

        if (PointsUsed() >= PlayerManager.instance.lvl) {
            return;
        }

        if (options[currentSelect].text == "Max HP") {
            maxHPInc += 1;
        } else if (options[currentSelect].text == "MP regen") {
            mpRegenInc += 1;
        } else if (options[currentSelect].text == "Speed") {
            speedInc += 1;
        } else if (options[currentSelect].text == "Attack") {
            if (attackInc < 2) {
                attackInc += 1;
            }
        } else if (options[currentSelect].text == "Luck") {
            luckInc += 1;
        } else if (options[currentSelect].text == "Cast spd") {
            if (castSpdInc < 8) {
                castSpdInc += 1;
            }
        } else if (options[currentSelect].text == "Shot spd") {
            shotSpdInc += 1;
        } else if (options[currentSelect].text == "Range") {
            rangeInc += 1;
        } else if (options[currentSelect].text == "Knockback") {
            if (knockbackInc < 5) {
                knockbackInc += 1;
            }
        }
    }

    public override void MoveLeft(InputAction.CallbackContext ctx) {
        if (!ctx.performed) {
            return;
        }
        if (options[currentSelect].text == "Max HP") {
            if (maxHPInc > 0) {
                maxHPInc -= 1;
            }
        } else if (options[currentSelect].text == "MP regen") {
            if (mpRegenInc > 0) {
                mpRegenInc -= 1;
            }
        } else if (options[currentSelect].text == "Speed") {
            if (speedInc > 0) {
                speedInc -= 1;
            }
        } else if (options[currentSelect].text == "Attack") {
            if (attackInc > 0) {
                attackInc -= 1;
            }
        } else if (options[currentSelect].text == "Luck") {
            if (luckInc > 0) {
                luckInc -= 1;
            }
        } else if (options[currentSelect].text == "Cast spd") {
            if (castSpdInc > 0) {
                castSpdInc -= 1;
            }
        } else if (options[currentSelect].text == "Shot spd") {
            if (shotSpdInc > 0) {
                shotSpdInc -= 1;
            }
        } else if (options[currentSelect].text == "Range") {
            if (rangeInc > 0) {
                rangeInc -= 1;
            }
        } else if (options[currentSelect].text == "Knockback") {
            if (knockbackInc > 0) {
                knockbackInc -= 1;
            }
        }

    }

    private void Apply() {
        if (committedChanges) {
            return;
        }

        PlayerManager.instance.maxHp += maxHPInc;
        PlayerManager.instance.hp = PlayerManager.instance.maxHp;
        PlayerManager.instance.regen += mpRegenInc;
        PlayerManager.instance.speed += speedInc;
        PlayerManager.instance.atk += attackInc;
        PlayerManager.instance.luk += luckInc;
        PlayerManager.instance.castSpeed += castSpdInc;
        PlayerManager.instance.shotSpeed += shotSpdInc;
        PlayerManager.instance.range += rangeInc;
        PlayerManager.instance.knockbackMult += knockbackInc;

        PlayerManager.instance.maxHPInc = maxHPInc;
        PlayerManager.instance.mpRegenInc = mpRegenInc;
        PlayerManager.instance.speedInc = speedInc;
        PlayerManager.instance.attackInc = attackInc;
        PlayerManager.instance.luckInc = luckInc;
        PlayerManager.instance.castSpdInc = castSpdInc;
        PlayerManager.instance.shotSpdInc = shotSpdInc;
        PlayerManager.instance.rangeInc = rangeInc;
        PlayerManager.instance.knockbackInc = knockbackInc;
        committedChanges = true;
    }


    private void Update() {
        if (committedChanges) {
            return;
        }

        var info = PlayerManager.instance;
        maxHP.text = (info.maxHp + maxHPInc).ToString();
        mpRegen.text = (info.regen + mpRegenInc).ToString();
        speed.text = (info.speed + speedInc).ToString();
        dmgMult.text = info.damageMult.ToString();
        attack.text = (info.atk + attackInc).ToString();
        luck.text = (info.luk + luckInc).ToString();
        castSpd.text = (info.castSpeed + castSpdInc).ToString();
        shotSpd.text = (info.shotSpeed + shotSpdInc).ToString();
        range.text = (info.range + rangeInc).ToString();
        spellCount.text = info.numShots.ToString();
        knockback.text = (info.knockbackMult + knockbackInc).ToString();
        level.text = "Level " + info.lvl;

        pointsLeft.text = "Points left: " + (info.lvl - PointsUsed());

        hpArrow.sprite = maxHPInc > 0 ? selectedArrow : unSelectedArrow;
        regenArrow.sprite = mpRegenInc > 0 ? selectedArrow : unSelectedArrow;
        speedArrow.sprite = speedInc > 0 ? selectedArrow : unSelectedArrow;
        attackArrow.sprite = attackInc > 0 ? selectedArrow : unSelectedArrow;
        luckArrow.sprite = luckInc > 0 ? selectedArrow : unSelectedArrow;
        castSpdArrow.sprite = castSpdInc > 0 ? selectedArrow : unSelectedArrow;
        shotSpdArrow.sprite = shotSpdInc > 0 ? selectedArrow : unSelectedArrow;
        rangeArrow.sprite = rangeInc > 0 ? selectedArrow : unSelectedArrow;
        knockbackArrow.sprite = knockbackInc > 0 ? selectedArrow : unSelectedArrow;
        attackArrowRight.sprite = attackInc < 2 ? selectedArrow : unSelectedArrow;
        castSpdArrowRight.sprite = castSpdInc < 8 ? selectedArrow : unSelectedArrow;
        knockbackArrowRight.sprite = knockbackInc < 5 ? selectedArrow : unSelectedArrow;
    }

    public override void Confirm(InputAction.CallbackContext ctx) {
        if (!ctx.performed) {
            return;
        }
        Apply();
        GameManager.instance.PlaySFX(startClip);
        GameManager.instance.StartGame();
    }
}
