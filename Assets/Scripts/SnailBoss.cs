using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnailBoss : BaseEnemy {
    [SerializeField]
    private int minShots;
    [SerializeField]
    private int maxShots;
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
        curAngle = GetRandom45Direction();
        StartCoroutine(ShootAndMoveRoutine());
        StartCoroutine(RandomMove());
    }

    protected override void Update() {
        base.Update();
        spriteRenderer.flipX = curAngle > 90 && curAngle < 270;
    }

    protected override void FixedUpdate() {
        base.FixedUpdate();

        if (currentState == EnemyState.Move && movePattern == EnemyMovePattern.Random) {
            rbd.AddForce(VectorFromAngle(curAngle) * moveForce);
        }
    }

    IEnumerator ShootAndMoveRoutine() {
        while (true) {
            if (Random.Range(0, 1f) <= 0.5f) {
                yield return FireLineAndSweep();
                yield return MoveInSpurts();
            }
            if (Random.Range(0, 1f) <= 0.5f) {
                yield return FireCardinalsAndSweep();
                yield return MoveInSpurts();
            }
            if (Random.Range(0, 1f) <= 0.5f) {
                yield return FireBursts();
                yield return MoveInSpurts();
            }
        }
    }

    IEnumerator MoveInSpurts() {
        for (int i = 0; i < Random.Range(1, 4); i++) {
            movePattern = EnemyMovePattern.Random;
            curAngle = GetRandom45Direction();
            yield return new WaitForSeconds(Random.Range(0.25f, 0.5f));
            movePattern = EnemyMovePattern.Stop;
            yield return new WaitForSeconds(Random.Range(0.125f, 0.25f));
        }
    }

    IEnumerator FireLineAndSweep() {
        float upAngle = Random.Range(0, 360);
        float downAngle = upAngle + 180;
        downAngle %= 360;

        int totalShots = Random.Range(minShots, maxShots);
        int direction = Random.Range(0, 1f) <= 0.5 ? 1 : -1;

        for (int i = 0; i < totalShots / 2; i++) {
            FireAngles(new float[] { upAngle, downAngle });
            yield return new WaitForSeconds(bulletFireDelay);
        }

        for (int i = 0; i < totalShots; i++) {
            upAngle -= angleShot * direction;
            upAngle += 360;
            upAngle %= 360;
            downAngle -= angleShot * direction;
            downAngle += 360;
            downAngle %= 360;
            for (int j = 0; j < 2; j++) {
                FireAngles(new float[] { upAngle, downAngle });
                yield return new WaitForSeconds(bulletFireDelay);
            }
        }
    }

    IEnumerator FireCardinalsAndSweep() {
        float[] angles = { 0, 90, 180, 270 };

        int totalShots = Random.Range(minShots, maxShots);
        int direction = Random.Range(0, 1f) <= 0.5 ? 1 : -1;

        for (int i = 0; i < totalShots / 2; i++) {
            FireAngles(angles);
            yield return new WaitForSeconds(bulletFireDelay);
        }

        for (int i = 0; i < totalShots * 2; i++) {
            for (int j = 0; j < angles.Length; j++) {
                angles[j] += angleShot * direction;
                angles[j] += 360;
                angles[j] %= 360;
            }
            FireAngles(angles);
            yield return new WaitForSeconds(bulletFireDelay);
        }

    }

    IEnumerator FireBursts() {
        float[] angles = new float[] { 0, 45, 90, 135, 180, 225, 270, 315 };
        for (int i = 0; i < Random.Range(6, 8); i++) {
            float angle = angles[Random.Range(0, angles.Length)];
            for (int j = 0; j < 3; j++) {
                Fire(VectorFromAngle(angle));
                yield return new WaitForSeconds(bulletFireDelay);
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision) {
        BounceOff(collision);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        BounceOff(collision);
    }

}
