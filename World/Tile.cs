namespace NetworkPractice
{
    public class Tile
    {
        public TileType TileType { get; private set; }
        public bool IsPassable { get; private set; }
        public Tile(TileType tileType)
        {
            TileType = tileType;
            IsPassable = (tileType != TileType.Blocked && tileType != TileType.Empty);
        }
    }
}