namespace YonatanMankovich.CommandLineMinesweeper.Core.Enums
{
    /// <summary>
    /// Specifies the states of a Minesweeper cell.
    /// </summary>
    public enum CellState
    {
        /// <summary>
        /// The cell has not been touched by the player.
        /// </summary>
        Untouched,

        /// <summary>
        /// The cell has been revealed by the player.
        /// </summary>
        Revealed,

        /// <summary>
        /// The cell has been flagged by the player.
        /// </summary>
        Flagged
    }
}