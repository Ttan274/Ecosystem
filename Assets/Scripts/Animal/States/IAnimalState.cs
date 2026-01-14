public interface IAnimalState
{
    void Enter();
    void Exit();
    void Tick();
    string GetStateName();
}