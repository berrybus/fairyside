using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stump : BaseEnemy {
    [SerializeField]
    private int minShots;
    [SerializeField]
    private int maxShots;

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
        StartCoroutine(ShootRoutine());
        StartCoroutine(MoveAndPause());
    }

    protected override void FixedUpdate() {
        base.FixedUpdate();
        if (currentState == EnemyState.Move && movePattern != EnemyMovePattern.Stop) {
            var curDirection = (Vector2)(Quaternion.Euler(0, 0, curAngle) * Vector2.right).normalized;
            rbd.AddForce(curDirection * moveForce);
        }
    }

    private void OnCollisionStay2D(Collision2D collision) {
        ReverseMoveAngle();
    }

    protected void OnCollisionEnter2D(Collision2D collision) {
        BounceOff(collision);
    }

    IEnumerator FireAtPlayer() {
        var direction = (playerTarget.transform.position - transform.position).normalized;
        var dirAngle = AngleFromVector(direction);
        Fire(direction, bulletSpeed);
        yield return new WaitForSeconds(bulletFireDelay);
        for (int j = 1; j <= 2; j++) {
            var angle1 = dirAngle + j * 3;
            var angle2 = dirAngle + j * -3;
            Fire(VectorFromAngle(angle1), bulletSpeed);
            Fire(VectorFromAngle(angle2), bulletSpeed);
            yield return new WaitForSeconds(bulletFireDelay);
        }
        for (int j = 1; j >= 1; j--) {
            var angle1 = dirAngle + j * 3;
            var angle2 = dirAngle + j * -3;
            Fire(VectorFromAngle(angle1), bulletSpeed);
            Fire(VectorFromAngle(angle2), bulletSpeed);
            yield return new WaitForSeconds(bulletFireDelay);
        }
        Fire(direction, bulletSpeed);
    }

    IEnumerator ShootRoutine() {
        yield return new WaitForSeconds(Random.Range(0.5f, 1.0f));
        while (true) {
            yield return FireAtPlayer();
            yield return new WaitForSeconds(Random.Range(1.0f, 2.0f));
        }
    }

    IEnumerator MoveAndPause() {
        yield return new WaitForSeconds(Random.Range(2.0f, 3.0f));
        while (true) {
            switch (movePattern) {
                case EnemyMovePattern.Random:
                    movePattern = EnemyMovePattern.Stop;
                    yield return new WaitForSeconds(Random.Range(0.5f, 1.0f));
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
