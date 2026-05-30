using Microsoft.Xna.Framework;

namespace NetworkPractice
{
    public class WorldGrid
    {
        private TileType[,] _grid;
        public int Floors { get; }
        public int Columns { get; }
        public int Rows { get; }
        public int TileSize { get; } = 64;
        private int _topFloorBuffer = 30; //May need to change this depending on the feel of the game
        private int _floorSpacing = 20;

        public WorldGrid(int floors, int columns)
        {
            Floors = floors;
            Columns = columns;
            Rows = floors * _floorSpacing + _topFloorBuffer;
            _grid = new TileType[Columns, Rows];
            InitializeGrid();
        }

        private void InitializeGrid()
        {
            //First lay the floors
            for (int i = 1; i <= Floors; i++)
            {
                int floorRow = i * _floorSpacing - (_floorSpacing / 2);
                for (int col = 0; col < Columns; col++)
                {
                    _grid[col, floorRow + _topFloorBuffer] = TileType.Floor;
                }
            }

            //Then lay the ladders
            int topFloorRow = _floorSpacing - (_floorSpacing / 2) + _topFloorBuffer;
            int bottomFloorRow = Floors * _floorSpacing - (_floorSpacing / 2) + _topFloorBuffer;
            for (int i = topFloorRow; i <= bottomFloorRow; i++)
            {
                _grid[1, i] = TileType.Ladder;
                _grid[Columns - 2, i] = TileType.Ladder;
            }

            //Then place left and right boundaries
            for (int row = 0; row < Rows; row++)
            {
                _grid[0, row] = TileType.Blocked;
                _grid[Columns - 1, row] = TileType.Blocked;
            }
        }

        public void SetTile(int col, int row, TileType tileType)
        {
            if (!InBounds(col, row)) return;
            _grid[col, row] = tileType;
        }

        public TileType GetTile(int col, int row)
        {
            if (!InBounds(col, row)) return TileType.Empty;
            return _grid[col, row];
        }

        public bool IsPassable(int col, int row)
        {
            return TileRegistry.Get(GetTile(col, row)).IsPassable;
        }

        public bool IsPassable(Vector2 worldPosition)
        {
            Point gridPos = PointToTile(worldPosition);
            return IsPassable(gridPos.X, gridPos.Y);
        }

        public Vector2 TileToPoint(int col, int row)
        {
            return new Vector2(col * TileSize, row * TileSize);
        }

        public Point PointToTile(Vector2 worldPosition)
        {
            return new Point((int)(worldPosition.X / TileSize), (int)(worldPosition.Y / TileSize));
        }

        private bool InBounds(int col, int row)
        {
            return col >= 0 && col < Columns && row >= 0 && row < Rows;
        }

        public bool InBounds(Vector2 worldPosition)
        {
            Point gridPos = PointToTile(worldPosition);
            return InBounds(gridPos.X, gridPos.Y);
        }
    }
}
