using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace NetworkPractice;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch? _spriteBatch;
    private NetworkManager? _networkManager;
    private ISimulation? _simulation;
    
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        WorldGrid grid = new WorldGrid(10, 1, 64);
        grid.SetTile(0, 0, TileType.Floor);
grid.SetTile(1, 0, TileType.Floor);
        grid.SetTile(2, 0, TileType.Floor);
        WorldState initialState = new WorldState
        {
            Tick = 0,
            PlayerStates = new PlayerState[]
            {
        new PlayerState
        {
            PlayerId = 1,
            Position = new System.Numerics.Vector2(0, 0),
            Velocity = System.Numerics.Vector2.Zero,
            FacingDirection = 1,
            Grounded = false,
            GroundVelocity = System.Numerics.Vector2.Zero
        }
            },
            PhysicsBodies = new PhysicsBodyState[0]
        };

        Console.WriteLine("Network practice- Press s for single player, h to host, or j to join.\n");
        string? choice = Console.ReadLine();
        switch (choice)
        {
            case "h":
                {
                    _networkManager = new NetworkManager();
                    _networkManager.StartServer(12345);
                    LocalSimulation local = new LocalSimulation(initialState, grid, new Dictionary<string, BodyDefinition>());
                    _simulation = new ServerSimulation(local, _networkManager, new InputBuffer());
                    break;
                }
            case "j":
                {
                    _networkManager = new NetworkManager();
                    _networkManager.StartClient("127.0.0.1", 12345);
                    _simulation = new ClientSimulation(_networkManager);
                    break;
                }
            case "s":
                {
                    _simulation = new LocalSimulation(initialState, grid, new Dictionary<string, BodyDefinition>());
                    break;
                }
            default:
                {
                    Exit();
                    break;
                }
        }

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        if (_simulation != null)
{
    float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
    PlayerInput localInput = new PlayerInput
    {
        PlayerId = 1,
        MoveLeft = Keyboard.GetState().IsKeyDown(Keys.Left),
        MoveRight = Keyboard.GetState().IsKeyDown(Keys.Right),
        Jump = Keyboard.GetState().IsKeyDown(Keys.Space)
    };
    WorldState state = _simulation.Update(new PlayerInput[] { localInput }, deltaTime);
        if (state.PlayerStates != null && state.PlayerStates.Length > 0)
        Console.WriteLine($"Tick: {state.Tick} Position: {state.PlayerStates[0].Position}");
base.Update(gameTime);
    }
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // TODO: Add your drawing code here

        base.Draw(gameTime);
    }
}
