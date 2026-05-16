using System.Numerics;

namespace NetworkPractice
{
    public struct PhysicsBodyState
    {
        public string Id;
        public string Type;
        public Vector2 Position;
        public Vector2 Velocity;
        public bool IsStatic;
    }
}