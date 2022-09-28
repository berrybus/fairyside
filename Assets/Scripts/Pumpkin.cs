using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pumpkin : BaseEnemy {
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
        StartCoroutine(FollowAndRandom());
        StartCoroutine(ShootRoutine());
    }

    protected override void FixedUpdate() {
        base.FixedUpdate();
        if (currentState == EnemyState.Move) {
            if (movePattern == EnemyMovePattern.Random) {
                Vector2 curDirection = (Vector2)(Quaternion.Euler(0, 0, curAngle) * Vector2.right).normalized;
                rbd.AddForce(curDirection * moveForce);
            } else if (movePattern == EnemyMovePattern.Follow) {
                Vector2 targetDirection = playerTarget.transform.position - gameObject.transform.position;
                targetDirection = targetDirection.normalized;
                curAngle = Vector2.SignedAngle(Vector2.right, targetDirection);
                if (curAngle < 0) {
                    curAngle += 360f;
                }
                rbd.AddForce(targetDirection * moveForce);
            }
        }
    }

    IEnumerator FollowAndRandom() {
        yield return new WaitForSeconds(Random.Range(1.0f, 1.5f));
        while (true) {
            switch (movePattern) {
                case EnemyMovePattern.Random:
                    movePattern = EnemyMovePattern.Follow;
                    yield return new WaitForSeconds(Random.Range(2.0f, 3.0f));
                    break;
                case EnemyMovePattern.Follow:
                    movePattern = EnemyMovePattern.Random;
                    yield return new WaitForSeconds(Random.Range(2.0f, 3.0f));
                    break;
                case EnemyMovePattern.Stop:
                    yield return null;
                    break;
            }
        }
    }

    IEnumerator ShootRoutine() {
        while (true) {
            yield return new WaitForSeconds(Random.Range(1.0f, 2.0f));
            if (movePattern == EnemyMovePattern.Random) {
                int numShots = Random.Range(minShots, maxShots);
                for (int i = 0; i < numShots; i++) {
                    FireHorizontal();
                    yield return new WaitForSeconds(0.2f);
                }
            }
        }
    }

    IEnumerator EnableRandomMovement() {
        yield return new WaitForSeconds(Random.Range(0.25f, 0.5f));
        movePattern = EnemyMovePattern.Random;
    }

    private void FireHorizontal() {
        Vector2[] directions = { Vector2.left, Vector2.right };
        foreach (var direction in directions) {
            Fire(direction);
        }
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
