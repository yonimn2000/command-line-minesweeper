using System.Drawing;
using YonatanMankovich.CommandLineMinesweeper.Core.Enums;
using YonatanMankovich.CommandLineMinesweeper.Core.Exceptions;

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

        public bool IsValidForMove()
        {
            return State == CellState.Untouched || State == CellState.Flagged;
        }

        public bool IsValidForReveal()
        {
            return State == CellState.Untouched;
        }

        internal void IncrementMinesAround() => MinesAround++;

        internal void Reveal()
        {
            if (State == CellState.Revealed)
                throw new RevealRevealedCellException(cell: this);

            if (State == CellState.Flagged)
                throw new FlaggedCellRevealException(cell: this);

            if (IsMine)
                throw new RevealedMineException(cell: this);

            State = CellState.Revealed;
        }

        internal void PlaceFlag()
        {
            switch (State)
            {
                case CellState.Untouched: State = CellState.Flagged; break;
                case CellState.Revealed: throw new CellFlagException("Attempted to place a flag at a revealed cell.", cell: this);
                case CellState.Flagged: throw new CellFlagException("Attempted to place an already placed flag.", cell: this);
                default: throw new NotImplementedException();
            }
        }

        internal void RemoveFlag()
        {
            switch (State)
            {
                case CellState.Untouched: throw new CellFlagException("Attempted to remove an already removed flag.", cell: this);
                case CellState.Revealed: throw new CellFlagException("Attempted to remove a flag at a revealed cell.", cell: this);
                case CellState.Flagged: State = CellState.Untouched; break;
                default: throw new NotImplementedException();
            }
        }

        internal void ToggleFlag()
        {
            switch (State)
            {
                case CellState.Untouched: PlaceFlag(); break;
                case CellState.Revealed: throw new CellFlagException("Attempted to toggle a flag at a revealed cell.", cell: this);
                case CellState.Flagged: RemoveFlag(); break;
                default: throw new NotImplementedException();
            }
        }

        internal void ClearState() => State = CellState.Untouched;
    }
}