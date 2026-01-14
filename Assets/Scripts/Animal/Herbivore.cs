using UnityEngine;

public class Herbivore : Animal
{
    protected override void Update()
    {
        base.Update();
    }

    #region Food
    public override void FindFood()
    {
        if (!canSearch)
            return;

        FindClosestFood();

        if (food == null)
            return;

        Tile current = Pathfinder.Instance.GetTileAtPosition(transform.position);
        Tile destination = Pathfinder.Instance.GetTileAtPosition(food.transform.position);

        if (current != null && destination != null)
            SetPath(current, destination);
    }

    private void FindClosestFood()
    {
        Plant[] allPlants = FindObjectsByType<Plant>(FindObjectsSortMode.None);

        if (allPlants.Length <= 0)
        {
            canSearch = false;
            return; //No plants left
        }

        float closestDist = Mathf.Infinity;
        Plant closest = null;

        foreach (Plant plant in allPlants)
        {
            float d = Vector3.Distance(transform.position, plant.transform.position);
            if (d < closestDist)
            {
                closestDist = d;
                closest = plant;
            }
        }

        food = closest.gameObject;
    }

    public override void Eat()
    {
        food.GetComponent<Plant>().Eat();
        base.Eat();
    }
    #endregion
   
    #region Mate

    public override Animal FindClosestMate()
    {
        Herbivore[] herbivores = FindObjectsByType<Herbivore>(FindObjectsSortMode.None);

        float closest = Mathf.Infinity;
        Herbivore result = null;

        foreach (Herbivore other in herbivores)
        {
            if (other == this || !other.IsReadyToMate || other.gender == this.gender || other.hasMate) continue;

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
