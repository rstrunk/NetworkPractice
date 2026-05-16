using Microsoft.Xna.Framework;

namespace NetworkPractice
{
    public class WorldGrid
    {
        private Tile[,] _grid;
        private int _columns;
        private int _rows;
        private int _tileSize;
        public int Columns => _columns;
        public int Rows => _rows;
        public int TileSize => _tileSize;

        public WorldGrid(int columns, int rows, int tileSize)
        {
            _columns = columns;
            _rows = rows;
            _tileSize = tileSize;
            _grid = new Tile[columns, rows];
            FillGrid(TileType.Empty);
        }

        private void FillGrid(TileType tileType)
        {
            for (int col = 0; col < _columns; col++)
            {
                for (int row = 0; row < _rows; row++)
                {
                    _grid[col, row] = new Tile(tileType);
                }
            }
        }

        public void SetTile(int col, int row, TileType tileType)
        {
            if (!InBounds(col, row)) return;
            _grid[col, row] = new Tile(tileType);
        }

        public Tile GetTile(int col, int row)
        {
            if (!InBounds(col, row)) return new Tile(TileType.Empty);
            return _grid[col, row];
        }

        public bool IsPassable(int col, int row)
        {
            return GetTile(col, row).IsPassable;
        }

public bool IsPassable(Vector2 worldPosition)
{
    Point gridPos = WorldToGrid(worldPosition);
    return IsPassable(gridPos.X, gridPos.Y);
}

        public Vector2 GridToWorld(int col, int row)
        {
            return new Vector2(col * _tileSize, row * _tileSize);
        }

        public Point WorldToGrid(Vector2 worldPosition)
        {
            return new Point((int)(worldPosition.X / _tileSize), (int)(worldPosition.Y / _tileSize));
        }

        private bool InBounds(int col, int row)
        {
            return col >= 0 && col < _columns && row >= 0 && row < _rows;
        }                

        public  bool InBounds(Vector2 worldPosition)
        {
            Point gridPos = WorldToGrid(worldPosition);
            return InBounds(gridPos.X, gridPos.Y);
    }
}
}