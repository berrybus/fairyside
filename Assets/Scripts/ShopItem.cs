using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum ShopItemType {
    Atk,
    Wis,
    Luk,
    Spd,
    HP,
    HPRestore,
    ShotSpd,
    CastSpd,
    DmgMult,
    Knockback,
    SpellCount,
    Homing,
    Phasing,
    Bouncing,
    ShotSpdDown,
    Piercing,
    Range,
    SizeUp
}

public class ShopItem : MonoBehaviour {
    public int price;
    public GameObject enemyDeath;
    public ItemDescription itemDescription;
    public ShopItemType type;
    public TMP_Text popupText;
    private string itemText;
    public SpriteRenderer popup;
    public AudioClip itemGetClip;
    public AudioClip textPopupClip;

    private void Start() {
        if (popupText) {
            itemText = popupText.text;
            popupText.text = "";
            popup.enabled = false;
        }
    }

    private void Activate() {
        switch (type) {
            case ShopItemType.Atk:
                PlayerManager.instance.atk += 1;
                break;
            case ShopItemType.Wis:
                PlayerManager.instance.regen += 1;
                break;
            case ShopItemType.CastSpd:
                PlayerManager.instance.castSpeed += 1;
                PlayerManager.instance.castSpeed = Mathf.Min(PlayerManager.instance.castSpeedCap, PlayerManager.instance.castSpeed);
                break;
            case ShopItemType.Luk:
                PlayerManager.instance.luk += 1;
                break;
            case ShopItemType.Spd:
                PlayerManager.instance.speed += 1;
                break;
            case ShopItemType.HP:
                PlayerManager.instance.hp += 1;
                PlayerManager.instance.maxHp += 1;
                break;
            case ShopItemType.HPRestore:
                PlayerManager.instance.hp = PlayerManager.instance.maxHp;
                break;
            case ShopItemType.ShotSpd:
                PlayerManager.instance.shotSpeed += 1;
                break;
            case ShopItemType.ShotSpdDown:
                PlayerManager.instance.shotSpeed -= 1;
                break;
            case ShopItemType.DmgMult:
                PlayerManager.instance.damageMult += 1;
                PlayerManager.instance.damageMult = Mathf.Min(PlayerManager.instance.damageMult, 4);
                PlayerManager.instance.castSpeed -= 1;
                PlayerManager.instance.spellColor = SpellColor.Red;
                break;
            case ShopItemType.SpellCount:
                PlayerManager.instance.numShots += 1;
                PlayerManager.instance.numShots = Mathf.Min(PlayerManager.instance.numShots, 4);
                PlayerManager.instance.spellColor = SpellColor.Green;
                break;
            case ShopItemType.Piercing:
                PlayerManager.instance.spellCanPierce = true;
                PlayerManager.instance.shotSpeed += 1;
                PlayerManager.instance.knockbackMult = 0;
                PlayerManager.instance.spellColor = SpellColor.Blue;
                break;
            case ShopItemType.Homing:
                PlayerManager.instance.spellCanHone = true;
                PlayerManager.instance.spellColor = SpellColor.Pink;
                break;
            case ShopItemType.Phasing:
                PlayerManager.instance.spellCanPhase = true;
                PlayerManager.instance.spellColor = SpellColor.Orange;
                break;
            case ShopItemType.Knockback:
                PlayerManager.instance.knockbackMult += 1;
                PlayerManager.instance.knockbackMult = Mathf.Min(PlayerManager.instance.knockbackMult, PlayerManager.instance.knockbackCap);
                break;
            case ShopItemType.Bouncing:
                PlayerManager.instance.spellCanBounce = true;
                PlayerManager.instance.spellColor = SpellColor.Yellow;
                break;
            case ShopItemType.Range:
                PlayerManager.instance.range += 1;
                break;
            case ShopItemType.SizeUp:
                PlayerManager.instance.bulletSize += 1;
                break;
        }
    }

    public void TryToPurchaseItem() {
        if (PlayerManager.instance.gemCount >= price) {
            PlayerManager.instance.gemCount -= price;
            Activate();
            Destroy(gameObject);
            GameObject poof = Instantiate(enemyDeath, transform.position, Quaternion.identity);
            poof.GetComponent<SpriteRenderer>().sortingOrder = 1;
            ItemDescription description = Instantiate(itemDescription, transform.position, Quaternion.identity);
            description.type = type;
            GameManager.instance.PlaySFX(itemGetClip);
        }
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            popup.enabled = true;
            popupText.text = itemText;
            GameManager.instance.PlaySFX(textPopupClip);
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            popupText.text = "";
            popup.enabled = false;
        }
    }
}
