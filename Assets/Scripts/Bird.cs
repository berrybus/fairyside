using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : BaseEnemy {
    protected override void OnEnable() {
        base.OnEnable();
        curAngle = 0;
        currentState = EnemyState.Unready;
        movePattern = EnemyMovePattern.Random;
    }

    protected override void StartActivity() {
        curAngle = GetRandom45Direction();

        if (currentState == EnemyState.Unready) {
            currentState = EnemyState.Move;
        }
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

    IEnumerator MoveAndPause() {
        yield return new WaitForSeconds(Random.Range(0.25f, 0.5f));
        while (true) {
            switch (movePattern) {
                case EnemyMovePattern.Random:
                    movePattern = EnemyMovePattern.Stop;
                    yield return new WaitForSeconds(Random.Range(0.125f, 0.25f));
                    break;
                case EnemyMovePattern.Stop:
                    movePattern = EnemyMovePattern.Random;
                    curAngle = Random.Range(0, 360);
                    yield return new WaitForSeconds(Random.Range(0.125f, 0.5f));
                    break;
                default:
                    yield return null;
                    break;
            }
        }
    }
}
