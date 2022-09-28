using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom : BaseEnemy {
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

    IEnumerator ShootRoutine() {
        while (true) {
            yield return new WaitForSeconds(Random.Range(1.0f, 4.0f));
            int numShots = Random.Range(minShots, maxShots);
            Vector2 target = playerTarget.position - transform.position;
            for (int i = 0; i < numShots; i++) {
                Fire(target);
                yield return new WaitForSeconds(0.1f);
            }
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
