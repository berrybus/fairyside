using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemDescription : MonoBehaviour {
    public TMP_Text description;
    [System.NonSerialized]
    public ShopItemType type;

    void Start() {
        StartCoroutine(FadeOut());
        StartCoroutine(MoveUp());

        switch (type) {
            case ShopItemType.Atk:
                description.text = "ATK Up!";
                break;
            case ShopItemType.Wis:
                description.text = "WIS Up!";
                break;
            case ShopItemType.Dex:
                description.text = "DEX Up!";
                break;
            case ShopItemType.Luk:
                description.text = "LUK Up!";
                break;
            case ShopItemType.Spd:
                description.text = "SPD Up!";
                break;
            case ShopItemType.HP:
                description.text = "HP Up!";
                break;
            case ShopItemType.HPRestore:
                description.text = "HP Full!";
                break;
            case ShopItemType.GrassGun:
                description.text = "New Spell!";
                break;
            case ShopItemType.WaterGun:
                description.text = "New Spell!";
                break;
            case ShopItemType.FireGun:
                description.text = "New Spell!";
                break;
            default:
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
