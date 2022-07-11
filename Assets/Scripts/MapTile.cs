using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTile : MonoBehaviour
{
    public int xCoord;
    public int yCoord;
    public bool visited = false;
    public Sprite selected;
    public Sprite unselected;
    public Sprite clear;
    public SpriteRenderer spriteRenderer;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
}
