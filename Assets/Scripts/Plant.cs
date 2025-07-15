using UnityEngine;

public class Plant : MonoBehaviour
{
    [SerializeField] private Tile parentTile;

    public void Initalize(Tile t) => parentTile = t;

    public void Eat()
    {
        parentTile.ResetTile();
        Destroy(gameObject);
    }
}
