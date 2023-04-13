using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MagneticItem {
    [SerializeField]
    private GemDeath death;

    private float speedLow = 1.0f;
    private float speedHigh = 5.0f;
    public AudioClip coin;

    public void Scatter(Vector2 dir) {
        rbd.AddForce(dir * Random.Range(speedLow, speedHigh), ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            PlayerManager.instance.gemCount += 1;
            if (PlayerManager.instance.gemCount >= 100) {
                GameManager.instance.UnlockAchievement(Achievement.HOARDER);
            }
            GameManager.instance.PlaySFX(coin);
            Instantiate(death, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }

    }


}
