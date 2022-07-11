using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatBoss : BaseEnemy {
    private float curAngle;
    private float targetAngle;
    private bool canBounceOffCollision = true;
    public Animator animator;

    [SerializeField]
    private float angleMove;
    [SerializeField]
    private int MaxHP = 120;
    [SerializeField]
    private float moveForce;
    [SerializeField]
    private EnemyFirepoint firepoint;
    [SerializeField]
    private float bulletSpeed;
    [SerializeField]
    private int maxGemCount;
    [SerializeField]
    private int minGemCount;

    protected override void Awake() {
        base.Awake();
        animator = GetComponent<Animator>();
    }

    private void Start() {
        base.hp = MaxHP;
        base.maxGems = maxGemCount;
        base.minGems = minGemCount;
        base.isBoss = true;
    }

    protected override void OnEnable() {
        base.OnEnable();
        curAngle = 0;
        targetAngle = 0;
        currentState = EnemyState.Unready;
    }

    protected override void StartActivity() {
        curAngle = GetRandomDirection();
        targetAngle = GetRandomDirection();

        if (currentState == EnemyState.Unready) {
            currentState = EnemyState.Move;
        }
        StartCoroutine(FollowPlayer());
        StartCoroutine(ShootRoutine());
    }

    protected override void FixedUpdate() {
        base.FixedUpdate();
        if (currentState == EnemyState.Move) {
            var curDirection = (Vector2)(Quaternion.Euler(0, 0, curAngle) * Vector2.right).normalized;
            rbd.AddForce(curDirection * moveForce);
        }
    }

    IEnumerator RandomMove() {
        while (true) {
            curAngle = Mathf.MoveTowardsAngle(curAngle, targetAngle, 20.0f * Time.fixedDeltaTime);
            curAngle %= 360;
            if (curAngle < 0) {
                curAngle += 360;
            }
            if (Mathf.Abs(curAngle - targetAngle) <= angleMove + 5.0f) {
                curAngle = targetAngle;
                yield return new WaitForSeconds(Random.Range(0.5f, 2.0f));
                targetAngle = GetRandomDirection();
            }
            yield return null;
        }
    }

    IEnumerator FollowPlayer() {
        while (true) {
            Vector2 direction = playerTarget.transform.position - gameObject.transform.position;
            direction = direction.normalized;
            curAngle = Vector2.SignedAngle(Vector2.right, direction);
            if (curAngle < 0) {
                curAngle += 360f;
            }
            yield return null;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (canBounceOffCollision && false) {
            curAngle = (curAngle + Random.Range(150, 210)) % 360;
            targetAngle = curAngle;
            canBounceOffCollision = false;
            StartCoroutine(EnableCollisionBounce());
        }
    }

    IEnumerator EnableCollisionBounce() {
        yield return new WaitForSeconds(1.0f);
        canBounceOffCollision = true;
    }

    private void FireBullets(float[] angles) {
        foreach (float angle in angles) {
            GameObject bullet = ObjectPool.instance.enemyBullets.Get();
            if (bullet == null) {
                continue;
            }
            var direction = (Vector2)(Quaternion.Euler(0, 0, angle) * Vector2.right).normalized;
            bullet.GetComponent<EnemyBullet>().direction = direction;
            bullet.GetComponent<EnemyBullet>().speed = bulletSpeed;
            bullet.transform.position = firepoint.transform.position;
            bullet.transform.SetParent(transform, true);
            bullet.SetActive(true);
        }
    }

    IEnumerator ShootRoutine() {
        while (true) {
            // Shoot 4 cardinals
            yield return new WaitForSeconds(Random.Range(2.0f, 4.0f));
            currentState = currentState == EnemyState.Knockback ? EnemyState.Knockback : EnemyState.Unready;
            float[] angles_90 = { 0, 90, 180, 270 };
            FireBullets(angles_90);

            // Shoot 4 directionals 
            yield return new WaitForSeconds(Random.Range(0.5f, 1.0f));
            float[] angles_diag = { 45, 135, 225, 315 };
            FireBullets(angles_diag);

            // Shoot 8 angles
            yield return new WaitForSeconds(Random.Range(0.5f, 1.0f));
            float[] angles = { 0, 45, 90, 135, 180, 225, 270, 315 };
            FireBullets(angles);

            currentState = currentState == EnemyState.Knockback ? EnemyState.Knockback : EnemyState.Move;

            // Same thing but in reverse
            yield return new WaitForSeconds(Random.Range(2.0f, 4.0f));

            currentState = currentState == EnemyState.Knockback ? EnemyState.Knockback : EnemyState.Unready;
            FireBullets(angles);
            yield return new WaitForSeconds(Random.Range(0.5f, 1.0f));
            FireBullets(angles_diag);
            yield return new WaitForSeconds(Random.Range(0.5f, 1.0f));
            FireBullets(angles_90);
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision) {
        base.OnTriggerEnter2D(collision);
    }

    float GetRandomDirection() {
        float[] angles = { 0, 45, 90, 135, 180, 225, 270, 315 };
        return angles[Random.Range(0, angles.Length)];
    }
}
