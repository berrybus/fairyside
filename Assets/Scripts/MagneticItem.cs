using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MagneticItem : MonoBehaviour {
    protected bool canPickup = true;
    private Collider2D playerTarget;
    protected Rigidbody2D rbd;
    protected BoxCollider2D boxCollider;
    private float movespeed = 2.0f;
    private float pickupRadius = 4.0f;

    void Awake() {
        rbd = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void FixedUpdate() {
        if (canPickup && PlayerManager.instance.itemsAreMagnetic && playerTarget == null) {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, pickupRadius);
            playerTarget = Array.Find(colliders, c => c.gameObject.CompareTag("Player"));
        }

        if (playerTarget) {
            float dist = (playerTarget.bounds.center - transform.position).magnitude;
            if (dist >= 1.0f && dist <= pickupRadius) {
                rbd.velocity = (playerTarget.bounds.center - transform.position).normalized * movespeed;
            }
        }
    }

}
