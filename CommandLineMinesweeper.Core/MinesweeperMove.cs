using System.Drawing;
using YonatanMankovich.CommandLineMinesweeper.Core.Enums;

namespace YonatanMankovich.CommandLineMinesweeper.Core
{
    /// <summary>
    /// Represents a Minesweeper game move.
    /// </summary>
    public class MinesweeperMove
    {
        /// <summary>
        /// Gets or sets the move coordinates.
        /// </summary>
        public Point Coordinates { get; set; }

        /// <summary>
        /// Gets or sets the move type.
        /// </summary>
        public MinesweeperMoveType MoveType { get; set; }

        /// <summary>
        /// Initializes an instance of the <see cref="MinesweeperMove"/> class with a <paramref name="moveType"/>
        /// and <paramref name="coordinates"/>.
        /// </summary>
        /// <param name="moveType">The move type.</param>
        /// <param name="coordinates">The coordinates.</param>
        public MinesweeperMove(MinesweeperMoveType moveType, Point coordinates)
        {
            MoveType = moveType;
            Coordinates = coordinates;
        }
    }
}