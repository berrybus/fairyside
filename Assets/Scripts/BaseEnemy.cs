using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState {
    Move,
    Knockback,
    Unready
}

public enum EnemyMovePattern {
    Random,
    Follow,
    Stop,
}

public class BaseEnemy : MonoBehaviour {
    protected Rigidbody2D rbd;
    protected SpriteRenderer spriteRenderer;
    protected Animator animator;

    protected EnemyState currentState;
    protected EnemyMovePattern movePattern;

    [System.NonSerialized]
    public Transform playerTarget;
    [System.NonSerialized]
    public Room room;

    public GameObject enemyDeath;
    public GameObject portal;
    public Transform origin;
    public bool isBoss;

    protected Vector2 knockback = Vector2.zero;
    protected bool canChangeAngle = true;

    protected float curAngle;
    protected float targetAngle;

    [SerializeField]
    protected float angleMoveThreshold;
    [SerializeField]
    protected float moveForce;
    [SerializeField]
    private Gem gem;
    [SerializeField]
    private HPDrop hpDrop;
    [SerializeField]
    private float knockbackForce;
    [SerializeField]
    protected EnemyFirepoint firepoint;
    [SerializeField]
    protected float bulletSpeed;
    [SerializeField]
    protected float bulletFireDelay;
    [SerializeField]
    private int hp;
    [SerializeField]
    private int minGems;
    [SerializeField]
    private int maxGems;
    [SerializeField]
    private Transform hpBar;

    private int maxHp;

    protected virtual void Awake() {
        currentState = EnemyState.Move;
        rbd = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        gameObject.SetActive(false);
        maxHp = hp;
    }

    protected virtual void OnEnable() {
        StartCoroutine(SpawnEnemy());
    }

    IEnumerator SpawnEnemy() {
        transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        while (Mathf.Abs(transform.localScale.magnitude - Vector3.one.magnitude) > 0.02f) {
            float increase = 2.0f * Time.deltaTime;
            transform.localScale += new Vector3(increase, increase, increase);
            transform.localScale = Vector3.ClampMagnitude(transform.localScale, Vector3.one.magnitude);
            yield return null;
        }
        transform.localScale = Vector3.one;
        yield return EnableEnemy();
    }

    IEnumerator EnableEnemy() {
        yield return new WaitForSeconds(0.4f);
        StartActivity();
    }

    protected virtual void Update() {
        if (currentState == EnemyState.Move || currentState == EnemyState.Unready) {
            animator.speed = 1.0f;
        } else {
            animator.speed = 0.0f;
        }
        if (hpBar) {
            float hpPercentage = (float)hp / (float)maxHp;
            hpBar.localScale = new Vector3(hpPercentage, 1, 1);
        }
    }

    protected virtual void FixedUpdate() {
        if (currentState == EnemyState.Knockback) {
            rbd.AddForce(knockback * knockbackForce);
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Bullet") && hp > 0) {
            DamageSelf(collision.gameObject.GetComponent<Bullet>().displayOnTop);
            currentState = EnemyState.Knockback;
            rbd.velocity = Vector2.zero;
            knockback = transform.position - collision.transform.position;
            knockback = knockback.normalized;
            knockback *= PlayerManager.instance.GunKnockbackMultiplier();
            if (collision.gameObject.GetComponent<Bullet>().destroyOnEnemyImpact) {
                collision.gameObject.GetComponent<Bullet>().RemoveSelf();
            }
            StartCoroutine(ResumeMovement());
            spriteRenderer.color = new Color(1.0f, 0.6f, 0.5f, 1.0f);

            // Enemy death
            if (hp <= 0) {
                PlayerManager.instance.EnemyDie();
                room.numEnemies -= 1;
                room.OpenDoorsIfPossible();
                if (isBoss && portal != null) {
                    Instantiate(portal, origin.position, Quaternion.identity);
                }
                Instantiate(enemyDeath, origin.position, Quaternion.identity);

                // Create gems
                for (var i = 0; i < Random.Range(minGems, maxGems); i++) {
                    Vector3 position = origin.position + new Vector3(0, -0.25f, 0);
                    Gem newGem = Instantiate(
                        gem,
                        position + new Vector3(Random.Range(-0.25f, 0.25f), Random.Range(-0.25f, 0.25f), 0),
                        Quaternion.identity
                    );
                    newGem.Scatter(knockback);
                }
                // Create possible HP drop
                float dropThreshold = isBoss ? 1.5f : 0.05f;
                float hpKnockback = isBoss ? 6.0f : 1.0f;
                if (hpDrop != null && Random.Range(0f, 1f) <= dropThreshold) {
                    Vector3 position = origin.position + new Vector3(0, -0.25f, 0);
                    HPDrop hp = Instantiate(
                        hpDrop,
                        position + new Vector3(Random.Range(-0.25f, 0.25f), Random.Range(-0.25f, 0.25f), 0),
                        Quaternion.identity
                    );
                    hp.Scatter(knockback * hpKnockback);
                }

                Destroy(gameObject);
            }
        }
    }

    protected virtual void DamageSelf(bool displayOnTop) {
        var dmg = PlayerManager.instance.EnemyHit(hp, origin.position, displayOnTop);
        hp -= dmg;
    }

    protected virtual void StartActivity() { }

    protected void Fire(Transform target) {
        Fire(Vector2.zero, bulletSpeed, target);
    }

    protected void Fire(Vector2 direction) {
        Fire(direction, bulletSpeed, null);
    }

    protected void Fire(Vector2 direction, float speed) {
        Fire(direction, speed, null);
    }

    protected void Fire(Vector2 direction, float speed, Transform target) {
        GameObject bullet = ObjectPool.instance.enemyBullets.Get();
        if (bullet != null) {
            bullet.GetComponent<EnemyBullet>().direction = direction.normalized;
            bullet.GetComponent<EnemyBullet>().speed = speed;
            bullet.GetComponent<EnemyBullet>().playerTarget = target;
            bullet.transform.position = firepoint.transform.position;
            bullet.transform.SetParent(transform, true);
            bullet.SetActive(true);
        }
    }

    protected void FireAngles(float[] angles) {
        foreach (float angle in angles) {
            Fire(VectorFromAngle(angle));
        }
    }

    // Movement functions

    protected IEnumerator RandomMove() {
        while (true) {
            curAngle = Mathf.MoveTowardsAngle(curAngle, targetAngle, 20.0f * Time.fixedDeltaTime);
            curAngle %= 360;
            if (curAngle < 0) {
                curAngle += 360;
            }
            if (Mathf.Abs(curAngle - targetAngle) <= angleMoveThreshold) {
                curAngle = targetAngle;
                yield return new WaitForSeconds(Random.Range(0.5f, 2.0f));
                if (canChangeAngle) {
                    targetAngle = GetRandom45Direction();
                }
            }
            yield return null;
        }
    }

    protected void ReverseMoveAngle() {
        if (canChangeAngle) {
            canChangeAngle = false;
            curAngle += 180;
            curAngle %= 360;
            targetAngle = curAngle;
            StartCoroutine(EnableAngleChange());
        }
    }

    protected void BounceOff(Collision2D collision) {
        if (canChangeAngle) {
            canChangeAngle = false;
            var bounceDirection = Vector2.Reflect(VectorFromAngle(curAngle), collision.GetContact(0).normal);
            curAngle = AngleFromVector(bounceDirection);
            targetAngle = curAngle;
            StartCoroutine(EnableAngleChange());
        }
    }

    protected IEnumerator EnableAngleChange() {
        yield return new WaitForSeconds(0.25f);
        canChangeAngle = true;
    }

    IEnumerator ResumeMovement() {
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        knockback = Vector2.zero;
        currentState = EnemyState.Move;
    }

    protected float GetRandom45Direction() {
        float[] angles = { 0, 45, 90, 135, 180, 225, 270, 315 };
        return angles[Random.Range(0, angles.Length)];
    }

    protected Vector2 VectorFromAngle(float angle) {
        return (Vector2)(Quaternion.Euler(0, 0, angle) * Vector2.right).normalized;
    }

    protected float AngleFromVector(Vector2 direction) {
        float angle = Vector2.SignedAngle(Vector2.right, direction);
        return angle % 360;
    }
}