using UnityEngine;

public class ThirstNeed : BaseNeed
{
    private float decay;

    public ThirstNeed(Animal animal, float decay, float threshold) 
        : base(animal, threshold, priority : 1.2f)
    {
        this.decay = decay;
        Value = 100f;
    }

    public override bool IsUrgent() => Value <= Threshold;

    public override void Resolve() => Animal.ChangeState(new SeekWaterState(Animal));

    public override void ResolveCompleted() => Value = 100f;

    public override void Update()
    {
        if(!Animal.isDead)
        {
            Value -= decay * Time.deltaTime;
            Value = Mathf.Clamp(Value, 0, 100f);

            Animal.SetThirst(Value);
        }
    }
}
