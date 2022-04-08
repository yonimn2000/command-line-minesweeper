namespace YonatanMankovich.CommandLineMinesweeper.Core.Exceptions
{
    public class CellFlagException : CellException
    {
        public CellFlagException() : base("Attempted to perform an invalid flag operation on a cell.") { }

        public CellFlagException(string message, Cell cell) : base(message, cell) { }

        public CellFlagException(Cell cell)
            : base("Attempted to perform an invalid flag operation on a cell at " + cell.Coordinates, cell) { }
    }
}