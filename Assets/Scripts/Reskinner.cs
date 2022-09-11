using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reskinner : MonoBehaviour {
    
    public List<Sprite> spriteList = new List<Sprite>();

    private SpriteRenderer spriteRenderer;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void Start() {
        int idx = (GameManager.instance.currentLevel / GameManager.levelsPerSkin) % spriteList.Count;
        spriteRenderer.sprite = spriteList[idx];
    }
}
