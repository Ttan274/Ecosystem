using UnityEngine;

public class Herbivore : Animal
{
    [SerializeField] private Herbivore mate;
    public bool IsHaveMate => mate != null;
    protected override void Start()
    {
        base.Start();
    }

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
}
