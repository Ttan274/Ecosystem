using System.Collections.Generic;
using UnityEngine;

public class MapGen : MonoBehaviour
{
    [Header("Map Data")]
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private int lakeCount;
    [SerializeField] private int lakeRadius;
    [SerializeField] private float irregularity = 0.5f;
    [SerializeField] private PlantableObject tree;
    [SerializeField] private PlantableObject bush;

    private Tile[,] tiles;
    private bool[,] lakeMap;
    public static MapGen Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        GenerateMap();
    }

    private void GenerateMap()
    {
        tiles = new Tile[width, height];
        lakeMap = new bool[width, height];

        // Step 1: Spawn all tiles as ground
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 pos = new Vector3(x, 0, z);
                Tile tile = Instantiate(tilePrefab, pos, Quaternion.identity, transform).GetComponent<Tile>();
                tile.SetType(TileType.Ground, x, z);
                tiles[x, z] = tile;
            }
        }

        // Step 2: Create lakes
        CreateLakes();

        // Step 3: Place trees and plants
        CreatePlantables(tree);
        CreatePlantables(bush);

        //Initalizing Pathfinder For Animals
        Pathfinder.Instance.Initialize(tiles, width, height);
        Simulation.Instance.Initalize(ShuffleGroundTiles());
        Camera.main.GetComponent<CameraController>().SetFocusPoint(tiles[width / 2, height / 2].transform);
    }

    private void CreateLakes()
    {
        int placed = 0;
        int attemps = 0;
        while (placed < lakeCount && attemps < 100)
        {
            attemps++;

            int cx = Random.Range(lakeRadius, width - lakeRadius);
            int cz = Random.Range(lakeRadius, height - lakeRadius);

            if (CheckOverlap(cx, cz, lakeRadius)) continue;

            for (int x = cx - lakeRadius; x <= cx + lakeRadius; x++)
            {
                for (int z = cz - lakeRadius; z <= cz + lakeRadius; z++)
                {
                    if (x < 0 || x >= width || z < 0 || z >= height)
                        continue;

                    float dist = Vector2.Distance(new Vector2(cx, cz), new Vector2(x, z));
                    if (dist <= lakeRadius)
                    {
                        // Add randomness to shape
                        float noise = Mathf.PerlinNoise(x * 0.3f, z * 0.3f);
                        float falloff = Mathf.InverseLerp(lakeRadius, 0, dist);
                        if (noise * irregularity + falloff * (1 - irregularity) > 0.5f)
                        {
                            tiles[x, z].SetType(TileType.Water, x, z);
                            lakeMap[x, z] = true;
                        }
                    }
                }
            }

            placed++;
        }
    }

    private List<Tile> ShuffleGroundTiles()
    {
        List<Tile> groundTiles = new List<Tile>();
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Tile t = tiles[x, z];
                if (t.tileType == TileType.Ground && !t.hasTree && !t.hasPlant)
                    groundTiles.Add(tiles[x, z]);
            }
        }
        //shuffle groundlist for trees
        for (int i = groundTiles.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (groundTiles[i], groundTiles[j]) = (groundTiles[j], groundTiles[i]);
        }

        return groundTiles;
    }

    private void CreatePlantables(PlantableObject plantable)
    {
        List<Tile> groundTiles = ShuffleGroundTiles();

        int placed = 0;
        foreach (Tile tile in groundTiles)
        {
            if (placed >= plantable.maxCount) break;
            if (Random.value < plantable.spawnChance)
            {
                switch (plantable.type)
                {
                    case PlantableObjectType.Tree:
                        tile.PlaceTree(plantable.prefab);
                        break;
                    case PlantableObjectType.Bush:
                        tile.PlacePlant(plantable.prefab);
                        break;
                }
                placed++;
            }
        }
    }

    public void CreateBush(int total)
    {
        List<Tile> groundTiles = ShuffleGroundTiles();

        for (int i = 0; i < total; i++)
        {
            int rand = Random.Range(0, groundTiles.Count);
            groundTiles[i].PlacePlant(bush.prefab);
        }
    }

    private bool CheckOverlap(int cx, int cz, int radius)
    {
        for (int x = cx - radius; x <= cx + radius; x++)
        {
            for (int z = cz - radius; z <= cz + radius; z++)
            {
                if (x >= 0 && x < width && z >= 0 && z < height)
                {
                    if (lakeMap[x, z])
                        return true;
                }
            }
        }
        return false;
    }
}

[System.Serializable]
public struct PlantableObject
{
    public PlantableObjectType type;
    public int maxCount;
    public float spawnChance;
    public GameObject prefab;
}

public enum PlantableObjectType
{
    Tree,
    Bush
}