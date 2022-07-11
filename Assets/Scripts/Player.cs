using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerState {
    Move,
    Knockback,
    Transition
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
    public BoxCollider2D boxCollider;
    public CameraController cameraController;
    public Animator animator;

    [SerializeField]
    private float teleportDistanceX;
    [SerializeField]
    private float teleportDistanceY;

    public float moveSpeed;
    public int maxDelay;
    public int curDelay = 0;

    private PlayerState currentState;
    private bool isInvincible;
    Vector2 movement;
    Vector2 fireDirection;
    readonly Vector2[] angles = { Vector2.right, Vector2.left, Vector2.up, Vector2.down };

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rbd = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();

        firePoints = new Firepoint[] { firePointRight, firePointLeft, firePointUp, firePointDown };
        currentState = PlayerState.Move;
        isInvincible = false;
    }

    private void Start() {
        firePointDown.spriteRenderer.enabled = true;
        firePointUp.spriteRenderer.enabled = false;
        firePointLeft.spriteRenderer.enabled = false;
        firePointRight.spriteRenderer.enabled = false;

        currentFirePoint = firePointDown;
    }
    public void Move(InputAction.CallbackContext ctx) {
        movement = ctx.ReadValue<Vector2>();

        if (movement != Vector2.zero && currentState != PlayerState.Knockback) {
            // animator.SetFloat("SpeedMultiplier", 1.0f);
            Vector2 moveDir = GetClosestAngle(movement);
            if (moveDir == Vector2.left) {
                animator.Play("ShirleyLeft", 0, 0.25f);
            }
            else if (moveDir == Vector2.right) {
                animator.Play("ShirleyRight", 0, 0.25f);
            }
            else if (moveDir == Vector2.up) {
                animator.Play("ShirleyUp", 0, 0.25f);
            }
            else {
                animator.Play("ShirleyDown", 0, 0.25f);
            }
        }
    }

    public void Teleport(Vector3 distance) {
        transform.position += distance;
    }

    public void Fire(InputAction.CallbackContext ctx) {
        fireDirection = ctx.ReadValue<Vector2>();
        if (fireDirection != Vector2.zero) {
            Firepoint oldFirepoint = currentFirePoint;
            fireDirection = GetClosestAngle(fireDirection);
            UpdateFirepoint(fireDirection);

            if (oldFirepoint != currentFirePoint) {
                // print("changing!");
                firePointDown.spriteRenderer.enabled = false;
                firePointUp.spriteRenderer.enabled = false;
                firePointLeft.spriteRenderer.enabled = false;
                firePointRight.spriteRenderer.enabled = false;
                currentFirePoint.spriteRenderer.enabled = true;
            }
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

    public void Update() {
        if (fireDirection != Vector2.zero && curDelay == 0 && PlayerManager.instance.mp >= PlayerManager.instance.ManaCost()) {
            PlayerManager.instance.mp -= PlayerManager.instance.ManaCost();
            Shoot();
            curDelay = maxDelay;
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
            }
        }
    }
    public void FixedUpdate() {
        if (currentState == PlayerState.Move) {
            rbd.MovePosition(rbd.position + moveSpeed * movement * Time.fixedDeltaTime);
        }
        curDelay -= 1;
        if (curDelay < 0) {
            curDelay = 0;
        }
    }

    private void Shoot() {
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

    private Vector3 GetJiggleDirection(Vector2 direction) {
        if (currentFirePoint == firePointLeft || currentFirePoint == firePointRight) {
            return new Vector2(direction.x, direction.y + Random.Range(-0.05f, 0.05f));
        }
        else {
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

    private void OnCollisionEnter2D(Collision2D collision) {
        EnemyCollision(collision.gameObject);
    }

    private void OnCollisionStay2D(Collision2D collision) {
        EnemyCollision(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        EnemyCollision(collision.gameObject);
        // print("trige enter!");

        if (collision.gameObject.CompareTag("Door") && currentState != PlayerState.Transition) {
            Door door = collision.gameObject.GetComponent<Door>();
            currentState = PlayerState.Transition;

            switch (door.doorType) {
                case Door.DoorType.Left:
                    transform.position += new Vector3(-(teleportDistanceX + boxCollider.bounds.size.x), 0, 0);
                    break;
                case Door.DoorType.Right:
                    transform.position += new Vector3(teleportDistanceX + boxCollider.bounds.size.x, 0, 0);
                    break;
                case Door.DoorType.Top:
                    transform.position += new Vector3(0, teleportDistanceY + boxCollider.bounds.size.y, 0);
                    break;
                case Door.DoorType.Bottom:
                    transform.position += new Vector3(0, -(teleportDistanceY + boxCollider.bounds.size.y), 0);
                    break;
            }
        }

    }

    private IEnumerator blinkRoutine;


    private void EnemyCollision(GameObject collision) {
        if (!isInvincible && (collision.CompareTag("Enemy") || collision.CompareTag("EnemyBullet"))) {
            PlayerManager.instance.PlayerHit();
            currentState = PlayerState.Knockback;
            isInvincible = true;
            rbd.velocity = Vector2.zero;
            // spriteRenderer.color = new Color(1.0f, 0.5f, 0.5f, 1.0f);
            Vector2 knockback = transform.position - collision.transform.position;
            knockback = knockback.normalized;
            rbd.velocity = Vector2.zero;
            animator.Play("ShirleyHurt", 0, 0.0f);
            animator.SetFloat("SpeedMultiplier", 0.0f);
            // print("knockback");
            if (collision.CompareTag("Enemy")) {
                rbd.AddForce(knockback * 2, ForceMode2D.Impulse);
            } else {
                collision.GetComponent<EnemyBullet>().RemoveSelf();
                rbd.AddForce(knockback * 4, ForceMode2D.Impulse);
            }
            StartCoroutine(ApplyKnockback());
            blinkRoutine = Blink();
            StartCoroutine(blinkRoutine);
        }
    }

    IEnumerator Blink() {
        // print("calling blink");
        while (isInvincible) {
            // print("switching" + Random.Range(1, 10).ToString());
            // I guess we'll allow for some fudge room
            if (spriteRenderer.color.a == 1.0f) {
                spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.7f);
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
        rbd.velocity = Vector2.zero;
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
                }
                else if (moveDir == Vector2.right) {
                    animator.Play("ShirleyRight", 0, 0.25f);
                }
                else if (moveDir == Vector2.up) {
                    animator.Play("ShirleyUp", 0, 0.25f);
                }
                else {
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
