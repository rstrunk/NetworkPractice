namespace NetworkPractice
{
    public class TileProfile
    {
        public string? FootstepSoundKey { get; }
        public string? BumpSoundKey { get; }
        public float VolumeScale { get; }
        public float SpeedMultiplier {get; }

        public TileProfile(string? footstepSoundKey, string? bumpSoundKey, float volumeScale = 1f, float speedMultiplier = 1f)
        {
            FootstepSoundKey = footstepSoundKey;
            BumpSoundKey = bumpSoundKey;
            VolumeScale = volumeScale;
            SpeedMultiplier = speedMultiplier;
        }
    }
}