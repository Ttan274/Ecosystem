using UnityEngine;

public class Plant : MonoBehaviour, IFoodSource
{
    private Tile parentTile;
    private bool isAlive = true;
    public Transform FoodTransform => transform;
    public bool IsAvailable => isAlive;
    
    public void Initalize(Tile t) => parentTile = t;

    public void Consume()
    {
        parentTile.ResetTile(WorldManager.Instance.IsDroughtActive);
        isAlive = false;
        gameObject.SetActive(false);
    }
}
