using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Note : MagneticItem {
    public TMP_Text popupText;
    public TMP_Text popupTitle;
    public ItemDescription pickupText;
    public SpriteRenderer popup;
    private float speedLow = 1.0f;
    private float speedHigh = 5.0f;
    public AudioClip click;
    public AudioClip paper;
    public int noteNumber;
    public bool isLore;

    public GameObject death;

    private void Start() {
        if (isLore) {
            if (PlayerManager.instance.writerDead) {
                noteNumber = 0;
            } else {
                noteNumber = Random.Range(1, GameManager.totalLoreNotes);
            }
        }
        boxCollider.isTrigger = isLore;
        popupText.text = "";
        popupTitle.text = "";
        popup.enabled = false;
        canPickup = !isLore;
    }

    public void Scatter(Vector2 dir) {
        rbd.AddForce(dir * Random.Range(speedLow, speedHigh), ForceMode2D.Impulse);
    }

    public void SetIsLore(bool _isLore) {
        isLore = _isLore;
        boxCollider.isTrigger = isLore;
    }

    private string GetPopupText() {
        if (isLore) {
            return NoteRepository.loreNotes[noteNumber].description;
        } else {
            return NoteRepository.monsterNotes[noteNumber].description;
        }
    }

    private string GetPopupTitle() {
        if (isLore) {
            return NoteRepository.loreNotes[noteNumber].name;
        } else {
            return NoteRepository.monsterNotes[noteNumber].name;
        }
    }

    public void PickUp() {
        if (isLore) {
            GameManager.instance.foundLoreNotes[noteNumber] = true;
            GameManager.instance.SaveLore();
        } else {
            GameManager.instance.foundMonsterNotes[noteNumber] = true;

        }
        ItemDescription pickup = Instantiate(pickupText, transform.position, Quaternion.identity);
        pickup.overrideText = "Found note!";
        GameManager.instance.PlaySFX(paper);
        Instantiate(death, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            popup.enabled = true;
            popupText.text = GetPopupText() + "\n(SPACE to pick up)";
            popupTitle.text = "A note on: " + GetPopupTitle();
            GameManager.instance.PlaySFX(click);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            PickUp();
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            popupText.text = "";
            popupTitle.text = "";
            popup.enabled = false;
        }
    }

}
