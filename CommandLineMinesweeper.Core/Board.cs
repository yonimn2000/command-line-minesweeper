namespace YonatanMankovich.CommandLineMinesweeper.Core
{
    internal class Board
    {
        public CellsGrid Grid { get; }

        public Board(BoardOptions options)
        {
            // Create a grid of untouched cells.
            Grid = new CellsGrid(options.Width, options.Height);

            // Place mines on board randomly.
            Random random = new Random();
            int numberOfMinesToPlace = options.GetNumberOfMines();
            for (int numberOfMinesPlaced = 0; numberOfMinesPlaced < numberOfMinesToPlace; numberOfMinesPlaced++)
            {
                int x = random.Next(options.Width);
                int y = random.Next(options.Height);
                Cell cell = Grid.GetCell(x, y);

                if (cell.IsMine) // If there is already a mine on the cell,
                    numberOfMinesPlaced--; // Ignore the current iteration.
                else
                {
                    cell.IsMine = true;
                    foreach (Cell neighbor in Grid.GetNeighboringCells(x, y))
                        neighbor.NumberOfMinesAround++;
                }
            }
        }

        public bool RevealCell(int x, int y)
        {
            Queue<Cell> cellsToReveal = new Queue<Cell>();
            cellsToReveal.Enqueue(Grid.GetCell(x, y));

            while (cellsToReveal.Count > 0)
            {
                Cell cell = cellsToReveal.Dequeue();
                if (cell.Reveal())
                    return true;

                if (cell.NumberOfMinesAround == 0)
                    foreach (Cell neighbor in Grid.GetNeighboringCells(cell.Coordinates.X, cell.Coordinates.Y)
                        .Where(n => !n.IsMine && n.State == CellState.Untouched))
                    {
                        if (neighbor.NumberOfMinesAround == 0)
                            cellsToReveal.Enqueue(neighbor);
                        else
                            neighbor.Reveal();
                    }
            }

            return Grid.GetCell(x, y).Reveal();
        }

        public int CountFlags()
        {
            return Grid.GetAllCells().Count(c => c.State == CellState.Flagged);
        }

        public bool IsFieldClear()
        {
            return Grid.GetAllCells().Any(c => c.State != CellState.Revealed && !c.IsMine);
        }
    }
}