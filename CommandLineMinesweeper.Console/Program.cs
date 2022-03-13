using YonatanMankovich.CommandLineMinesweeper.Console;
using YonatanMankovich.CommandLineMinesweeper.Core;

Console.WriteLine("Welcome to Yonatan's command line Minesweeper game!\n");

Game game = new Game();

//game.Board.Grid.GetCell(3, 5).ToggleFlag();

for (int y = 0; y < game.Board.Grid.Height; y++)
{
    for (int x = 0; x < game.Board.Grid.Width; x++)
    {
        Cell cell = game.Board.Grid.GetCell(x, y)!;

        if (cell.State == CellState.Flagged)
            ConsoleDrawer.WriteInColor("F ", ConsoleColor.DarkGray, ConsoleColor.Black);
        else if (cell.IsMine)
            ConsoleDrawer.WriteInColor("  ", ConsoleColor.Red, ConsoleColor.White);
        else if (cell.NumberOfMinesAround > 0)
        {
            ConsoleColor textColor = ConsoleColor.White;
            switch (cell.NumberOfMinesAround)
            {
                case 1: textColor = ConsoleColor.Blue; break;
                case 2: textColor = ConsoleColor.DarkGreen; break;
                case 3: textColor = ConsoleColor.Red; break;
                case 4: textColor = ConsoleColor.DarkBlue; break;
                case 5: textColor = ConsoleColor.DarkRed; break;
                case 6: textColor = ConsoleColor.DarkCyan; break;
                case 7: textColor = ConsoleColor.Black; break;
                case 8: textColor = ConsoleColor.DarkGray; break;
            }
            ConsoleDrawer.WriteInColor(cell.NumberOfMinesAround + " ", ConsoleColor.Gray, textColor);
        }
        else
            ConsoleDrawer.WriteInColor("  ", ConsoleColor.DarkGray, ConsoleColor.White);
    }
    Console.WriteLine();
}