using YonatanMankovich.CommandLineMinesweeper.Core.Enums;

namespace YonatanMankovich.CommandLineMinesweeper.Core
{
    internal class Board
    {
        internal CellsGrid Grid { get; }

        internal Board(BoardOptions options)
        {
            // Create a grid of untouched cells.
            Grid = new CellsGrid(options.Width, options.Height);
            PlaceRandomMines(options);
        }

        private void PlaceRandomMines(BoardOptions options)
        {
            // Shuffle all cells and take as many as the number of mines to place.
            foreach (Cell cell in Grid.GetAllCells().OrderBy(c => Guid.NewGuid()).Take(options.GetNumberOfMines()))
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

            if (cellRevealResult != CellRevealResult.Clear)
                return cellRevealResult;

            Queue<Cell> cellsToReveal = new Queue<Cell>();
            cellsToReveal.Enqueue(selectedCell);

            while (cellsToReveal.Count > 0)
            {
                Cell cell = cellsToReveal.Dequeue();
                cell.Reveal();

                // If has one or more mines around, do not reveal cells around.
                if (cell.MinesAround != 0)
                    continue;

                // Reveal all cells around blank cells around the current cell.
                foreach (Cell neighbor in Grid.GetNeighborsOfCell(cell)
                    .Where(n => n.State == CellState.Untouched))
                {
                    if (neighbor.MinesAround == 0)
                        cellsToReveal.Enqueue(neighbor);
                    else
                        neighbor.Reveal();
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