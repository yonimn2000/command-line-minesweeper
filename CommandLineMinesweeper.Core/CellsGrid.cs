namespace YonatanMankovich.CommandLineMinesweeper.Core
{
    public class CellsGrid
    {
        public int Width => Grid.GetLength(0);
        public int Height => Grid.GetLength(1);
        private Cell[,] Grid { get; }

        public CellsGrid(int width, int height)
        {
            Grid = new Cell[width, height];
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    Grid[x, y] = new Cell();
        }

        public Cell? GetCell(int x, int y)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height) // Check inside the grid.
                return Grid[x, y];
            return null;
        }

        public List<Cell> GetNeighboringCells(int x, int y)
        {
            List<Cell> neighbors = new List<Cell>(8);

            // Add all neighbors around the current cell.
            for (int xOffset = -1; xOffset < 2; xOffset++)
            {
                for (int yOffset = -1; yOffset < 2; yOffset++)
                {
                    if (xOffset != 0 || yOffset != 0) // Do not include current cell.
                    {
                        Cell? cell = GetCell(x + xOffset, y + yOffset);
                        if (cell != null) // Cell is null if it outside the grid.
                            neighbors.Add(cell); 
                    }
                }
            }

            return neighbors;
        }
    }
}