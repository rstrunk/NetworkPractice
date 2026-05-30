using System.Numerics;
using System.Collections.Generic;

namespace NetworkPractice
{
    public class LocalSimulation : ISimulation
    {
        public WorldState GameState;
        private WorldGrid _grid;
        private Dictionary<string, EntityDefinition> _definitions;

        private const float Gravity = 400f;

        public LocalSimulation(WorldState gameState, WorldGrid grid, Dictionary<string, EntityDefinition> definitions)
        {
            GameState = gameState;
            _grid = grid;
            _definitions = definitions;
        }

        public WorldState? Update(ControllerInput[] ControllerInputs, float deltaTime)
        {
            Dictionary<string, ControllerInput> inputMap = new();
            foreach (ControllerInput input in ControllerInputs)
            {
                if (input.EntityId != null)
                    inputMap[input.EntityId] = input;
            }

            foreach (EntityState entity in GameState.Entities)
            {
                if (inputMap.TryGetValue(entity.Id ?? "", out ControllerInput input))
                    ApplyInput(entity, input);
            }

            ApplyGravity(deltaTime);
            ResolveCollisions(deltaTime);
            IntegratePositions(deltaTime);
            GameState.Tick++;
            return GameState;
        }

        private void ApplyInput(EntityState entity, ControllerInput input)
        {
            if (_definitions.TryGetValue(entity.Type ?? "", out EntityDefinition? definition) &&
                definition is ControllableDefinition controllable)
            {
                if (input.MoveRight)
                    entity.Velocity.X = controllable.MoveSpeed;
                else if (input.MoveLeft)
                    entity.Velocity.X = -controllable.MoveSpeed;
                else
                    entity.Velocity.X = 0f;

                if (input.Jump && entity.Grounded)
                {
                    entity.Velocity.Y = controllable.JumpForce;
                    entity.Grounded = false;
                }
            }
        }

        private void ApplyGravity(float deltaTime)
        {
            foreach (EntityState entity in GameState.Entities)
            {
                if (!entity.IsStatic)
                    entity.Velocity.Y += Gravity * deltaTime;
            }
        }

        private void ResolveCollisions(float deltaTime)
        {
            // Reset ground velocity for all entities
            foreach (EntityState entity in GameState.Entities)
                entity.GroundVelocity = Vector2.Zero;

            // Resolve non-static entities against tiles
            foreach (EntityState entity in GameState.Entities)
            {
                if (!entity.IsStatic)
                    ResolveEntityTileCollisions(entity, deltaTime);
            }

            // Resolve non-static entities against each other
            for (int i = 0; i < GameState.Entities.Length; i++)
            {
                for (int j = i + 1; j < GameState.Entities.Length; j++)
                {
                    if (!GameState.Entities[i].IsStatic || !GameState.Entities[j].IsStatic)
                        ResolveEntityCollision(GameState.Entities[i], GameState.Entities[j]);
                }
            }
        }

        private void ResolveEntityTileCollisions(EntityState entity, float deltaTime)
        {
            if (!_definitions.TryGetValue(entity.Type ?? "", out EntityDefinition? definition))
                return;

            Vector2 intendedPosition = entity.Position + (entity.Velocity * deltaTime);
            // Horizontal check
            Vector2 horizontalIntended = new Vector2(intendedPosition.X, entity.Position.Y);
            var (hTopLeft, hTopRight, hBottomLeft, hBottomRight) = GetBounds(horizontalIntended, definition);

            if (entity.Velocity.X > 0)
            {
                if (!_grid.IsPassable(hTopRight) || !_grid.IsPassable(hBottomRight))
                    entity.Velocity.X = 0f;
            }
            else if (entity.Velocity.X < 0)
            {
                if (!_grid.IsPassable(hTopLeft) || !_grid.IsPassable(hBottomLeft))
                    entity.Velocity.X = 0f;
            }

            // Vertical check
            Vector2 verticalIntended = new Vector2(entity.Position.X, intendedPosition.Y);
            var (vTopLeft, vTopRight, vBottomLeft, vBottomRight) = GetBounds(verticalIntended, definition);

            if (entity.Velocity.Y > 0)
            {
                if (!_grid.IsPassable(vBottomLeft) || !_grid.IsPassable(vBottomRight))
                {
                    entity.Grounded = true;
                    entity.Velocity.Y = 0f;
                }
            }
            else if (entity.Velocity.Y < 0)
            {
                if (!_grid.IsPassable(vTopLeft) || !_grid.IsPassable(vTopRight))
                    entity.Velocity.Y = 0f;
            }
            else
                entity.Grounded = false;
        }

        private void ResolveEntityCollision(EntityState a, EntityState b)
        {
            if (!_definitions.TryGetValue(a.Type ?? "", out EntityDefinition? definitionA))
                return;
            if (!_definitions.TryGetValue(b.Type ?? "", out EntityDefinition? definitionB))
                return;

            // Skip if either entity is passable
            if (definitionA.IsPassable || definitionB.IsPassable)
                return;

            if (!OverlapsEntity(a, b, definitionA, definitionB))
                return;

            var (aTopLeft, aTopRight, aBottomLeft, aBottomRight) = GetBounds(a.Position, definitionA);
            var (bTopLeft, bTopRight, bBottomLeft, bBottomRight) = GetBounds(b.Position, definitionB);

            // Vertical resolution
            if (a.Velocity.Y > 0 && definitionB is PhysicsBodyDefinition)
            {
                a.Position.Y = bTopLeft.Y - (definitionA.HeightInTiles * _grid.TileSize);
                a.Grounded = true;
                a.GroundVelocity.X = b.Velocity.X;
                a.Velocity.Y = 0f;
            }
            else if (a.Velocity.Y < 0)
            {
                a.Position.Y = bBottomLeft.Y;
                a.Velocity.Y = 0f;
            }

            // Horizontal resolution
            if (a.Velocity.X > 0)
            {
                a.Position.X = bTopLeft.X - (definitionA.WidthInTiles * _grid.TileSize);
                if (!b.IsStatic && definitionB is PhysicsBodyDefinition physicsB && physicsB.IsPushable)
                    b.Velocity.X = a.Velocity.X;
            }
            else if (a.Velocity.X < 0)
            {
                a.Position.X = bTopRight.X;
                if (!b.IsStatic && definitionB is PhysicsBodyDefinition physicsB && physicsB.IsPushable)
                    b.Velocity.X = a.Velocity.X;
            }
        }

        private bool OverlapsEntity(EntityState a, EntityState b, EntityDefinition definitionA, EntityDefinition definitionB)
        {
            var (aTopLeft, aTopRight, aBottomLeft, aBottomRight) = GetBounds(a.Position, definitionA);
            var (bTopLeft, bTopRight, bBottomLeft, bBottomRight) = GetBounds(b.Position, definitionB);

            return aTopLeft.X < bTopRight.X &&
                   aTopRight.X > bTopLeft.X &&
                   aTopLeft.Y < bBottomLeft.Y &&
                   aBottomLeft.Y > bTopLeft.Y;
        }

        private (Vector2 topLeft, Vector2 topRight, Vector2 bottomLeft, Vector2 bottomRight) GetBounds(Vector2 position, EntityDefinition definition)
        {
            float width = definition.WidthInTiles * _grid.TileSize;
            float height = definition.HeightInTiles * _grid.TileSize;

            return (
                position,
                new Vector2(position.X + width, position.Y),
                new Vector2(position.X, position.Y + height),
                new Vector2(position.X + width, position.Y + height)
            );
        }

        private void IntegratePositions(float deltaTime)
        {
            foreach (EntityState entity in GameState.Entities)
            {
                if (!entity.IsStatic)
                    entity.Position += (entity.Velocity + entity.GroundVelocity) * deltaTime;
            }
        }
    }
}