namespace NetworkPractice
{
    public interface ISimulation
    {
        WorldState? Update(PlayerInput[] playerInputs, float deltaTime);
    }
}