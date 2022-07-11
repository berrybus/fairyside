using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState {
    Move,
    Knockback,
    Unready
}

public class BaseEnemy : MonoBehaviour {
    public Rigidbody2D rbd;
    protected EnemyState currentState;
    public Transform playerTarget;
    public SpriteRenderer spriteRenderer;
    public Room room;
    public GameObject enemyDeath;
    protected int hp = 100;
    protected int maxGems = 2;
    protected int minGems = 0;
    protected bool isBoss = false;

    protected Vector2 knockback = Vector2.zero;

    [SerializeField]
    private Gem gem;

    [SerializeField]
    private float knockbackForce = 12000.0f;

    protected virtual void Awake() {
        currentState = EnemyState.Move;
        rbd = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        gameObject.SetActive(false);
    }

    protected virtual void OnEnable() {
        StartCoroutine(EnableEnemy());
    }

    IEnumerator EnableEnemy() {
        yield return new WaitForSeconds(0.4f);
        StartActivity();
    }

    protected virtual void FixedUpdate() {
        if (currentState == EnemyState.Knockback) {
            rbd.AddForce(knockback * knockbackForce);
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Bullet") && hp > 0) {
            DamageSelf();
            currentState = EnemyState.Knockback;
            rbd.velocity = Vector2.zero;
            knockback = transform.position - collision.transform.position;
            knockback = knockback.normalized;
            collision.gameObject.GetComponent<Bullet>().RemoveSelf();
            StartCoroutine(ResumeMovement());
            spriteRenderer.color = new Color(1.0f, 0.6f, 0.5f, 1.0f);

            // Enemy death
            if (hp <= 0) {
                if (!isBoss) {
                    room.numEnemies -= 1;
                    room.OpenDoorsIfPossible();
                }
                Instantiate(enemyDeath, transform.position, Quaternion.identity);
                for (var i = 0; i < Random.Range(minGems, maxGems); i++) {
                    Gem newGem = Instantiate(
                        gem,
                        transform.position + new Vector3(Random.Range(-0.25f, 0.25f), Random.Range(-0.25f, 0.25f), 0),
                        Quaternion.identity
                    );
                    newGem.Scatter(knockback);
                }
                Destroy(gameObject);
            }
        }
    }

    protected virtual void DamageSelf() {
        var dmg = PlayerManager.instance.PlayerAttackVal();
        hp -= dmg;
        PlayerManager.instance.EnemyHit(dmg, hp, transform.position);
    }

    protected virtual void StartActivity() { }

    IEnumerator ResumeMovement() {
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        knockback = Vector2.zero;
        currentState = EnemyState.Move;
    }
}