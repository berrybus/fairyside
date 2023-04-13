using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatUpIndicator : MonoBehaviour {
    public TMP_Text text;
    [System.NonSerialized]
    public int newLvl;
    // Start is called before the first frame update
    void Start() {
        StartCoroutine(FadeOut());
        StartCoroutine(MoveUp());

        if ((newLvl - 1) % 5 == 0) {
            text.text = "Level Up!";
        } else if ((newLvl % 5) == 0) {
            text.text = "Level Up!";
        } else if ((newLvl + 1) % 5 == 0) {
            text.text = "Level Up!";
        } else if ((newLvl + 2) % 5 == 0) {
            text.text = "Level Up!";
        } else {
            text.text = "Level Up!";
        }
    }

    IEnumerator FadeOut() {
        yield return new WaitForSeconds(0.5f);
        while (text.alpha >= 0) {
            text.alpha -= 1.0f * Time.deltaTime;
            text.alpha = Mathf.Max(0f, text.alpha);
            yield return null;
        }
        Destroy(gameObject);
    }

    IEnumerator MoveUp() {
        yield return new WaitForSeconds(0.5f);
        while (true) {
            transform.position += new Vector3(0, 8f / 16f * Time.deltaTime, 0);
            yield return null;
        }
    }
}
