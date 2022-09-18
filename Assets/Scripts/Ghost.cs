using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : BaseEnemy {
    [SerializeField]
    private int minShots;
    [SerializeField]
    private int maxShots;
    [SerializeField]
    private int angleShot;
    [SerializeField]
    private bool shouldReverse;
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
            yield return FireSpiral();
            yield return new WaitForSeconds(Random.Range(1.0f, 3.0f));
        }
    }

    IEnumerator FireSpiral() {
        int startAngle = (int) AngleFromVector(playerTarget.transform.position - transform.position);
        int currentAngle = startAngle;
        int totalShots = Random.Range(minShots, maxShots);
        for (int i = 0; i < totalShots; i += 1) {
            currentAngle += angleShot;
            currentAngle %= 360;
            Fire(VectorFromAngle(currentAngle));
            yield return new WaitForSeconds(0.1f);
        }

        if (shouldReverse) {
            for (int i = 0; i < totalShots; i++) {
                currentAngle -= angleShot;
                curAngle += 360;
                currentAngle %= 360;
                Fire(VectorFromAngle(currentAngle));
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    protected override void Update() {
        base.Update();
        spriteRenderer.flipX = curAngle > 90 && curAngle < 270;
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
                    yield return new WaitForSeconds(Random.Range(0.5f, 1.0f));
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
