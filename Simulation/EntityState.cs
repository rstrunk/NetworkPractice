using System.Numerics;

namespace NetworkPractice
{
    public class EntityState
    {
        public string? Id;
        public string? Type;
        public Vector2 Position;
         public Vector2 Velocity;
         public int FacingDirection;
         public bool Grounded;
         public Vector2 GroundVelocity;
         public int CurrentHealth;
         public int CurrentMaxHealth;
         public bool IsStatic;
    }
}