using System;
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
            _networkManager.ControllerInputReceived += input => _inputBuffer.AddInput(_localSimulation.GameState.Tick, input);
            _networkManager.OnPlayerConnected = (peer, assignedId) => SpawnPlayer(peer, assignedId);
        }

        private void SpawnPlayer(LiteNetLib.NetPeer peer, int assignedId)
        {
            string entityId = $"player_{assignedId}";

            EntityState newPlayer = new EntityState
            {
                Id = entityId,
                Type = "player",
                Position = new System.Numerics.Vector2(64, 0), // placeholder spawn
                Velocity = System.Numerics.Vector2.Zero,
                FacingDirection = 1,
                Grounded = false,
                GroundVelocity = System.Numerics.Vector2.Zero,
                CurrentHealth = 100,
                CurrentMaxHealth = 100,
                IsStatic = false
            };

            // Add new player to world state
            EntityState[] current = _localSimulation.GameState.Entities;
            EntityState[] updated = new EntityState[current.Length + 1];
            Array.Copy(current, updated, current.Length);
            updated[current.Length] = newPlayer;
            _localSimulation.GameState.Entities = updated;

            // Send PlayerJoined packet to the connecting client
            _networkManager.SendPlayerJoined(peer, entityId, _localSimulation.GameState);
        }

        public WorldState? Update(ControllerInput[] localInputs, float deltaTime)
        {
            _networkManager.PollEvents();
            int tick = _localSimulation.GameState.Tick;

            foreach (ControllerInput input in localInputs)
            {
                _inputBuffer.AddInput(tick, input);
            }

            WorldState? newState = _localSimulation.Update(_inputBuffer.GetReadyInputs(tick), deltaTime);
            if (newState != null)
                _networkManager.SendWorldState(newState);
            return newState;
        }
    }
}