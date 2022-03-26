using YonatanMankovich.CommandLineMinesweeper.Core.Enums;

namespace YonatanMankovich.CommandLineMinesweeper.Core
{
    public class MinesweeperAutoPlayer
    {
        public Minesweeper Minesweeper { get; set; }

        public MinesweeperAutoPlayer(Minesweeper minesweeper)
        {
            Minesweeper = minesweeper;
        }

        public ISet<Cell> GetSureMineCells()
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
                            throw new Exception("Tried to add a clear cell into mine cells list.");

                        mineCells.Add(neighbor);
                    }
            }

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
                            throw new Exception("Tried to add a clear cell into mine cells list.");

                        mineCells.Add(mineCell);
                    }
                }
            }

            return mineCells;
        }

        public ISet<Cell> GetSureClearCells()
        {
            ISet<Cell> clearCells = new HashSet<Cell>();

            foreach (Cell cell in GetAllRevealedNumberedCells())
            {
                ISet<Cell> neighbors = Minesweeper.Grid.GetNeighborsOfCell(cell);
                if (neighbors.Count(n => n.State == CellState.Flagged) == cell.MinesAround)
                    foreach (Cell neighbor in neighbors.Where(n => n.State == CellState.Untouched))
                    {
                        if (neighbor.IsMine) // In case the algorithm is wrong.
                            throw new Exception("Tried to add a mine into clear cells list.");

                        clearCells.Add(neighbor);
                    }
            }

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
                            throw new Exception("Tried to add a mine into clear cells list.");

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