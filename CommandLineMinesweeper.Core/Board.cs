using YonatanMankovich.CommandLineMinesweeper.Core.Enums;

namespace YonatanMankovich.CommandLineMinesweeper.Core
{
    internal class Board
    {
        internal CellsGrid Grid { get; }

        internal Board(BoardOptions options)
        {
            Grid = new CellsGrid(options.Width, options.Height);
            PlaceRandomMines(options);
        }

        private void PlaceRandomMines(BoardOptions options)
        {
            // Shuffle all cells and take as many as the number of mines to place.
            Random random = options.RandomSeed == null ? new Random() : new Random((int)options.RandomSeed);
            foreach (Cell cell in Grid.GetAllCells().OrderBy(c => random.Next()).Take(options.GetNumberOfMines()))
            {
                cell.IsMine = true;
                foreach (Cell neighbor in Grid.GetNeighborsOfCell(cell))
                    neighbor.IncrementMinesAround();
            }
        }

        internal CellRevealResult RevealCell(int x, int y)
        {
            Cell selectedCell = Grid.GetCell(x, y);
            CellRevealResult cellRevealResult = selectedCell.Reveal();

            if (cellRevealResult == CellRevealResult.Invalid || cellRevealResult == CellRevealResult.Mine)
                return cellRevealResult;

            // Expand blank area around blank cell.
            if (selectedCell.MinesAround == 0)
            {
                Queue<Cell> cellsToReveal = new Queue<Cell>();
                cellsToReveal.Enqueue(selectedCell);

                while (cellsToReveal.Count > 0)
                {
                    Cell cell = cellsToReveal.Dequeue();

                    // Foreach untouched neighbor of the current cell...
                    foreach (Cell neighbor in Grid.GetNeighborsOfCell(cell)
                        .Where(n => n.State == CellState.Untouched))
                    {
                        // Reveal neighbor and enqueue if blank.
                        CellRevealResult neighborRevealResult = neighbor.Reveal();
                        if (neighborRevealResult != CellRevealResult.Clear)
                            throw new Exception($"Unexpected cell reveal result. Expected {CellRevealResult.Clear}; Got {neighborRevealResult}");

                        if (neighbor.MinesAround == 0)
                            cellsToReveal.Enqueue(neighbor);
                    }
                }
            }

            return cellRevealResult;
        }

        internal void ToggleCellFlag(int x, int y)
        {
            Grid.GetCell(x, y).ToggleFlag();
        }

        internal int CountFlags()
        {
            return Grid.GetAllCells().Count(c => c.State == CellState.Flagged);
        }

        internal bool IsFieldClear()
        {
            return !Grid.GetAllCells().Any(c => c.State != CellState.Revealed && !c.IsMine);
        }
    }
}