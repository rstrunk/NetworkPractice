namespace NetworkPractice
{
    public class TileDefinition
    {
        public bool IsPassable;
        public bool IsClimbable;
        public bool IsPlatform;
        public bool DoesDamage;
        public float SpeedMultiplier = 1f;
        public string? FootstepSoundKey;
    }
}