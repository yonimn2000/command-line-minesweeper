namespace YonatanMankovich.CommandLineMinesweeper.Core.Enums
{
    /// <summary>
    /// Specifies the Minesweeper game move results. 
    /// </summary>
    public enum MinesweeperMoveResult
    {
        /// <summary>
        /// The game is still going on.
        /// </summary>
        Playing,

        /// <summary>
        /// The player has revealed a mine.
        /// </summary>
        RevealedMine,

        /// <summary>
        /// The player made an invalid move.
        /// </summary>
        InvalidMove,

        /// <summary>
        /// The mine field has been cleared.
        /// </summary>
        AllClear
    }
}