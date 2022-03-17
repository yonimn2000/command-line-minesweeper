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

                    if (cell.IsMine && drawOption == MinesweeperDrawOption.AllClear)
                        ConsoleDrawer.WriteInColor("# ", ConsoleColor.Green, ConsoleColor.Black);
                    else if (cell.IsMine && cell.Coordinates == HitMineCoordinates && drawOption == MinesweeperDrawOption.ShowHitMine)
                        ConsoleDrawer.WriteInColor("# ", ConsoleColor.Yellow, ConsoleColor.Black);
                    else if (cell.State == CellState.Flagged)
                        ConsoleDrawer.WriteInColor("F ", ConsoleColor.Magenta, ConsoleColor.Black);
                    else if (cell.IsMine && (drawOption == MinesweeperDrawOption.ShowEverything || drawOption == MinesweeperDrawOption.ShowHitMine))
                        ConsoleDrawer.WriteInColor("# ", ConsoleColor.Red, ConsoleColor.Black);
                    else if (cell.MinesAround == 0 && (drawOption == MinesweeperDrawOption.ShowEverything || cell.State == CellState.Revealed))
                        ConsoleDrawer.WriteInColor("  ", ConsoleColor.Gray, ConsoleColor.Black);
                    else if (cell.MinesAround > 0 && (drawOption == MinesweeperDrawOption.ShowEverything || cell.State == CellState.Revealed))
                    {
                        ConsoleColor textColor = ConsoleColor.White;
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
                        ConsoleDrawer.WriteInColor(cell.MinesAround + " ", ConsoleColor.Gray, textColor);
                    }
                    else if (cell.Coordinates == SelectedCoordinates)
                        ConsoleDrawer.WriteInColor("  ", ConsoleColor.Yellow, ConsoleColor.White);
                    else
                        ConsoleDrawer.WriteInColor("  ", ConsoleColor.DarkGray, ConsoleColor.White);
                }
                System.Console.WriteLine();
            }
            System.Console.WriteLine();
        }
    }
}