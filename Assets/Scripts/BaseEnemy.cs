using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState {
    Move,
    Knockback,
    Pause
}

public class BaseEnemy : MonoBehaviour {
    public Rigidbody2D rbd;
    protected EnemyState currentState;
    public Transform playerTarget;
    public SpriteRenderer spriteRenderer;
    public Room room;
    public GameObject enemyDeath;
    public GameObject portal;
    public Transform origin;
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
            DamageSelf(collision.gameObject.GetComponent<Bullet>().displayOnTop);
            currentState = EnemyState.Knockback;
            rbd.velocity = Vector2.zero;
            knockback = transform.position - collision.transform.position;
            knockback = knockback.normalized;
            knockback *= PlayerManager.instance.GunKnockbackMultiplier();
            if (collision.gameObject.GetComponent<Bullet>().destroyOnEnemyImpact) {
                collision.gameObject.GetComponent<Bullet>().RemoveSelf();
            }
            StartCoroutine(ResumeMovement());
            spriteRenderer.color = new Color(1.0f, 0.6f, 0.5f, 1.0f);

            // Enemy death
            if (hp <= 0) {
                room.numEnemies -= 1;
                room.OpenDoorsIfPossible();
                if (isBoss && portal != null) {
                    Instantiate(portal, origin.position, Quaternion.identity);
                }
                Instantiate(enemyDeath, origin.position, Quaternion.identity);
                for (var i = 0; i < Random.Range(minGems, maxGems); i++) {
                    Vector3 position = origin.position + new Vector3(0, -0.25f, 0);
                    Gem newGem = Instantiate(
                        gem,
                        position + new Vector3(Random.Range(-0.25f, 0.25f), Random.Range(-0.25f, 0.25f), 0),
                        Quaternion.identity
                    );
                    newGem.Scatter(knockback);
                }
                Destroy(gameObject);
            }
        }
    }

    protected virtual void DamageSelf(bool displayOnTop) {
        var dmg = PlayerManager.instance.EnemyHit(hp, origin.position, displayOnTop);
        hp -= dmg;
    }

    protected virtual void StartActivity() { }

    IEnumerator ResumeMovement() {
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        knockback = Vector2.zero;
        currentState = EnemyState.Move;
    }
}