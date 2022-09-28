using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassBullet : Bullet {
    private void FixedUpdate() {
        var dist = (gameObject.transform.position - base.initialPosition).magnitude;
        if (dist >= 120f / 16f) {
            RemoveSelf();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Door")) {
            RemoveSelf();
        }
    }
}
