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
        int currentAngle = 0;
        for (int i = 0; i < Random.Range(minShots, maxShots); i += 1) {
            currentAngle = i * angleShot + startAngle;
            currentAngle %= 360;
            Fire(VectorFromAngle(currentAngle));
            yield return new WaitForSeconds(0.1f);
        }
    }

    protected override void Update() {
        base.Update();

        int flipX = (curAngle > 90 && curAngle < 270) ? -1 : 1;
        transform.localScale = new Vector3(flipX, 1, 1);
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
            if (currentState == EnemyState.Move) {
                currentState = EnemyState.Unready;
                yield return new WaitForSeconds(Random.Range(0.125f, 0.25f));
            } else if (currentState == EnemyState.Unready) {
                currentState = EnemyState.Move;
                yield return new WaitForSeconds(Random.Range(1.0f, 3.0f));
            } else {
                // Knockback, so wait for that to finish before trying to change states
                yield return new WaitForSeconds(Random.Range(2.0f, 3.0f));
            }
        }
    }
}
