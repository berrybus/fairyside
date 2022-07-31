using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    public float speed;
    public Rigidbody2D rbd;
    public Vector2 direction;
    public Vector2 playerDirection;
    protected bool released = false;
    protected Vector3 initialPosition;
    public bool destroyOnEnemyImpact = true;
    public bool displayOnTop = false;

    private void Awake() {
        rbd = GetComponent<Rigidbody2D>();
    }
    private void OnEnable() {
        rbd.velocity = direction * speed + playerDirection * speed / 4;
        released = false;
        displayOnTop = false;
        initialPosition = gameObject.transform.position;
    }

    public virtual void RemoveSelf() { }
}
