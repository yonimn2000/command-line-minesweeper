using System.Drawing;

namespace YonatanMankovich.CommandLineMinesweeper.Core
{
    internal class Cell
    {
        public Point Coordinates { get; set; }
        public CellState State { get; set; } = CellState.Untouched;
        public bool IsMine { get; set; } = false;
        public int NumberOfMinesAround { get; set; } = 0;

        public Cell(Point coordinates)
        {
            Coordinates = coordinates;
        }

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