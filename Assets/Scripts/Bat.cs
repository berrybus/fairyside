using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : BaseEnemy {
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
        StartCoroutine(MoveAndPause());
        StartCoroutine(ShootRoutine());
    }

    protected override void FixedUpdate() {
        base.FixedUpdate();
        if (currentState == EnemyState.Move) {
            var curDirection = Vector2.zero;
            if (movePattern == EnemyMovePattern.Random) {
                curDirection = VectorFromAngle(curAngle);
            } else if (movePattern == EnemyMovePattern.Follow) {
                curDirection = (playerTarget.transform.position - transform.position).normalized;
                curAngle = AngleFromVector(curDirection);
            }
            rbd.AddForce(curDirection * moveForce);
        }
    }

    IEnumerator ShootRoutine() {
        while (true) {
            yield return new WaitForSeconds(Random.Range(1.0f, 2.0f));
            var angles = new float[] { 0, 90, 180, 270 };
            for (int i = 0; i < Random.Range(minShots, maxShots); i++) {
                FireAngles(angles);
                yield return new WaitForSeconds(0.25f);
            }
            yield return new WaitForSeconds(Random.Range(1.0f, 2.0f));
            angles = new float[] { 45, 135, 225, 315 };
            for (int i = 0; i < Random.Range(minShots, maxShots); i++) {
                FireAngles(angles);
                yield return new WaitForSeconds(0.25f);
            }
            yield return new WaitForSeconds(Random.Range(2.0f, 3.0f));
            angles = new float[] { 0, 45, 90, 135, 180, 225, 270, 315 };
            for (int i = 0; i < Random.Range(minShots, maxShots); i++) {
                FireAngles(angles);
                yield return new WaitForSeconds(0.25f);
            }
            yield return new WaitForSeconds(Random.Range(3.0f, 4.0f));
        }
    }

    private void OnCollisionStay2D(Collision2D collision) {
        ReverseMoveAngle();
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Player")
            && movePattern == EnemyMovePattern.Follow) {
            movePattern = EnemyMovePattern.Stop;
            StartCoroutine(EnableRandomMovement());
            return;
        }

        BounceOff(collision);
    }

    IEnumerator EnableRandomMovement() {
        yield return new WaitForSeconds(Random.Range(0.5f, 0.75f));
        movePattern = EnemyMovePattern.Random;
    }

    IEnumerator MoveAndPause() {
        yield return new WaitForSeconds(Random.Range(1.0f, 3.0f));
        while (true) {
            switch (movePattern) {
                case EnemyMovePattern.Random:
                    movePattern = EnemyMovePattern.Follow;
                    yield return new WaitForSeconds(Random.Range(0.5f, 1.0f));
                    break;
                case EnemyMovePattern.Follow:
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