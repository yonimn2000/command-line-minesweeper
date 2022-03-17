using System.Drawing;
using YonatanMankovich.CommandLineMinesweeper.Core.Enums;

namespace YonatanMankovich.CommandLineMinesweeper.Core
{
    public class Cell
    {
        public Point Coordinates { get; }
        public CellState State { get; private set; }
        public bool IsMine { get; internal set; }
        public int MinesAround { get; private set; }

        internal Cell(Point coordinates)
        {
            Coordinates = coordinates;
            State = CellState.Untouched;
            IsMine = false;
            MinesAround = 0;
        }

        internal void IncrementMinesAround() => MinesAround++;

        internal CellRevealResult Reveal()
        {
            if (State == CellState.Flagged || State == CellState.Revealed)
                return CellRevealResult.Invalid;

            if (IsMine)
                return CellRevealResult.Mine;

            State = CellState.Revealed;
            return CellRevealResult.Clear;
        }

        internal void ToggleFlag()
        {
            if (State == CellState.Untouched)
                State = CellState.Flagged;
            else if (State == CellState.Flagged)
                State = CellState.Untouched;
        }
    }
}