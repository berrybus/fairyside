using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour {
    public float speed;
    public Rigidbody2D rbd;
    public Vector2 direction;
    private bool released = false;
    private Animator animator;
    [System.NonSerialized]
    public Transform playerTarget;
    private bool canHone = false;

    private void Awake() {
        rbd = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void OnEnable() {
        released = false;
        canHone = false;
        StartCoroutine(RemoveSelfAfterTimePeriod());
        StartCoroutine(EnableMove());
    }

    IEnumerator RemoveSelfAfterTimePeriod() {
        yield return new WaitForSeconds(10.0f);
        RemoveSelf();
    }

    private void FixedUpdate() {
        if (playerTarget && canHone) {
            var distance = playerTarget.transform.position - transform.position;
            if (distance.magnitude > 1f) {
                direction = distance.normalized;
                rbd.velocity = direction * speed;
            } else {
                canHone = false;
            }
        }
    }

    IEnumerator DisableHoning() {
        yield return new WaitForSeconds(1.5f);
        canHone = false;
    }

    IEnumerator EnableMove() {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        rbd.velocity = direction * speed;
        transform.SetParent(null);
        canHone = true;
        StartCoroutine(DisableHoning());
        animator.Play("EnemyBulletContract");
    }

    public void RemoveSelf() {
        if (!released) {
            transform.SetParent(null);
            ObjectPool.instance.enemyBullets.Release(gameObject);
            GameObject particle = ObjectPool.instance.enemyBulletParticles.Get();
            if (particle != null) {
                particle.transform.position = transform.position;
                particle.GetComponent<ParticleSystem>().Play();
            }
            released = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Wall") ||
            collision.gameObject.CompareTag("Door")) {
            RemoveSelf();
        }
    }
}
