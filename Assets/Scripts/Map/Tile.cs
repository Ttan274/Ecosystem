using UnityEngine;

public class Tile : MonoBehaviour
{
    public TileType tileType;
    public bool hasTree = false;
    public bool hasPlant = false;
    public int x, z;

    public void SetType(TileType type, int x, int z)
    {
        tileType = type;
        this.x = x;
        this.z = z;

        Color color = Color.white;

        switch (tileType)
        {
            case TileType.Ground:
                color = new Color(0.4f, 0.8f, 0.4f);
                break;
            case TileType.Water:
                color = new Color(0.2f, 0.4f, 0.9f);
                break;
        }

        GetComponent<MeshRenderer>().material.color = color;
    }

    public bool IsWalkable() => tileType == TileType.Ground && !hasTree;

    public void PlaceTree(GameObject treePrefab)
    {
        if (hasTree) return;

        Place(treePrefab, 0);
        hasTree = true;
    }

    public void PlacePlant(GameObject plantPrefab)
    {
        if (hasPlant) return;

        Place(plantPrefab, 0.2f, true);
        hasPlant = true;
    }

    private void Place(GameObject g, float yOffset, bool isPlant = false)
    {
        Vector3 objectPos = transform.position + new Vector3(0, yOffset, 0);
        GameObject placed = Instantiate(g, objectPos, Quaternion.identity, transform.parent);

        if (isPlant)
            placed.GetComponent<Plant>().Initalize(this);
    }

    public void ResetTile()
    {
        hasPlant = false;
        MapGen.Instance.CreateBush();
    }
}

public enum TileType
{
    Ground,
    Water
}