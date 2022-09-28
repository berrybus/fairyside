using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogBoss : BaseEnemy {
    [SerializeField]
    private int angleShot;

    protected override void OnEnable() {
        base.OnEnable();
        currentState = EnemyState.Unready;
        movePattern = EnemyMovePattern.Stop;
    }

    protected override void StartActivity() {
        if (currentState == EnemyState.Unready) {
            currentState = EnemyState.Move;
        }
        StartCoroutine(ShootRoutine());
    }

    IEnumerator ShootRoutine() {
        while (true) {
            yield return new WaitForSeconds(Random.Range(0.5f, 1.0f));
            yield return FireHoning();
            if (Random.Range(0f, 1f) <= 0.5) {
                yield return new WaitForSeconds(Random.Range(0.5f, 1.0f));
                yield return FireArrowBursts();
            }
            if (Random.Range(0f, 1f) <= 0.5) {
                yield return new WaitForSeconds(Random.Range(0.5f, 1.0f));
                yield return FireStar(true);
            }
            for (int i = 0; i < Random.Range(2, 4); i++) {
                yield return new WaitForSeconds(Random.Range(0.5f, 1.0f));
                yield return FireStar(false);
            }
            yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));
            yield return FireSpiral();
        }
    }

    IEnumerator FireHoning() {
        for (int i = 0; i < Random.Range(6, 8); i++) {
            Fire(Vector2.zero, 6.0f, playerTarget);
            yield return new WaitForSeconds(bulletFireDelay * 2);
        }
    }

    IEnumerator FireArrowBursts() {
        float[] angles = new float[] { 0, 45, 90, 135, 180, 225, 270, 315 };
        angles.Shuffle();
        angles = angles[..4];
        foreach (float angle in angles) {
            Fire(VectorFromAngle(angle));
            for (int j = 0; j < 2; j++) {
                var angle1 = angle + (j + 1) * 10;
                angle1 += 360;
                angle1 %= 360;
                var angle2 = angle + (j + 1) * -10;
                angle2 += 360;
                angle2 %= 360;
                yield return new WaitForSeconds(bulletFireDelay);
                Fire(VectorFromAngle(angle1));
                Fire(VectorFromAngle(angle2));
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator FireStar(bool usePartial) {
        float[] anglesCardinal = new float[] { 0, 90, 180, 270 };
        float[] anglesDiagonal = new float[] { 45, 135, 215, 315 };
        float[] angles = Random.Range(0f, 1f) <= 0.5 ? anglesCardinal : anglesDiagonal;
        if (usePartial) {
            angles = new float[] { 0, 45, 90, 135, 180, 215, 270, 315 };
            angles.Shuffle();
            angles = angles[..Random.Range(2, angles.Length - 1)];
        }
        foreach (var angle in angles) {
            Fire(VectorFromAngle(angle));
        }
        yield return new WaitForSeconds(bulletFireDelay);
        for (int i = 1; i <= 3; i++) {
            foreach (var angle in angles) {
                var angle1 = angle + i * 10;
                angle1 += 360;
                angle1 %= 360;
                var angle2 = angle + i * -10;
                angle2 += 360;
                angle2 %= 360;
                Fire(VectorFromAngle(angle1));
                Fire(VectorFromAngle(angle2));
            }
            yield return new WaitForSeconds(bulletFireDelay);
        }
    }

    IEnumerator FireSpiral() {
        Vector2 playerDir = (playerTarget.transform.position - transform.position).normalized;
        float angle = AngleFromVector(playerDir);
        for (int i = 0; i < Random.Range(2, 7); i++) {
            for (int j = 0; j < Random.Range(24, 36); j++) {
                Fire(VectorFromAngle(angle));
                yield return new WaitForSeconds(bulletFireDelay / 2);
                if (i % 2 == 0) {
                    angle += angleShot;
                } else {
                    angle -= angleShot;
                }
                angle += 360;
                angle %= 360;
            }
        }
    }

    protected void OnCollisionEnter2D(Collision2D collision) {
        BounceOff(collision);
    }

    private void OnCollisionStay2D(Collision2D collision) {
        BounceOff(collision);
    }
}
