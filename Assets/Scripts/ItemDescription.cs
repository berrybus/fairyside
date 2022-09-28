using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemDescription : MonoBehaviour {
    public TMP_Text description;
    [System.NonSerialized]
    public ShopItemType type;
    public string overrideText = "";

    void Start() {
        StartCoroutine(FadeOut());
        StartCoroutine(MoveUp());

        if (overrideText != "") {
            description.text = overrideText;
            return;
        }

        switch (type) {
            case ShopItemType.Atk:
                description.text = "Attack up!";
                break;
            case ShopItemType.Wis:
                description.text = "Regen up!";
                break;
            case ShopItemType.CastSpd:
                description.text = "Cast spd up!";
                break;
            case ShopItemType.Luk:
                description.text = "Luck up!";
                break;
            case ShopItemType.Spd:
                description.text = "Speed up!";
                break;
            case ShopItemType.HP:
                description.text = "HP up!";
                break;
            case ShopItemType.HPRestore:
                description.text = "HP full!";
                break;
            case ShopItemType.DmgMult:
                description.text = "Dmg mult up!";
                break;
            case ShopItemType.SpellCount:
                description.text = "Spell count up!";
                break;
            case ShopItemType.ShotSpd:
                description.text = "Shot spd up!";
                break;
            case ShopItemType.ShotSpdDown:
                description.text = "Shot spd down!";
                break;
            case ShopItemType.Knockback:
                description.text = "Knockback up!";
                break;
            case ShopItemType.Bouncing:
                description.text = "Bouncing spells!";
                break;
            case ShopItemType.Homing:
                description.text = "Homing spells!";
                break;
            case ShopItemType.Phasing:
                description.text = "Phasing spells!";
                break;
            case ShopItemType.Piercing:
                description.text = "Piercing spells!";
                break;
            case ShopItemType.Range:
                description.text = "Range up!";
                break;
            case ShopItemType.SizeUp:
                description.text = "Size up!";
                break;
        }
    }

    IEnumerator FadeOut() {
        yield return new WaitForSeconds(0.5f);
        while (description.alpha >= 0) {
            description.alpha -= 0.75f * Time.deltaTime;
            description.alpha = Mathf.Max(0f, description.alpha);
            yield return null;
        }
        Destroy(gameObject);
    }

    IEnumerator MoveUp() {
        yield return new WaitForSeconds(0.5f);
        while (true) {
            transform.position += new Vector3(0, 4f / 16f * Time.deltaTime, 0);
            yield return null;
        }
    }
}
