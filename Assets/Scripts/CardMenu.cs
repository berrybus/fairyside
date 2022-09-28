using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class CardMenu : UIScreen {

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

    private void Update() {
        var info = PlayerManager.instance;
        maxHP.text = "Max HP: " + info.maxHp;
        mpRegen.text = "MP regen: " + info.regen;
        speed.text = "Speed: " + info.speed;
        dmgMult.text = "Dmg mult: " + info.damageMult;
        attack.text = "Attack: " + info.atk;
        luck.text = "Luck: " + info.luk;
        castSpd.text = "Cast spd: " + info.castSpeed;
        shotSpd.text = "Shot spd: " + info.shotSpeed;
        range.text = "Range: " + info.range;
        spellCount.text = "Spell count: " + info.numShots;
        knockback.text = "Knockback: " + info.knockbackMult;
        level.text = "Level " + info.lvl;
    }

    public override void MoveUp(InputAction.CallbackContext ctx) { }

    public override void MoveDown(InputAction.CallbackContext ctx) { }

    public override void Confirm(InputAction.CallbackContext ctx) {
        if (!ctx.performed) {
            return;
        }
        if (manager) {
            manager.Cancel(ctx);
        } else {
            Cancel(ctx);
        }
    }
}
