using System.Collections;
using UnityEngine;

public class PlantManager : MonoBehaviour
{
    [Header("Regrow Settings")]
    [SerializeField] private float baseRegrowTime = 5f;
    [SerializeField] private float droughtRegrowMultiplier = 2f;

    public static PlantManager Instance;

    private void Awake()
    {
        if(Instance != this && Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    #region Enable/Disable
    private void OnEnable()
    {
        WorldEvents.OnPlantConsumed += HandlePlantConsumed;
    }
  
    private void OnDisable()
    {
        WorldEvents.OnPlantConsumed -= HandlePlantConsumed;
    }

    #endregion
   
    private void HandlePlantConsumed(Plant plant)
    {
        //Getting tile
        Tile tile = plant.parentTile;
        if (tile == null) return;

        //Resetting tile
        tile.hasPlant = false;
        tile.plant = null;

        //Setting regrow delay
        float delay = baseRegrowTime;
        if (WorldManager.Instance.IsDroughtActive)
            delay *= droughtRegrowMultiplier;

        StartCoroutine(RegrowCoroutine(delay));
    }

    private IEnumerator RegrowCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        MapGen.Instance.CreateBush();
    }
}
