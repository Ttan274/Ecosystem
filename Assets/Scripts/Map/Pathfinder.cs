using System;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    public static Pathfinder Instance;
    private Tile[,] tiles;
    private int width, height;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void Initialize(Tile[,] tiles, int width, int height)
    {
        this.tiles = tiles;
        this.width = width;
        this.height = height;
    }

    public List<Tile> CreatePath(Tile start, Tile end)
    {
        //Setup
        List<Tile> path = new List<Tile>();
        HashSet<Tile> visited = new HashSet<Tile>();
        Dictionary<Tile, Tile> cameFrom = new Dictionary<Tile, Tile>();
        Dictionary<Tile, float> costs = new Dictionary<Tile, float>();
        
        path.Add(start);
        costs[start] = 0f;

        while(path.Count > 0)
        {
            Tile current = path[0];
            float bestCost = costs[current] + Heuristic(current, end);
            foreach (Tile t in path)
            {
                float totalCost = costs[t] + Heuristic(t, end);
                if(totalCost < bestCost)
                {
                    current = t;
                    bestCost = totalCost;
                }
            }

            if (current == end)
                return ConstructPath(cameFrom, current);

            path.Remove(current);
            visited.Add(current);

            //checking for negighbours
            foreach (Tile neighbour in GetNeighbors(current))
            {
                if (visited.Contains(neighbour) || !neighbour.IsWalkable()) continue;

                float val = costs[current] + 1;
                if (!costs.ContainsKey(neighbour) || val < costs[neighbour])
                {
                    cameFrom[neighbour] = current;
                    costs[neighbour] = val;
                    if(!path.Contains(neighbour))
                        path.Add(neighbour);
                }
            }
        }

        return null;
    }

    private List<Tile> GetNeighbors(Tile current)
    {
        List<Tile> neighbors = new List<Tile>();
        Vector2Int[] directions = new Vector2Int[]
        {
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
        };

        foreach (Vector2Int dir in directions)
        {
            int pX = current.x + dir.x;
            int pZ = current.z + dir.y;
            if (pX >= 0 && pX < width && pZ >= 0 && pZ < height)
                neighbors.Add(tiles[pX, pZ]);
        }

        return neighbors;
    }

    private List<Tile> ConstructPath(Dictionary<Tile, Tile> cameFrom, Tile current)
    {
        List<Tile> path = new List<Tile>();

        while(cameFrom.ContainsKey(current))
        {
            path.Insert(0, current);
            current = cameFrom[current];
        }
        return path;
    }

    private float Heuristic(Tile a, Tile b)
    {
        return Vector2.Distance(new Vector2(a.x, a.z), new Vector2(b.x, b.z));
    }

    public Tile GetTileAtPosition(Vector3 position)
    {
        int x = Mathf.RoundToInt(position.x);
        int z = Mathf.RoundToInt(position.z);

        Tile dest = GetTileGrid(x, z);

        return dest;
    }

    private bool IsInsideMap(int pX, int pZ) => pX >= 0 && pX < width && pZ >= 0 && pZ < height;
  
    public Tile GetTileGrid(int pX, int pZ)
    {
        if (IsInsideMap(pX, pZ))
        {
            return tiles[pX, pZ];
        }
        return null;
    }

    public Tile GetClosestWaterTile(Tile current)
    {
        Tile closest = null;
        float minDist = Mathf.Infinity;

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Tile tile = tiles[x, z];
                if (!tile.IsWalkable()) continue;

                foreach (Tile neighbor in GetNeighbors(tile))
                {
                    if(neighbor.tileType == TileType.Water)
                    {
                        float distance = Vector3.Distance(current.transform.position, tile.transform.position);
                        if (distance < minDist)
                        {
                            minDist = distance;
                            closest = tile;
                        }
                        break;
                    }
                }
            }
        }

        return closest;
    }
}
