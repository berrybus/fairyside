using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firepoint : MonoBehaviour {
  public SpriteRenderer spriteRenderer;
  public Animator animator;

  private void Awake() {
    spriteRenderer = GetComponent<SpriteRenderer>();
    animator = GetComponent<Animator>();
  }

  private void Start() {
    // animator.Play("Firepoint", 0, 0.50f);
    animator.speed = 0.0f;
  }

  public void Animate() {
    animator.Play("Firepoint", 0, 0.0f);
    animator.speed = 1.0f;
    // print("animate firepoint");
  }
}
