using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class Shopkeeper : MonoBehaviour {
    public ShopItem[] items;
    public ShopItem pickedItem;
    public TMP_Text priceDisplay;
    public Transform itemPosition;
    public Color red;
    // Start is called before the first frame update
    void Start() {
        pickedItem = items[Random.Range(0, items.Length)];
        Instantiate(pickedItem, itemPosition.position, Quaternion.identity);
    }

    // Update is called once per frame
    void Update() {
        if (pickedItem != null) {
            priceDisplay.text = pickedItem.price.ToString();
            if (PlayerManager.instance.gemCount < pickedItem.price) {
                priceDisplay.color = red;
            } else {
                priceDisplay.color = Color.white;
            }
        }

    }
}
