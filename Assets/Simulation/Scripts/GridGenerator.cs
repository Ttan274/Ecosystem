using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private int width;
    [SerializeField] private int height;

    [Header("Tile Prefab & Materials")]
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Material material;

    private Tile[,] grid;
    private TileType[,] currentMap;
    private float[,] perlinMap;

    public static GridGenerator instance;

    private void Awake()
    {
        instance = this;
    }

    public void MapSetup(int w, int h)
    {
        width = w; 
        height = h;
    }

    private void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.S))
            SaveManager.Instance.SaveMap(currentMap, perlinMap, width, height);

        if (Input.GetKeyDown(KeyCode.L))
            if (SaveManager.Instance.LoadMap() != (null, null))
                GenerateFromLoadedMap(SaveManager.Instance.LoadMap().Item1, SaveManager.Instance.LoadMap().Item2);
    */
    }

    private void GenerateFromLoadedMap(TileType[,] tileTypes, float[,] perlin)
    {
        // Clear existing tiles
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        grid = new Tile[width, height];
        currentMap = tileTypes;
        perlinMap = perlin;
        
        InstantiateTiles(tileTypes);
    }

    public void GenerateGrid()
    {
        // Clear existing tiles
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        grid = new Tile[width, height];
        currentMap = new TileType[width, height];
        perlinMap = new float[width, height];

        GenerateWithPerlin(currentMap);
        AddMuddyEdges(currentMap);
        InstantiateTiles(currentMap);
    }

    private void GenerateWithPerlin(TileType[,] typeMap)
    {
        float scale = 0.1f; // smaller = larger features
        float offsetX = Random.Range(0f, 1000f);
        float offsetY = Random.Range(0f, 1000f);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float xCoord = offsetX + x * scale;
                float yCoord = offsetY + y * scale;
                float value = Mathf.PerlinNoise(xCoord, yCoord);

                if (value < 0.25f)
                    typeMap[x, y] = TileType.Water;
                else if (value < 0.72f)
                    typeMap[x, y] = TileType.Walkable;
                else
                    typeMap[x, y] = TileType.Blocked;

                perlinMap[x, y] = value;
            }
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


                tile.Initialize(new Vector2Int(x, y), typeMap[x, y], material, GetColorShade(typeMap[x, y], perlinMap[x, y]));
                grid[x, y] = tile;
            }
        }
    }

    private void AddMuddyEdges(TileType[,] typeMap)
    {
        Vector2Int[] directions = {
        Vector2Int.up, Vector2Int.down,
        Vector2Int.left, Vector2Int.right
    };

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (typeMap[x, y] != TileType.Walkable)
                    continue;

                foreach (var dir in directions)
                {
                    Vector2Int neighbor = new Vector2Int(x, y) + dir;
                    if (IsInBounds(neighbor) && typeMap[neighbor.x, neighbor.y] == TileType.Water)
                    {
                        typeMap[x, y] = TileType.Mud;
                        break;
                    }
                }
            }
        }
    }

    private Color GetColorShade(TileType type, float p)
    {
        float intensity = 1f;

        switch (type)
        {
            case TileType.Water:
                intensity = Mathf.Lerp(0.3f, 0.6f, p / 0.25f); // deeper = darker
                return new Color(0.2f, 0.4f, 1f) * intensity;

            case TileType.Walkable:
                intensity = Mathf.Lerp(0.6f, 1.0f, (p - 0.25f) / 0.55f);
                return new Color(0.5f, 1f, 0.5f) * intensity;

            case TileType.Blocked:
                intensity = Mathf.Lerp(0.4f, 0.7f, (p - 0.8f) / 0.2f);
                return new Color(0.4f, 0.4f, 0.4f) * intensity;

            case TileType.Mud:
                float mudIntensity = Mathf.Lerp(0.5f, 0.7f, p);
                return new Color(0.5f, 0.35f, 0.2f) * mudIntensity;

            default:
                return Color.white;
        }
    }

    private bool IsInBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
    }

    //debugging methods
    private int CountTilesOfType(TileType[,] typeMap, TileType type)
    {
        int count = 0;
        foreach (var t in typeMap)
            if (t == type) count++;
        return count;
    }
}
