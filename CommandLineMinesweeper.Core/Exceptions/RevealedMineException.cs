namespace YonatanMankovich.CommandLineMinesweeper.Core.Exceptions
{
    /// <summary>
    /// Represents errors that occur while revealing a mine Minesweeper <see cref="Cell"/>.
    /// </summary>
    public class RevealedMineException : CellException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RevealedMineException"/> class.
        /// </summary>
        public RevealedMineException() : base("Revealed a mine.") { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RevealedMineException"/> class with a <see cref="Cell"/>.
        /// </summary>
        /// <param name="cell">The <see cref="Cell"/>.</param>
        public RevealedMineException(Cell cell) : base("Revealed a mine at " + cell.Coordinates, cell) { }
    }
}