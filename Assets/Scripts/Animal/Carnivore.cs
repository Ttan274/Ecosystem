using UnityEngine;

public class Carnivore : Animal
{
    [Header("Carnivore Details")]
    [SerializeField] private float checkPrey;
    private float checkTimer = 0;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        checkTimer += Time.deltaTime;
    }

    protected override void FoodSearch()
    {
        if(food == null || checkTimer >= checkPrey)
        {
            FindFood();
            checkTimer = 0;
        }

        if(food != null)
        {
            Tile current = Pathfinder.Instance.GetTileAtPosition(transform.position);
            Tile destination = Pathfinder.Instance.GetTileAtPosition(food.transform.position);
            SetPath(current, destination);

            float distance = Vector3.Distance(transform.position, food.transform.position);
            if(distance < eatDistance)
            {
                Eat();
                return;
            }

            FollowPath();
        }
    }

    protected override void FindFood()
    {
        Herbivore[] herbivores = FindObjectsOfType<Herbivore>();
        float closestDist = Mathf.Infinity;
        Herbivore closest = null;

        foreach (Herbivore animal in herbivores)
        {
            float d = Vector3.Distance(transform.position, animal.transform.position);
            if(d < closestDist)
            {
                closestDist = d;
                closest = animal;
            }
        }

        food = closest.gameObject;
    }

    protected override void Eat()
    {
        food.GetComponent<Herbivore>().GotEaten();
        base.Eat();
    }
}
