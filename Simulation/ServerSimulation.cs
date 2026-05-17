using System.Collections.Generic;

namespace NetworkPractice
{
    public class ServerSimulation : ISimulation
    {
        private readonly LocalSimulation _localSimulation;
        private readonly NetworkManager _networkManager;
        private readonly List<PlayerInput> _inputBuffer;

        public ServerSimulation(LocalSimulation localSimulation, NetworkManager networkManager)
        {
            _localSimulation = localSimulation;
            _networkManager = networkManager;
            _inputBuffer = new List<PlayerInput>();
        }

        public WorldState Update(PlayerInput[] localInputs, float deltaTime)
        {
            _networkManager.PollEvents();

            while (_networkManager.IncomingInputs.TryDequeue(out PlayerInput input))
            {
                _inputBuffer.Add(input);
            }

            // combine local and remote inputs
            foreach (PlayerInput input in localInputs)
            {
                _inputBuffer.Add(input);
            }

            WorldState newState = _localSimulation.Update(_inputBuffer.ToArray(), deltaTime);
            _inputBuffer.Clear();

            _networkManager.SendWorldState(newState); //sends to everyone but the server's machine.

            return newState; //sends to the server's machine, which is also the host player.
        }
    }
}