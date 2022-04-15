using System.Drawing;
using YonatanMankovich.CommandLineMinesweeper.Core.Enums;
using YonatanMankovich.CommandLineMinesweeper.Core.Exceptions;

namespace YonatanMankovich.CommandLineMinesweeper.Core
{
    /// <summary>
    /// Represents a Minesweeper cell.
    /// </summary>
    public class Cell
    {
        /// <summary>
        /// Gets the coordinates of the cell.
        /// </summary>
        public Point Coordinates { get; }

        /// <summary>
        /// Gets the state of the cell.
        /// </summary>
        public CellState State { get; private set; }

        /// <summary>
        /// Gets the value representing whether the cell is a mine or not.
        /// </summary>
        public bool IsMine { get; private set; }

        /// <summary>
        /// Gets the number of mines around the cell (the number on the cell).
        /// </summary>
        public int MinesAround { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Cell"/> class with coordinates.
        /// </summary>
        /// <param name="coordinates">The grid coordinates of the cell.</param>
        internal Cell(Point coordinates)
        {
            Coordinates = coordinates;
            State = CellState.Untouched;
            IsMine = false;
            MinesAround = 0;
        }

        /// <summary>
        /// Gets a value indicating whether the cell is valid for a player move.
        /// </summary>
        /// <returns><see langword="true"/> if the cell is untouched or flagged; <see langword="false"/> otherwise.</returns>
        public bool IsValidForMove() => State == CellState.Untouched || State == CellState.Flagged;

        /// <summary>
        /// Gets a value indicating whether the cell is valid for a player reveal.
        /// </summary>
        /// <returns><see langword="true"/> if the cell is untouched; <see langword="false"/> otherwise.</returns>
        public bool IsValidForReveal() => State == CellState.Untouched;

        /// <summary>
        /// Increments <see cref="MinesAround"/> by one.
        /// </summary>
        internal void IncrementMinesAround() => MinesAround++;

        /// <summary>
        /// Places a mine on the cell.
        /// </summary>
        internal void PlaceMine() => IsMine = true;

        /// <summary>
        /// Reveals the cell.
        /// </summary>
        /// <exception cref="RevealRevealedCellException"></exception>
        /// <exception cref="FlaggedCellRevealException"></exception>
        /// <exception cref="RevealedMineException"></exception>
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

        /// <summary>
        /// Sets the <see cref="State"/> of the cell to <see cref="CellState.Flagged"/>.
        /// </summary>
        /// <exception cref="CellFlagException"></exception>
        /// <exception cref="NotImplementedException"></exception>
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

        /// <summary>
        /// Sets the <see cref="State"/> of the cell to <see cref="CellState.Untouched"/>.
        /// </summary>
        /// <exception cref="CellFlagException"></exception>
        /// <exception cref="NotImplementedException"></exception>
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

        /// <summary>
        /// Places a flag on the cell if it is not flagged and removes the flag if it is flagged.
        /// </summary>
        /// <exception cref="CellFlagException"></exception>
        /// <exception cref="NotImplementedException"></exception>
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

        /// <summary>
        /// Clears the <see cref="State"/> by setting it to <see cref="CellState.Untouched"/>.
        /// </summary>
        internal void ClearState() => State = CellState.Untouched;
    }
}