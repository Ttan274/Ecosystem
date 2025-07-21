using UnityEngine;

public class Herbivore : Animal
{
    protected override void Update()
    {
        base.Update();
    }

    protected override void FoodSearch()
    {
        if (food == null)
        {
            FindFood();
            if (food != null)
            {
                Tile current = Pathfinder.Instance.GetTileAtPosition(transform.position);
                Tile destination = Pathfinder.Instance.GetTileAtPosition(food.transform.position);
                SetPath(current, destination);
            }
        }

        if (food != null)
        {
            float distance = Vector3.Distance(transform.position, food.transform.position);
            if (distance < eatDistance)
            {
                Eat();
                return;
            }
            FollowPath();
        }
    }

    protected override void FindFood()
    {
        Plant[] allPlants = FindObjectsByType<Plant>(FindObjectsSortMode.None);
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

    protected override void Eat()
    {
        food.GetComponent<Plant>().Eat();
        base.Eat();
    }

    public void GotEaten() => Destroy(gameObject);

    protected override void FindMate()
    {
        Herbivore[] herbivores = FindObjectsByType<Herbivore>(FindObjectsSortMode.None);

        foreach (Herbivore other in herbivores)
        {
            if (other == this || !other.IsReadyToMate || other.gender == this.gender) continue;

            float distance = Vector3.Distance(transform.position, other.transform.position);
            if(distance <= matingDistance)
            {
                Vector3 pos = (this.gender == Gender.Female) ? this.transform.position : other.transform.position;
                Breed(other, pos);
                break;
            }
        }
    }

    private void Breed(Herbivore partner, Vector3 pos)
    {
        //Instantiate a child herbivore
        Simulation.Instance.GenerateAnimal(Simulation.Instance.herbivore, pos);
        
        //debugging
        Debug.Log($"{this.name} and {partner.name} have bred!");

        //resetting mating datas;
        matingTimer = 0;
        partner.matingTimer = 0;
    }
}
