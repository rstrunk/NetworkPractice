namespace NetworkPractice
{
    public class ClientSimulation : ISimulation
    {
        private readonly NetworkManager _networkManager;
        private WorldState _latestState;

        public ClientSimulation(NetworkManager networkManager)
        {
            _networkManager = networkManager;
        }

        public WorldState Update(PlayerInput[] localInputs, float deltaTime)
        {
            _networkManager.PollEvents();

            while (_networkManager.IncomingStates.TryDequeue(out WorldState state))
            {
                if (state.Tick > _latestState.Tick)
                    _latestState = state;
            }

            if (localInputs.Length > 0)
                _networkManager.SendPlayerInput(localInputs[0]);

            return _latestState;
        }
    }
}