using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    private bool disabled = false;

    protected void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player") && !disabled) {
            SceneSwitcher.instance.NextLevel();
            disabled = true;
        }
    }
}
