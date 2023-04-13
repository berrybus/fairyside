using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {
    public SpriteRenderer spriteRenderer;
    public RoomController roomController;
    public CameraController cameraController;
    public BoxCollider2D boxCollider;
    public Animator animator;
    public GameObject blocker;

    public enum DoorType {
        Left, Right, Top, Bottom
    }

    public DoorType doorType;

    public int xCoordDest;
    public int yCoordDest;

    private void Awake() {
        boxCollider = GetComponent<BoxCollider2D>();
        roomController = GetComponent<RoomController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void Start() {
        if (!boxCollider.isTrigger) {
            animator.speed = 0.0f;
        }
    }

    public void Open() {
        animator.speed = 1.0f;
        Destroy(blocker);
        boxCollider.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player") && cameraController.IsNotSwitchingScene()) {
            Room nextRoom = roomController.GetRoomFromPosition(xCoordDest, yCoordDest);
            if (nextRoom != null) {
                nextRoom.SetEnemiesActive(collision.gameObject.transform);
                nextRoom.OpenDoorsIfPossible();
                roomController.minimap.ChangeRoom(nextRoom);
                roomController.StartMusic(nextRoom.type);
            }
            switch (doorType) {
                case DoorType.Left:
                    roomController.minimap.MoveLeft();
                    cameraController.MoveCameraLeft();
                    break;
                case DoorType.Right:
                    roomController.minimap.MoveRight();
                    cameraController.MoveCameraRight();
                    break;
                case DoorType.Top:
                    roomController.minimap.MoveUp();
                    cameraController.MoveCameraUp();
                    break;
                case DoorType.Bottom:
                    roomController.minimap.MoveDown();
                    cameraController.MoveCameraDown();
                    break;
            }
        }
    }
}
