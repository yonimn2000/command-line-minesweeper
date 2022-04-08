namespace YonatanMankovich.CommandLineMinesweeper.Core.Exceptions
{
    public class FlaggedCellRevealException : CellException
    {
        public FlaggedCellRevealException() : base("Attempted to reveal a flagged cell.") { }

        public FlaggedCellRevealException(Cell cell) : base("Attempted to reveal a flagged cell at " + cell.Coordinates, cell) { }
    }
}