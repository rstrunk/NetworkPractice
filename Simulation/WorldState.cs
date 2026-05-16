using System.Collections.Generic;

namespace NetworkPractice
{
    public struct WorldState
    {
        public PlayerState[] PlayerStates;
        public PhysicsBodyState[] PhysicsBodies;
        public int Tick;
    }
}