namespace YonatanMankovich.CommandLineMinesweeper.Core.Exceptions
{
    /// <summary>
    /// Represents errors that occur while performing operations on a Minesweeper <see cref="Core.Cell"/>.
    /// </summary>
    public class CellException : Exception
    {
        /// <summary>
        /// Gets the <see cref="Core.Cell"/> that threw the exception.
        /// </summary>
        public Cell? Cell { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CellException"/> class.
        /// </summary>
        public CellException() : base("An exception has occurred when performing an operation on a cell.") { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CellException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message.</param>
        public CellException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CellException"/> class with a specified error message 
        /// and a <see cref="Core.Cell"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="cell">The <see cref="Core.Cell"/>.</param>
        public CellException(string message, Cell cell) : base(message) => Cell = cell;

        /// <summary>
        /// Initializes a new instance of the <see cref="CellException"/> class with a <see cref="Core.Cell"/>.
        /// </summary>
        /// <param name="cell">The <see cref="Core.Cell"/>.</param>
        public CellException(Cell cell)
            : this($"An exception has occurred when performing an operation on a cell at {cell.Coordinates}.", cell) { }
    }
}