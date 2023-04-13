using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomType {
    Boss,
    Shop,
    Start,
    Regular,
    Library
}

public class Room : MonoBehaviour {
    public int xCoord;
    public int yCoord;

    public RoomType type;

    public Door leftDoor;
    public Door rightDoor;
    public Door topDoor;
    public Door bottomDoor;

    public GameObject leftDoorWall;
    public GameObject rightDoorWall;
    public GameObject topDoorWall;
    public GameObject bottomDoorWall;

    public GameObject leftWall;
    public GameObject rightWall;
    public GameObject topWall;
    public GameObject bottomWall;

    public List<GameObject> enemyBank = new List<GameObject>();
    public List<BaseEnemy> enemies = new List<BaseEnemy>();
    public GameObject goldenSlime;

    public List<GameObject> spawnPoints = new List<GameObject>();

    public int numEnemies = 0;

    public RoomController roomController;
    public CameraController cameraController;

    public GameObject instructions;

    private void AssignDest(Door doorObject, int xCoord, int yCoord) {
        doorObject.roomController = roomController;
        doorObject.cameraController = cameraController;
        doorObject.xCoordDest = xCoord;
        doorObject.yCoordDest = yCoord;
    }

    private void Start() {
        RemoveUnconnectedDoors();
        AssignDest(leftDoor, xCoord - 1, yCoord);
        AssignDest(rightDoor, xCoord + 1, yCoord);
        AssignDest(topDoor, xCoord, yCoord + 1);
        AssignDest(bottomDoor, xCoord, yCoord - 1);
        GenerateEnemies();
        OpenDoorsIfPossible();

        if (instructions != null && ! (GameManager.instance.currentLevel == 0 && xCoord == 0 && yCoord == 0)) {
            Destroy(instructions);
        }
    }

    private void RemoveUnconnectedDoors() {
        if (roomController.DoesPlannedRoomExist(xCoord - 1, yCoord)) {
            leftWall.SetActive(false);
        }
        else {
            leftDoorWall.SetActive(false);
        }

        if (roomController.DoesPlannedRoomExist(xCoord + 1, yCoord)) {
            rightWall.SetActive(false);
        }
        else {
            rightDoorWall.SetActive(false);
        }

        if (roomController.DoesPlannedRoomExist(xCoord, yCoord + 1)) {
            topWall.SetActive(false);
        }
        else {
            topDoorWall.SetActive(false);
        }

        if (roomController.DoesPlannedRoomExist(xCoord, yCoord - 1)) {
            bottomWall.SetActive(false);
        }
        else {
            bottomDoorWall.SetActive(false);
        }
    }
    public void SetEnemiesActive(Transform playerTarget) {
        foreach (BaseEnemy enemy in enemies) {
            enemy.playerTarget = playerTarget;
            enemy.gameObject.SetActive(true);
        }
    }
    public bool OpenDoorsIfPossible() {
        if (numEnemies <= 0) {
            leftDoor.Open();
            rightDoor.Open();
            topDoor.Open();
            bottomDoor.Open();
            enemies.Clear();
            return true;
        } else {
            return false;
        }
    }

    public void GenerateEnemies() {
        for (int i = 0; i < numEnemies; i++) {
            InstantiateEnemy(enemyBank[Random.Range(0, enemyBank.Count)]);
        }

        if (spawnPoints.Count > 0 && numEnemies > 0 && Random.Range(0f, 1.0f) <= 0.005f) {
            InstantiateEnemy(goldenSlime);
        }
    }

    private void InstantiateEnemy(GameObject enemy) {
        var idx = Random.Range(0, spawnPoints.Count);
        GameObject spawnPoint = spawnPoints[idx];
        spawnPoints.RemoveAt(idx);
        Vector3 pos = new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y, 0);
        GameObject newEnemy = Instantiate(enemy, pos, Quaternion.identity);
        BaseEnemy enemyController = newEnemy.GetComponent<BaseEnemy>();
        enemyController.room = this;
        enemies.Add(enemyController);
    }
}
