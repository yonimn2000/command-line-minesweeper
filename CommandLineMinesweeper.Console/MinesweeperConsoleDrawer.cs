using System.Drawing;
using YonatanMankovich.CommandLineMinesweeper.Core;
using YonatanMankovich.CommandLineMinesweeper.Core.Enums;

namespace YonatanMankovich.CommandLineMinesweeper.Console
{
    internal class MinesweeperConsoleDrawer
    {
        public int? ConsoleLine { get; set; }
        public Point? HitMineCoordinates { get; set; }
        public Point? SelectedCoordinates { get; set; }
        private Minesweeper Minesweeper { get; }

        public MinesweeperConsoleDrawer(Minesweeper minesweeper)
        {
            Minesweeper = minesweeper;
        }

        public void Draw(MinesweeperDrawOption drawOption = MinesweeperDrawOption.Normal)
        {
            if (ConsoleLine != null)
                System.Console.CursorTop = (int)ConsoleLine;

            if (drawOption == MinesweeperDrawOption.ShowHitMine && HitMineCoordinates == null)
                throw new InvalidOperationException("HitMineCoordinates must be set before showing its location.");

            for (int y = 0; y < Minesweeper.Grid.Height; y++)
            {
                for (int x = 0; x < Minesweeper.Grid.Width; x++)
                {
                    Cell cell = Minesweeper.Grid.GetCell(x, y);
                    char symbol = ' ';
                    ConsoleColor textColor = ConsoleColor.Black;
                    ConsoleColor backColor = ConsoleColor.DarkGray;

                    if (cell.IsMine && drawOption == MinesweeperDrawOption.AllClear)
                    {
                        symbol = '#';
                        backColor = ConsoleColor.Green;
                    }
                    else if (cell.IsMine && cell.Coordinates == HitMineCoordinates && drawOption == MinesweeperDrawOption.ShowHitMine)
                    {
                        symbol = '#';
                        backColor = ConsoleColor.Yellow;
                    }
                    else if (cell.State == CellState.Flagged)
                    {
                        symbol = 'F';
                        backColor = ConsoleColor.Magenta;
                    }
                    else if (cell.IsMine && (drawOption == MinesweeperDrawOption.ShowEverything || drawOption == MinesweeperDrawOption.ShowHitMine))
                    {
                        symbol = '#';
                        backColor = ConsoleColor.Red;
                    }
                    else if (cell.MinesAround == 0 && (drawOption == MinesweeperDrawOption.ShowEverything || cell.State == CellState.Revealed))
                    {
                        backColor = ConsoleColor.Gray;
                    } else if (cell.MinesAround > 0 && (drawOption == MinesweeperDrawOption.ShowEverything || cell.State == CellState.Revealed))
                    {
                        switch (cell.MinesAround)
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
                        backColor = ConsoleColor.Gray;
                        symbol = (char)(cell.MinesAround + 48); // ASCII '0' is 48, '1' is 49, and so on.
                    }
                    else if (cell.Coordinates == SelectedCoordinates)
                    {
                        backColor = ConsoleColor.Yellow;
                    }

                    ConsoleDrawer.WriteInColor(symbol + " ", backColor, textColor);
                }
                System.Console.WriteLine();
            }
            System.Console.WriteLine();
        }
    }
}