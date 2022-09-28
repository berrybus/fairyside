using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeBoss : BaseEnemy {
    private Vector2 curDirection;
    [SerializeField]
    private int minShots;
    [SerializeField]
    private int maxShots;
    [SerializeField]
    private int angleShot;

    protected override void OnEnable() {
        base.OnEnable();
        curDirection = Vector2.zero;
        currentState = EnemyState.Unready;
        movePattern = EnemyMovePattern.Random;
    }

    protected override void StartActivity() {
        while (curDirection.magnitude == 0) {
            curDirection = RandomDiagonalDirection();
        }

        if (currentState == EnemyState.Unready) {
            currentState = EnemyState.Move;
        }
        StartCoroutine(RandomlyChangeDirections());
        StartCoroutine(ShootRoutine());
    }

    protected override void FixedUpdate() {
        base.FixedUpdate();
        if (currentState == EnemyState.Move && movePattern == EnemyMovePattern.Random) {
            rbd.AddForce(curDirection * moveForce);
        }
    }

    protected override void Update() {
        base.Update();
        int flipX = (curDirection.x < 0) ? 1 : -1;
        spriteRenderer.flipX = curDirection.x >= 0;
    }

    IEnumerator ShootRoutine() {
        while (true) {
            yield return new WaitForSeconds(Random.Range(0.5f, 1.0f));
            yield return ShootAlternating(Random.Range(0f, 1f) <= 0.5f);
            yield return new WaitForSeconds(Random.Range(1.0f, 2.0f));
            yield return FireAtPlayer();
        }
    }

    IEnumerator ShootAlternating(bool shouldCurve) {
        int direction = Random.Range(0f, 1f) <= 0.5 ? 1 : -1;
        for (int i = 0; i < 4; i++) {
            FireAngles(new float[] { 0 , 90 , 180 , 270 });
            yield return new WaitForSeconds(bulletFireDelay);
            FireAngles(new float[] { 45 , 135 , 225 , 315 });
            yield return new WaitForSeconds(bulletFireDelay);
        }

        for (int i = 0; i < Random.Range(minShots, maxShots); i++) {
            int offset = 0;
            if (shouldCurve) {
                offset = i * angleShot * direction;
            }
            float[] angles = new float[] { 0, 90, 180, 270 };
            FireAngles(AdjustAnglesWithOffset(angles, offset));
            yield return new WaitForSeconds(bulletFireDelay);
            angles = new float[] { 45, 135, 225, 315 };
            FireAngles(AdjustAnglesWithOffset(angles, offset));
            yield return new WaitForSeconds(bulletFireDelay);
        }
    }

    IEnumerator FireAtPlayer() {
        for (int i = 0; i < 3; i++) {
            var direction = (playerTarget.transform.position - transform.position).normalized;
            var dirAngle = AngleFromVector(direction);
            Fire(direction, bulletSpeed * 2f);
            for (int j = 0; j < 2; j++) {
                var angle1 = dirAngle + (j + 1) * 5;
                angle1 += 360;
                angle1 %= 360;
                var angle2 = dirAngle + (j + 1) * -5;
                angle2 += 360;
                angle2 %= 360;
                yield return new WaitForSeconds(bulletFireDelay / 2);
                Fire(VectorFromAngle(angle1), bulletSpeed * 2f);
                Fire(VectorFromAngle(angle2), bulletSpeed * 2f);
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    float[] AdjustAnglesWithOffset(float[] angles, float offset) {
        for (int j = 0; j < angles.Length; j++) {
            angles[j] += offset;
            angles[j] += 360;
            angles[j] %= 360;
        }
        return angles;
    }


    IEnumerator RandomlyChangeDirections() {
        while (true) {
            yield return new WaitForSeconds(Random.Range(1.0f, 2.0f));
            if (canChangeAngle) {
                curDirection = RandomDiagonalDirection();
            }
        }
    }

    protected void BounceOffVector(Collision2D collision) {
        if (canChangeAngle) {
            canChangeAngle = false;
            curDirection = Vector2.Reflect(curDirection, collision.GetContact(0).normal);
            StartCoroutine(EnableAngleChange());
        }
    }


    protected void OnCollisionEnter2D(Collision2D collision) {
        BounceOffVector(collision);
    }

    private void OnCollisionStay2D(Collision2D collision) {
        BounceOffVector(collision);
    }

    private Vector2 RandomDiagonalDirection() {
        int x = Random.Range(0f, 1f) <= 0.5 ? -1 : 1;
        int y = Random.Range(0f, 1f) <= 0.5 ? -1 : 1;
        return new Vector2(x, y).normalized;
    }
}
