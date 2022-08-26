using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : BaseEnemy {
    private float curAngle;
    private float targetAngle;
    private bool canBounceOffCollision = true;
    public Animator animator;

    [SerializeField]
    private float angleMove;
    [SerializeField]
    private int MaxHP = 100;
    [SerializeField]
    private float moveForce;
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
        currentState = EnemyState.Pause;
    }

    protected override void StartActivity() {
        curAngle = GetRandomDirection();
        targetAngle = GetRandomDirection();

        if (currentState == EnemyState.Pause) {
            currentState = EnemyState.Move;
        }
        StartCoroutine(RandomMove());
        StartCoroutine(MoveAndPause());
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
        if (currentState == EnemyState.Move || currentState == EnemyState.Pause) {
            animator.speed = 1.0f;
        } else {
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

    IEnumerator MoveAndPause() {
        yield return new WaitForSeconds(Random.Range(1.0f, 3.0f));
        while (true) {
            if (currentState == EnemyState.Move) {
                currentState = EnemyState.Pause;
                yield return new WaitForSeconds(Random.Range(0.125f, 0.25f));
            } else if (currentState == EnemyState.Pause) {
                currentState = EnemyState.Move;
                yield return new WaitForSeconds(Random.Range(1.0f, 3.0f));
            } else {
                // Knockback, so wait for that to finish before trying to change states
                yield return new WaitForSeconds(Random.Range(2.0f, 3.0f));
            }
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
