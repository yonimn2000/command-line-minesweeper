using System.Drawing;
using YonatanMankovich.CommandLineMinesweeper.Core;
using YonatanMankovich.CommandLineMinesweeper.Core.Enums;
using YonatanMankovich.ConsoleDiffWriter.Data;
using YonatanMankovich.ConsoleDiffWriter.Diff;

namespace YonatanMankovich.CommandLineMinesweeper.Console
{
    internal class MinesweeperConsoleDrawer
    {
        public Point? SelectedCoordinates { get; set; }
        private Minesweeper Minesweeper { get; }
        private ConsoleDiffLines DiffLines { get; }

        public MinesweeperConsoleDrawer(Minesweeper minesweeper, int line = 0)
        {
            Minesweeper = minesweeper;
            DiffLines = new ConsoleDiffLines(new Point(0, line));
            DiffLines.FillArea(new Size(Minesweeper.Grid.Width * 2, Minesweeper.Grid.Height), ConsoleColor.DarkGray);
        }

        public void Draw(MinesweeperDrawOption drawOption = MinesweeperDrawOption.Normal)
        {
            ConsoleLines consoleLines = new ConsoleLines();

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
                        symbol = 'F';
                        backColor = ConsoleColor.Green;
                    }
                    else if (cell.State == CellState.Flagged)
                    {
                        symbol = 'F';
                        backColor = ConsoleColor.Magenta;
                    }
                    else if (cell.IsMine && (drawOption == MinesweeperDrawOption.ShowEverything || drawOption == MinesweeperDrawOption.ShowMines))
                    {
                        symbol = '#';
                        backColor = ConsoleColor.Red;
                    }
                    else if (cell.MinesAround == 0 && (drawOption == MinesweeperDrawOption.ShowEverything || cell.State == CellState.Revealed))
                    {
                        backColor = ConsoleColor.Gray;
                    }
                    else if (cell.MinesAround > 0 && (drawOption == MinesweeperDrawOption.ShowEverything || cell.State == CellState.Revealed))
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

                    if (cell.Coordinates == SelectedCoordinates && drawOption != MinesweeperDrawOption.AllClear)
                    {
                        backColor = ConsoleColor.Yellow;
                    }

                    consoleLines.AddToEndOfLastLine(new ConsoleString(symbol + " ", textColor, backColor));
                }
                consoleLines.AddLine();
            }
            consoleLines.AddLine(new ConsoleString("Remaining mines: " + Minesweeper.GetNumberOfRemainingMines()));
            consoleLines.AddLine(new ConsoleString("Game completeness: " + (100 * Minesweeper.GetGameCompleteness()).ToString("N0") + "%"));
            consoleLines.AddLine();
            DiffLines.WriteDiff(consoleLines);
            DiffLines.BringCursorToEnd();
        }
    }
}