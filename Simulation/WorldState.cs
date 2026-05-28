using System.Collections.Generic;

namespace NetworkPractice
{
    public class WorldState
    {
        public EntityState[] Entities;
        public int Tick;

        public WorldState Clone()
        {
            WorldState clone = new WorldState();
            clone.Tick = Tick;
            clone.Entities = new EntityState[Entities.Length];
            for (int i = 0; i < Entities.Length; i++)
            {
                clone.Entities[i] = new EntityState
                {
                    Id = Entities[i].Id,
                    Type = Entities[i].Type,
                    Position = Entities[i].Position,
                    Velocity = Entities[i].Velocity,
                    FacingDirection = Entities[i].FacingDirection,
                    Grounded = Entities[i].Grounded,
                    GroundVelocity = Entities[i].GroundVelocity,
                    CurrentHealth = Entities[i].CurrentHealth,
                    CurrentMaxHealth = Entities[i].CurrentMaxHealth,
                    IsStatic = Entities[i].IsStatic
                };
            }
            return clone;
        }
    }
}