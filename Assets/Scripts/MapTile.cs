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
    public Sprite iconShop;
    public Sprite iconBoss;
    public Sprite iconStart;
    public Sprite iconLibrary;
    public bool isIconHidden = false;
    public SpriteRenderer spriteRenderer;
    public SpriteRenderer iconRenderer;
}
