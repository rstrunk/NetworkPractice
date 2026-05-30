namespace NetworkPractice
{
    public interface ISimulation
    {
        WorldState? Update(ControllerInput[] ControllerInputs, float deltaTime);
    }
}