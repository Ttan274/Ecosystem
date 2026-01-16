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
