using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomFirstDungeonGenerator : SimpleRandomWalkDungeonGenerator
{
    [SerializeField]
    private int minRoomWidth = 4, minRoomHeight = 4;

    [SerializeField]
    private int dungeonWidth = 20, dungeonHeight = 20;

    [SerializeField]
    [Range(0, 10)]
    private int offset = 1;

    [SerializeField]
    private bool randomWalkRooms = false;

    [Space(10)]

    public int minObjNum = 50, maxObjNum = 100;
    public GameObject turret;
    public List<GameObject> spawnedObjects = new List<GameObject>();
    public List<GameObject> spawnedLights = new List<GameObject> ();

    [Space(10)]
    public GameObject roomCentreLight;

    private GameObject wallTilemap;

    protected override void RunProceduralGeneration()
    {
        tilemapVisualizer.Clear();

        wallTilemap = transform.parent.transform.Find("Grid").transform.Find("WallTileMap").gameObject;
        CreateRooms();
        GenerateShadows(wallTilemap);
    }

    private void GenerateObjectsOnFloor(HashSet<Vector2Int> floor)
    {
        foreach (var item in spawnedObjects)
        {
            DestroyImmediate(item);
        }
        spawnedObjects.Clear();

        var num = UnityEngine.Random.Range(minObjNum, maxObjNum);

        int objectsSpawned = 0;

        while(objectsSpawned <= num)
        {
            GameObject turrett = Instantiate(turret, (Vector2)floor.ElementAt(UnityEngine.Random.Range(0, floor.Count)), Quaternion.identity);
            turrett.transform.parent = gameObject.transform.parent;
            spawnedObjects.Add(turrett);
            objectsSpawned++;
        }
        
    }

    public override void Clear()
    {
        foreach (var item in spawnedObjects)
        {
            DestroyImmediate(item);
        }
        spawnedObjects.Clear();

        foreach (var item in spawnedLights)
        {
            DestroyImmediate(item);
        }
        spawnedLights.Clear();

        tilemapVisualizer.Clear();
    }

    public void GenerateShadows(GameObject wallTilemap)
    {
        
    }

    private void CreateRooms()
    {      
        var roomList = ProceduralGenerationAlgorithms.BinarySpacePartitioning(new BoundsInt((Vector3Int)new Vector2Int(startPos.x - dungeonWidth / 2, startPos.y - dungeonHeight / 2),
            new Vector3Int(dungeonWidth, dungeonHeight, 0)), minRoomWidth, minRoomHeight);

        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();

        if (randomWalkRooms)
        {
            floor = CreateRoomsRandomWalk(roomList);
        }
        else
        {
            floor = CreateSimpleRooms(roomList);
        }

        List<Vector2Int> roomCentres = new List<Vector2Int>();

        roomCentres.Add(startPos);

        foreach (var room in roomList)
        {
            roomCentres.Add((Vector2Int)Vector3Int.RoundToInt(room.center));
        }

        HashSet<Vector2Int> corridors = ConnectRooms(roomCentres);
        floor.UnionWith(corridors);

        tilemapVisualizer.PaintFloorTiles(floor);
        WallGenerator.CreateWalls(floor, tilemapVisualizer);
    }    

    private HashSet<Vector2Int> CreateRoomsRandomWalk(List<BoundsInt> roomList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();

        foreach (var item in spawnedLights)
        {
            DestroyImmediate(item);
        }
        spawnedLights.Clear();

        for (int i = 0; i < roomList.Count; i++)
        {
            var roombounds = roomList[i];
            var roomCenter = new Vector2Int(Mathf.RoundToInt(roombounds.center.x), Mathf.RoundToInt(roombounds.center.y));            

            if(UnityEngine.Random.Range(0, 101) < 20)
            {
                GameObject light = Instantiate(roomCentreLight, (Vector2)roomCenter, Quaternion.identity);
                light.transform.parent = gameObject.transform.parent;
                spawnedLights.Add(light);
            }            

            var roomFloor = RunRandomWalk(randomWalkParameters, roomCenter);
            foreach (var position in roomFloor)
            {
                if (position.x >= (roombounds.xMin + offset) && position.x <= (roombounds.xMax - offset) && position.y >= (roombounds.yMin - offset) && position.y <= (roombounds.yMax - offset))
                {
                    floor.Add(position);
                }
            }

            GenerateObjectsOnFloor(floor);

        }
        return floor;
    }

    private HashSet<Vector2Int> ConnectRooms(List<Vector2Int> roomCentres)
    {
        HashSet<Vector2Int> corridors = new HashSet<Vector2Int>();
        var currentRoomCenter = roomCentres[UnityEngine.Random.Range(0, roomCentres.Count)];
        roomCentres.Remove(currentRoomCenter);

        while (roomCentres.Count > 0)
        {
            Vector2Int closest = FindClosestPointTo(currentRoomCenter, roomCentres);
            roomCentres.Remove(closest);
            HashSet<Vector2Int> newCorridor = CreateCorridor(currentRoomCenter, closest);
            currentRoomCenter = closest;
            corridors.UnionWith(newCorridor);
        }
        return corridors;
    }

    private HashSet<Vector2Int> CreateCorridor(Vector2Int currentRoomCenter, Vector2Int destination)
    {
        HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
        var position = currentRoomCenter;
        corridor.Add(position);

        while (position.y != destination.y)
        {
            if (destination.y > position.y)
            {
                position += Vector2Int.up;
            }
            else if (destination.y < position.y)
            {
                position += Vector2Int.down;
            }
            corridor.Add(position);
        }
        while (position.x != destination.x)
        {
            if (destination.x > position.x)
            {
                position += Vector2Int.right;
            }
            else if (destination.x < position.x)
            {
                position += Vector2Int.left;
            }
            corridor.Add(position);
        }
        return corridor;
    }

    private Vector2Int FindClosestPointTo(Vector2Int currentRoomCenter, List<Vector2Int> roomCentres)
    {
        Vector2Int closest = Vector2Int.zero;
        float distance = float.MaxValue;

        foreach (var position in roomCentres)
        {
            float currentDistance = Vector2.Distance(position, currentRoomCenter);
            if (currentDistance < distance)
            {
                distance = currentDistance;
                closest = position;
            }
        }

        return closest;
    }

    private HashSet<Vector2Int> CreateSimpleRooms(List<BoundsInt> roomList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();

        foreach (var item in spawnedLights)
        {
            DestroyImmediate(item);
        }
        spawnedLights.Clear();

        foreach (var room in roomList)
        {
            var roombounds = room;
            var roomCenter = new Vector2Int(Mathf.RoundToInt(roombounds.center.x), Mathf.RoundToInt(roombounds.center.y));

            if (UnityEngine.Random.Range(0, 101) < 20)
            {
                GameObject light = Instantiate(roomCentreLight, (Vector2)roomCenter, Quaternion.identity);
                light.transform.parent = gameObject.transform.parent;
                spawnedLights.Add(light);
            }

            for (int col = offset; col < room.size.x - offset; col++)
            {
                for (int row = offset; row < room.size.y - offset; row++)
                {
                    Vector2Int position = (Vector2Int)room.min + new Vector2Int(col, row);
                    floor.Add(position);
                }
            }

            GenerateObjectsOnFloor(floor);
        }                

        return floor;
    }
}
