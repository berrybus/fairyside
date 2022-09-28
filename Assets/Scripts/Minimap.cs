using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    [SerializeField]
    MapTile mapTile;

    public static readonly float tileHeight = 5f;
    public static readonly float tileWidth = 7f;

    private List<MapTile> mapTiles;

    private float maxX = 0;
    private float maxY = 0;

    private float minX = int.MaxValue;
    private float minY = int.MaxValue;

    public void Setup(List<Room> rooms) {
        mapTiles = new List<MapTile>();
        foreach (var room in rooms) {
            MapTile tile = Instantiate(mapTile, gameObject.transform);
            tile.transform.localScale = new Vector3(16, 16, 1);
            tile.transform.localPosition = new Vector3(room.xCoord * tileWidth, room.yCoord * tileHeight, 0);
            tile.xCoord = room.xCoord;
            tile.yCoord = room.yCoord;
            maxX = Mathf.Max(maxX, room.xCoord);
            maxY = Mathf.Max(maxY, room.yCoord);
            minX = Mathf.Min(minX, room.xCoord);
            minY = Mathf.Min(minY, room.yCoord);

            switch (room.type) {
                case RoomType.Boss:
                    tile.iconRenderer.sprite = tile.iconBoss;
                    tile.isIconHidden = true;
                    break;
                case RoomType.Shop:
                    tile.iconRenderer.sprite = tile.iconShop;
                    tile.isIconHidden = false;
                    break;
                case RoomType.Start:
                    tile.iconRenderer.sprite = tile.iconStart;
                    tile.isIconHidden = false;
                    break;
                case RoomType.Library:
                    tile.iconRenderer.sprite = tile.iconLibrary;
                    tile.isIconHidden = true;
                    break;
                case RoomType.Regular:
                    break;
            }
            mapTiles.Add(tile);
        }
    }

    public void ChangeRoom(Room room) {
        foreach (MapTile tile in mapTiles) {
            if (tile.xCoord == room.xCoord && tile.yCoord == room.yCoord) {
                tile.visited = true;
                tile.spriteRenderer.sprite = tile.selected;
            } else {
                if (tile.visited) {
                    tile.spriteRenderer.sprite = tile.clear;
                } else {
                    tile.spriteRenderer.sprite = tile.unselected;
                }
            }

            tile.iconRenderer.enabled = tile.visited || !tile.isIconHidden;
        }
    }

    public void MoveLeft() {
        gameObject.transform.localPosition += new Vector3(tileWidth, 0, 0);
    }

    public void MoveRight() {
        gameObject.transform.localPosition -= new Vector3(tileWidth, 0, 0);
    }

    public void MoveUp() {
        gameObject.transform.localPosition -= new Vector3(0, tileHeight, 0);
    }

    public void MoveDown() {
        gameObject.transform.localPosition += new Vector3(0, tileHeight, 0);
    }
}
