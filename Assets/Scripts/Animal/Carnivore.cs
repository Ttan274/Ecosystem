using System;
using System.Linq;
using UnityEngine;

public class Carnivore : Animal
{
    protected override void Update()
    {
        base.Update();
    }

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
