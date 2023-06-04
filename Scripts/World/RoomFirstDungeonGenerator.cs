using System.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;

public class RoomFirstDungeonGenerator : SimpleRandWalkMapGen
{
    [Header("Prefabs")]
    public GameObject mobParent;
    public GameObject sturctureParent;
    public Slime slime;
    public Knight knight;
    public GameObject merchantStore;
    public GameObject[] torches;

    [Header("UI")]
    public Slider waveSlider;
    public TextMeshProUGUI waveStartingText;

    [Header("Debugging/Info")]
    [SerializeField] private bool spawnKnight;
    [SerializeField] private bool spawnSlime;
    [SerializeField] private float slimeSpawnRate;
    [SerializeField] private float knightSpawnRate;
    public List<Vector2Int> potentialTorchPos = new List<Vector2Int>();
    [SerializeField] private int minRoomWidth = 4, minRoomHeight = 4, dungeonWidth = 20, dungeonHeight = 20, offset = 1;
    [SerializeField] private bool randomWalkRooms = false;

    private HashSet<Vector2Int> potentialMobPos = new HashSet<Vector2Int>();
    public List<Vector2> centers = new List<Vector2>();

    private void Update()
    {
        if (mobParent.transform.childCount < 3 && spawnKnight && spawnSlime)
        {
            StartCoroutine(SpawnNextWave(2f));
        }
        waveSlider.value = mobParent.transform.childCount;
    }
    private IEnumerator SpawnNextWave(float delay)
    {
        StartCoroutine(NextWaveText(delay));
        yield return new WaitForSeconds(delay);
        DeleteAllObjectsInParent(mobParent);
        PlaceMobs(potentialMobPos);
        waveSlider.maxValue = mobParent.transform.childCount;
        StopAllCoroutines();
        yield return null;
    }
    private IEnumerator NextWaveText(float delay)
    {
        waveStartingText.gameObject.SetActive(true);
        yield return new WaitForSeconds(delay);
        waveStartingText.gameObject.SetActive(false);
    }
    protected override void RunProceduralGeneration()
    {
        #region Clear previous data
        ClearHashSets();
        DeleteAllObjectsInParent(mobParent);
        DeleteAllObjectsInParent(sturctureParent);
        #endregion
        
        CreateRooms();

        #region Place Prefabs
        PlaceMobs(potentialMobPos);
        PlaceTorches(potentialTorchPos);
        PlaceMerchantShop();

        waveSlider.maxValue = mobParent.transform.childCount;
        waveSlider.value = mobParent.transform.childCount;
        #endregion
    }
    private void CreateRooms()
    {
        var roomList = ProceduralGenerationAlgorithms.BinarySpacePartitioning(new BoundsInt((Vector3Int) startPos, new Vector3Int(dungeonWidth, dungeonHeight, 0)), minRoomWidth, minRoomHeight);
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();

        floor = (randomWalkRooms) ?  CreateRoomsRandomly(roomList) : CreateSimpleRooms(roomList);

        List<Vector2Int> roomCenters = new List<Vector2Int>();
        int minX = 0, maxX = 0, minY = 0, maxY = 0; 
        foreach (var room in roomList)
        {
            #region Mob Positions
            roomCenters.Add((Vector2Int) Vector3Int.RoundToInt(room.center));
            centers.Add((Vector2Int) Vector3Int.RoundToInt(room.center));

            minX = Mathf.RoundToInt(room.center.x - room.size.x / 2);
            maxX = Mathf.RoundToInt(room.center.x + room.size.x / 2);
            minY = Mathf.RoundToInt(room.center.y - room.size.y / 2);
            maxY = Mathf.RoundToInt(room.center.y + room.size.y / 2);

            System.Random rand = new System.Random();
            potentialMobPos.Add(new Vector2Int(rand.Next(minX + 3, maxX - 4), rand.Next(minY + 2, maxY - 2)));
            potentialMobPos.Add(new Vector2Int(rand.Next(minX + 3, maxX - 4), rand.Next(minY + 2, maxY - 2)));
            #endregion

            #region Torch Positions
            potentialTorchPos.Add(new Vector2Int(room.xMax - 3, room.yMax - 3));
            potentialTorchPos.Add(new Vector2Int(room.xMin + 2, room.yMin + 2));
            potentialTorchPos.Add(new Vector2Int(room.xMin + 2, room.yMax - 2));
            potentialTorchPos.Add(new Vector2Int(room.xMax - 3, room.yMin + 2));
            #endregion
        }

        HashSet<Vector2Int> corridors = ConnectRooms(roomCenters);
        floor.UnionWith(corridors);

        mapVisualizer.Draw(floor);
        WallGenerator.CreateWalls(floor, mapVisualizer);
    }
    private void PlaceMerchantShop()
    {
        var spawnPos = centers[Random.Range(0, centers.Count - 1)];
        GameObject shop = Instantiate(merchantStore, spawnPos, Quaternion.identity);
        shop.transform.parent = sturctureParent.transform;
    }
    private void PlaceTorches(List<Vector2Int> positions)
    {
        foreach (var pos in positions)
        {
            float rand = Random.Range(0f, 1f);
            #region Torch
            if (rand >= 0.5f)
            {
                GameObject torch = Instantiate(torches[Random.Range(0, torches.Length - 1)], new Vector3(pos.x, pos.y, 0), Quaternion.identity).gameObject;
                torch.transform.parent = sturctureParent.transform;
            }
            #endregion
        }
    }
    private void PlaceMobs(HashSet<Vector2Int> positions)
    {
        foreach (var pos in positions)
        {
            System.Random r = new System.Random();
            if (r.NextDouble() < slimeSpawnRate && spawnSlime)
            {
                GameObject slimeGo = Instantiate(slime, new Vector3(pos.x, pos.y, 0), Quaternion.identity).gameObject;
                slimeGo.transform.parent = mobParent.transform;
            }
            else if (r.NextDouble() < knightSpawnRate && spawnKnight)
            {
                GameObject knightGo = Instantiate(knight, new Vector3(pos.x, pos.y, 0), Quaternion.identity).gameObject;
                knightGo.transform.parent = mobParent.transform;
            }
        }
    }
    private void DeleteAllObjectsInParent(GameObject parentObject)
    {
        if (parentObject != null)
        {
            int childCount = parentObject.transform.childCount;

            for (int i = childCount - 1; i >= 0; i--)
            {
                GameObject child = parentObject.transform.GetChild(i).gameObject;
                DestroyImmediate(child);
            }
        }
    }
    private HashSet<Vector2Int> CreateRoomsRandomly(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        for (int i = 0; i < roomsList.Count; i++)
        {
            var roomBounds = roomsList[i];
            var roomCenter = new Vector2Int(Mathf.RoundToInt(roomBounds.center.x), Mathf.RoundToInt(roomBounds.center.y));
            var roomFloor = StartRandomWalk(randomWalkParameters, roomCenter);

            foreach (var pos in roomFloor)
            {
                if (pos.x >= (roomBounds.xMin + offset) && pos.x <= (roomBounds.xMax - offset) && pos.y >= (roomBounds.yMin - offset) && pos.y <= (roomBounds.yMax - offset))
                {
                    floor.Add(pos);
                }
            }
        }
        return floor;
    }
    private HashSet<Vector2Int> ConnectRooms(List<Vector2Int> roomCenters)
    {
        HashSet<Vector2Int> corridors = new HashSet<Vector2Int>();
        var currentRoomCenter = roomCenters[Random.Range(0, roomCenters.Count)];
        roomCenters.Remove(currentRoomCenter);

        while (roomCenters.Count > 0)
        {
            Vector2Int closest = FindClosestPointTo(currentRoomCenter, roomCenters);
            roomCenters.Remove(closest);
            HashSet<Vector2Int> newCorridor = CreateCorridor(currentRoomCenter, closest);
            currentRoomCenter = closest;
            corridors.UnionWith(newCorridor);
        }
        return corridors;
    }
    private HashSet<Vector2Int> CreateCorridor(Vector2Int currentRoomCenter, Vector2Int destination)
    {
        HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
        var pos = currentRoomCenter;
        corridor.Add(pos);

        while (pos.y != destination.y)
        {
            if (destination.y > pos.y)
                pos += Vector2Int.up;
            else if (destination.y < pos.y)
                pos += Vector2Int.down;

            corridor.Add(pos);
        }

        while (pos.x != destination.x)
        {
            if (destination.x > pos.x)
                pos += Vector2Int.right;
            else if (destination.x < pos.x)
                pos += Vector2Int.left;

            corridor.Add(pos);
        }

        return corridor;
    }
    private Vector2Int FindClosestPointTo(Vector2Int currentRoomCenter, List<Vector2Int> roomCenters)
    {
        Vector2Int closest = Vector2Int.zero;
        float dist = float.MaxValue;

        foreach (var pos in roomCenters)
        {
            float currDist = Vector2.Distance(pos, currentRoomCenter);
            if (currDist < dist)
            {
                dist = currDist;
                closest = pos;
            }
        }
        return closest;
    }
    private HashSet<Vector2Int> CreateSimpleRooms(List<BoundsInt> roomList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        foreach (var room in roomList)
        {
            for (int col = offset; col < room.size.x - offset; col++)
            {
                for (int row = offset; row < room.size.y - offset; row++)
                {
                    Vector2Int pos = (Vector2Int) room.min + new Vector2Int(col, row);
                    floor.Add(pos);
                }
            }
        }

        return floor;
    }
    private void ClearHashSets()
    {
        potentialMobPos.Clear();
        centers.Clear();
        potentialTorchPos.Clear();
    }
}