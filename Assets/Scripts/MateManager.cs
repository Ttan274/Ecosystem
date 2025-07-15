using System.Collections.Generic;
using UnityEngine;

public class MateManager : MonoBehaviour
{
    [SerializeField] private List<Herbivore> herbivores = new List<Herbivore>();

    //Instance
    public static MateManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        Instance = this;
    }

    #region Registeration
    public void Register(Herbivore h)
    {
        if(!herbivores.Contains(h))
            herbivores.Add(h);
    }

    public void Unregister(Herbivore h)
    {
        if (!herbivores.Contains(h))
            return;

        herbivores.Remove(h);
    }
    #endregion
    
    public Herbivore SeekMate(Herbivore seeker)
    {
        Herbivore closest = null;
        float bestDist = Mathf.Infinity;

        foreach (Herbivore other in herbivores)
        {
            if (other == seeker || other.gender == Gender.Male) continue;
            float d = Vector3.Distance(seeker.transform.position, other.transform.position);
            if (d < bestDist)
            {
                bestDist = d;
                closest = other;
            }
        }

        return closest;
    }

}