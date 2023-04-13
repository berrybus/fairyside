using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meffy : BaseEnemy {
    [SerializeField]
    private AudioClip cackle;

    private IEnumerator animateRoutine;

    protected override void OnEnable() {
        base.OnEnable();
        animateRoutine = AnimateBrew();
        curAngle = 0;
        targetAngle = 0;
        currentState = EnemyState.Unready;
        movePattern = EnemyMovePattern.Random;
    }

    protected override void StartActivity() {
        curAngle = Random.Range(0, 360);
        targetAngle = GetRandom45Direction();

        if (currentState == EnemyState.Unready) {
            currentState = EnemyState.Move;
        }

        StartCoroutine(SendHealth());
        StartCoroutine(MainShootRoutine());
        StartCoroutine(SideShootRoutine());
    }

    IEnumerator SendHealth() {
        float[] angles = { 0, 90, 180, 270 };
        while (hpDrop != null) {
            yield return new WaitForSeconds(Random.Range(5.0f, 8.0f));
            var offset = Random.Range(0, 45);
            foreach (var angle in angles) {
                Vector3 dir = VectorFromAngle(angle + offset);
                HPDrop hp = Instantiate(
                    hpDrop,
                    origin.position + dir * 1.0f,
                    Quaternion.identity
                );
                hp.shouldRemoveSelf = true;
                hp.Scatter(dir * Random.Range(4f, 10f));
            }
        }
    }

    IEnumerator MainShootRoutine() {
        yield return new WaitForSeconds(Random.Range(0.25f, 0.5f));
        while (true) {
            StartAnimation();
            int rand = Random.Range(0, 7);
            switch (rand) {
                case 0:
                    yield return new WaitForSeconds(Random.Range(0.5f, 1.0f));
                    yield return FireTargetingArrows();
                    break;
                case 1:
                    yield return Fire4Way();
                    break;
                case 2:
                    yield return Fire8Twirling();
                    break;
                case 3:
                    yield return FireTargetCircles();
                    yield return new WaitForSeconds(Random.Range(0.5f, 1.0f));
                    break;
                case 4:
                    yield return FireTargetFragments();
                    yield return new WaitForSeconds(Random.Range(0.5f, 1.0f));
                    break;
                case 5:
                    yield return FireStars();
                    break;
                case 6:
                    yield return new WaitForSeconds(Random.Range(0.5f, 1.0f));
                    yield return FireRotatingArrows();
                    break;
            }
            yield return new WaitForSeconds(Random.Range(2.0f, 2.5f));
        }
    }

    IEnumerator SideShootRoutine() {
        yield return new WaitForSeconds(Random.Range(0.25f, 0.5f));
        while (true) {
            StartAnimation();
            int rand = Random.Range(0, 4);
            switch (rand) {
                case 0:
                    FireCircle();
                    yield return new WaitForSeconds(Random.Range(2.0f, 4.0f));
                    break;
                case 1:
                    yield return FireAlternatingBullets();
                    yield return new WaitForSeconds(Random.Range(3.0f, 5.0f));
                    break;
                case 2:
                    yield return FireLine();
                    yield return new WaitForSeconds(Random.Range(1.0f, 2.0f));
                    break;
                case 3:
                    yield return FireSlow();
                    yield return new WaitForSeconds(Random.Range(4.0f, 6.0f));
                    break;
            }
        }
    }

    IEnumerator FireRotatingArrows() {
        var dir = (playerTarget.position - transform.position).normalized;
        float fireAngle = AngleFromVector(dir);
        for (int j = 0; j < 8; j++) {
            fireAngle += 45;
            StartCoroutine(FireArrow(fireAngle));
            yield return new WaitForSeconds(bulletFireDelay * 2);
        }
    }

    IEnumerator FireTargetingArrows() {
        for (int j = 0; j < 2; j++) {
            var dir = (playerTarget.position - transform.position).normalized;
            float fireAngle = AngleFromVector(dir);
            StartCoroutine(FireArrow(fireAngle));
            yield return new WaitForSeconds(bulletFireDelay * 4);
        }
    }

    IEnumerator FireStars() {
        for (int i = 0; i < 2; i++) {
            yield return FireStar(0);
            yield return new WaitForSeconds(bulletFireDelay * 2);
        }
        yield return new WaitForSeconds(bulletFireDelay * 2);
        for (int i = 0; i < 2; i++) {
            yield return FireStar(45);
            yield return new WaitForSeconds(bulletFireDelay * 2);
        }
    }

    IEnumerator FireStar(float offset) {
        float[] angles = new float[] { 0, 90, 180, 270 };
        for (int i = 0; i < angles.Length; i++) {
            angles[i] += offset;
        }

        foreach (var angle in angles) {
            Fire(VectorFromAngle(angle));
        }
        yield return new WaitForSeconds(bulletFireDelay);
        for (int i = 1; i <= 3; i++) {
            foreach (var angle in angles) {
                var angle1 = angle + i * 10;
                var angle2 = angle + i * -10;
                Fire(VectorFromAngle(angle1));
                Fire(VectorFromAngle(angle2));
            }
            yield return new WaitForSeconds(bulletFireDelay);
        }
    }

    IEnumerator FireArrow(float angle) {
        Fire(VectorFromAngle(angle), 6.0f);
        for (int j = 1; j <= 3; j++) {
            var angle1 = angle + j * 10;
            var angle2 = angle - j * 10;
            yield return new WaitForSeconds(bulletFireDelay * 2);
            Fire(VectorFromAngle(angle1), 6.0f);
            Fire(VectorFromAngle(angle2), 6.0f);
        }
    }

    IEnumerator Fire4Way() {
        float[] angles = { 0, 90, 180, 270 };
        var offset = Random.Range(0, 22);
        for (int i = 0; i < Random.Range(5, 8); i++) {
            for (int j = 0; j < Random.Range(2, 4); j++) {
                foreach (float angle in angles) {
                    Fire(VectorFromAngle(angle + offset));
                }
                yield return new WaitForSeconds(bulletFireDelay);
            }
            yield return new WaitForSeconds(bulletFireDelay);
        }
    }

    IEnumerator Fire8Twirling() {
        float[] angles = { 0, 45, 90, 135, 180, 225, 270, 315 };
        float offset = Random.Range(0, 15);
        for (int i = 0; i < angles.Length; i++) {
            angles[i] += offset;
        }

        for (int i = 0; i < Random.Range(6, 8); i++) {
            foreach (var angle in angles) {
                Fire(VectorFromAngle(angle));
            }
            yield return new WaitForSeconds(bulletFireDelay);
        }

        int moveDir = Random.Range(0f, 1f) <= 0.5 ? 1 : -1;
        for (int i = 0; i < Random.Range(6, 8); i++) {
            foreach (var angle in angles) {
                Fire(VectorFromAngle(angle + 15 * i * moveDir), bulletSpeed / 2);
            }
            yield return new WaitForSeconds(bulletFireDelay);
        }

        float endTime = Time.time + Random.Range(4f, 6f);
        moveDir *= -1;

        while (Time.time < endTime) {
            float turnEnd = Time.time + Random.Range(1.0f, 1.5f);
            while (Time.time < turnEnd) {
                for (int i = 0; i < Random.Range(6, 8); i++) {
                    foreach (var angle in angles) {
                        Fire(VectorFromAngle(angle + Random.Range(12, 18) * i * moveDir), bulletSpeed / 2);
                    }
                    yield return new WaitForSeconds(bulletFireDelay);
                }
            }
            moveDir *= -1;
        }
    }

    IEnumerator FireTargetFragments() {
        for (int i = 0; i < Random.Range(2, 4); i++) {
            FireTargetFragment();
            yield return new WaitForSeconds(bulletFireDelay * 6);
        }
    }

    private void FireTargetFragment() {
        float centerAngle = AngleFromVector((playerTarget.position - transform.position).normalized);
        FireAndTargetLater(VectorFromAngle(centerAngle));
        FireAndTargetLater(VectorFromAngle(centerAngle + 30));
        FireAndTargetLater(VectorFromAngle(centerAngle - 30));
    }

    IEnumerator FireTargetCircles() {
        for (int i = 0; i < Random.Range(1, 3); i++) {
            yield return FireTargetCircle();
            yield return new WaitForSeconds(bulletFireDelay * 4);
        }
    }

    IEnumerator FireTargetCircle() {
        for (int i = 0; i < 8; i++) {
            FireAndTargetLater(VectorFromAngle(i * 45));
            yield return new WaitForSeconds(bulletFireDelay);
        }
    }

    IEnumerator FireAlternatingBullets() {
        float initialOffset = Random.Range(0, 20);
        for (int i = 0; i < Random.Range(12, 16); i++) {
            float offset = 22.5f * i;
            for (int j = 0; j < 8; j++) {
                Fire(VectorFromAngle(j * 45 + offset + initialOffset), 6.0f);
            }
            yield return new WaitForSeconds(bulletFireDelay);
        }
    }

    IEnumerator FireSlow() {
        for (int i = 0; i < Random.Range(40, 50); i++) {
            for (int j = 0; j < 8; j++) {
                float offset = Random.Range(12, 18) * i;
                Fire(VectorFromAngle(j * 45 + offset), bulletSpeed / 4);
            }
            yield return new WaitForSeconds(bulletFireDelay * 4);
        }
    }

    private void FireCircle() {
        float offset = Random.Range(0f, 15f);
        for (int j = 0; j < 24; j++) {
            Fire(VectorFromAngle(j * 15 + offset), 6.0f);
        }
    }

    IEnumerator FireLine() {
        float[] angles = { 0, 180 };
        var offset = Random.Range(0, 90);
        for (int i = 0; i < Random.Range(15, 20); i++) {
            foreach (var angle in angles) {
                Fire(VectorFromAngle(angle + offset));
            }
            yield return new WaitForSeconds(bulletFireDelay);
        }
    }

    private void FireAndTargetLater(Vector2 direction) {
        GameObject bullet = ObjectPool.instance.enemyBullets.Get();
        if (bullet != null) {
            bullet.GetComponent<EnemyBullet>().direction = direction.normalized;
            bullet.GetComponent<EnemyBullet>().speed = bulletSpeed;
            bullet.GetComponent<EnemyBullet>().playerTarget = playerTarget;
            bullet.GetComponent<EnemyBullet>().canTargetImmediately = false;
            bullet.transform.position = firepoint.transform.position;
            bullet.transform.SetParent(transform, true);
            bullet.SetActive(true);
        }
    }

    private void StartAnimation() {
        StopCoroutine(animateRoutine);
        animateRoutine = AnimateBrew();
        StartCoroutine(animateRoutine);
    }
    private IEnumerator AnimateBrew() {
        animator.Play("WitchBrew");
        yield return new WaitForSeconds(1.0f);
        animator.Play("WitchBrewStill");
    }

}
