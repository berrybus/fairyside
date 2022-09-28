using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatBoss : BaseEnemy {
    protected override void OnEnable() {
        base.OnEnable();
        currentState = EnemyState.Unready;
        movePattern = EnemyMovePattern.Follow;
    }

    protected override void StartActivity() {
        if (currentState == EnemyState.Unready) {
            currentState = EnemyState.Move;
        }
        StartCoroutine(ShootRoutine());
    }

    protected override void FixedUpdate() {
        base.FixedUpdate();

        if (currentState == EnemyState.Move && movePattern == EnemyMovePattern.Follow) {
            Vector2 direction = (playerTarget.transform.position - gameObject.transform.position).normalized;
            rbd.AddForce(direction * moveForce);
        }
    }

    private void FireBullets(float[] angles) {
        foreach (float angle in angles) {
            Fire(VectorFromAngle(angle));
        }
    }

    IEnumerator ShootRoutine() {
        while (true) {
            // Shoot 4 cardinals
            yield return new WaitForSeconds(Random.Range(1.0f, 2.0f));

            movePattern = Random.Range(0f, 1f) <= 0.5f ? EnemyMovePattern.Random : EnemyMovePattern.Stop;
            float[] angles_90 = { 0, 90, 180, 270 };
            FireBullets(angles_90);

            // Shoot 4 directionals 
            yield return new WaitForSeconds(Random.Range(0.5f, 1.0f));
            float[] angles_diag = { 45, 135, 225, 315 };
            FireBullets(angles_diag);

            // Shoot 8 angles
            yield return new WaitForSeconds(Random.Range(0.5f, 1.0f));
            float[] angles = { 0, 45, 90, 135, 180, 225, 270, 315 };
            FireBullets(angles);
            yield return new WaitForSeconds(Random.Range(0.5f, 1f));
            movePattern = EnemyMovePattern.Follow;


            // Same thing but in reverse
            yield return new WaitForSeconds(Random.Range(2.0f, 4.0f));

            movePattern = Random.Range(0f, 1f) <= 0.5f ? EnemyMovePattern.Random : EnemyMovePattern.Stop;
            FireBullets(angles);
            yield return new WaitForSeconds(Random.Range(0.3f, 0.8f));
            FireBullets(angles_diag);
            yield return new WaitForSeconds(Random.Range(0.3f, 0.8f));
            FireBullets(angles_90);
            yield return new WaitForSeconds(Random.Range(0.5f, 1f));
            movePattern = EnemyMovePattern.Follow;
        }
    }

    IEnumerator EnableFollow() {
        yield return new WaitForSeconds(Random.Range(0.5f, 1.0f));
        movePattern = EnemyMovePattern.Follow;
    }
    protected void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Player")
            && movePattern == EnemyMovePattern.Follow) {
            movePattern = EnemyMovePattern.Stop;
            StartCoroutine(EnableFollow());
            return;
        }
    }
}
