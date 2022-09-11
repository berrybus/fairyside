using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerState {
    Move,
    Knockback,
    Transition,
    Frozen
}

public class Player : MonoBehaviour {
    public Firepoint firePointRight;
    public Firepoint firePointLeft;
    public Firepoint firePointUp;
    public Firepoint firePointDown;
    public Firepoint currentFirePoint;
    private Firepoint[] firePoints;

    public SpriteRenderer spriteRenderer;
    public Rigidbody2D rbd;
    public CapsuleCollider2D capsuleCollider;
    public CameraController cameraController;
    public Animator animator;

    [SerializeField]
    private float teleportDistanceX;
    [SerializeField]
    private float teleportDistanceY;

    [System.NonSerialized]
    public float baseSpeed = 7.5f;
    [System.NonSerialized]
    public int maxDelay = 12;
    private int curDelay = 0;

    private PlayerState currentState;
    private bool isInvincible;
    Vector2 movement;
    Vector2 fireDirection;
    readonly Vector2[] angles = { Vector2.right, Vector2.left, Vector2.up, Vector2.down };
    private const int fireArrowCoolLen = 45;
    private int fireArrowCooldown = fireArrowCoolLen;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rbd = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();

        firePoints = new Firepoint[] { firePointRight, firePointLeft, firePointUp, firePointDown };
        currentState = PlayerState.Move;
        isInvincible = false;
    }

    private void Start() {
        firePointDown.spriteRenderer.enabled = false;
        firePointUp.spriteRenderer.enabled = false;
        firePointLeft.spriteRenderer.enabled = false;
        firePointRight.spriteRenderer.enabled = false;

        currentFirePoint = firePointDown;
    }
    public void OnMove(InputAction.CallbackContext ctx) {
        movement = ctx.ReadValue<Vector2>();

        if (movement != Vector2.zero && currentState != PlayerState.Knockback) {
            // animator.SetFloat("SpeedMultiplier", 1.0f);
            Vector2 moveDir = GetClosestAngle(movement);
            if (moveDir == Vector2.left) {
                animator.Play("ShirleyLeft", 0, 0.25f);
            } else if (moveDir == Vector2.right) {
                animator.Play("ShirleyRight", 0, 0.25f);
            } else if (moveDir == Vector2.up) {
                animator.Play("ShirleyUp", 0, 0.25f);
            } else {
                animator.Play("ShirleyDown", 0, 0.25f);
            }
        }
    }

    public void Teleport(Vector3 distance) {
        transform.position += distance;
    }

    public void OnFire(InputAction.CallbackContext ctx) {
        fireDirection = ctx.ReadValue<Vector2>();
        if (fireDirection != Vector2.zero) {
            Firepoint oldFirepoint = currentFirePoint;
            fireDirection = GetClosestAngle(fireDirection);
            UpdateFirepoint(fireDirection);

            firePointDown.spriteRenderer.enabled = false;
            firePointUp.spriteRenderer.enabled = false;
            firePointLeft.spriteRenderer.enabled = false;
            firePointRight.spriteRenderer.enabled = false;
            currentFirePoint.spriteRenderer.enabled = true;
        }
    }

    private void UpdateFirepoint(Vector2 newAngle) {
        for (int i = 0; i < angles.Length; i++) {
            if (newAngle == angles[i]) {
                currentFirePoint = firePoints[i];
            }
        }
    }

    private Vector2 GetClosestAngle(Vector2 angle) {
        float min = float.MaxValue;
        Vector2 minAngle = Vector2.zero;
        for (int i = 0; i < angles.Length; i++) {
            float curAngle = Vector2.Angle(angle, angles[i]);
            if (curAngle <= min) {
                min = curAngle;
                minAngle = angles[i];
            }
        }
        return minAngle;
    }

    public void OnCheat(InputAction.CallbackContext ctx) {
        if (!ctx.performed) {
            return;
        }
        GameManager.instance.NextLevel();
    }

    public void Update() {
        if (currentState == PlayerState.Frozen) {
            return;
        }

        if (fireDirection != Vector2.zero && curDelay == 0 && PlayerManager.instance.mp >= PlayerManager.instance.ManaCost()) {
            PlayerManager.instance.mp -= PlayerManager.instance.ManaCost();
            Shoot();
            curDelay = Mathf.Max(4, (maxDelay - PlayerManager.instance.dex));
        }

        if (currentState == PlayerState.Transition && cameraController.IsNotSwitchingScene()) {
            currentState = PlayerState.Move;
        }

        if (currentState == PlayerState.Move || currentState == PlayerState.Transition) {
            if (movement != Vector2.zero) {
                animator.SetFloat("SpeedMultiplier", 1.0f);
            } else {
                animator.SetFloat("SpeedMultiplier", 0.0f);
                AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
                animator.Play(info.shortNameHash, 0, 0.0f);
                // print("not mobing");
            }
        }
    }
    public void FixedUpdate() {
        if (currentState == PlayerState.Frozen) {
            return;
        }

        if (currentState == PlayerState.Move) {
            float speedMult = baseSpeed + PlayerManager.instance.spd * 0.5f;
            // rbd.MovePosition(rbd.position + speedMult * movement * Time.fixedDeltaTime);
            rbd.AddForce(speedMult * movement, ForceMode2D.Impulse);
            rbd.velocity = Vector2.ClampMagnitude(rbd.velocity, speedMult);
        }
        curDelay -= 1;
        if (curDelay < 0) {
            curDelay = 0;
        }

        fireArrowCooldown -= 1;
        if (fireArrowCooldown <= 0) {
            firePointDown.spriteRenderer.enabled = false;
            firePointUp.spriteRenderer.enabled = false;
            firePointLeft.spriteRenderer.enabled = false;
            firePointRight.spriteRenderer.enabled = false;
        }
    }

    public void DidDie() {
        currentState = PlayerState.Frozen;
        rbd.constraints = RigidbodyConstraints2D.FreezeAll;
        firePointDown.spriteRenderer.enabled = false;
        firePointUp.spriteRenderer.enabled = false;
        firePointLeft.spriteRenderer.enabled = false;
        firePointRight.spriteRenderer.enabled = false;
        animator.Play("ShirleyDead", 0, 0.0f);
    }

    private void Shoot() {
        switch (PlayerManager.instance.curGun) {
            case PlayerGun.Basic:
                ShootBasic();
                break;
            case PlayerGun.Grass:
                ShootGrass();
                break;
            case PlayerGun.Water:
                ShootWater();
                break;
            case PlayerGun.Fire:
                ShootFire();
                break;
        }
        fireArrowCooldown = fireArrowCoolLen;
    }

    private void ShootBasic() {
        GameObject bullet = ObjectPool.instance.basicBullets.Get();
        if (bullet != null) {
            bullet.GetComponent<Bullet>().direction = GetJiggleDirection(fireDirection);
            bullet.GetComponent<Bullet>().playerDirection = movement;
            bullet.transform.position = GetJigglePosition(currentFirePoint.transform.position);
            bullet.transform.rotation = Quaternion.identity;
            bullet.transform.Rotate(currentFirePoint.transform.rotation.eulerAngles);
            bullet.SetActive(true);
            currentFirePoint.Animate();
        }
    }

    private void ShootGrass() {
        GameObject bullet1 = ObjectPool.instance.grassBullets.Get();
        GameObject bullet2 = ObjectPool.instance.grassBullets.Get();
        Vector2 direction = GetJiggleDirection(fireDirection);
        float jiggle = Random.Range(-0.125f, 0.125f);
        GameObject[] bullets = new GameObject[] { bullet1, bullet2 };
        for (int i = 0; i < bullets.Length; i++) {
            GameObject bullet = bullets[i];
            if (bullet == null) {
                continue;
            }
            bullet.GetComponent<Bullet>().direction = direction;
            bullet.GetComponent<Bullet>().playerDirection = movement;
            var position = GetShootTwicePosition(currentFirePoint.transform.position, jiggle, i);
            bullet.transform.position = position;
            bullet.transform.rotation = Quaternion.identity;
            bullet.transform.Rotate(currentFirePoint.transform.rotation.eulerAngles);
            bullet.SetActive(true);
        }
        if (bullet2) {
            bullet2.GetComponent<Bullet>().displayOnTop = true;
        }
        currentFirePoint.Animate();
    }

    private void ShootWater() {
        GameObject bullet = ObjectPool.instance.waterBullets.Get();
        if (bullet != null) {
            bullet.GetComponent<Bullet>().destroyOnEnemyImpact = false;
            bullet.GetComponent<Bullet>().direction = GetJiggleDirection(fireDirection);
            bullet.GetComponent<Bullet>().playerDirection = movement;
            bullet.transform.position = GetJigglePosition(currentFirePoint.transform.position);
            bullet.transform.rotation = Quaternion.identity;
            bullet.transform.Rotate(currentFirePoint.transform.rotation.eulerAngles);
            bullet.SetActive(true);
            currentFirePoint.Animate();
        }
    }

    private void ShootFire() {
        GameObject bullet = ObjectPool.instance.fireBullets.Get();
        if (bullet != null) {
            bullet.GetComponent<Bullet>().direction = GetJiggleDirection(fireDirection);
            bullet.GetComponent<Bullet>().playerDirection = movement;
            bullet.transform.position = GetJigglePosition(currentFirePoint.transform.position);
            bullet.transform.rotation = Quaternion.identity;
            bullet.transform.Rotate(currentFirePoint.transform.rotation.eulerAngles);
            bullet.SetActive(true);
            currentFirePoint.Animate();
        }
    }

    private Vector3 GetJiggleDirection(Vector2 direction) {
        if (currentFirePoint == firePointLeft || currentFirePoint == firePointRight) {
            return new Vector2(direction.x, direction.y + Random.Range(-0.05f, 0.05f));
        } else {
            return new Vector2(direction.x + Random.Range(-0.05f, 0.05f), direction.y);
        }
    }

    private Vector3 GetJigglePosition(Vector3 position) {
        if (currentFirePoint == firePointLeft || currentFirePoint == firePointRight) {
            return new Vector3(position.x, position.y + Random.Range(-0.125f, 0.125f), position.z);
        } else {
            return new Vector3(position.x + Random.Range(-0.125f, 0.125f), position.y, position.z);
        }
    }

    private Vector3 GetShootTwicePosition(Vector3 position, float jiggle, int version) {
        if (version % 2 == 0) {
            if (currentFirePoint == firePointLeft || currentFirePoint == firePointRight) {
                return new Vector3(position.x, position.y - 0.175f + jiggle, position.z);
            } else {
                return new Vector3(position.x - 0.175f + jiggle, position.y, position.z);
            }
        } else {
            if (currentFirePoint == firePointLeft || currentFirePoint == firePointRight) {
                return new Vector3(position.x, position.y + 0.175f + jiggle, position.z);
            } else {
                return new Vector3(position.x + 0.175f + jiggle, position.y, position.z);
            }
        }
    }

    public void RotateBullet(InputAction.CallbackContext ctx) {
        if (!ctx.performed) {
            return;
        }

        int nextGun = (int)PlayerManager.instance.curGun + 1;
        nextGun %= 4;
        PlayerManager.instance.curGun = (PlayerGun)nextGun;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        EnemyCollision(collision.gameObject);
    }

    private void OnCollisionStay2D(Collision2D collision) {
        EnemyCollision(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        EnemyCollision(collision.gameObject);
        MoveRoom(collision.gameObject);
    }

    private void OnTriggerStay2D(Collider2D collision) {
        // We only want the stay collision for spikes
        if (collision.gameObject.CompareTag("Spikes")) {
            EnemyCollision(collision.gameObject);
        }
    }

    public void WillChangeLevels() {
        currentState = PlayerState.Frozen;
    }

    private IEnumerator blinkRoutine;

    private void MoveRoom(GameObject collision) {
        if (currentState == PlayerState.Transition
            || currentState == PlayerState.Frozen) {
            return;
        }

        if (collision.CompareTag("Door") && currentState != PlayerState.Transition) {
            Door door = collision.GetComponent<Door>();
            currentState = PlayerState.Transition;

            switch (door.doorType) {
                case Door.DoorType.Left:
                    // Give 1 px of wiggle room
                    transform.position += new Vector3(-(teleportDistanceX + capsuleCollider.bounds.size.x + 1f / 16f), 0, 0);
                    break;
                case Door.DoorType.Right:
                    transform.position += new Vector3(teleportDistanceX + capsuleCollider.bounds.size.x + 1f / 16f, 0, 0);
                    break;
                case Door.DoorType.Top:
                    transform.position += new Vector3(0, teleportDistanceY + capsuleCollider.bounds.size.y + 1f / 16f, 0);
                    break;
                case Door.DoorType.Bottom:
                    transform.position += new Vector3(0, -(teleportDistanceY + capsuleCollider.bounds.size.y + 1f / 116f), 0);
                    break;
            }
        }
    }

    private void EnemyCollision(GameObject collision) {
        if (isInvincible
            || currentState == PlayerState.Transition
            || currentState == PlayerState.Frozen) {
            return;
        }

        if (collision.CompareTag("Enemy") || collision.CompareTag("EnemyBullet") || collision.CompareTag("Spikes")) {
            if (PlayerManager.instance.PlayerHit()) {
                DidDie();
                return;
            }
            currentState = PlayerState.Knockback;
            isInvincible = true;
            rbd.velocity = Vector2.zero;
            Vector2 knockback = transform.position - collision.transform.position;
            knockback = knockback.normalized;
            rbd.velocity = Vector2.zero;
            animator.Play("ShirleyHurt", 0, 0.0f);
            animator.SetFloat("SpeedMultiplier", 0.0f);
            if (collision.CompareTag("Enemy")) {
                rbd.AddForce(knockback * 4, ForceMode2D.Impulse);
            } else if (collision.CompareTag("EnemyBullet")) {
                collision.GetComponent<EnemyBullet>().RemoveSelf();
                rbd.AddForce(knockback * 6, ForceMode2D.Impulse);
            } else if (collision.CompareTag("Spikes")) {
                rbd.AddForce(knockback * 6, ForceMode2D.Impulse);
            }
            StartCoroutine(ApplyKnockback());
            blinkRoutine = Blink();
            StartCoroutine(blinkRoutine);
        }
    }

    IEnumerator Blink() {
        while (isInvincible) {
            // print("switching" + Random.Range(1, 10).ToString());
            // I guess we'll allow for some fudge room
            if (spriteRenderer.color.a == 1.0f) {
                spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            } else {
                spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            }
            yield return new WaitForSeconds(0.25f);
        }
        spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        yield break;
    }

    IEnumerator ApplyKnockback() {
        yield return new WaitForSeconds(0.05f);
        // rbd.velocity = Vector2.zero;
        StartCoroutine(ResumeMovement());
    }

    IEnumerator ResumeMovement() {
        yield return new WaitForSeconds(0.15f);
        if (currentState == PlayerState.Knockback) {
            // animator.Play("ShirleyDown", 0, 0.0f);
            currentState = PlayerState.Move;
            if (movement != Vector2.zero) {
                // animator.SetFloat("SpeedMultiplier", 1.0f);
                Vector2 moveDir = GetClosestAngle(movement);
                if (moveDir == Vector2.left) {
                    animator.Play("ShirleyLeft", 0, 0.25f);
                } else if (moveDir == Vector2.right) {
                    animator.Play("ShirleyRight", 0, 0.25f);
                } else if (moveDir == Vector2.up) {
                    animator.Play("ShirleyUp", 0, 0.25f);
                } else {
                    animator.Play("ShirleyDown", 0, 0.25f);
                }
            } else {
                animator.Play("ShirleyDown");
            }

        }
        StartCoroutine(ResumeVulnerability());
    }

    IEnumerator ResumeVulnerability() {
        yield return new WaitForSeconds(0.8f);
        isInvincible = false;
        StopCoroutine(blinkRoutine);
        spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }

}
