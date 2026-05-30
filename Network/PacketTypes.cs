namespace NetworkPractice
{
    public enum PacketTypes : byte
{
    ControllerInput = 1,
    WorldState= 2,
    ChatMessage = 3,
    PlayerJoined = 4
}
}