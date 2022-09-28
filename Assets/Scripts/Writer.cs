using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Writer : MonoBehaviour {
    public TMP_Text popupText;
    public SpriteRenderer popup;
    public AudioClip click;

    public string initialText;

    private string[] textBank = new string[] {
        "My notes, my notes...!",
        "My head hurts...",
        "Who are you...?",
        "I haven't finished writing yet!",
        "I lost everything!",
    };

    private void Start() {
        popupText.text = "";
        popup.enabled = false;
        initialText = textBank[Random.Range(0, textBank.Length)];
        int dir = Random.Range(0f, 1f) <= 0.5 ? -1 : 1;
        float yOffset = dir == 1 ? Random.Range(1, 1.5f) : Random.Range(1f, 2.5f);
        transform.position += dir * new Vector3(Random.Range(2f, 4f), yOffset, 0f);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            popup.enabled = true;
            popupText.text = initialText;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            popupText.text = "";
            popup.enabled = false;
        }
    }

}
