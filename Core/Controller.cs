using Microsoft.Xna.Framework.Input;

namespace NetworkPractice
{
    public class Controller
    {
        private string _controlledEntityId;
        public Controller(string controlledEntityId)
        {
            _controlledEntityId = controlledEntityId;
        }

        public PlayerInput Update(KeyboardState currentState, KeyboardState previousState)
        {
            PlayerInput input = new PlayerInput();
            input.EntityId = _controlledEntityId;
            if (currentState.IsKeyDown(Keys.A) || currentState.IsKeyDown(Keys.Left))
                input.MoveLeft = true;
            if (currentState.IsKeyDown(Keys.D) || currentState.IsKeyDown(Keys.Right))
                input.MoveRight = true;
            if (currentState.IsKeyDown(Keys.W) || currentState.IsKeyDown(Keys.Up))
                input.MoveUp = true;
            if (currentState.IsKeyDown(Keys.S) || currentState.IsKeyDown(Keys.Down))
                input.MoveDown = true;
            if (currentState.IsKeyDown(Keys.Space) && previousState.IsKeyUp(Keys.Space))
                input.Jump = true;
            return input;
        }

        public void SetControlledEntity(string entityId)
        {
            _controlledEntityId = entityId;
        }
    }
}