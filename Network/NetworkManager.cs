using LiteNetLib;
using LiteNetLib.Utils;
using System.Net;
using System.Net.Sockets;
using System.Collections.Concurrent;

namespace NetworkPractice
{
    public class NetworkManager : INetEventListener
    {
        private NetManager _netManager;
        private readonly NetDataWriter _writer = new NetDataWriter();
        private NetPeer? _serverPeer;

        public bool IsServer { get; private set; }
        public ConcurrentQueue<PlayerInput> IncomingInputs { get; } = new();
        private ConcurrentQueue<PlayerInput> _outgoingInputs = new();
        public ConcurrentQueue<WorldState> IncomingStates { get; } = new();
        private ConcurrentQueue<WorldState> _outgoingStates = new();

        public NetworkManager()
        {
            _netManager = new NetManager(this);
        }

        public void StartServer(int port)
        {
            IsServer = true;
            _netManager.Start(port);
        }

        public void StartClient(string hostAddress, int port)
        {
            IsServer = false;
            _netManager.Start();
            _netManager.Connect(hostAddress, port, "NetworkPractice");
        }

        public void PollEvents()
        {
            _netManager.PollEvents();
        }

        public void Stop()
        {
            _netManager.Stop();
        }

        // --- INetEventListener callbacks ---

        public void OnPeerConnected(NetPeer peer)
        {
            if (IsServer)
            {
                // a client just connected
            }
            else
            {
                _serverPeer = peer; // client stores its connection to the server
            }
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            if (!IsServer)
                _serverPeer = null;
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
        {
            switch (reader.GetByte())
            {
                case (byte)PacketTypes.PlayerInput:
                    HandlePlayerEvents(reader);
                    break;
                case (byte)PacketTypes.WorldState:
                    HandleWorldStateEvents(reader);
                    break;
            }
            reader.Recycle();
        }

        private void HandlePlayerEvents(NetPacketReader reader)
        {
            int playerId = reader.GetInt();
            bool moveLeft = reader.GetBool();
            bool moveRight = reader.GetBool();
            bool jump = reader.GetBool();
            IncomingInputs.Enqueue(new PlayerInput
            {
                PlayerId = playerId,
                MoveLeft = moveLeft,
                MoveRight = moveRight,
                Jump = jump
            });
        }

        public void SendPlayerInput(PlayerInput input)
        {
            _writer.Reset();
            _writer.Put((byte)PacketTypes.PlayerInput);
            _writer.Put(input.PlayerId);
            _writer.Put(input.MoveLeft);
            _writer.Put(input.MoveRight);
            _writer.Put(input.Jump);
            _serverPeer?.Send(_writer, DeliveryMethod.Unreliable);
        }

        private void HandleWorldStateEvents(NetPacketReader reader)
        {
            int tick = reader.GetInt();
            int playerCount = reader.GetInt();
            PlayerState[] playerStates = new PlayerState[playerCount];
            for(int i = 0; i < playerCount; i++)
            {
                playerStates[i].PlayerId = reader.GetInt();
                playerStates[i].Position.X = reader.GetFloat();
                playerStates[i].Position.Y = reader.GetFloat();
                playerStates[i].Velocity.X = reader.GetFloat();
                playerStates[i].Velocity.Y = reader.GetFloat();
                playerStates[i].FacingDirection = reader.GetInt();
                playerStates[i].Grounded = reader.GetBool();
                playerStates[i].GroundVelocity.X = reader.GetFloat();
                playerStates[i].GroundVelocity.Y = reader.GetFloat();
            }
            IncomingStates.Enqueue(new WorldState
            {
                Tick = tick,
                PlayerStates = playerStates
            });
        }

        public void SendWorldState(WorldState state)
        {
            _writer.Reset();
            _writer.Put((byte)PacketTypes.WorldState);
            _writer.Put(state.Tick);
            _writer.Put(state.PlayerStates.Length);
            for (int i = 0; i < state.PlayerStates.Length; i++)
            {
                _writer.Put(state.PlayerStates[i].PlayerId);
                _writer.Put(state.PlayerStates[i].Position.X);
                _writer.Put(state.PlayerStates[i].Position.Y);
                _writer.Put(state.PlayerStates[i].Velocity.X);
                _writer.Put(state.PlayerStates[i].Velocity.Y);
                _writer.Put(state.PlayerStates[i].FacingDirection);
                _writer.Put(state.PlayerStates[i].Grounded);
                _writer.Put(state.PlayerStates[i].GroundVelocity.X);
                _writer.Put(state.PlayerStates[i].GroundVelocity.Y);
            }
            _netManager.SendToAll(_writer, DeliveryMethod.Unreliable);
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError) { }
        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType) { }
        public void OnNetworkLatencyUpdate(NetPeer peer, int latency) { }
        public void OnConnectionRequest(ConnectionRequest request)
        {
            request.AcceptIfKey("NetworkPractice");
        }
    }
}