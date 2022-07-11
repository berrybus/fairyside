using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    //public static CameraController instance;
    public Room curRoom;

    [SerializeField]
    private float moveSpeedX;
    [SerializeField]
    private float moveSpeedY;

    private float moveSpeed;
    private Vector3 targetPos = new Vector3(0, 0, -1);

    private void Update() {
        UpdatePosition();
    }

    public void MoveCameraLeft() {
        targetPos -= new Vector3(RoomController.roomWidth, 0);
        moveSpeed = moveSpeedX;
    }

    public void MoveCameraRight() {
        targetPos += new Vector3(RoomController.roomWidth, 0);
        moveSpeed = moveSpeedX;
    }

    public void MoveCameraUp() {
        targetPos += new Vector3(0, RoomController.roomHeight);
        moveSpeed = moveSpeedY;
    }

    public void MoveCameraDown() {
        targetPos -= new Vector3(0, RoomController.roomHeight);
        moveSpeed = moveSpeedY;
    }

    public void UpdatePosition() {
        transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * moveSpeed);
    }

    public bool IsNotSwitchingScene() {
        return transform.position.Equals(targetPos);
    }
}
