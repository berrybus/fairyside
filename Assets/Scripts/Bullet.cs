using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    public float speed;
    public Rigidbody2D rbd;
    public Vector2 direction;
    public Vector2 playerDirection;
    private bool released = false;

    private void Awake() {
        rbd = GetComponent<Rigidbody2D>();
    }
    private void OnEnable() {
        rbd.velocity = direction * speed + playerDirection * speed / 4;
        released = false;
        StartCoroutine(RemoveSelfAfterTimePeriod());
    }

    IEnumerator RemoveSelfAfterTimePeriod() {
        yield return new WaitForSeconds(10.0f);
        RemoveSelf();
        // particle.Play();
    }

    public void RemoveSelf() {
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
