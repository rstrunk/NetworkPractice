namespace AudioGameLearning
{
    public class GameStateManager
    {
        public GameState CurrentState { get; private set; } = GameState.MainMenu;

        public void TransitionTo(GameState newState)
        {
            CurrentState = newState;
            OnStateEntered(newState);
        }

        private void OnStateEntered(GameState state)
        {
            switch (state)
            {
                case GameState.MainMenu:
                    System.Console.WriteLine("Entered main menu.");
                    break;
                case GameState.Playing:
                    System.Console.WriteLine("Entered playing state.");
                    break;
                case GameState.GameOver:
                    System.Console.WriteLine("Entered game over state");
                    break;
            }
        }

        public bool Is(GameState state)
        {
            return CurrentState == state;
        }
    }
}