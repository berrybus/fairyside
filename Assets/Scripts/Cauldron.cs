using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Cauldron : MonoBehaviour {
    public TMP_Text popupText;
    private string itemText;
    public SpriteRenderer popup;
    public AudioClip textPopupClip;

    private void Start() {
        if (popupText) {
            itemText = popupText.text;
            popupText.text = "";
            popup.enabled = false;
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
