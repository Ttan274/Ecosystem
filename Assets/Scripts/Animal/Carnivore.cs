using System.Linq;
using UnityEngine;

public class Carnivore : Animal
{
    [Header("Carnivore Details")]
    [SerializeField] private float checkPrey;
    private float checkTimer = 0;

    protected override void Update()
    {
        base.Update();
        checkTimer += Time.deltaTime;
    }

    #region Food
    protected override void FoodSearch()
    {
        if (!canSearch)
        {
            //No food left for carnivores
            return;
        }

        if (food == null || checkTimer >= checkPrey)
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
        Herbivore[] herbivores = FindObjectsByType<Herbivore>(FindObjectsSortMode.None).Where(x => x.gameObject.activeInHierarchy).ToArray();

        if (herbivores.Length <= 0)
        {
            canSearch = false;
            return;
        }

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
        food = null;
    }
    #endregion

    #region Mate
    protected override void FindMate()
    {
        Carnivore[] carnivores = FindObjectsByType<Carnivore>(FindObjectsSortMode.None);

        foreach (Carnivore other in carnivores)
        {
            if (other == this || !other.IsReadyToMate || other.gender == this.gender || other.hasMate) continue;

            float distance = Vector3.Distance(transform.position, other.transform.position);
            if (distance <= matingDistance)
            {
                hasMate = true;
                other.hasMate = true;
                Vector3 pos = (this.gender == Gender.Female) ? this.transform.position : other.transform.position;
                Breed(other, pos);
                break;
            }
        }
    }

    private void Breed(Carnivore partner, Vector3 pos)
    {
        //Instantiate a child herbivore
        SpawnManager.Instance.GenerateAnimal(false, pos);
        childCount++;
        partner.childCount++;

        //resetting mating datas;
        matingTimer = 0;
        partner.matingTimer = 0;
        hasMate = false;
        partner.hasMate = false;
    }
    #endregion
}
