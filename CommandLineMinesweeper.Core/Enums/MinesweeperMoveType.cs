namespace YonatanMankovich.CommandLineMinesweeper.Core.Enums
{
    /// <summary>
    /// Specifies the move types of a Minesweeper game.
    /// </summary>
    public enum MinesweeperMoveType
    {
        /// <summary>
        /// Place a flag on the cell.
        /// </summary>
        PlaceFlag,

        /// <summary>
        /// Remove a flag from the cell.
        /// </summary>
        RemoveFlag,

        /// <summary>
        /// Toggle the flag on the cell.
        /// </summary>
        ToggleFlag,

        /// <summary>
        /// Reveal the cell.
        /// </summary>
        RevealCell
    }
}