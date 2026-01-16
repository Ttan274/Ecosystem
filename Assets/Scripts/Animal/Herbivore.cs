using UnityEngine;

public class Herbivore : Animal
{
    protected override void Update()
    {
        base.Update();
    }

    public override bool CanEat(IFoodSource source) => source is Plant;

    #region Mate

    public override void Breed()
    {
        childCount++;
        matingTimer = 0;

        if (gender == Gender.Female)
        {
            SpawnManager.Instance.GenerateAnimal(true, transform.position);
        }
    }

    #endregion

    //Þuan iþlevsiz carnivore yok zaten ortada
    public void GotEaten()
    {
        deathBehaviour.SetDeathBehaviour(0f, DeathType.Predator, true);
        Hurt();
    } 
}
