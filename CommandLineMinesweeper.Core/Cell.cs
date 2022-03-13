namespace YonatanMankovich.CommandLineMinesweeper.Core
{
    public class Cell
    {
        public CellState State { get; set; } = CellState.Untouched;
        public bool IsMine { get; set; } = false;
        public int NumberOfMinesAround { get; set; } = 0;

        public bool Reveal() // True if revealed a mine.
        {
            if (State == CellState.Untouched)
            {
                if (IsMine)
                    return true;

                State = CellState.Revealed;
            }

            return false;
        }

        public void ToggleFlag()
        {
            if (State == CellState.Untouched)
                State = CellState.Flagged;
            else if (State == CellState.Flagged)
                State = CellState.Untouched;
        }
    }
}