namespace YonatanMankovich.CommandLineMinesweeper.Core
{
    public class Board
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
                Cell cell = Grid.GetCell(x, y)!;

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
    }
}