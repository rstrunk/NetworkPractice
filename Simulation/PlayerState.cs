using System.Numerics;

namespace NetworkPractice
{
    public struct PlayerState
    {
        public int PlayerId;
        public Vector2 Position;
        public Vector2 Velocity;
        public int FacingDirection; // -1 for left, 1 for right
        public bool Grounded;
        public Vector2 GroundVelocity;
    }
}