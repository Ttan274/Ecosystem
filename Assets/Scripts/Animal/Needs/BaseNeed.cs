using UnityEngine;

public abstract class BaseNeed
{
    protected Animal Animal;
    public float Value { get; protected set; }
    public float Threshold { get; protected set; }
    public float Priority { get; protected set; }

    protected BaseNeed(Animal animal , float threshold, float priority)
    {
        Animal = animal;
        Threshold = threshold;
        Priority = priority;
    }

    public float UrgencyScore() => (Threshold - Value) * Priority;

    public abstract void Update();
    public abstract bool IsUrgent();
    public abstract void Resolve();
    public abstract void ResolveCompleted();
}
