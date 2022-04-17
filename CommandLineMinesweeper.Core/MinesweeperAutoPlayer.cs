using System.Drawing;
using YonatanMankovich.CommandLineMinesweeper.Core.Enums;
using YonatanMankovich.CommandLineMinesweeper.Core.Exceptions;

namespace YonatanMankovich.CommandLineMinesweeper.Core
{
    /// <summary>
    /// Represents an automatic <see cref="Minesweeper"/> game player and provides static methods for finding sure moves.
    /// </summary>
    public class MinesweeperAutoPlayer : MinesweeperPlayer
    {
        /// <summary>
        /// The optional random seed for the necessary random moves.
        /// </summary>
        public int? RandomSeed { get; }

        /// <summary>
        /// Gets or sets the value indicating whether the auto player should make all sure moves at once.
        /// </summary>
        public bool MoveInBatches { get; set; }

        /// <summary>
        /// Gets the number of guess moves made by the player.
        /// </summary>
        public int GuessMoves { get; private set; }

        /// <summary>
        /// Initializes an instance of the <see cref="MinesweeperAutoPlayer"/> class with an instance of the
        /// <see cref="Minesweeper"/> game class.
        /// </summary>
        /// <param name="minesweeper">The <see cref="Minesweeper"/> game.</param>
        /// <param name="randomSeed">An optional random seed for the necessary random moves.</param>
        public MinesweeperAutoPlayer(Minesweeper minesweeper, int? randomSeed = null) : base(minesweeper)
        {
            RandomSeed = randomSeed;
        }

        /// <inheritdoc/>
        public override void PlayToEnd()
        {
            Random random = RandomSeed == null ? new Random() : new Random((int)RandomSeed);
            MinesweeperMoveResult lastMoveResult = MinesweeperMoveResult.Playing;
            while (lastMoveResult != MinesweeperMoveResult.RevealedMine && lastMoveResult != MinesweeperMoveResult.AllClear)
            {
                ISet<Cell> sureMineCells = GetSureMineCells(Minesweeper);
                ISet<Cell> sureClearCells = GetSureClearCells(Minesweeper);
                if (sureMineCells.Count == 0 && sureClearCells.Count == 0)
                {
                    GuessMoves++;
                    // Select a random untouched cell.
                    Cell? randomUntouchedCell = Minesweeper.GetUntouchedCells().OrderBy(c => random.Next()).FirstOrDefault();
                    if (randomUntouchedCell == null)
                    {
                        lastMoveResult = MinesweeperMoveResult.AllClear;
                        break;
                    }
                    else
                    {
                        lastMoveResult = Minesweeper.MakeMove(MinesweeperMoveType.RevealCell, randomUntouchedCell.Coordinates);
                        if (lastMoveResult == MinesweeperMoveResult.RevealedMine)
                        {
                            AfterRevealedMineCallback?.Invoke(randomUntouchedCell.Coordinates);
                            break;
                        }
                        else
                            AfterMoveCallback?.Invoke(randomUntouchedCell.Coordinates);
                    }
                }

                foreach (Point point in sureMineCells.Select(c => c.Coordinates))
                {
                    lastMoveResult = Minesweeper.MakeMove(MinesweeperMoveType.PlaceFlag, point);
                    if (!MoveInBatches)
                        AfterMoveCallback?.Invoke(point);
                }
                if (MoveInBatches)
                    AfterMoveCallback?.Invoke(null);

                foreach (Point point in sureClearCells.Select(c => c.Coordinates))
                {
                    lastMoveResult = Minesweeper.MakeMove(MinesweeperMoveType.RevealCell, point);
                    if (!MoveInBatches)
                        AfterMoveCallback?.Invoke(point);
                }
                if (MoveInBatches)
                    AfterMoveCallback?.Invoke(null);
            }

            if (lastMoveResult == MinesweeperMoveResult.AllClear)
                AfterFieldClearedCallback?.Invoke();
        }

        /// <summary>
        /// Gets an <see cref="ISet{T}"/> the sure mine <see cref="Cell"/>s of a <see cref="Minesweeper"/> 
        /// game based on the currently revealed and flagged <see cref="Cell"/>s.
        /// </summary>
        /// <param name="minesweeper">The <see cref="Minesweeper"/> game.</param>
        /// <returns>An <see cref="ISet{T}"/> of sure mine <see cref="Cell"/>s.</returns>
        /// <exception cref="CellException">Thrown if something went wrong in the algorithm.</exception>
        public static ISet<Cell> GetSureMineCells(Minesweeper minesweeper)
        {
            ISet<Cell> mineCells = new HashSet<Cell>();

            foreach (Cell cell in GetAllRevealedNumberedCells(minesweeper))
            {
                ISet<Cell> neighbors = minesweeper.Grid.GetNeighborsOfCell(cell);
                ISet<Cell> untouchedNeighbors = neighbors.Where(n => n.State == CellState.Untouched).ToHashSet();
                int flaggedNeighbors = neighbors.Count(n => n.State == CellState.Flagged);
                if (untouchedNeighbors.Count + flaggedNeighbors == cell.MinesAround)
                    foreach (Cell neighbor in untouchedNeighbors)
                    {
                        if (!neighbor.IsMine) // In case the algorithm is wrong.
                            throw new CellException("Tried to add a clear cell into mine cells list.", neighbor);

                        mineCells.Add(neighbor);
                    }
            }

            if (mineCells.Count == 0)
                return GetAdvancedSureMineCells(minesweeper);

            return mineCells;
        }

        /// <summary>
        /// Gets an <see cref="ISet{T}"/> of the advanced sure mine <see cref="Cell"/>s of a <see cref="Minesweeper"/>
        /// game based on the currently revealed and flagged <see cref="Cell"/>s.
        /// Unlike <see cref="GetSureMineCells(Minesweeper)"/>, this algorithm checks each neighbor 
        /// <see cref="Cell"/> of a revealed <see cref="Cell"/> if there are any sure mine <see cref="Cell"/>s 
        /// next to it, thus making this algorithm more computationally expensive; therefore, it should only 
        /// be run if the previous algorithm did not find any sure mine <see cref="Cell"/>s.
        /// </summary>
        /// <param name="minesweeper">The <see cref="Minesweeper"/> game.</param>
        /// <returns>An <see cref="ISet{T}"/> of sure mine <see cref="Cell"/>s.</returns>
        /// <exception cref="CellException">Thrown if something went wrong in the algorithm.</exception>
        private static ISet<Cell> GetAdvancedSureMineCells(Minesweeper minesweeper)
        {
            // There is some magic going on in this method. The algorithm was made by trial and error (and a bit of logic).
            ISet<Cell> mineCells = new HashSet<Cell>();

            foreach (Cell cell in GetAllRevealedNumberedCells(minesweeper))
            {
                ISet<Cell> neighbors = minesweeper.Grid.GetNeighborsOfCell(cell);
                ISet<Cell> sureMines = neighbors.Where(n => n.State == CellState.Flagged).ToHashSet(); // AKA flagged.
                ISet<Cell> possibleMines = neighbors.Where(n => n.State == CellState.Untouched).ToHashSet(); // AKA untouched
                int minesRemaining = cell.MinesAround - sureMines.Count;

                if (possibleMines.Count == 0 || possibleMines.Count > 2 || possibleMines.Count + sureMines.Count > cell.MinesAround + 1)
                    continue;

                ISet<Cell> revealedNeighbors = neighbors.Where(n => n.State == CellState.Revealed && n.MinesAround > 0).ToHashSet();
                foreach (Cell neighbor in revealedNeighbors)
                {
                    ISet<Cell> neighborsOfNeighbor = minesweeper.Grid.GetNeighborsOfCell(neighbor);
                    ISet<Cell> sureMinesOfNeighbor = neighborsOfNeighbor.Where(n => n.State == CellState.Flagged).ToHashSet(); // AKA flagged.
                    ISet<Cell> possibleMinesOfNeighbor = neighborsOfNeighbor.Where(n => n.State == CellState.Untouched).ToHashSet(); // AKA untouched
                    int minesRemainingOfNeighbor = neighbor.MinesAround - sureMinesOfNeighbor.Count;

                    if (minesRemainingOfNeighbor < 2 || possibleMinesOfNeighbor.Count - minesRemainingOfNeighbor > 1)
                        continue;

                    ISet<Cell> commonCells = possibleMinesOfNeighbor.Intersect(possibleMines).ToHashSet();
                    if (commonCells.Count < 2)
                        continue;

                    ISet<Cell> mineCellsAroundNeighbor = possibleMinesOfNeighbor.Except(commonCells).ToHashSet();
                    foreach (Cell mineCell in mineCellsAroundNeighbor)
                    {
                        if (!mineCell.IsMine) // In case the algorithm is wrong.
                            throw new CellException("Tried to add a clear cell into mine cells list.", mineCell);

                        mineCells.Add(mineCell);
                    }
                }
            }

            return mineCells;
        }

        /// <summary>
        /// Gets an <see cref="ISet{T}"/> of the sure clear (non-mine) <see cref="Cell"/>s of a <see cref="Minesweeper"/> 
        /// game based on the currently revealed and flagged <see cref="Cell"/>s.
        /// </summary>
        /// <param name="minesweeper">The <see cref="Minesweeper"/> game.</param>
        /// <returns>An <see cref="ISet{T}"/> of sure clear <see cref="Cell"/>s.</returns>
        /// <exception cref="CellException">Thrown if something went wrong in the algorithm.</exception>
        public static ISet<Cell> GetSureClearCells(Minesweeper minesweeper)
        {
            ISet<Cell> clearCells = new HashSet<Cell>();

            foreach (Cell cell in GetAllRevealedNumberedCells(minesweeper))
            {
                ISet<Cell> neighbors = minesweeper.Grid.GetNeighborsOfCell(cell);
                if (neighbors.Count(n => n.State == CellState.Flagged) == cell.MinesAround)
                    foreach (Cell neighbor in neighbors.Where(n => n.State == CellState.Untouched))
                    {
                        if (neighbor.IsMine) // In case the algorithm is wrong.
                            throw new CellException("Tried to add a mine into clear cells list.", neighbor);

                        clearCells.Add(neighbor);
                    }
            }

            if (clearCells.Count == 0)
                return GetAdvancedSureClearCells(minesweeper);

            return clearCells;
        }

        /// <summary>
        /// Gets an <see cref="ISet{T}"/> of the advanced sure clear <see cref="Cell"/>s of a <see cref="Minesweeper"/> game based on the currently 
        /// revealed and flagged <see cref="Cell"/>s. Unlike <see cref="GetSureClearCells(Minesweeper)"/>, this algorithm
        /// checks each neighbor of a revealed <see cref="Cell"/> if there are any sure clear <see cref="Cell"/>s next 
        /// to it, thus making this algorithm more computationally expensive; therefore, it should only be run if the 
        /// previous algorithm did not find any sure clear <see cref="Cell"/>s.
        /// </summary>
        /// <param name="minesweeper">The <see cref="Minesweeper"/> game.</param>
        /// <returns>An <see cref="ISet{T}"/> of sure clear <see cref="Cell"/>s.</returns>
        /// <exception cref="CellException">Thrown if something went wrong in the algorithm.</exception>
        private static ISet<Cell> GetAdvancedSureClearCells(Minesweeper minesweeper)
        {
            // There is some magic going on in this method. The algorithm was made by trial and error (and a bit of logic).
            ISet<Cell> clearCells = new HashSet<Cell>();

            foreach (Cell cell in GetAllRevealedNumberedCells(minesweeper))
            {
                ISet<Cell> neighbors = minesweeper.Grid.GetNeighborsOfCell(cell);
                ISet<Cell> sureMines = neighbors.Where(n => n.State == CellState.Flagged).ToHashSet(); // AKA flagged.
                ISet<Cell> possibleMines = neighbors.Where(n => n.State == CellState.Untouched).ToHashSet(); // AKA untouched
                int minesRemaining = cell.MinesAround - sureMines.Count;

                if (possibleMines.Count == 0 || possibleMines.Count + sureMines.Count > cell.MinesAround + 1)
                    continue;

                ISet<Cell> revealedNeighbors = neighbors.Where(n => n.State == CellState.Revealed && n.MinesAround > 0).ToHashSet();
                foreach (Cell neighbor in revealedNeighbors)
                {
                    ISet<Cell> neighborsOfNeighbor = minesweeper.Grid.GetNeighborsOfCell(neighbor);
                    ISet<Cell> sureMinesOfNeighbor = neighborsOfNeighbor.Where(n => n.State == CellState.Flagged).ToHashSet(); // AKA flagged.
                    ISet<Cell> possibleMinesOfNeighbor = neighborsOfNeighbor.Where(n => n.State == CellState.Untouched).ToHashSet(); // AKA untouched

                    // Skip if number of remaining mines of the neighbor is over 1.
                    if (neighbor.MinesAround - sureMinesOfNeighbor.Count > 1)
                        continue;

                    ISet<Cell> commonCells = possibleMinesOfNeighbor.Intersect(possibleMines).ToHashSet();
                    if (commonCells.Count < 2)
                        continue;

                    ISet<Cell> clearCellsAroundNeighbor = possibleMinesOfNeighbor.Except(commonCells).ToHashSet();
                    foreach (Cell clearCell in clearCellsAroundNeighbor)
                    {
                        if (clearCell.IsMine) // In case the algorithm is wrong.
                            throw new CellException("Tried to add a mine into clear cells list.", clearCell);

                        clearCells.Add(clearCell);
                    }
                }
            }

            return clearCells;
        }

        /// <summary>
        /// Gets an <see cref="IEnumerable{T}"/> of all the revealed <see cref="Cell"/>s that have a number on them,
        /// that is, they have at least one mine <see cref="Cell"/> around them.
        /// </summary>
        /// <param name="minesweeper">The <see cref="Minesweeper"/> game.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of all the revealed <see cref="Cell"/>s that have a number on them.</returns>
        private static IEnumerable<Cell> GetAllRevealedNumberedCells(Minesweeper minesweeper)
            => minesweeper.Grid.GetAllCells().Where(c => c.State == CellState.Revealed && c.MinesAround > 0);
    }
}