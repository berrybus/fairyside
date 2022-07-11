using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour {
    [SerializeField]
    private GemDeath death;
    private BoxCollider2D boxCollider;
    private Rigidbody2D rbd;
    private float speedLow = 1.0f;
    private float speedHigh = 5.0f;
    // Start is called before the first frame update
    void Awake() {
        boxCollider = GetComponent<BoxCollider2D>();
        rbd = GetComponent<Rigidbody2D>();
    }

    public void Scatter(Vector2 dir) {
        rbd.AddForce(dir * Random.Range(speedLow, speedHigh), ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            PlayerManager.instance.gemCount += 1;
            Instantiate(death, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }

    }


}
