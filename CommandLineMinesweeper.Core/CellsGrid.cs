using System.Drawing;

namespace YonatanMankovich.CommandLineMinesweeper.Core
{
    public class CellsGrid
    {
        public int Width => Grid.GetLength(0);
        public int Height => Grid.GetLength(1);
        private Cell[,] Grid { get; }

        // Creates a grid of untouched cells.
        internal CellsGrid(int width, int height)
        {
            Grid = new Cell[width, height];
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    Grid[x, y] = new Cell(new Point(x, y));
        }

        public Cell GetCell(int x, int y)
        {
            if (IsPointOnGrid(x, y))
                return Grid[x, y];

            throw new IndexOutOfRangeException("The provided point coordinates were outside of the grid.");
        }

        public bool IsPointOnGrid(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

        internal ISet<Cell> GetNeighborsOfCell(Cell cell)
        {
            ISet<Cell> neighbors = new HashSet<Cell>(8); // Max 8 neighbors.

            // Add all neighbors around the current cell.
            for (int xOffset = -1; xOffset < 2; xOffset++)
                for (int yOffset = -1; yOffset < 2; yOffset++)
                    // Exclude current cell and cells outside of the grid.
                    if ((xOffset != 0 || yOffset != 0) && IsPointOnGrid(cell.Coordinates.X + xOffset, cell.Coordinates.Y + yOffset))
                        neighbors.Add(GetCell(cell.Coordinates.X + xOffset, cell.Coordinates.Y + yOffset));

            return neighbors;
        }

        internal IEnumerable<Cell> GetAllCells()
        {
            foreach (Cell cell in Grid)
                yield return cell;
        }
    }
}