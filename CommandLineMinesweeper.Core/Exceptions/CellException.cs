namespace YonatanMankovich.CommandLineMinesweeper.Core.Exceptions
{
    public class CellException : Exception
    {
        public Cell Cell { get; }

        public CellException() : base("An exception has occurred when performing an operation on a cell.") { }

        public CellException(string message) : base(message) { }

        public CellException(string message, Cell cell) : base(message)
        {
            Cell = cell;
        }

        public CellException(Cell cell)
            : this($"An exception has occurred when performing an operation on a cell at {cell.Coordinates}.", cell) { }
    }
}