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
    private Transform owner;
    public List<GameObject> visibleTargets = new();

    private void Awake()
    {
        owner = transform;
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

        Collider[] targetsInview = Physics.OverlapSphere(
            owner.position,
            viewRadius);

        foreach (Collider coll in targetsInview)
        {
            Tile t;
            if (!coll.TryGetComponent(out t))
                continue;
         
            if (t.tileType != TileType.Water)
                continue;

            visibleTargets.Add(coll.gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadius);
    }
}
