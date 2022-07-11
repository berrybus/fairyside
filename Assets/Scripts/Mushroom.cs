using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom : BaseEnemy {
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
        StartCoroutine(RandomMove());
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

    private void Update() {
        if (currentState == EnemyState.Move || currentState == EnemyState.Unready) {
            animator.speed = 1.0f;
        }
        else {
            animator.speed = 0.0f;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (canBounceOffCollision) {
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

    IEnumerator ShootRoutine() {
        while (true) {
            yield return new WaitForSeconds(Random.Range(1.0f, 4.0f));
            FireAtPlayer();
        }
    }

    private void FireAtPlayer() {
        GameObject bullet = ObjectPool.instance.enemyBullets.Get();
        if (bullet != null) {
            bullet.GetComponent<EnemyBullet>().direction = (playerTarget.position - transform.position).normalized;
            bullet.GetComponent<EnemyBullet>().speed = bulletSpeed;
            bullet.transform.position = firepoint.transform.position;
            bullet.transform.SetParent(transform, true);
            bullet.SetActive(true);
            // firepoint.Animate();
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
