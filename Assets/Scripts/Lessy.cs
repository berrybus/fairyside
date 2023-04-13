using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lessy: BaseEnemy
{
    [SerializeField]
    private List<BaseEnemy> enemyPool;

    [SerializeField]
    private List<BaseEnemy> bossPool;

    [SerializeField]
    private AudioClip cackle;

    protected override void OnEnable() {
        base.OnEnable();
        curAngle = 0;
        targetAngle = 0;
        currentState = EnemyState.Unready;
        movePattern = EnemyMovePattern.Random;
    }

    protected override void Update() {
        base.Update();
        if (movePattern == EnemyMovePattern.Stop) {
            animator.Play("WitchSummon");
        } else {
            animator.Play("WitchMove");
        }
    }

    protected override void FixedUpdate() {
        base.FixedUpdate();
        if (currentState == EnemyState.Move && movePattern != EnemyMovePattern.Stop) {
            var curDirection = (Vector2)(Quaternion.Euler(0, 0, curAngle) * Vector2.right).normalized;
            rbd.AddForce(curDirection * moveForce);
        }
    }

    protected override void StartActivity() {
        curAngle = GetRandom45Direction();
        targetAngle = GetRandom45Direction();

        if (currentState == EnemyState.Unready) {
            currentState = EnemyState.Move;
        }
        StartCoroutine(RandomMove());
        StartCoroutine(MobRoutine());
    }

    private void SummonEnemy(List<BaseEnemy> pool) {
        int idx = Random.Range(0, pool.Count);
        BaseEnemy enemy = pool[idx];
        BaseEnemy newEnemy = Instantiate(enemy, startPos, Quaternion.identity);
        newEnemy.room = room;
        newEnemy.playerTarget = playerTarget;
        room.numEnemies += 1;
        newEnemy.wasSummoned = true;
        newEnemy.portal = portal;
        newEnemy.gameObject.SetActive(true);
    }

    IEnumerator MobRoutine() {
        yield return new WaitForSeconds(Random.Range(0.25f, 0.5f));
        int totalSummons = 0;
        while (true) {
            if (room.numEnemies >= 10) {
                yield return null;
                continue;
            }
            movePattern = EnemyMovePattern.Stop;
            GameManager.instance.PlaySFX(cackle);
            yield return new WaitForSeconds(1.5f);
            SummonEnemy(totalSummons < 2 ? bossPool : enemyPool);
            movePattern = EnemyMovePattern.Random;
            if (totalSummons < 2) {
                yield return new WaitForSeconds(Random.Range(6f, 8f));
            } else {
                yield return new WaitForSeconds(Random.Range(4.0f, 6.0f));
            }
            totalSummons += 1;
        }
    }

    private void OnCollisionStay2D(Collision2D collision) {
        ReverseMoveAngle();
    }

    protected void OnCollisionEnter2D(Collision2D collision) {
        BounceOff(collision);
    }
}
