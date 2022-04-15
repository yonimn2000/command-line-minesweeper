namespace YonatanMankovich.CommandLineMinesweeper.Core.Exceptions
{
    /// <summary>
    /// Represents errors that occur while revealing an already revealed Minesweeper <see cref="Cell"/>.
    /// </summary>
    public class RevealRevealedCellException : CellException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RevealRevealedCellException"/> class.
        /// </summary>
        public RevealRevealedCellException() : base("Attempted to reveal an already revealed cell.") { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RevealRevealedCellException"/> class with a <see cref="Cell"/>.
        /// </summary>
        /// <param name="cell">The <see cref="Cell"/>.</param>
        public RevealRevealedCellException(Cell cell)
            : base("Attempted to reveal an already revealed cell at " + cell.Coordinates, cell) { }
    }
}