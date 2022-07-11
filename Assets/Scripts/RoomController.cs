using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public struct RoomInfo {
    public Room room;
    public int xCoord;
    public int yCoord;
    public int numEnemies;
}

public static class ListExtensions {
    private static System.Random rng = new System.Random();
    public static void Shuffle<T>(this IList<T> list) {
        int n = list.Count;
        while (n > 1) {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}

public class RoomController : MonoBehaviour {

    //public static RoomController instance;
    public List<Room> roomTemplates;
    public Room startRoom;
    public Room shopRoom;
    public Room bossRoom;

    public List<Room> loadedRooms = new List<Room>();
    public HashSet<Vector2Int> roomCoords = new HashSet<Vector2Int>();
    public Queue<RoomInfo> plannedRooms = new Queue<RoomInfo>();

    public List<GameObject> enemyBank = new List<GameObject>();
    public GameObject boss;

    public CameraController cameraController;

    public Minimap minimap;

    public int minRooms = 12;
    public int maxRooms = 24;

    public static readonly float roomHeight = 180f / 16f;
    public static readonly float roomWidth = 320f / 16f;


    private void Start() {
        GenerateRandomRooms();
        SetupRooms();
    }

    public void OnPlayerEnterRoom(Room room) {
        cameraController.curRoom = room;
    }

    private void AddRandomPosition(Queue<(Vector2Int, Vector2Int)> queue, Vector2Int curPos, Vector2Int lastDir) {
        List<Vector2Int> directions = new List<Vector2Int>() { Vector2Int.left, Vector2Int.right, Vector2Int.up, Vector2Int.down };
        directions.Remove(lastDir * -1);
        int numIter = Random.Range(1, directions.Count);
        for (int i = 0; i < numIter; i++) {
            Vector2Int next = Vector2Int.zero;
            while (next == Vector2Int.zero) {
                foreach (var dir in directions) {
                    if (dir == lastDir) {
                        if (Random.Range(0, 8) >= 1) {
                            next = dir;
                            break;
                        }
                    }
                    else {
                        if (Random.Range(0, 8) == 1) {
                            next = dir;
                            break;
                        }
                    }
                }
            }
            directions.Remove(next);
            queue.Enqueue((next + curPos, next));
        }
    }

    // REMINDER: UPDATE WHEN ADDING NEW ROOMS
    private Room GenerateRandomRoom(Vector2Int coord) {
        if (coord == Vector2Int.zero) {
            return startRoom;
        }
        else {
            return roomTemplates[Random.Range(0, roomTemplates.Count)];
        }
    }
    private Queue<RoomInfo> GetAvailableRooms(List<RoomInfo> tempRooms) {
        List<RoomInfo> threeWalls = new List<RoomInfo>();
        List<RoomInfo> twoWalls = new List<RoomInfo>();
        foreach (RoomInfo room in tempRooms) {
            if (room.xCoord == 0 && room.yCoord == 0) {
                continue;
            }
            int leftWall = DoesPlannedRoomExist(room.xCoord - 1, room.yCoord) ? 0 : 1;
            int rightWall = DoesPlannedRoomExist(room.xCoord + 1, room.yCoord) ? 0 : 1;
            int upWall = DoesPlannedRoomExist(room.xCoord, room.yCoord + 1) ? 0 : 1;
            int downWall = DoesPlannedRoomExist(room.xCoord, room.yCoord - 1) ? 0 : 1;

            int total = leftWall + rightWall + upWall + downWall;

            if (total >= 3) {
                // print("adding 3 room");
                threeWalls.Add(room);
            }
            else if (total >= 2) {
                // print("adding 2 room");
                twoWalls.Add(room);
            }
        }

        threeWalls.Shuffle();
        twoWalls.Shuffle();
        threeWalls.AddRange(twoWalls);
        Queue<RoomInfo> avail = new Queue<RoomInfo>();
        foreach (var room in threeWalls) {
            avail.Enqueue(room);
        }
        return avail;
    }

    private void SetupRooms() {
        //print("Random rooms:");
        List<RoomInfo> tempRooms = new List<RoomInfo>();

        foreach (var coord in roomCoords) {
            RoomInfo info;
            info.xCoord = coord.x;
            info.yCoord = coord.y;
            info.room = GenerateRandomRoom(coord);
            info.numEnemies = coord != Vector2Int.zero ? Random.Range(2, System.Math.Min(8, info.room.spawnPoints.Count)) : 0;
            // info.numEnemies = coord != Vector2Int.zero ? 1 : 0;
            tempRooms.Add(info);
        }

        Queue<Room> special = new Queue<Room>();
        special.Enqueue(shopRoom);
        special.Enqueue(bossRoom);

        Queue<RoomInfo> avail = GetAvailableRooms(tempRooms);

        while (special.Count > 0 && avail.Count > 0) {
            RoomInfo newSpecial = avail.Dequeue();
            tempRooms.Remove(newSpecial);
            newSpecial.room = special.Dequeue();
            newSpecial.numEnemies = (newSpecial.room == bossRoom) ? 1 : 0;
            if (newSpecial.xCoord == 0 && newSpecial.yCoord == 0) {
                print("VERY BAD THING!!!!!!");
            }
            tempRooms.Add(newSpecial);
        }

        foreach (var room in tempRooms) {
            plannedRooms.Enqueue(room);
        }

        // Create empty rooms
        foreach (RoomInfo info in plannedRooms) {
            //StartCoroutine(CreateScenes());
            Vector2 newPos = new Vector2(info.xCoord * roomWidth, info.yCoord * roomHeight);
            Room newRoom = Instantiate(info.room, newPos, Quaternion.identity);
            newRoom.roomController = this;
            newRoom.cameraController = cameraController;
            newRoom.enemyBank = (info.room == bossRoom) ? new List<GameObject> { boss } : enemyBank;
            newRoom.xCoord = info.xCoord;
            newRoom.yCoord = info.yCoord;
            newRoom.numEnemies = info.numEnemies;
            // newRoom.OpenDoorsIfPossible();
            loadedRooms.Add(newRoom);
        }

        minimap.Setup(loadedRooms);
        Room startRoom = GetRoomFromPosition(0, 0);
        if (startRoom != null) {
            minimap.ChangeRoom(startRoom);
        }
    }

    // remember to check nullness
    public Room GetRoomFromPosition(int xCoord, int yCoord) {
        return loadedRooms.Find(room => room.xCoord == xCoord && room.yCoord == yCoord);
    }

    private void GenerateRandomRooms() {
        //print("generating");
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        Vector2Int startPos = new Vector2Int(0, 0);
        visited.Add(startPos);
        var queue = new Queue<(Vector2Int, Vector2Int)>();
        AddRandomPosition(queue, startPos, Vector2Int.zero);
        int numRooms = Random.Range(minRooms, maxRooms);
        while (queue.Count != 0 && visited.Count < numRooms) {
            var (curPos, dir) = queue.Dequeue();
            if (!visited.Contains(curPos)) {
                visited.Add(curPos);
                AddRandomPosition(queue, curPos, dir);
            }
        }

        roomCoords = visited;

    }

    public bool DoesPlannedRoomExist(int x, int y) {
        foreach (var coord in roomCoords) {
            if (coord.x == x && coord.y == y) {
                return true;
            }
        }
        return false;
    }

}
