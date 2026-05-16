using System.Numerics;
using System.Collections.Generic;

namespace NetworkPractice
{
    public class LocalSimulation : ISimulation
    {
        public WorldState GameState;
        private WorldGrid _grid;
        private Dictionary<string, BodyDefinition> _bodyDefinitions;

        private const float MoveSpeed = 150f;
        private const float JumpForce = -400f;
        private const float Gravity = 400f;

        public LocalSimulation(WorldState gameState, WorldGrid grid, Dictionary<string, BodyDefinition> bodyDefinitions)
        {
            GameState = gameState;
            _grid = grid;
            _bodyDefinitions = bodyDefinitions;
        }

        public WorldState Update(PlayerInput[] playerInputs, float deltaTime)
        {
            for (int i = 0; i < playerInputs.Length; i++)
            {
                for (int j = 0; j < GameState.PlayerStates.Length; j++)
                {
                    ApplyInput(ref GameState.PlayerStates[j], playerInputs[i]);
                }
            }
            ApplyGravity(deltaTime);
            ResolveCollisions(deltaTime);
            IntegratePositions(deltaTime);
            GameState.Tick++;
            return GameState;
        }

        private void ApplyInput(ref PlayerState playerState, PlayerInput input)
        {
            if (input.PlayerId == playerState.PlayerId)
            {
                if (input.MoveRight)
                    playerState.Velocity.X = MoveSpeed;
                else if (input.MoveLeft)
                    playerState.Velocity.X = -MoveSpeed;
                else
                    playerState.Velocity.X = 0f;
                if (input.Jump && playerState.Grounded)
                {
                    playerState.Velocity.Y = JumpForce;
                    playerState.Grounded = false;
                }
            }
        }

        private void ApplyGravity(float deltaTime)
        {
            for (int i = 0; i < GameState.PlayerStates.Length; i++)
            {
                GameState.PlayerStates[i].Velocity.Y += (Gravity * deltaTime);
            }
            for (int i = 0; i < GameState.PhysicsBodies.Length; i++)
            {
                if (!GameState.PhysicsBodies[i].IsStatic)
                    GameState.PhysicsBodies[i].Velocity.Y += (Gravity * deltaTime);
            }
        }

        private void ResolveCollisions(float deltaTime)
        {
            for (int i = 0; i < GameState.PlayerStates.Length; i++)
            {
                GameState.PlayerStates[i].GroundVelocity = Vector2.Zero;
            }
            for (int i = 0; i < GameState.PhysicsBodies.Length; i++)
            {
                ResolvePhysicsBodyTileCollisions(ref GameState.PhysicsBodies[i], deltaTime);
            }
            for (int i = 0; i < GameState.PlayerStates.Length; i++)
            {
                for (int j = 0; j < GameState.PhysicsBodies.Length; j++)
                {
                    ResolvePlayerPhysicsBodyCollisions(ref GameState.PlayerStates[i], ref GameState.PhysicsBodies[j]);
                }
                ResolvePlayerTileCollisions(ref GameState.PlayerStates[i], deltaTime);
            }
                    }

        private void ResolvePlayerTileCollisions(ref PlayerState playerState, float deltaTime)
        {
            Vector2 intended = playerState.Position + (playerState.Velocity * deltaTime);

            //Check each axis separately
            Vector2 horizontalIntended = new Vector2(intended.X, playerState.Position.Y);
            if (!_grid.IsPassable(horizontalIntended) || !_grid.InBounds(horizontalIntended)) //horizontal check
                playerState.Velocity.X = 0f; //Don't move horizontally if there's a collision

            Vector2 verticalIntended = new Vector2(playerState.Position.X, intended.Y);
            if (!_grid.IsPassable(verticalIntended) || !_grid.InBounds(verticalIntended)) //Vertical Check        
            {
                if (playerState.Velocity.Y > 0) //Collided while falling, so we're grounded
                {
                    playerState.Grounded = true;
                }
                playerState.Velocity.Y = 0f;
            }
            else
                playerState.Grounded = false;
        }

        public void ResolvePlayerPhysicsBodyCollisions(ref PlayerState playerState, ref PhysicsBodyState bodyState)
        {
            bool overlaps = OverlapsBody(playerState.Position, bodyState);

            //vertical collision checks
            if (overlaps && playerState.Velocity.Y > 0)
            {
                playerState.Position.Y = bodyState.Position.Y - 1;
                playerState.Grounded = true;
                playerState.GroundVelocity.X = bodyState.Velocity.X; //Move with the platform if it's moving horizontally
                playerState.Velocity.Y = 0f;
            }
            if (overlaps && playerState.Velocity.Y < 0)
                playerState.Velocity.Y = 0f;

            //Horizontal collision checks
            if (overlaps && playerState.Velocity.X > 0)
            {
                playerState.Position.X = bodyState.Position.X - 1;
                if (!bodyState.IsStatic && _bodyDefinitions[bodyState.Type].IsPushable)
                    bodyState.Velocity.X = playerState.Velocity.X;
            }
            if (overlaps && playerState.Velocity.X < 0)
            {
                playerState.Position.X = bodyState.Position.X + 1;
                if (!bodyState.IsStatic && _bodyDefinitions[bodyState.Type].IsPushable)
                    bodyState.Velocity.X = playerState.Velocity.X;
            }
        }

        private void ResolvePhysicsBodyTileCollisions(ref PhysicsBodyState bodyState, float deltaTime)
        {
            if (bodyState.IsStatic)
                return;

            //Initialize the variables we need
            Vector2 horizontalIntended = bodyState.Position;
            Vector2 verticalIntended = bodyState.Position;

            //Bodies can have multi-tile dimensions, so we need to check the edges rather than a single point
            //Check each axis separately            
            if (bodyState.Velocity.X != 0)
            {
                if (bodyState.Velocity.X > 0) //Moving right, so set to the right edge + velocity
                    horizontalIntended.X = (bodyState.Position.X + (_bodyDefinitions[bodyState.Type].WidthInTiles * _grid.TileSize)) + (bodyState.Velocity.X * deltaTime);
                if (bodyState.Velocity.X < 0) //Moving left, so set to the left edge + velocity
                    horizontalIntended.X = bodyState.Position.X + (bodyState.Velocity.X * deltaTime);
                if (!_grid.IsPassable(horizontalIntended) || !_grid.InBounds(horizontalIntended))
                    bodyState.Velocity.X = 0f;
            }
            if (bodyState.Velocity.Y != 0)
            {
                if (bodyState.Velocity.Y > 0) //Moving down, so set to the bottom edge + velocity
                    verticalIntended.Y = (bodyState.Position.Y + (_bodyDefinitions[bodyState.Type].HeightInTiles * _grid.TileSize)) + (bodyState.Velocity.Y * deltaTime);
                if (bodyState.Velocity.Y < 0) //Moving up, so set to the top edge + velocity
                    verticalIntended.Y = bodyState.Position.Y + (bodyState.Velocity.Y * deltaTime);
                if (!_grid.IsPassable(verticalIntended) || !_grid.InBounds(verticalIntended))
                    bodyState.Velocity.Y = 0f;
            }
        }

        private bool OverlapsBody(Vector2 position, PhysicsBodyState bodyState)
        {
            int bodyRightEdge = (int)(bodyState.Position.X + (_bodyDefinitions[bodyState.Type].WidthInTiles * _grid.TileSize));
            int bodyBottomEdge = (int)(bodyState.Position.Y + (_bodyDefinitions[bodyState.Type].HeightInTiles * _grid.TileSize));

            return position.X >= bodyState.Position.X &&
           position.X <= bodyRightEdge &&
           position.Y >= bodyState.Position.Y &&
           position.Y <= bodyBottomEdge;
        }
    
    private void IntegratePositions(float deltaTime)
        {
            for (int i = 0; i < GameState.PlayerStates.Length; i++)
            {
                GameState.PlayerStates[i].Position += (GameState.PlayerStates[i].Velocity + GameState.PlayerStates[i].GroundVelocity) * deltaTime;
            }
            for (int i = 0; i < GameState.PhysicsBodies.Length; i++)
            {
                if (!GameState.PhysicsBodies[i].IsStatic)
                    GameState.PhysicsBodies[i].Position += GameState.PhysicsBodies[i].Velocity * deltaTime;
            }
        }
    }
}