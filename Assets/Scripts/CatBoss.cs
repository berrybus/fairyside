using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatBoss : BaseEnemy {
    private Vector2 curDirection;
    [SerializeField]
    private int minShots;
    [SerializeField]
    private int maxShots;

    protected override void OnEnable() {
        base.OnEnable();
        curDirection = Vector2.zero;
        currentState = EnemyState.Unready;
        movePattern = EnemyMovePattern.Random;
    }

    protected override void StartActivity() {
        while (curDirection.magnitude == 0) {
            curDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        }

        if (currentState == EnemyState.Unready) {
            currentState = EnemyState.Move;
        }
        StartCoroutine(RandomlyChangeDirections());
        StartCoroutine(HorizontalRoutine());
        StartCoroutine(VerticalRoutine());
    }

    protected override void FixedUpdate() {
        base.FixedUpdate();
        if (currentState == EnemyState.Move && movePattern == EnemyMovePattern.Random) {
            rbd.AddForce(curDirection * moveForce);
        }
    }

    protected override void Update() {
        base.Update();
        int flipX = (curDirection.x < 0) ? -1 : 1;
        transform.localScale = new Vector3(flipX, 1, 1);
    }

    IEnumerator HorizontalRoutine() {
        while (true) {
            yield return new WaitForSeconds(Random.Range(1.0f, 2.0f));
                int numShots = Random.Range(minShots, maxShots);
                for (int i = 0; i < numShots; i++) {
                    Fire(curDirection.x < 0 ? Vector2.right : Vector2.left);
                    yield return new WaitForSeconds(0.25f);
                }
        }
    }

    IEnumerator VerticalRoutine() {
        while (true) {
            yield return new WaitForSeconds(Random.Range(2.0f, 4.0f));
            for (int i = 0; i < 4; i++) {
                Fire(Vector2.up);
                Fire(Vector2.down);
                yield return new WaitForSeconds(0.25f);
            }
        }
    }

    IEnumerator RandomlyChangeDirections() {
        while (true) {
            yield return new WaitForSeconds(Random.Range(3.0f, 4.0f));
            curDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        }
    }

    protected void BounceOffVector(Collision2D collision) {
        if (canBounceOffCollision) {
            curDirection = Vector2.Reflect(curDirection, collision.GetContact(0).normal);
            canBounceOffCollision = false;
            StartCoroutine(EnableCollisionBounce());
        }
    }


    private void OnCollisionEnter2D(Collision2D collision) {
        BounceOffVector(collision);
    }

    private void OnCollisionStay2D(Collision2D collision) {
        BounceOffVector(collision);
    }
}
