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
    public List<Room> roomTemplates;
    public Room startRoom;
    public Room shopRoom;
    public Room bossRoom;
    public Room libraryRoom;

    public List<Room> loadedRooms = new List<Room>();
    public HashSet<Vector2Int> roomCoords = new HashSet<Vector2Int>();
    public Queue<RoomInfo> plannedRooms = new Queue<RoomInfo>();

    private List<Room> possibleRoomTemplates;

    // Enemies and bosses list
    public List<GameObject> enemies0 = new List<GameObject>();
    public List<GameObject> enemies1 = new List<GameObject>();
    public List<GameObject> enemies2 = new List<GameObject>();
    public List<GameObject> enemies3 = new List<GameObject>();
    public List<GameObject> enemies4 = new List<GameObject>();
    public List<GameObject> enemies5 = new List<GameObject>();
    public List<GameObject> enemies6 = new List<GameObject>();
    public List<GameObject> enemies7 = new List<GameObject>();
    public List<GameObject> enemies8 = new List<GameObject>();

    public GameObject goldenSlime;

    private List<List<GameObject>> enemiesList = new List<List<GameObject>>();

    public List<GameObject> bossList = new List<GameObject>();

    public List<GameObject> enemyBank = new List<GameObject>();
    public GameObject boss;

    public CameraController cameraController;

    public Minimap minimap;

    public int baseRoomNum;

    public static readonly float roomHeight = 180f / 16f;
    public static readonly float roomWidth = 320f / 16f;

    public static int minSpawnAmount = 1;
    public static int maxSpawnAmount = 6;

    public AudioClip lorelei;
    public AudioClip gretchen;
    public AudioClip nacht;
    public AudioClip writerTheme;
    public AudioClip shopTheme;
    public AudioClip bossTheme;

    private void Awake() {
        enemiesList.Add(enemies0);
        enemiesList.Add(enemies1);
        enemiesList.Add(enemies2);
        enemiesList.Add(enemies3);
        enemiesList.Add(enemies4);
        enemiesList.Add(enemies5);
        enemiesList.Add(enemies6);
        enemiesList.Add(enemies7);
        enemiesList.Add(enemies8);
        possibleRoomTemplates = new List<Room>(roomTemplates);
    }

    private void Start() {
        enemyBank = enemiesList[GameManager.instance.currentLevel];
        boss = bossList[GameManager.instance.currentLevel];
        GenerateRandomRooms();
        SetupRooms();
        StartMusic(RoomType.Regular);
        PlayerManager.instance.DidStartLevel(GameManager.instance.currentLevel);
        GameManager.instance.DidStartLevel();
    }

    public void StartMusic(RoomType roomType) {
        switch (roomType) {
            case RoomType.Regular: case RoomType.Start:
                if (GameManager.instance.currentLevel < 3) {
                    GameManager.instance.PlayMusic(lorelei);
                } else if (GameManager.instance.currentLevel < 6) {
                    GameManager.instance.PlayMusic(gretchen);
                } else {
                    GameManager.instance.PlayMusic(nacht);
                }
                break;
            case RoomType.Shop:
                GameManager.instance.PlayMusic(shopTheme);
                break;
            case RoomType.Library:
                GameManager.instance.PlayMusic(writerTheme);
                break;
            case RoomType.Boss:
                GameManager.instance.PlayMusic(bossTheme);
                break;
        }
    }

    public void OnPlayerEnterRoom(Room room) {
        cameraController.curRoom = room;
    }

    private bool NeighborInQueue(
        Vector2Int curPos,
        Vector2Int lastDir,
        Queue<(Vector2Int, Vector2Int)> queue,
        HashSet<Vector2Int> visited) {
        List<Vector2Int> directions = new List<Vector2Int>() { Vector2Int.left, Vector2Int.right, Vector2Int.up, Vector2Int.down };
        directions.Remove(lastDir * -1);
        foreach (var dir in directions) {
            foreach (var fromDir in directions) {
                if (queue.Contains((curPos + dir, fromDir))) {
                    // print("found niehggbor in queue");
                    return true;
                }
            }

            if (visited.Contains(curPos + dir)) {
                return true;
            }
        }
        return false;
    }

    private void AddRandomPosition(
        Queue<(Vector2Int, Vector2Int)> queue,
        Vector2Int curPos,
        Vector2Int lastDir,
        HashSet<Vector2Int> visited) {
        List<Vector2Int> directions = new List<Vector2Int>() { Vector2Int.left, Vector2Int.right, Vector2Int.up, Vector2Int.down };
        directions.Remove(lastDir * -1);

        int desiredNeighbors = Random.Range(1, directions.Count);
        int totalAdded = 0;
        while (totalAdded < desiredNeighbors) {
            foreach (var dir in directions) {
                if (NeighborInQueue(dir + curPos, dir, queue, visited)) {
                    if (Random.Range(0, 8) == 0) {
                        queue.Enqueue((dir + curPos, dir));
                        return;
                    }
                } else if (dir == lastDir) {
                    if (Random.Range(0, 6) >= 1) {
                        queue.Enqueue((dir + curPos, dir));
                        totalAdded += 1;
                    }
                } else {
                    if (Random.Range(0, 3) >= 1) {
                        queue.Enqueue((dir + curPos, dir));
                        totalAdded += 1;
                    }
                }
            }
        }
    }

    // REMINDER: UPDATE WHEN ADDING NEW ROOMS
    private Room GenerateRandomRoom(Vector2Int coord) {
        if (coord == Vector2Int.zero) {
            return startRoom;
        } else {
            if (possibleRoomTemplates.Count == 0) {
                possibleRoomTemplates = new List<Room>(roomTemplates);
            }

            int idx = Random.Range(0, possibleRoomTemplates.Count);
            Room retRoom = possibleRoomTemplates[idx];
            possibleRoomTemplates.RemoveAt(idx);
            return retRoom;
        }
    }
    private Queue<RoomInfo> GetAvailableRooms(List<RoomInfo> tempRooms) {
        List<RoomInfo> threeWalls = new List<RoomInfo>();
        List<RoomInfo> twoWalls = new List<RoomInfo>();
        // We should never have to use this list, but as a fail-safe, I guess
        List<RoomInfo> junkWalls = new List<RoomInfo>();
        foreach (RoomInfo room in tempRooms) {
            if (room.xCoord == 0 && room.yCoord == 0) {
                continue;
            }

            bool leftRoomExists = DoesPlannedRoomExist(room.xCoord - 1, room.yCoord);
            bool rightRoomExists = DoesPlannedRoomExist(room.xCoord + 1, room.yCoord);
            bool upRoomExists = DoesPlannedRoomExist(room.xCoord, room.yCoord + 1);
            bool downRoomExists = DoesPlannedRoomExist(room.xCoord, room.yCoord - 1);

            int leftWall = leftRoomExists ? 0 : 1;
            int rightWall = rightRoomExists ? 0 : 1;
            int upWall = upRoomExists ? 0 : 1;
            int downWall = downRoomExists ? 0 : 1;

            int total = leftWall + rightWall + upWall + downWall;

            if (total >= 3) {
                threeWalls.Add(room);
            } else if ((!leftRoomExists && !upRoomExists)
                     || (!upRoomExists && !rightRoomExists)
                     || (!rightRoomExists && !downRoomExists)
                     || (!downRoomExists && !leftRoomExists)) {
                twoWalls.Add(room);
            } else {
                junkWalls.Add(room);
            }
        }

        threeWalls.Shuffle();
        twoWalls.Shuffle();
        threeWalls.AddRange(twoWalls);
        threeWalls.AddRange(junkWalls);
        Queue<RoomInfo> avail = new Queue<RoomInfo>();
        foreach (var room in threeWalls) {
            avail.Enqueue(room);
        }
        return avail;
    }

    private void SetupRooms() {
        List<RoomInfo> tempRooms = new List<RoomInfo>();

        foreach (var coord in roomCoords) {
            RoomInfo info;
            info.xCoord = coord.x;
            info.yCoord = coord.y;
            info.room = GenerateRandomRoom(coord);
            if (coord == Vector2Int.zero || info.room.spawnPoints.Count < minSpawnAmount) {
                info.numEnemies = 0;
            } else {
                info.numEnemies = Random.Range(minSpawnAmount, Mathf.Min(info.room.spawnPoints.Count, maxSpawnAmount));
            }
            tempRooms.Add(info);
        }

        Queue<Room> special = new Queue<Room>();
        special.Enqueue(bossRoom);
        special.Enqueue(shopRoom);
        if (GameManager.instance.currentLevel < 3) {
            if (Random.Range(0f, 1f) <= 0.25f) {
                special.Enqueue(shopRoom);
            }
        } else if (GameManager.instance.currentLevel < 6) {
            if (Random.Range(0f, 1f) <= 0.5f) {
                special.Enqueue(shopRoom);
            }
        } else {
            if (Random.Range(0f, 1f) <= 0.75f) {
                special.Enqueue(shopRoom);
            }
        }
        special.Enqueue(libraryRoom);

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
            Vector2 newPos = new Vector2(info.xCoord * roomWidth, info.yCoord * roomHeight);
            Room newRoom = Instantiate(info.room, newPos, Quaternion.identity);
            newRoom.roomController = this;
            newRoom.cameraController = cameraController;
            newRoom.enemyBank = (info.room == bossRoom) ? new List<GameObject> { boss } : enemyBank;
            newRoom.goldenSlime = goldenSlime;
            newRoom.xCoord = info.xCoord;
            newRoom.yCoord = info.yCoord;
            newRoom.numEnemies = info.numEnemies;

            // Determine room type
            if (info.room == bossRoom) {
                newRoom.type = RoomType.Boss;
            } else if (info.room == shopRoom) {
                newRoom.type = RoomType.Shop;
            } else if (info.xCoord == 0 && info.yCoord == 0) {
                newRoom.type = RoomType.Start;
            } else if (info.room == libraryRoom) {
                newRoom.type = RoomType.Library;
            } else {
                newRoom.type = RoomType.Regular;
            }

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
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        Vector2Int startPos = new Vector2Int(0, 0);
        visited.Add(startPos);
        var queue = new Queue<(Vector2Int, Vector2Int)>();
        AddRandomPosition(queue, startPos, Vector2Int.zero, visited);
        int numRooms = Random.Range(0, 3) + baseRoomNum + GameManager.instance.currentLevel;
        // numRooms = 4;
        while (queue.Count != 0 && visited.Count < numRooms) {
            var (curPos, dir) = queue.Dequeue();
            if (!visited.Contains(curPos)) {
                visited.Add(curPos);
                AddRandomPosition(queue, curPos, dir, visited);
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
