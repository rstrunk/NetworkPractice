using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Net;
using System.Net.Sockets;

namespace NetworkPractice
{
    public class NetworkManager : INetEventListener
    {
        private NetManager _netManager;
        private readonly NetDataWriter _writer = new NetDataWriter();
        private NetPeer? _serverPeer;
        public event Action<ControllerInput>? ControllerInputReceived;
        public event Action<WorldState>? WorldStateReceived;
        public event Action<string, WorldState>? PlayerJoined;
        private readonly PacketSerializer _serializer = new();

        public bool IsServer { get; private set; }
        private int _nextPlayerId = 2; // host is always 1
        public Action<NetPeer, int>? OnPlayerConnected;

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
        int assignedId = _nextPlayerId++;
        OnPlayerConnected?.Invoke(peer, assignedId);
    }
    else
        _serverPeer = peer;
}

public void SendPlayerJoined(LiteNetLib.NetPeer peer, string entityId, WorldState state)
{
    _writer.Reset();
    _serializer.WritePlayerJoined(_writer, entityId, state);
    peer.Send(_writer, DeliveryMethod.ReliableOrdered);
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
                case (byte)PacketTypes.ControllerInput:
                    ControllerInputReceived?.Invoke(_serializer.ReadControllerInput(reader));
                    break;
                case (byte)PacketTypes.WorldState:
                    WorldStateReceived?.Invoke(_serializer.ReadWorldState(reader));
                    break;
                    case (byte)PacketTypes.PlayerJoined:
    var (entityId, state) = _serializer.ReadPlayerJoined(reader);
    PlayerJoined?.Invoke(entityId, state);
    break;
            }
            reader.Recycle();
        }

        public void SendControllerInput(ControllerInput input)
        {
            _writer.Reset();
            _serializer.WriteControllerInput(_writer, input);
            _serverPeer?.Send(_writer, DeliveryMethod.Unreliable);
        }

        public void SendWorldState(WorldState state)
        {
            _writer.Reset();
            _serializer.WriteWorldState(_writer, state);
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