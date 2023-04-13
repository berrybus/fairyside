using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPDrop: MagneticItem {
    [SerializeField]
    private GemDeath death;
    private float speedLow = 1.0f;
    private float speedHigh = 5.0f;

    public AudioClip coin;

    public bool shouldRemoveSelf = false;

    public void Start() {
        if (shouldRemoveSelf) {
            StartCoroutine(RemoveSelf());
        }
    }

    public void Scatter(Vector2 dir) {
        rbd.AddForce(dir * Random.Range(speedLow, speedHigh), ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Player") && PlayerManager.instance.hp < PlayerManager.instance.maxHp) {
            PlayerManager.instance.hp += 1;
            PlayerManager.instance.hp = Mathf.Min(PlayerManager.instance.maxHp, PlayerManager.instance.hp);
            GameManager.instance.PlaySFX(coin);
            Instantiate(death, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }

    }

    private IEnumerator RemoveSelf() {
        yield return new WaitForSeconds(5.0f);
        Destroy(gameObject);
    }


}
