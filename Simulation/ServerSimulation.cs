using System.Collections.Generic;

namespace NetworkPractice
{
    public class ServerSimulation : ISimulation
    {
        private readonly LocalSimulation _localSimulation;
        private readonly NetworkManager _networkManager;
        private readonly InputBuffer _inputBuffer;

        public ServerSimulation(LocalSimulation localSimulation, NetworkManager networkManager, InputBuffer inputBuffer)
        {
            _localSimulation = localSimulation;
            _networkManager = networkManager;
            _inputBuffer = inputBuffer;
        }

        public WorldState Update(PlayerInput[] localInputs, float deltaTime)
        {
            _networkManager.PollEvents();
            int tick = _localSimulation.GameState.Tick;

            while (_networkManager.IncomingInputs.TryDequeue(out PlayerInput input))
            {
                _inputBuffer.AddInput(tick, input);
            }

            // combine local and remote inputs
            foreach (PlayerInput input in localInputs)
            {
                _inputBuffer.AddInput(tick, input);
            }

            WorldState newState = _localSimulation.Update(_inputBuffer.GetReadyInputs(tick), deltaTime);

            _networkManager.SendWorldState(newState); //sends to everyone but the server's machine.

            return newState; //sends to the server's machine, which is also the host player.
        }
    }
}