namespace YonatanMankovich.CommandLineMinesweeper.Console
{
    /// <summary>
    /// Specified Minesweeper draw options.
    /// </summary>
    public enum MinesweeperDrawOption
    {
        /// <summary>
        /// Show the current board state.
        /// </summary>
        Normal,

        /// <summary>
        /// Show all numbers and mines.
        /// </summary>
        ShowEverything,

        /// <summary>
        /// Show current state and all mines.
        /// </summary>
        ShowMines,

        /// <summary>
        /// Show the winning board.
        /// </summary>
        AllClear
    }
}