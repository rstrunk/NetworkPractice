using LiteNetLib;
using LiteNetLib.Utils;

namespace NetworkPractice
{
    public class PacketSerializer
    {
        public EntityState ReadEntity(NetPacketReader reader)
        {
            return new EntityState
            {
                Id = reader.GetString(),
                Type = reader.GetString(),
                Position = new System.Numerics.Vector2(reader.GetFloat(), reader.GetFloat()),
                Velocity = new System.Numerics.Vector2(reader.GetFloat(), reader.GetFloat()),
                FacingDirection = reader.GetInt(),
                Grounded = reader.GetBool(),
                GroundVelocity = new System.Numerics.Vector2(reader.GetFloat(), reader.GetFloat()),
                CurrentHealth = reader.GetInt(),
                CurrentMaxHealth = reader.GetInt(),
                IsStatic = reader.GetBool()
            };
        }

        public void WriteEntity(NetDataWriter writer, EntityState entity)
        {
            writer.Put(entity.Id ?? "");
            writer.Put(entity.Type ?? "");
            writer.Put(entity.Position.X);
            writer.Put(entity.Position.Y);
            writer.Put(entity.Velocity.X);
            writer.Put(entity.Velocity.Y);
            writer.Put(entity.FacingDirection);
            writer.Put(entity.Grounded);
            writer.Put(entity.GroundVelocity.X);
            writer.Put(entity.GroundVelocity.Y);
            writer.Put(entity.CurrentHealth);
            writer.Put(entity.CurrentMaxHealth);
            writer.Put(entity.IsStatic);
        }

        public ControllerInput ReadControllerInput(NetPacketReader reader)
        {
            return new ControllerInput
            {
                EntityId = reader.GetString(),
                MoveLeft = reader.GetBool(),
                MoveRight = reader.GetBool(),
                Jump = reader.GetBool()
            };
        }

        private void WriteControllerInputPayload(NetDataWriter writer, ControllerInput input)
{
    writer.Put(input.EntityId ?? "");
    writer.Put(input.MoveLeft);
    writer.Put(input.MoveRight);
    writer.Put(input.Jump);
}

public void WriteControllerInput(NetDataWriter writer, ControllerInput input)
{
    writer.Put((byte)PacketTypes.ControllerInput);
    WriteControllerInputPayload(writer, input);
}

        public WorldState ReadWorldState(NetPacketReader reader)
        {
            int tick = reader.GetInt();
            int entityCount = reader.GetInt();
            EntityState[] entities = new EntityState[entityCount];
            for (int i = 0; i < entityCount; i++)
                entities[i] = ReadEntity(reader);
            return new WorldState
            {
                Tick = tick,
                Entities = entities
            };
        }

        private void WriteWorldStatePayload(NetDataWriter writer, WorldState state)
{
    writer.Put(state.Tick);
    writer.Put(state.Entities.Length);
    for (int i = 0; i < state.Entities.Length; i++)
        WriteEntity(writer, state.Entities[i]);
}

public void WriteWorldState(NetDataWriter writer, WorldState state)
{
    writer.Put((byte)PacketTypes.WorldState);
    WriteWorldStatePayload(writer, state);
}

public void WritePlayerJoined(NetDataWriter writer, string entityId, WorldState state)
{
    writer.Put((byte)PacketTypes.PlayerJoined);
    writer.Put(entityId);
    WriteWorldStatePayload(writer, state);
}        

public (string entityId, WorldState state) ReadPlayerJoined(NetPacketReader reader)
{
    string entityId = reader.GetString();
    WorldState state = ReadWorldState(reader);
    return (entityId, state);
}
    }
}