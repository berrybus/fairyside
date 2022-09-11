using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShopItemType {
    Atk,
    Wis,
    Dex,
    Luk,
    Spd,
    HP,
    HPRestore,
    GrassGun,
    WaterGun,
    FireGun
}

public class ShopItem : MonoBehaviour {
    public int price;
    public GameObject enemyDeath;
    public ItemDescription itemDescription;
    public ShopItemType type;

    private void Activate() {
        switch (type) {
            case ShopItemType.Atk:
                PlayerManager.instance.atk += 1;
                break;
            case ShopItemType.Wis:
                PlayerManager.instance.wis += 1;
                break;
            case ShopItemType.Dex:
                PlayerManager.instance.dex += 1;
                break;
            case ShopItemType.Luk:
                PlayerManager.instance.luk += 1;
                break;
            case ShopItemType.Spd:
                PlayerManager.instance.spd += 1;
                break;
            case ShopItemType.HP:
                PlayerManager.instance.hp += 1;
                PlayerManager.instance.maxHp += 1;
                break;
            case ShopItemType.HPRestore:
                PlayerManager.instance.hp = PlayerManager.instance.maxHp;
                break;
            case ShopItemType.GrassGun:
                PlayerManager.instance.curGun = PlayerGun.Grass;
                break;
            case ShopItemType.WaterGun:
                PlayerManager.instance.curGun = PlayerGun.Water;
                break;
            case ShopItemType.FireGun:
                PlayerManager.instance.curGun = PlayerGun.Fire;
                break;
            default:
                break;
        }
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player") && PlayerManager.instance.gemCount >= price) {
            PlayerManager.instance.gemCount -= price;
            Activate();
            Destroy(gameObject);
            GameObject poof = Instantiate(enemyDeath, transform.position, Quaternion.identity);
            poof.GetComponent<SpriteRenderer>().sortingOrder = 1;
            ItemDescription description = Instantiate(itemDescription, transform.position, Quaternion.identity);
            description.type = type;
        }
    }
}
