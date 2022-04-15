namespace YonatanMankovich.CommandLineMinesweeper.Core.Exceptions
{
    /// <summary>
    /// Represents errors that occur while performing flag operations on a Minesweeper <see cref="Cell"/>.
    /// </summary>
    public class CellFlagException : CellException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CellFlagException"/> class.
        /// </summary>
        public CellFlagException() : base("Attempted to perform an invalid flag operation on a cell.") { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CellFlagException"/> class with a specified error message 
        /// and a <see cref="Cell"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="cell">The <see cref="Cell"/>.</param>
        public CellFlagException(string message, Cell cell) : base(message, cell) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CellFlagException"/> class with a <see cref="Cell"/>.
        /// </summary>
        /// <param name="cell">The <see cref="Cell"/>.</param>
        public CellFlagException(Cell cell)
            : base("Attempted to perform an invalid flag operation on a cell at " + cell.Coordinates, cell) { }
    }
}