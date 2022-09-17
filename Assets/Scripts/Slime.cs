using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : BaseEnemy {
    public bool shouldFireSingle;

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
        if (firepoint) {
            StartCoroutine(ShootRoutine());
        }
    }

    protected override void FixedUpdate() {
        base.FixedUpdate();
        if (currentState == EnemyState.Move && movePattern != EnemyMovePattern.Stop) {
            var curDirection = VectorFromAngle(curAngle);
            rbd.AddForce(curDirection * moveForce);
        }
    }

    IEnumerator ShootRoutine() {
        while (true) {
            yield return new WaitForSeconds(Random.Range(1.0f, 2.0f));
            if (shouldFireSingle) {
                FireSingle();
                yield return new WaitForSeconds(Random.Range(3.0f, 4.0f));
            } else {
                FireDirectional();
                yield return new WaitForSeconds(Random.Range(1.0f, 3.0f));
            }
        }
    }

    private void FireDirectional() {
        if (firepoint == null) {
            return;
        }

        int offset = Random.Range(0f, 1f) <= 0.5f ? 0 : 45;

        int[] angles = new int[] { 0, 90, 180, 270 };

        foreach (int angle in angles) {
            Fire(VectorFromAngle(angle + offset));
        }
    }

    private void FireSingle() {
        if (firepoint == null) {
            return;
        }

        Fire((playerTarget.transform.position - transform.position).normalized);
    }

    private void OnCollisionStay2D(Collision2D collision) {
        ReverseMoveAngle();
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        BounceOff(collision);
    }

    IEnumerator MoveAndPause() {
        yield return new WaitForSeconds(Random.Range(1.0f, 3.0f));
        while (true) {
            switch (movePattern) {
                case EnemyMovePattern.Random:
                    movePattern = EnemyMovePattern.Stop;
                    yield return new WaitForSeconds(Random.Range(0.125f, 0.25f));
                    break;
                case EnemyMovePattern.Stop:
                    movePattern = EnemyMovePattern.Random;
                    yield return new WaitForSeconds(Random.Range(1.0f, 3.0f));
                    break;
                default:
                    yield return null;
                    break;
            }
        }
    }
}