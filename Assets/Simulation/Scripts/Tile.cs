#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Vector2Int gridPos;
    public TileType tileType;

    public void Initialize(Vector2Int pos, TileType type, Material material, Color? tint = null)
    {
        gridPos = pos;
        tileType = type;

        Renderer renderer = GetComponent<Renderer>();
        renderer.material = new Material(material);

        if(tint.HasValue)
            renderer.material.color = tint.Value;
        
        //Debugging
        gameObject.name = $"Tile_{gridPos.x}_{gridPos.y}";
    }

    #if UNITY_EDITOR
        void OnDrawGizmos()
        {
            Vector3 labelPos = transform.position + Vector3.up * 0.6f;

            Handles.color = Color.black;
            Handles.Label(labelPos, $"{tileType}\n({gridPos.x},{gridPos.y})");
        }
    #endif
}

[System.Serializable]
public enum TileType
{
    None,
    Walkable,
    Water,
    Blocked,
    Mud
}
