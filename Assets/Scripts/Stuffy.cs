using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stuffy : BaseEnemy {
    [SerializeField]
    private BaseEnemy stuffyClone;

    [SerializeField]
    private float flyForce;

    [SerializeField]
    private AudioClip cackle;

    private bool didSummon = false;

    private IEnumerator shootRoutine;

    protected override void OnEnable() {
        base.OnEnable();
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
        StartCoroutine(RandomlyChangeDirections());
        shootRoutine = FlyingShootRoutine();
        StartCoroutine(shootRoutine);
        StartCoroutine(TargetShootRoutine());
    }

    protected override void Update() {
        base.Update();
        if (IsFlying()) {
            spriteRenderer.flipX = curAngle > 90 && curAngle < 270;
            animator.Play("WitchFly");
        } else if (didSummon) {
            spriteRenderer.flipX = false;
            if (movePattern == EnemyMovePattern.Stop) {
                animator.Play("WitchStill");
            } else {
                animator.Play("WitchMove");
            }
        }
    }

    protected override void FixedUpdate() {
        base.FixedUpdate();
        if (currentState == EnemyState.Move && movePattern == EnemyMovePattern.Random) {
            float adjustedMoveForce = IsFlying() ? flyForce : moveForce;
            var curDirection = (Vector2)(Quaternion.Euler(0, 0, curAngle) * Vector2.right).normalized;
            rbd.AddForce(curDirection * adjustedMoveForce);
        }
    }

    protected override void HPDidChange(int oldHP) {
        float oldPercentage = (float)oldHP / (float)maxHp;
        float newPercentage = (float)hp / (float)maxHp;
        if (oldPercentage >= 0.5 && newPercentage < 0.5) {
            StartCoroutine(SummonClone());
        }
    }

    private bool IsFlying() {
        return (float)hp / (float)maxHp >= 0.5;
    }

    IEnumerator FlyingShootRoutine() {
        yield return new WaitForSeconds(Random.Range(1.0f, 2.0f));
        while (true) {
            yield return FireDiagonals();
            yield return new WaitForSeconds(Random.Range(1.0f, 2.0f));
        }
    }

    IEnumerator TargetShootRoutine() {
        yield return new WaitForSeconds(Random.Range(0.5f, 1.0f));
        while (true) {
            yield return FireArrow();
            yield return new WaitForSeconds(Random.Range(1.0f, 2.0f));
        }
    }

    IEnumerator WalkingShootRoutine() {
        yield return new WaitForSeconds(Random.Range(0.5f, 1.0f));
        while (true) {
            yield return FireAlmostCircleTurn();
            yield return new WaitForSeconds(Random.Range(1.0f, 2.0f));
        }
    }

    IEnumerator FireDiagonals() {
        float[] angles = new float[] { 45, 135, 225, 315 };
        for (int i = 0; i < Random.Range(4, 6); i++) {
            foreach (float angle in angles) {
                Fire(VectorFromAngle(angle));
            }
            yield return new WaitForSeconds(bulletFireDelay);
        }
    }

    IEnumerator FireAlmostCircleTurn() {
        float startAngle = AngleFromVector((playerTarget.transform.position - transform.position).normalized);
        for (int j = 0; j <= 12; j++) {
            float offset = Mathf.Max(0, j - 5) * 15;
            for (int i = 1; i <= 6; i++) {
                float newAngle = startAngle + 45 * i + offset;
                Fire(VectorFromAngle(newAngle), 6.0f);
            }
            yield return new WaitForSeconds(bulletFireDelay * 2);
        }
    }

    IEnumerator FireArrow() {
        float startAngle = AngleFromVector((playerTarget.transform.position - transform.position).normalized);
        float angleLeft = startAngle + 3;
        float angleRight = startAngle - 3;
        Fire(VectorFromAngle(startAngle), 12f);
        yield return new WaitForSeconds(0.05f);
        Fire(VectorFromAngle(angleLeft), 12f);
        Fire(VectorFromAngle(angleRight), 12f);
    }

    private IEnumerator SummonClone() {
        animator.Play("WitchSummon");
        GameManager.instance.PlaySFX(cackle);
        movePattern = EnemyMovePattern.Stop;
        StopCoroutine(shootRoutine);
        yield return new WaitForSeconds(1.5f);
        BaseEnemy newEnemy = Instantiate(stuffyClone, startPos, Quaternion.identity);
        newEnemy.room = room;
        newEnemy.playerTarget = playerTarget;
        room.numEnemies += 1;
        newEnemy.wasSummoned = true;
        newEnemy.portal = portal;
        newEnemy.gameObject.SetActive(true);
        didSummon = true;
        StartCoroutine(RandomMove());
        StartCoroutine(MoveAndPause());
        StartCoroutine(WalkingShootRoutine());
    }

    IEnumerator RandomlyChangeDirections() {
        while (IsFlying()) {
            yield return new WaitForSeconds(Random.Range(3.0f, 4.0f));
            curAngle = Random.Range(0, 360);
        }
    }

    IEnumerator MoveAndPause() {
        while (true) {
            switch (movePattern) {
                case EnemyMovePattern.Random:
                    movePattern = EnemyMovePattern.Stop;
                    yield return new WaitForSeconds(Random.Range(0.5f, 1.0f));
                    break;
                case EnemyMovePattern.Stop:
                    movePattern = EnemyMovePattern.Random;
                    yield return new WaitForSeconds(Random.Range(2.0f, 3.0f));
                    break;
                default:
                    yield return null;
                    break;
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision) {
        ReverseMoveAngle();
    }

    protected void OnCollisionEnter2D(Collision2D collision) {
        BounceOff(collision);
    }
}
