using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Writer : MonoBehaviour {
    public TMP_Text popupText;
    public SpriteRenderer popup;
    public AudioClip click;
    public AudioClip awawa;
    public AudioClip honk;
    private int hp;
    private int maxHp = 1000;

    public string initialText;

    private int damageQueueCooldown = 8;
    private int totalDamageTexts = 0;

    public GameObject enemyDeath;

    [SerializeField]
    private Transform hpBar;
    [SerializeField]
    private SpriteRenderer hpBarSprite;
    [SerializeField]
    private SpriteRenderer hpOutlineSprite;

    private void Awake() {
        hp = maxHp;
    }

    private string[] textBankSchool = new string[] {
        "I'm working!",
        "Who are you...?",
        "I'm on the verge of a breakthrough!",
        "You should be careful here",
        "Fools, the lot of them!",
    };

    private string[] textBankCastle = new string[] {
        "What are you doing here?",
        "I haven't finished writing yet!",
        "Please don't come any closer!",
        "Do you believe me?",
        "This place is awful",
    };

    private string[] textBankForest = new string[] {
        "My notes, my notes...!",
        "My head hurts...",
        "I've lost everything!",
        "It was true after all...",
        "I can't think at all!",
        "I'm scared!",
    };

    private void Start() {
        if (PlayerManager.instance.writerDead) {
            Destroy(gameObject);
        }
        popupText.text = "";
        popup.enabled = false;
        string[][] textBankAll = new string[][] { textBankSchool, textBankCastle, textBankForest };
        string[] textBank = textBankAll[(GameManager.instance.currentLevel / 3) % 3];
        initialText = textBank[Random.Range(0, textBank.Length)];
        int dir = Random.Range(0f, 1f) <= 0.5 ? -1 : 1;
        float yOffset = dir == 1 ? Random.Range(1, 1.5f) : Random.Range(1f, 2.5f);
        transform.position += dir * new Vector3(Random.Range(2f, 4f), yOffset, 0f);
    }

    private void ShowDamageText(int dmg, bool didCrit) {
        float textOffset = 0.625f * totalDamageTexts;
        var text = Instantiate(PlayerManager.instance.damageText, transform.position + new Vector3(Random.Range(-0.125f, 0.125f), 0.125f + textOffset, 0), Quaternion.identity);
        text.damage.text = dmg.ToString();
        if (didCrit) {
            text.damage.fontSize = 10;
            text.damage.color = new Color(255f / 255f, 212f / 255f, 95f / 255f, 1.0f);
        }
    }

    protected virtual void Update() {
        float hpPercentage = (float)hp / (float)maxHp;
        hpBar.localScale = new Vector3(hpPercentage, 1, 1);
        hpBarSprite.enabled = hp < maxHp;
        hpOutlineSprite.enabled = hp < maxHp;
    }

    protected virtual void FixedUpdate() {
        if (damageQueueCooldown == 0) {
            totalDamageTexts = 0;
        } else {
            damageQueueCooldown -= 1;
        }

    }

    protected virtual void DamageSelf() {
        var status = PlayerManager.instance.EnemyHit();
        int dmg = status.Item1;
        bool didCrit = status.Item2;
        hp -= dmg;
        ShowDamageText(dmg, didCrit);
        totalDamageTexts += 1;
        damageQueueCooldown = 8;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            popup.enabled = true;
            popupText.text = initialText;
        }
        if (collision.gameObject.CompareTag("Bullet")) {
            DamageSelf();

            if (hp <= 0) {
                PlayerManager.instance.writerDead = true;
                GameManager.instance.UnlockAchievement(Achievement.AUTHOR_DEATH);
                GameManager.instance.PlaySFX(awawa);
                GameManager.instance.SaveGame();
                Instantiate(enemyDeath, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
            GameManager.instance.PlaySFX(honk);
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            popupText.text = "";
            popup.enabled = false;
        }
    }

}
