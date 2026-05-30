using System.Collections.Generic;

namespace NetworkPractice
{
    public static class TileRegistry
    {
        private static readonly Dictionary<TileType, TileDefinition> _definitions = new()
        {
            [TileType.Empty] = new TileDefinition { IsPassable = true },
            [TileType.Floor] = new TileDefinition { IsPassable = false, FootstepSoundKey = "footstep_floor" },
            [TileType.Ladder] = new TileDefinition { IsPassable = true, IsClimbable = true, FootstepSoundKey = "footstep_ladder" },
            [TileType.WallTop] = new TileDefinition { IsPassable = false, IsPlatform = true, FootstepSoundKey = "footstep_wall" },
            [TileType.WallPlatform] = new TileDefinition { IsPassable = false, IsPlatform = true, FootstepSoundKey = "footstep_wall" },
            [TileType.Blocked] = new TileDefinition { IsPassable = false },
            [TileType.Ice] = new TileDefinition { IsPassable = false, SpeedMultiplier = 1.5f, FootstepSoundKey = "footstep_ice" },
            [TileType.Lava] = new TileDefinition { IsPassable = true, DoesDamage = true, FootstepSoundKey = "footstep_lava" },
            [TileType.PitWall] = new TileDefinition { IsPassable = true, IsClimbable = true, FootstepSoundKey = "footstep_pit_wall" },
            [TileType.PitBottom] = new TileDefinition { IsPassable = false, DoesDamage = false, FootstepSoundKey = "footstep_pit_bottom" }
        };
        public static TileDefinition Get(TileType type) => _definitions[type];
    }
}