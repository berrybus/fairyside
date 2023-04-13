using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    private bool disabled = false;

    protected void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player") && !disabled) {
            collision.gameObject.GetComponent<Shirley>().WillChangeLevels();
            GameManager.instance.NextLevel();
            // GameManager.instance.RepeatRun();
            disabled = true;
        }
    }
}
