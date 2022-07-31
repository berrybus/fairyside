using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicBullet : Bullet {
    private void FixedUpdate() {
        var dist = (gameObject.transform.position - initialPosition).magnitude;
        if (dist >= 150f / 16f) {
            RemoveSelf();
        }
    }

    public override void RemoveSelf() {
        if (!released) {
            ObjectPool.instance.basicBullets.Release(gameObject);
            GameObject particle = ObjectPool.instance.basicBulletParticles.Get();
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
