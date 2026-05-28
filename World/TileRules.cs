using System.Collections.Generic;

namespace NetworkPractice
{
    public class TileRules
    {
        private Dictionary<TileType, TileProfile> _profiles = new();

        public TileRules()
        {
            _profiles[TileType.Floor] = new TileProfile(footstepSoundKey: "footstep_floor", bumpSoundKey: null);                        
            _profiles[TileType.Wall]  = new TileProfile(footstepSoundKey: null, bumpSoundKey: "bump_wall");
            _profiles[TileType.Empty] = new TileProfile(footstepSoundKey: null, bumpSoundKey: "bump_wall");
        }

        public TileProfile GetProfile(TileType tileType)
        {
            if (_profiles.TryGetValue(tileType, out TileProfile? profile))
                return profile;

            return _profiles[TileType.Floor];
        }
    }
}