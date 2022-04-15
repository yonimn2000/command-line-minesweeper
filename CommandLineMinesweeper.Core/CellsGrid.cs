using System.Drawing;

namespace YonatanMankovich.CommandLineMinesweeper.Core
{
    /// <summary>
    /// Represents a Minesweeper grid of <see cref="Cell"/>s.
    /// </summary>
    public class CellsGrid
    {
        /// <summary>
        /// Gets the width of the grid.
        /// </summary>
        public int Width => Grid.GetLength(0);

        /// <summary>
        /// Gets the height of the grid.
        /// </summary>
        public int Height => Grid.GetLength(1);

        /// <summary>
        /// The 2D array of <see cref="Cell"/>s.
        /// </summary>
        private Cell[,] Grid { get; }

        /// <summary>
        /// Creates a grid of untouched cells of the specified <paramref name="width"/> and <paramref name="height"/>.
        /// </summary>
        /// <param name="width">The grid width.</param>
        /// <param name="height">The grid height.</param>
        internal CellsGrid(int width, int height)
        {
            Grid = new Cell[width, height];
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    Grid[x, y] = new Cell(new Point(x, y));
        }

        /// <summary>
        /// Gets the <see cref="Cell"/> at the specified coordinates.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <returns>The <see cref="Cell"/> at the specified coordinates.</returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public Cell GetCell(int x, int y)
        {
            if (IsPointOnGrid(x, y))
                return Grid[x, y];

            throw new IndexOutOfRangeException("The provided point coordinates were outside of the grid.");
        }

        /// <summary>
        /// Gets the <see cref="Cell"/> at the specified coordinates.
        /// </summary>
        /// <param name="point">The coordinates</param>
        /// <returns>The <see cref="Cell"/> at the specified coordinates.</returns>
        public Cell GetCell(Point point) => GetCell(point.X, point.Y);

        /// <summary>
        /// Gets a value representing whether the given coordinates are on the grid.
        /// </summary>
        /// <param name="point">The coordinates.</param>
        /// <returns><see langword="true"/> if the coordinates are on the grid; <see langword="false"/> otherwise.</returns>
        public bool IsPointOnGrid(Point point) => IsPointOnGrid(point.X, point.Y);

        /// <summary>
        /// Gets a value representing whether the given coordinates are on the grid.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <returns><see langword="true"/> if the coordinates are on the grid; <see langword="false"/> otherwise.</returns>
        public bool IsPointOnGrid(int x, int y) => IsBetween(x, 0, Width) && IsBetween(y, 0, Height);

        /// <summary>
        /// Determines whether the given <paramref name="value"/> is between the <paramref name="lowerBoundInclusive"/>
        /// and the <paramref name="upperBoundExclusive"/>.
        /// </summary>
        /// <param name="value">The value to test.</param>
        /// <param name="lowerBoundInclusive">The lower inclusive bound.</param>
        /// <param name="upperBoundExclusive">The upper exclusive bound.</param>
        /// <returns><see langword="true"/> if the value is between the bounds; <see langword="false"/> otherwise.</returns>
        private static bool IsBetween(int value, int lowerBoundInclusive, int upperBoundExclusive)
            => lowerBoundInclusive <= value && value < upperBoundExclusive;

        /// <summary>
        /// Gets the <see cref="ISet{T}"/> of neighboring <see cref="Cell"/>s of the given <see cref="Cell"/>.
        /// </summary>
        /// <param name="cell">The cell.</param>
        /// <returns>The <see cref="ISet{T}"/> of the neighbors <see cref="Cell"/>s of the given <see cref="Cell"/>.</returns>
        internal ISet<Cell> GetNeighborsOfCell(Cell cell)
        {
            ISet<Cell> neighbors = new HashSet<Cell>(8); // Max 8 neighbors.

            // Add all neighbors around the current cell.
            for (int xOffset = -1; xOffset < 2; xOffset++)
                for (int yOffset = -1; yOffset < 2; yOffset++)
                    // Exclude current cell and cells outside of the grid.
                    if (!(xOffset == 0 && yOffset == 0) && IsPointOnGrid(cell.Coordinates.X + xOffset, cell.Coordinates.Y + yOffset))
                        neighbors.Add(GetCell(cell.Coordinates.X + xOffset, cell.Coordinates.Y + yOffset));

            return neighbors;
        }

        /// <summary>
        /// Enumerates all the <see cref="Cell"/>s of the grid.
        /// </summary>
        /// <returns>The <see cref="IEnumerable{T}"/> of <see cref="Cell"/>s.</returns>
        public IEnumerable<Cell> GetAllCells()
        {
            foreach (Cell cell in Grid)
                yield return cell;
        }
    }
}