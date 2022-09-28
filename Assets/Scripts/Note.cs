using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Note : MonoBehaviour {
    public TMP_Text popupText;
    public TMP_Text popupTitle;
    public ItemDescription pickupText;
    public SpriteRenderer popup;
    private Rigidbody2D rbd;
    private float speedLow = 1.0f;
    private float speedHigh = 5.0f;
    public AudioClip click;
    public AudioClip paper;
    public int noteNumber;
    public bool isLore;
    private BoxCollider2D boxCollider;

    public GameObject death;

    void Awake() {
        rbd = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Start() {
        if (isLore) {
            noteNumber = Random.Range(0, GameManager.totalLoreNotes);
        }
        boxCollider.isTrigger = isLore;
        popupText.text = "";
        popupTitle.text = "";
        popup.enabled = false;
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
