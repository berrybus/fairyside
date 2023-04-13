using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CreditsManager : MonoBehaviour {
    public Transform credits;
    public float height;
    public AudioClip creation;

    private void Start() {
        StartCoroutine(AllowMove());
        GameManager.instance.PlayMusic(creation);
    }

    IEnumerator AllowMove() {
        yield return new WaitForSeconds(2.0f);
        StartCoroutine(Scroll());
    }

    IEnumerator Scroll() {
        while (credits.localPosition.y < height) {
            credits.localPosition += new Vector3(0f, 24f * Time.unscaledDeltaTime, 0f);
            yield return null;
        }

        yield return new WaitForSecondsRealtime(3.0f);
        GameManager.instance.GoToMenu();
    }

}
