public class Carnivore : Animal
{
    protected override void Update()
    {
        base.Update();
    }

    public override void Breed()
    {
        childCount++;
        matingTimer = 0;

        if (gender == Gender.Female)
            SpawnManager.Instance.GenerateAnimal(false, transform.position);
    }

    public override bool CanEat(IFoodSource source) => source is Herbivore;
}
