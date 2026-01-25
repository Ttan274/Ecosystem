using System.Collections.Generic;
using UnityEngine;

public class VisionSensor : MonoBehaviour
{
    [Header("Vision Settings")]
    [SerializeField] private float viewRadius;

    [Header("Scan Settings")]
    [SerializeField] private float scanInterval = 0.4f;
    private float scanTimer = 0;

    //Detected Targets
    public List<GameObject> visibleTargets = new(); //Water
    public List<IFoodSource> foodSources = new();   //Food (Plant, Herbivore)
    public List<Animal> visibleAnimals = new();     //Mate

    //Reference
    private Animal animal;

    private void Awake()
    {
       animal = GetComponent<Animal>();
    }

    private void Update()
    {
        scanTimer += Time.deltaTime;
        if(scanTimer >= scanInterval)
        {
            scanTimer = 0;
            Scan();
        }
    }

    private void Scan()
    {
        visibleTargets.Clear();
        foodSources.Clear();
        visibleAnimals.Clear();

        Collider[] targetsInview = Physics.OverlapSphere(
            transform.position,
            viewRadius);

        foreach (Collider coll in targetsInview)
        {
            //Animal Check
            Animal other;
            if(coll.TryGetComponent(out other))
            {
                if(other != animal && !other.isDead)
                {
                    //Mate Check
                    visibleAnimals.Add(other);
                    if(animal.CanMate(other))
                        animal.Remember(MemoryType.Mate, other, 8f);

                    //Food Check (Carnivore)
                    IFoodSource source = other as IFoodSource;
                    if (source != null && source.IsAvailable)
                    {
                        if(animal.CanEat(source))
                        {
                            foodSources.Add(source);
                            animal.Remember(MemoryType.Food, other, 8f);
                        }
                    }
                }
                continue;
            }

            //Tile Check
            if (!coll.TryGetComponent(out Tile t))
                continue;

            //Tile Food Check (Herbivore)
            if (t.tileType == TileType.Ground && t.hasPlant && t.plant.IsAvailable)
            {
                if(animal.CanEat(t.plant))
                {
                    animal.Remember(MemoryType.Food, t.transform.position, 8f);
                    foodSources.Add(t.plant);
                }
            }

            //Tile Water Check
            if(t.tileType == TileType.Water)
            {
                animal.Remember(MemoryType.Water, t.transform.position, 8f);
                visibleTargets.Add(coll.gameObject);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadius);
    }
}
