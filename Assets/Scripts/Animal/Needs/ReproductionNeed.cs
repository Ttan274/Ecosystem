using UnityEngine;

public class ReproductionNeed : BaseNeed
{
    public ReproductionNeed(Animal animal)
        : base(animal, threshold : 1f, priority : 0.5f)
    {
        Value = 0f;
    }

    public override bool IsUrgent()
    {
        return Value >= Animal.matingThreshold &&
                Animal.matingTimer >= Animal.matingCooldown;
    }

    public override void Resolve()
    {
        if (Animal.searchIntent.IsActive)
            return;

        Animal.searchIntent.Start(SearchIntentType.Mate, 8f);
        Animal.ChangeState(new MateState(Animal));
    }

    public override void ResolveCompleted()
    {
        //Empty
    }

    public override void Update()
    {
        if (!Animal.isAdult || Animal.hasMate || Animal.isInfected)
        {
            Value = 0f;
            return;
        }

        float hungerScore = Animal.currentHunger / 100f;
        float thirstScore = Animal.currentThirst / 100f;

        Value = (hungerScore + thirstScore) * 0.5f;
    }
}
