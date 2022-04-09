using System.Drawing;
using YonatanMankovich.CommandLineMinesweeper.Core.Enums;
using YonatanMankovich.CommandLineMinesweeper.Core.Exceptions;

namespace YonatanMankovich.CommandLineMinesweeper.Core
{
    public class MinesweeperAutoPlayer
    {
        public Minesweeper Minesweeper { get; private set; }
        public Action DrawBoardCallback { get; set; }
        public MinesweeperMoveResult LastMoveResult { get; private set; }
        public Point LastMoveCoordinates { get; private set; }
        private int? RandomSeed { get; }

        public MinesweeperAutoPlayer(Minesweeper minesweeper, int? randomSeed = null)
        {
            Minesweeper = minesweeper;
            RandomSeed = randomSeed;
            LastMoveResult = MinesweeperMoveResult.Playing;
            LastMoveCoordinates = default;
        }

        public void PlayToEnd()
        {
            Random random = RandomSeed == null ? new Random() : new Random((int)RandomSeed);
            while (LastMoveResult != MinesweeperMoveResult.RevealedMine && LastMoveResult != MinesweeperMoveResult.AllClear)
            {
                ISet<Cell> sureMineCells = GetSureMineCells(includeAdvanced: true);
                ISet<Cell> sureClearCells = GetSureClearCells(includeAdvanced: true);
                if (sureMineCells.Count == 0 && sureClearCells.Count == 0)
                {
                    // Select a random untouched cell.
                    Cell? randomUntouchedCell = Minesweeper.GetUntouchedCells().OrderBy(c => random.Next()).FirstOrDefault();
                    if (randomUntouchedCell == null)
                        LastMoveResult = MinesweeperMoveResult.AllClear;
                    else
                    {
                        LastMoveCoordinates = randomUntouchedCell.Coordinates;
                        LastMoveResult = Minesweeper.MakeMove(MinesweeperMoveType.Reveal, randomUntouchedCell.Coordinates);
                        DrawBoardCallback?.Invoke();
                    }
                }

                foreach (Point point in sureMineCells.Select(c => c.Coordinates))
                {
                    LastMoveCoordinates = point;
                    LastMoveResult = Minesweeper.MakeMove(MinesweeperMoveType.PlaceFlag, point);
                    DrawBoardCallback?.Invoke();
                }

                foreach (Point point in sureClearCells.Select(c => c.Coordinates))
                {
                    LastMoveCoordinates = point;
                    LastMoveResult = Minesweeper.MakeMove(MinesweeperMoveType.Reveal, point);
                    DrawBoardCallback?.Invoke();
                }
            }
            DrawBoardCallback?.Invoke();
        }

        public void Reset()
        {
            Minesweeper.Reset();
            LastMoveResult = MinesweeperMoveResult.Playing;
            LastMoveCoordinates = default;
        }

        public ISet<Cell> GetSureMineCells(bool includeAdvanced = false)
        {
            ISet<Cell> mineCells = new HashSet<Cell>();

            foreach (Cell cell in GetAllRevealedNumberedCells())
            {
                ISet<Cell> neighbors = Minesweeper.Grid.GetNeighborsOfCell(cell);
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

            if (mineCells.Count == 0 && includeAdvanced)
                return GetAdvancedSureMineCells();

            return mineCells;
        }

        public ISet<Cell> GetAdvancedSureMineCells()
        {
            // There is some magic going on in this method. The algorithm was made by trial and error (and a bit of logic).
            ISet<Cell> mineCells = new HashSet<Cell>();

            foreach (Cell cell in GetAllRevealedNumberedCells())
            {
                ISet<Cell> neighbors = Minesweeper.Grid.GetNeighborsOfCell(cell);
                ISet<Cell> sureMines = neighbors.Where(n => n.State == CellState.Flagged).ToHashSet(); // AKA flagged.
                ISet<Cell> possibleMines = neighbors.Where(n => n.State == CellState.Untouched).ToHashSet(); // AKA untouched
                int minesRemaining = cell.MinesAround - sureMines.Count;

                if (possibleMines.Count == 0 || possibleMines.Count > 2 || possibleMines.Count + sureMines.Count > cell.MinesAround + 1)
                    continue;

                ISet<Cell> revealedNeighbors = neighbors.Where(n => n.State == CellState.Revealed && n.MinesAround > 0).ToHashSet();
                foreach (Cell neighbor in revealedNeighbors)
                {
                    ISet<Cell> neighborsOfNeighbor = Minesweeper.Grid.GetNeighborsOfCell(neighbor);
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

        public ISet<Cell> GetSureClearCells(bool includeAdvanced = false)
        {
            ISet<Cell> clearCells = new HashSet<Cell>();

            foreach (Cell cell in GetAllRevealedNumberedCells())
            {
                ISet<Cell> neighbors = Minesweeper.Grid.GetNeighborsOfCell(cell);
                if (neighbors.Count(n => n.State == CellState.Flagged) == cell.MinesAround)
                    foreach (Cell neighbor in neighbors.Where(n => n.State == CellState.Untouched))
                    {
                        if (neighbor.IsMine) // In case the algorithm is wrong.
                            throw new CellException("Tried to add a mine into clear cells list.", neighbor);

                        clearCells.Add(neighbor);
                    }
            }

            if (clearCells.Count == 0 && includeAdvanced)
                return GetAdvancedSureClearCells();

            return clearCells;
        }

        public ISet<Cell> GetAdvancedSureClearCells()
        {
            // There is some magic going on in this method. The algorithm was made by trial and error (and a bit of logic).
            ISet<Cell> clearCells = new HashSet<Cell>();

            foreach (Cell cell in GetAllRevealedNumberedCells())
            {
                ISet<Cell> neighbors = Minesweeper.Grid.GetNeighborsOfCell(cell);
                ISet<Cell> sureMines = neighbors.Where(n => n.State == CellState.Flagged).ToHashSet(); // AKA flagged.
                ISet<Cell> possibleMines = neighbors.Where(n => n.State == CellState.Untouched).ToHashSet(); // AKA untouched
                int minesRemaining = cell.MinesAround - sureMines.Count;

                if (possibleMines.Count == 0 || possibleMines.Count + sureMines.Count > cell.MinesAround + 1)
                    continue;

                ISet<Cell> revealedNeighbors = neighbors.Where(n => n.State == CellState.Revealed && n.MinesAround > 0).ToHashSet();
                foreach (Cell neighbor in revealedNeighbors)
                {
                    ISet<Cell> neighborsOfNeighbor = Minesweeper.Grid.GetNeighborsOfCell(neighbor);
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

        private IEnumerable<Cell> GetAllRevealedNumberedCells()
            => Minesweeper.Grid.GetAllCells().Where(c => c.State == CellState.Revealed && c.MinesAround > 0);
    }
}