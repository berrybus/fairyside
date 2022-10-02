using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Bullet : MonoBehaviour {
    public float speed;
    public Rigidbody2D rbd;
    public Vector2 initialDirection;
    public Vector2 playerDirection;
    private Vector2 direction;
    protected bool released = false;
    protected Vector3 initialPosition;
    private Collider2D enemyCollider;
    private Vector2 lastPosition;
    private float totalDist = 0;
    private Animator animator;

    [System.NonSerialized]
    public bool destroyOnEnemyImpact = true;
    [System.NonSerialized]
    public bool canBounce = false;
    [System.NonSerialized]
    public bool canHone = false;
    [System.NonSerialized]
    public bool canPhase = false;
    RaycastHit2D hit;

    public AudioClip obstacleClip;

    private void Awake() {
        rbd = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void OnEnable() {
        speed = PlayerManager.instance.shotSpeed * 2;
        direction = initialDirection + playerDirection * 0.25f;
        released = false;
        initialPosition = gameObject.transform.position;
        enemyCollider = null;
        totalDist = 0;
        lastPosition = transform.position;
        hit = new RaycastHit2D();
        PlayAnimation();
    }

    private void PlayAnimation() {
        switch (PlayerManager.instance.spellColor) {
            case SpellColor.White:
                animator.Play("BasicBullet");
                break;
            case SpellColor.Green:
                animator.Play("GreenBullet");
                break;
            case SpellColor.Blue:
                animator.Play("BlueBullet");
                break;
            case SpellColor.Red:
                animator.Play("RedBullet");
                break;
            case SpellColor.Pink:
                animator.Play("PinkBullet");
                break;
            case SpellColor.Yellow:
                animator.Play("YellowBullet");
                break;
            case SpellColor.Orange:
                animator.Play("OrangeBullet");
                break;
        }

    }

    private void FixedUpdate() {
        rbd.velocity = direction.normalized * speed;

        if (canHone) {
            if (enemyCollider == null) {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 2.0f);
                colliders = colliders.Where(c => c.gameObject.CompareTag("Enemy")).ToArray();
                if (colliders.Length > 0) {
                    Collider2D closest = colliders[0];
                    foreach (var collider in colliders) {
                        var currentDist = (transform.position - closest.bounds.center).magnitude;
                        var newDist = (transform.position - collider.bounds.center).magnitude;
                        if (newDist < currentDist) {
                            closest = collider;
                        }
                    }
                    enemyCollider = closest;
                    StartCoroutine(DisableHoning());
                }
            } else {
                direction = (enemyCollider.bounds.center - transform.position).normalized;
            }
        }

        totalDist += Vector2.Distance(lastPosition, transform.position);
        lastPosition = transform.position;

        if (canBounce) {
            hit = Physics2D.Raycast(transform.position, direction, transform.localScale.x * 2f);
        }

        if (totalDist >= (120f + 30f * (PlayerManager.instance.range)) / 16f) {
            RemoveSelf();
        }
    }

    IEnumerator DisableHoning() {
        yield return new WaitForSeconds(1.0f);
        canHone = false;
    }

    public void RemoveSelf() {
        if (!released) {
            ObjectPool.instance.bullets.Release(gameObject);
            GameObject particle = ObjectPool.instance.bulletParticles.Get();
            if (particle != null) {
                particle.transform.position = transform.position;
                particle.transform.localScale = Vector3.one * (PlayerManager.instance.bulletSize + 1) / 2f;
            }
            released = true;
            GameManager.instance.PlaySFX(obstacleClip);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Wall")
            || collision.gameObject.CompareTag("Door")
            || (collision.gameObject.CompareTag("Rock") && !canPhase)) {
            if (canBounce) {
                if (hit.normal != Vector2.zero) {
                    direction = Vector2.Reflect(direction, hit.normal).normalized;
                } else {
                    direction *= -1f;
                }
            } else {
                RemoveSelf();
            }
        } else if (collision.gameObject.CompareTag("Enemy")) {
            if (destroyOnEnemyImpact) {
                RemoveSelf();
            } else {
                canHone = false;
            }
        } else if (collision.gameObject.CompareTag("Writer")) {
            RemoveSelf();
        }
    }
}
