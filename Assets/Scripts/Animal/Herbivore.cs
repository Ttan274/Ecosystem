using UnityEngine;

public class Herbivore : Animal
{
    protected override void Update()
    {
        base.Update();
    }

    public override bool CanEat(IFoodSource source) => source is Plant;

    #region Mate

    public override Animal FindClosestMate()
    {
        Herbivore[] herbivores = FindObjectsByType<Herbivore>(FindObjectsSortMode.None);

        float closest = Mathf.Infinity;
        Herbivore result = null;

        foreach (Herbivore other in herbivores)
        {
            if (other == this || other.gender == this.gender || other.hasMate) continue;

            float distance = Vector3.Distance(transform.position, other.transform.position);
            if(distance <= closest)
            {
                closest = distance;
                result = other;
            }
        }

        return result;
    }

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

    //Helper method when eaten by carnivores
    public void GotEaten()
    {
        deathBehaviour.SetDeathBehaviour(0f, DeathType.Predator, true);
        Hurt();
    } 
}
