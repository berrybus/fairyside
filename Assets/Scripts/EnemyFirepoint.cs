using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFirepoint : MonoBehaviour {
    public SpriteRenderer spriteRenderer;
    public Animator animator;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Start() {
        animator.speed = 0.0f;
    }

    public void Animate() {
        animator.Play("EnemyFirepoint", 0, 0.25f);
        animator.speed = 1.0f;

    }
}
