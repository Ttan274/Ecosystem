/*using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Tile))]
public class TileEditor : Editor
{
    private void OnSceneGUI()
    {
        Tile tile = (Tile)target;

        if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.control)
        {
            if (SceneView.currentDrawingSceneView.camera == null)
                return;

            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            if(Physics.Raycast(ray, out RaycastHit hit))
            {
                if(hit.collider.gameObject == tile.gameObject)
                {
                    var generator = GameObject.FindObjectOfType<GridGenerator>();
                    tile.ChangeTileType(generator.materials);

                    Event.current.Use();
                    EditorUtility.SetDirty(tile);
                }
            }
        }
    }
}*/
