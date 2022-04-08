namespace YonatanMankovich.CommandLineMinesweeper.Core.Exceptions
{
    public class RevealRevealedCellException : CellException
    {
        public RevealRevealedCellException() : base("Attempted to reveal an already revealed cell.") { }

        public RevealRevealedCellException(Cell cell)
            : base("Attempted to reveal an already revealed cell at " + cell.Coordinates, cell) { }
    }
}