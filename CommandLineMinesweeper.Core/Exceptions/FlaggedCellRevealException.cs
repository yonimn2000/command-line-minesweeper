namespace YonatanMankovich.CommandLineMinesweeper.Core.Exceptions
{
    /// <summary>
    /// Represents errors that occur while revealing a flagged Minesweeper <see cref="Cell"/>.
    /// </summary>
    public class FlaggedCellRevealException : CellException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FlaggedCellRevealException"/> class.
        /// </summary>
        public FlaggedCellRevealException() : base("Attempted to reveal a flagged cell.") { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FlaggedCellRevealException"/> class with a <see cref="Cell"/>.
        /// </summary>
        /// <param name="cell">The <see cref="Cell"/>.</param>
        public FlaggedCellRevealException(Cell cell) : base("Attempted to reveal a flagged cell at " + cell.Coordinates, cell) { }
    }
}