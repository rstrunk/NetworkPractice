using System;
using System.Collections.Generic;

namespace NetworkPractice
{
    public class ClientSimulation : ISimulation
    {
        private readonly NetworkManager _networkManager;
        private LocalSimulation? _localSimulation;
        private WorldState? _latestState;
        private readonly WorldGrid _grid;
private readonly Dictionary<string, EntityDefinition> _definitions;
        private string? _localEntityId;
        private readonly List<(int Tick, ControllerInput Input)> _inputHistory = new();
        private const float ReconciliationThreshold = 0.01f;
        private const int MaxHistorySize = 60;
        
        public ClientSimulation(NetworkManager networkManager, WorldGrid grid, Dictionary<string, EntityDefinition> definitions)
        {
    _networkManager = networkManager;
    _grid = grid;
    _definitions = definitions;
    _networkManager.WorldStateReceived += OnServerStateReceived;
    _networkManager.PlayerJoined += OnPlayerJoined;
}

        public WorldState? Update(ControllerInput[] localInputs, float deltaTime)
        {
            _networkManager.PollEvents();

            if (_localSimulation == null || _localEntityId == null)
                return null;

            if (localInputs.Length > 0)
            {
                ControllerInput input = new ControllerInput
                {
                    EntityId = _localEntityId,
                    MoveLeft = localInputs[0].MoveLeft,
                    MoveRight = localInputs[0].MoveRight,
                    Jump = localInputs[0].Jump
                };

                _inputHistory.Add((_localSimulation.GameState.Tick, input));

                if (_inputHistory.Count > MaxHistorySize)
                    _inputHistory.RemoveAt(0);

                _networkManager.SendControllerInput(input);
            }

            return _localSimulation.Update(localInputs, deltaTime);
        }

        private void OnPlayerJoined(string entityId, WorldState state)
{
    _localEntityId = entityId;
    _latestState = state.Clone();
    _localSimulation = new LocalSimulation(state.Clone(), _grid, _definitions);
}

        private void OnServerStateReceived(WorldState serverState)
        {
            if (_latestState == null || serverState.Tick > _latestState.Tick)
                _latestState = serverState.Clone();

            if (_localSimulation == null)
                return;

            // Find local entity in server state
            EntityState? serverEntity = null;
            foreach (EntityState entity in serverState.Entities)
            {
                if (entity.Id == _localEntityId)
                {
                    serverEntity = entity;
                    break;
                }
            }

            if (serverEntity == null)
                return;

            // Find predicted entity in local simulation
            EntityState? predictedEntity = null;
            foreach (EntityState entity in _localSimulation.GameState.Entities)
            {
                if (entity.Id == _localEntityId)
                {
                    predictedEntity = entity;
                    break;
                }
            }

            if (predictedEntity == null)
                return;

            // Check if reconciliation is needed
            float positionError = System.Numerics.Vector2.Distance(serverEntity.Position, predictedEntity.Position);
            if (positionError <= ReconciliationThreshold)
                return;

            // Rewind to server state and replay inputs
            _localSimulation.GameState = serverState.Clone();

            List<(int Tick, ControllerInput Input)> inputsToReplay = _inputHistory.FindAll(
                entry => entry.Tick > serverState.Tick
            );

            foreach (var (tick, input) in inputsToReplay)
            {
                _localSimulation.Update(new ControllerInput[] { input }, 1f / 60f);
            }
        }
    }
}