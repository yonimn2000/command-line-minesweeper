using System.Drawing;
using YonatanMankovich.CommandLineMinesweeper.Core;

namespace YonatanMankovich.CommandLineMinesweeper.Console
{
    internal static class ConsoleDrawer
    {
        public static void DrawBoard(Game game, bool showEverything = false)
        {
            for (int y = 0; y < game.Board.Grid.Height; y++)
            {
                for (int x = 0; x < game.Board.Grid.Width; x++)
                {
                    Cell cell = game.Board.Grid.GetCell(x, y);

                    if (cell.State == CellState.Flagged)
                        WriteInColor("F ", ConsoleColor.DarkGray, ConsoleColor.Black);
                    else if (cell.IsMine && showEverything)
                        WriteInColor("# ", ConsoleColor.Red, ConsoleColor.Black);
                    else if (cell.NumberOfMinesAround == 0 && (showEverything || cell.State == CellState.Revealed))
                        WriteInColor("  ", ConsoleColor.Gray, ConsoleColor.Black);
                    else if (cell.NumberOfMinesAround > 0 && (showEverything || cell.State == CellState.Revealed))
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
                        WriteInColor(cell.NumberOfMinesAround + " ", ConsoleColor.Gray, textColor);
                    }
                    else
                        WriteInColor("  ", ConsoleColor.DarkGray, ConsoleColor.White);
                }
                System.Console.WriteLine();
            }
            System.Console.WriteLine();
        }

        public static void WriteInColor(string text, ConsoleColor bgColor, ConsoleColor fgColor, bool isSelected = false)
        {
            ConsoleColor prevBgColor = System.Console.BackgroundColor;
            ConsoleColor prevFgColor = System.Console.ForegroundColor;
            if (!isSelected)
            {
                System.Console.BackgroundColor = bgColor;
                System.Console.ForegroundColor = fgColor;
            }
            else
            {
                System.Console.BackgroundColor = ConsoleColor.Yellow;
                System.Console.ForegroundColor = ConsoleColor.Black;
            }
            System.Console.Write(text);
            System.Console.BackgroundColor = prevBgColor;
            System.Console.ForegroundColor = prevFgColor;
        }

        public static void WriteTextOnLine(string text, int line)
        {
            System.Console.SetCursorPosition(0, line);
            System.Console.Write(text);
        }

        public static void ClearLine(int line)
        {
            ClearArea(new Point(0, line), new Point(System.Console.BufferWidth - 1, line));
        }

        public static void ClearArea(Point start, Point end)
        {
            for (int y = start.Y; y <= end.Y; y++)
            {
                for (int x = start.X; x <= end.X; x++)
                {
                    System.Console.CursorLeft = x;
                    System.Console.CursorTop = y;
                    System.Console.Write(' ');
                }
                System.Console.WriteLine();
            }
        }
    }
}