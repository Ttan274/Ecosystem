using System;
using System.Collections.Generic;
using UnityEngine;

public class VisionSensor : MonoBehaviour
{
    [Header("Vision Settings")]
    [SerializeField] private float viewRadius;
    [Range(0f, 360f)]
    [SerializeField] private float viewAngle;

    [Header("Scan Settings")]
    [SerializeField] private float scanInterval = 0.4f;
    private float scanTimer = 0;
    public List<GameObject> visibleTargets = new(); //Water
    public List<IFoodSource> foodSources = new();   //Food

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

        Collider[] targetsInview = Physics.OverlapSphere(
            transform.position,
            viewRadius);

        foreach (Collider coll in targetsInview)
        {
            Tile t;
            if (!coll.TryGetComponent(out t))
                continue;

            //Plant eklemek için ???
            if (t.tileType == TileType.Ground && t.hasPlant && t.plant.IsAvailable)
            {
                animal.Remember(MemoryType.Food, t.transform.position, 8f);
                foodSources.Add(t.plant);
            }

            if (t.tileType != TileType.Water)
                continue;

            animal.Remember(MemoryType.Water, t.transform.position, 8f);
            visibleTargets.Add(coll.gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadius);
    }
}
