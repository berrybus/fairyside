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

    public override void RemoveSelf() {
        if (!released) {
            ObjectPool.instance.grassBullets.Release(gameObject);
            GameObject particle = ObjectPool.instance.grassBulletParticles.Get();
            if (particle != null) {
                particle.transform.position = transform.position;
                particle.GetComponent<ParticleSystem>().Play();
            }
            released = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Door")) {
            RemoveSelf();
        }
    }
}
