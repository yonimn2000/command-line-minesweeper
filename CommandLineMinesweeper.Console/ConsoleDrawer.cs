using System.Drawing;

namespace YonatanMankovich.CommandLineMinesweeper.Console
{
    internal static class ConsoleDrawer
    {
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