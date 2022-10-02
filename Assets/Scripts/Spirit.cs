using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpiritVariant {
    Green,
    Yellow,
    Pink
}

public class Spirit : BaseEnemy {
    [SerializeField]
    private int minShots;
    [SerializeField]
    private int maxShots;

    public SpiritVariant variant;

    protected override void OnEnable() {
        base.OnEnable();
        curAngle = 0;
        targetAngle = 0;
        currentState = EnemyState.Unready;
        movePattern = EnemyMovePattern.Random;
    }

    protected override void StartActivity() {
        curAngle = GetRandom45Direction();
        targetAngle = GetRandom45Direction();

        if (currentState == EnemyState.Unready) {
            currentState = EnemyState.Move;
        }
        StartCoroutine(RandomMove());
        StartCoroutine(MoveAndPause());
        StartCoroutine(ShootRoutine());
    }

    protected override void Update() {
        base.Update();
        spriteRenderer.flipX = curAngle > 90 && curAngle < 270;
    }

    protected override void FixedUpdate() {
        base.FixedUpdate();
        if (currentState == EnemyState.Move && movePattern != EnemyMovePattern.Stop) {
            var curDirection = VectorFromAngle(curAngle);
            rbd.AddForce(curDirection * moveForce);
        }
    }

    IEnumerator ShootRoutine() {
        yield return new WaitForSeconds(Random.Range(0.5f, 1.0f));
        while (true) {
            switch (variant) {
                case SpiritVariant.Green:
                    yield return FireHoming();
                    break;
                case SpiritVariant.Pink:
                    yield return FireAlmostCircle();
                    yield return new WaitForSeconds(Random.Range(0.5f, 1.0f));
                    break;
                case SpiritVariant.Yellow:
                    yield return FireArrow();
                    break;
            }
            yield return new WaitForSeconds(Random.Range(1.0f, 4.0f));
        }
    }

    private void OnCollisionStay2D(Collision2D collision) {
        ReverseMoveAngle();
    }

    protected void OnCollisionEnter2D(Collision2D collision) {
        BounceOff(collision);
    }

    IEnumerator FireHoming() {
        for (int i = 0; i < Random.Range(minShots, maxShots); i++) {
            Fire(Vector2.zero, bulletSpeed, playerTarget);
            yield return new WaitForSeconds(bulletFireDelay * 2);
        }
    }

    IEnumerator FireAlmostCircle() {
        float startAngle = AngleFromVector((playerTarget.transform.position - transform.position).normalized);
        for (int j = 0; j < 3; j++) {
            for (int i = 1; i <= 11; i++) {
                float newAngle = startAngle + 30 * i;
                Fire(VectorFromAngle(newAngle));
            }
            yield return new WaitForSeconds(bulletFireDelay);
        }
    }

    IEnumerator FireArrow() {
        float startAngle = AngleFromVector((playerTarget.transform.position - transform.position).normalized);
        float angleLeft = startAngle + 2;
        float angleRight = startAngle - 2;
        FireWithMoreTime(VectorFromAngle(startAngle), 1.0f);
        yield return new WaitForSeconds(bulletFireDelay);
        FireWithMoreTime(VectorFromAngle(angleLeft), 1.0f);
        FireWithMoreTime(VectorFromAngle(angleRight), 1.0f);
    }

    IEnumerator MoveAndPause() {
        yield return new WaitForSeconds(Random.Range(2.0f, 3.0f));
        while (true) {
            switch (movePattern) {
                case EnemyMovePattern.Random:
                    movePattern = EnemyMovePattern.Stop;
                    yield return new WaitForSeconds(Random.Range(0.25f, 0.5f));
                    break;
                case EnemyMovePattern.Stop:
                    movePattern = EnemyMovePattern.Random;
                    yield return new WaitForSeconds(Random.Range(2.0f, 3.0f));
                    break;
                default:
                    yield return null;
                    break;
            }
        }
    }
}