using System.Drawing;
using YonatanMankovich.CommandLineMinesweeper.Core;
using YonatanMankovich.CommandLineMinesweeper.Core.Enums;
using YonatanMankovich.ConsoleDiffWriter.Data;
using YonatanMankovich.ConsoleDiffWriter.Diff;

namespace YonatanMankovich.CommandLineMinesweeper.Console
{
    /// <summary>
    /// Represents a <see cref="Core.Minesweeper"/> <see cref="System.Console"/> drawer.
    /// </summary>
    internal class MinesweeperConsoleDrawer
    {
        /// <summary>
        /// Gets or sets any coordinates to highlight on the drawn grid.
        /// </summary>
        public Point? SelectedCoordinates { get; set; }

        /// <summary>
        /// Gets or sets any additional <see cref="ConsoleLines"/> to add at the end of the drawn game board.
        /// </summary>
        public ConsoleLines? AdditionalText { get; set; }

        /// <summary>
        /// The <see cref="Core.Minesweeper"/> game.
        /// </summary>
        private Minesweeper Minesweeper { get; }

        /// <summary>
        /// The structure that holds and generates the drawn character differences.
        /// </summary>
        private ConsoleDiffLines DiffLines { get; }

        /// <summary>
        /// Initializes an instance of the <see cref="MinesweeperConsoleDrawer"/> class with a 
        /// <see cref="Core.Minesweeper"/> game and an optional line number at which to draw the game.
        /// If a line number is not specified, the game will be drawn at the beginning of the current line.
        /// </summary>
        /// <param name="minesweeper">The <see cref="Core.Minesweeper"/> game.</param>
        /// <param name="line">The optional console line number from the top at which to draw the game.</param>
        public MinesweeperConsoleDrawer(Minesweeper minesweeper, int? line = null)
        {
            Minesweeper = minesweeper;
            DiffLines = new ConsoleDiffLines(new Point(0, line ?? System.Console.CursorTop));
        }

        /// <summary>
        /// Draws the background of the board more efficiently.
        /// </summary>
        public void DrawBase()
        {
            DiffLines.FillArea(new Size(Minesweeper.Grid.Width * 2, Minesweeper.Grid.Height), ConsoleColor.DarkGray);
        }

        /// <summary>
        /// Draws the current game state with the specified <see cref="MinesweeperDrawOption"/>.
        /// </summary>
        /// <param name="drawOption">The draw option.</param>
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
                    else if (drawOption == MinesweeperDrawOption.ShowMines && cell.State == CellState.Flagged && !cell.IsMine)
                    {
                        symbol = 'X';
                        backColor = ConsoleColor.DarkMagenta;
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
                        textColor = cell.MinesAround switch
                        {
                            1 => ConsoleColor.Blue,
                            2 => ConsoleColor.DarkGreen,
                            3 => ConsoleColor.Red,
                            4 => ConsoleColor.DarkBlue,
                            5 => ConsoleColor.DarkRed,
                            6 => ConsoleColor.DarkCyan,
                            7 => ConsoleColor.Black,
                            8 => ConsoleColor.DarkGray,
                            _ => throw new Exception("Impossible mines around value given.")
                        };
                        backColor = ConsoleColor.Gray;
                        symbol = (char)(cell.MinesAround + 48); // ASCII '0' is 48, '1' is 49, '2' is 50, and so on.
                    }

                    if (cell.Coordinates == SelectedCoordinates && drawOption != MinesweeperDrawOption.AllClear)
                        backColor = ConsoleColor.Yellow;

                    consoleLines.AddToEndOfLastLine(new ConsoleString(symbol + " ", textColor, backColor));
                }
                consoleLines.AddLine();
            }
            consoleLines.AddLine(new ConsoleString("Remaining mines: " + Minesweeper.GetNumberOfRemainingMines()));
            consoleLines.AddLine(new ConsoleString("Game completeness: " + (100 * Minesweeper.GetGameCompleteness()).ToString("N0") + "%"));
            consoleLines.AddLine();

            if (AdditionalText != null)
                consoleLines.AddLines(AdditionalText);

            DiffLines.WriteDiff(consoleLines);
            DiffLines.BringCursorToEnd();
        }
    }
}