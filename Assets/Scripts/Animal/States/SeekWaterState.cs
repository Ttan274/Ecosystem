using UnityEngine;

public class SeekWaterState : IAnimalState
{
    private Animal animal;

    public SeekWaterState(Animal animal)
    {
        this.animal = animal;
    }

    public void Enter()
    {
       
    }

    public void Exit()
    {
        
    }

    public string GetStateName() => "Seek-Water";

    public void Tick()
    {
        animal.WaterSearch();

        if(animal.currentThirst >= 100f)
        {
            animal.ChangeState(new WanderState(animal));
        }
    }
}
