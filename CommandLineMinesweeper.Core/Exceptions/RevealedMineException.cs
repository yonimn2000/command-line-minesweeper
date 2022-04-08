namespace YonatanMankovich.CommandLineMinesweeper.Core.Exceptions
{
    public class RevealedMineException : CellException
    {
        public RevealedMineException() : base("Revealed a mine.") { }

        public RevealedMineException(Cell cell) : base("Revealed a mine at " + cell.Coordinates, cell) { }
    }
}