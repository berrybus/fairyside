using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StuffyClone : BaseEnemy {
    protected override void OnEnable() {
        base.OnEnable();
        curAngle = 0;
        targetAngle = 0;
        currentState = EnemyState.Unready;
        movePattern = EnemyMovePattern.Random;
    }

    protected override void FixedUpdate() {
        base.FixedUpdate();
        if (currentState == EnemyState.Move && movePattern == EnemyMovePattern.Follow) {
            var curDirection = (playerTarget.position - transform.position).normalized;
            spriteRenderer.flipX = curDirection.x < 0;
            rbd.AddForce(curDirection * moveForce);
        }
    }

    protected override void StartActivity() {
        if (currentState == EnemyState.Unready) {
            currentState = EnemyState.Move;
        }
        StartCoroutine(ChargeAndPause());
    }


    IEnumerator ChargeAndPause() {
        yield return new WaitForSeconds(Random.Range(0.5f, 1.0f));
        while (true) {
            switch (movePattern) {
                case EnemyMovePattern.Random:
                    movePattern = EnemyMovePattern.Follow;
                    yield return new WaitForSeconds(Random.Range(1.0f, 2.0f));
                    break;
                case EnemyMovePattern.Follow:
                    movePattern = EnemyMovePattern.Random;
                    yield return new WaitForSeconds(Random.Range(1.0f, 1.5f));
                    break;
                case EnemyMovePattern.Stop:
                    yield return null;
                    break;
            }
        }
    }

    IEnumerator EnableRandomMovement() {
        yield return new WaitForSeconds(Random.Range(1.0f, 1.5f));
        movePattern = EnemyMovePattern.Random;
    }

    private void OnCollisionStay2D(Collision2D collision) {
        ReverseMoveAngle();
    }

    protected void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Player")
            && movePattern == EnemyMovePattern.Follow) {
            movePattern = EnemyMovePattern.Stop;
            StartCoroutine(EnableRandomMovement());
            return;
        }

        BounceOff(collision);
    }
}
