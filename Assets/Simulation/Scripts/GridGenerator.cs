using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private int lakeCount;
    [SerializeField] private int lakeSize;

    [Header("Tile Prefab & Materials")]
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Material[] materials;

    private Tile[,] grid;

    private void Start()
    {
        GenerateGrid();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
            GenerateGrid();
    }

    private void GenerateGrid()
    {
        // Clear existing tiles
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        grid = new Tile[width, height];
        TileType[,] typeMap = new TileType[width, height];

        GenerateLakes(typeMap);
        AssignRemainingTiles(typeMap);
        InstantiateTiles(typeMap);
    }

    private void GenerateLakes(TileType[,] typeMap)
    {
        for (int i = 0; i < lakeCount; i++)
        {
            Vector2Int center = new Vector2Int(
                Random.Range(2, width - 2),
                Random.Range(2, height - 2)
            );
            FillLake(typeMap, center, lakeSize);
        }
    }

    private void FillLake(TileType[,] typeMap, Vector2Int center, int maxTiles)
    {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        queue.Enqueue(center);
        visited.Add(center);
        typeMap[center.x, center.y] = TileType.Water;

        int filled = 1;
        Vector2Int[] directions = {
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
        };

        while (queue.Count > 0 && filled < maxTiles)
        {
            Vector2Int current = queue.Dequeue();

            foreach (var dir in directions)
            {
                Vector2Int next = current + dir;
                if (IsInBounds(next) && !visited.Contains(next) && typeMap[next.x, next.y] == TileType.None)
                {
                    typeMap[next.x, next.y] = TileType.Water;
                    queue.Enqueue(next);
                    visited.Add(next);
                    if (++filled >= maxTiles) break;
                }
            }
        }
    }

    private void AssignRemainingTiles(TileType[,] typeMap)
    {
        List<Vector2Int> unassigned = new();

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                if (typeMap[x, y] == TileType.None)
                    unassigned.Add(new Vector2Int(x, y));

        Shuffle(unassigned);

        int totalTiles = width * height;
        int waterCount = CountTilesOfType(typeMap, TileType.Water);
        int walkableCount = (int)((totalTiles - waterCount) * 0.95f);

        for (int i = 0; i < unassigned.Count; i++)
        {
            Vector2Int pos = unassigned[i];
            typeMap[pos.x, pos.y] = i < walkableCount ? TileType.Walkable : TileType.Blocked;
        }
    }

    private void InstantiateTiles(TileType[,] typeMap)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 position = new Vector3(x, 0, y);
                GameObject tileObj = Instantiate(tilePrefab, position, Quaternion.identity, transform);
                Tile tile = tileObj.GetComponent<Tile>();

                TileType type = typeMap[x, y];
                tile.Initialize(new Vector2Int(x, y), type, GetMaterial(type));

                grid[x, y] = tile;
            }
        }
    }

    private Material GetMaterial(TileType type)
    {
        return type switch
        {
            TileType.Walkable => materials[0],
            TileType.Water => materials[1],
            TileType.Blocked => materials[2],
            _ => materials[0],
        };
    }

    private bool IsInBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
    }

    private void Shuffle(List<Vector2Int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }

    private int CountTilesOfType(TileType[,] typeMap, TileType type)
    {
        int count = 0;
        foreach (var t in typeMap)
            if (t == type) count++;
        return count;
    }
}
