using System.Drawing;
using YonatanMankovich.CommandLineMinesweeper.Core.Enums;
using YonatanMankovich.CommandLineMinesweeper.Core.Exceptions;

namespace YonatanMankovich.CommandLineMinesweeper.Core
{
    /// <summary>
    /// Represents a Minesweeper game.
    /// </summary>
    public class Minesweeper
    {
        /// <summary>
        /// Gets the total number of mines on the board.
        /// </summary>
        public int TotalMines { get; }

        /// <summary>
        /// Gets the <see cref="CellsGrid"/> of the game.
        /// </summary>
        public CellsGrid Grid { get; }

        /// <summary>
        /// Initializes an instance of the game with <see cref="MinesweeperOptions"/>.
        /// </summary>
        /// <param name="options">The options to initialize the game with.</param>
        public Minesweeper(MinesweeperOptions options)
        {
            TotalMines = options.Mines;
            Grid = new CellsGrid(options.Width, options.Height);
            PlaceRandomMines(options);
        }

        /// <summary>
        /// Places mines randomly on the grid.
        /// </summary>
        /// <param name="options">The options the game is initialized with.</param>
        private void PlaceRandomMines(MinesweeperOptions options)
        {
            // Shuffle all cells and take as many as the number of mines to place.
            Random random = options.RandomSeed == null ? new Random() : new Random((int)options.RandomSeed);
            foreach (Cell cell in Grid.GetAllCells().OrderBy(c => random.Next()).Take(options.Mines))
            {
                cell.PlaceMine();
                foreach (Cell neighbor in Grid.GetNeighborsOfCell(cell))
                    neighbor.IncrementMinesAround();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the specified cell coordinates are valid for a player move.
        /// </summary>
        /// <param name="point">The coordinates.</param>
        /// <returns><see langword="true"/> if the move is valid; <see langword="false"/> otherwise.</returns>
        public bool IsCellValidForMove(Point point) => Grid.GetCell(point).IsValidForMove();

        /// <summary>
        /// Gets a value indicating whether the specified cell coordinates are valid for a player reveal.
        /// </summary>
        /// <param name="point">The coordinates.</param>
        /// <returns><see langword="true"/> if the cell coordinates are valid for revealing; <see langword="false"/> otherwise.</returns>
        public bool IsCellValidForReveal(Point point) => Grid.GetCell(point).IsValidForReveal();

        /// <summary>
        /// Makes the specified <see cref="MinesweeperMove"/> and gets the <see cref="MinesweeperMoveResult"/>.
        /// </summary>
        /// <param name="move">The move.</param>
        /// <returns>The move result.</returns>
        public MinesweeperMoveResult MakeMove(MinesweeperMove move) => MakeMove(move.MoveType, move.Coordinates);

        /// <summary>
        /// Makes the specified <see cref="MinesweeperMoveType"/> at the given coordinates
        /// and gets the <see cref="MinesweeperMoveResult"/>.
        /// </summary>
        /// <param name="moveType">The move type.</param>
        /// <param name="x">The X coordinate of the move.</param>
        /// <param name="y">The Y coordinate of the move.</param>
        /// <returns>The move result.</returns>
        public MinesweeperMoveResult MakeMove(MinesweeperMoveType moveType, int x, int y) => MakeMove(moveType, new Point(x, y));

        /// <summary>
        /// Makes the specified <see cref="MinesweeperMoveType"/> at the given coordinates
        /// and gets the <see cref="MinesweeperMoveResult"/>.
        /// </summary>
        /// <param name="moveType">The move type.</param>
        /// <param name="coordinates">The move coordinates.</param>
        /// <returns>The move result.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public MinesweeperMoveResult MakeMove(MinesweeperMoveType moveType, Point coordinates)
        {
            try
            {
                switch (moveType)
                {
                    case MinesweeperMoveType.PlaceFlag: PlaceCellFlag(coordinates); break;
                    case MinesweeperMoveType.RemoveFlag: RemoveCellFlag(coordinates); break;
                    case MinesweeperMoveType.ToggleFlag: ToggleCellFlag(coordinates); break;
                    case MinesweeperMoveType.RevealCell: RevealCell(coordinates); break;
                    default: throw new NotImplementedException();
                }
            }
            catch (RevealedMineException)
            {
                return MinesweeperMoveResult.RevealedMine;
            }
            catch (CellException ce) when
                (ce is FlaggedCellRevealException || ce is RevealRevealedCellException || ce is CellFlagException)
            {
                return MinesweeperMoveResult.InvalidMove;
            }

            if (IsFieldClear())
                return MinesweeperMoveResult.AllClear;

            return MinesweeperMoveResult.Playing;
        }

        /// <summary>
        /// Places a flag on the cell at a point.
        /// </summary>
        /// <param name="point">The point.</param>
        private void PlaceCellFlag(Point point) => Grid.GetCell(point).PlaceFlag();

        /// <summary>
        /// Removes a flag from the cell at a point.
        /// </summary>
        /// <param name="point">The point.</param>
        private void RemoveCellFlag(Point point) => Grid.GetCell(point).RemoveFlag();

        /// <summary>
        /// Toggles a flag on the cell at a point.
        /// </summary>
        /// <param name="point">The point.</param>
        private void ToggleCellFlag(Point point) => Grid.GetCell(point).ToggleFlag();

        /// <summary>
        /// Reveals the cell at a point.
        /// </summary>
        /// <param name="point">The point.</param>
        private void RevealCell(Point point)
        {
            Cell selectedCell = Grid.GetCell(point);
            selectedCell.Reveal();

            // Expand blank area around blank cell.
            if (selectedCell.MinesAround == 0)
            {
                Queue<Cell> cellsToReveal = new Queue<Cell>();
                cellsToReveal.Enqueue(selectedCell);

                while (cellsToReveal.Count > 0)
                {
                    Cell cell = cellsToReveal.Dequeue();

                    // For each untouched neighbor of the current cell...
                    foreach (Cell neighbor in Grid.GetNeighborsOfCell(cell)
                        .Where(n => n.State == CellState.Untouched))
                    {
                        // Reveal neighbor and enqueue if blank.
                        neighbor.Reveal();

                        if (neighbor.MinesAround == 0)
                            cellsToReveal.Enqueue(neighbor);
                    }
                }
            }
        }

        /// <summary>
        /// Counts the number of placed flags on the board and returns the number.
        /// </summary>
        /// <returns>The number of flagged cells.</returns>
        public int CountFlags() => Grid.GetAllCells().Count(c => c.State == CellState.Flagged);

        /// <summary>
        /// Determines whether all the non-mine cells have been revealed, that is, the game is complete.
        /// </summary>
        /// <returns><see langword="true"/> if the field is clear; <see langword="false"/> otherwise.</returns>
        public bool IsFieldClear() => !Grid.GetAllCells().Any(c => c.State != CellState.Revealed && !c.IsMine);

        /// <summary>
        /// Gets the number of remaining mines on the board. The number is determined by the difference of 
        /// the total mines and the number of placed flags.
        /// </summary>
        /// <returns>The number of remaining mines on the board.</returns>
        public int GetNumberOfRemainingMines() => IsFieldClear() ? 0 : TotalMines - CountFlags();

        /// <summary>
        /// Gets the percent of the game completeness based on the number of untouched cells.
        /// </summary>
        /// <returns>The percent of the game completeness.</returns>
        public double GetGameCompleteness()
            => IsFieldClear() ? 1 : 1 - ((double)GetUntouchedCells().Count() / (Grid.Width * Grid.Height));

        /// <summary>
        /// Gets the <see cref="IEnumerable{T}"/> of all the untouched <see cref="Cell"/>s.
        /// </summary>
        /// <returns>All the untouched <see cref="Cell"/>s.</returns>
        public IEnumerable<Cell> GetUntouchedCells() => Grid.GetAllCells().Where(c => c.State == CellState.Untouched);

        /// <summary>
        /// Resets the state of the game to be played again.
        /// </summary>
        public void Reset()
        {
            foreach (Cell cell in Grid.GetAllCells())
                cell.ClearState();
        }
    }
}