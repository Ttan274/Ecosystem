using System;
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
    public override void FindFood()
    {
        if (!canSearch)
            return;   //No food left for carnivores

        if (food == null && checkTimer >= checkPrey)
        {
            FindClosestFood();

            if(showDebug)
            {
                Debug.Log(animalName + ":" + food.name);
            }

            checkTimer = 0;

            if (food == null)
                return;

            Tile current = Pathfinder.Instance.GetTileAtPosition(transform.position);
            Tile destination = Pathfinder.Instance.GetTileAtPosition(food.transform.position);

            if (current != null && destination != null)
                SetPath(current, destination);
        }
    }

    private void FindClosestFood()
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
            if (d < closestDist)
            {
                closestDist = d;
                closest = animal;
            }
        }

        food = closest.gameObject;
    }

    public override void Eat()
    {
        food.GetComponent<Herbivore>().GotEaten();
        base.Eat();
        food = null;
    }
    #endregion

    #region Mate
    //public override void FindMate()
    //{
    //    Carnivore[] carnivores = FindObjectsByType<Carnivore>(FindObjectsSortMode.None);

    //    foreach (Carnivore other in carnivores)
    //    {
    //        if (other == this || !other.IsReadyToMate || other.gender == this.gender || other.hasMate) continue;

    //        float distance = Vector3.Distance(transform.position, other.transform.position);
    //        if (distance <= matingDistance)
    //        {
    //            hasMate = true;
    //            other.hasMate = true;
    //            Vector3 pos = (this.gender == Gender.Female) ? this.transform.position : other.transform.position;
    //            Breed(other, pos);
    //            break;
    //        }
    //    }
    //}

    public override Animal FindClosestMate()
    {
        return base.FindClosestMate();
    }

    public override void Breed()
    {
        childCount++;
        matingTimer = 0;

        if (gender == Gender.Female)
        {
            SpawnManager.Instance.GenerateAnimal(false, transform.position);
        }
    }
    #endregion
}
