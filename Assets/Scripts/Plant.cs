using UnityEngine;

public class Plant : MonoBehaviour
{
    [SerializeField] private Tile parentTile;

    public void Initalize(Tile t) => parentTile = t;

    public void Eat()
    {
        Simulation.Instance.plantsEaten++;
        if (!Simulation.Instance.IsDroughtEnabled)
            parentTile.ResetTile();
        Destroy(gameObject);
    }
}
