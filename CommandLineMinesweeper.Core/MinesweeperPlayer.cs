using System.Drawing;

namespace YonatanMankovich.CommandLineMinesweeper.Core
{
    /// <summary>
    /// Represents an abstract <see cref="Core.Minesweeper"/> player.
    /// </summary>
    public abstract class MinesweeperPlayer
    {
        /// <summary>
        /// The <see cref="Core.Minesweeper"/> game the current <see cref="MinesweeperPlayer"/> plays.
        /// </summary>
        public Minesweeper Minesweeper { get; set; }

        /// <summary>
        /// The action to call after the current <see cref="MinesweeperPlayer"/> makes a move.
        /// Passes the move coordinates.
        /// </summary>
        public Action<Point?>? AfterMoveCallback { get; set; }

        /// <summary>
        /// The action to call after the current <see cref="MinesweeperPlayer"/> makes an invalid move.
        /// Passes the attempted move coordinates.
        /// </summary>
        public Action<Point, string>? AfterInvalidMoveCallback { get; set; }

        /// <summary>
        /// The action to call after the current <see cref="MinesweeperPlayer"/> makes a cell selection.
        /// Passes the selected coordinates.
        /// </summary>
        public Action<Point>? AfterCellSelectedCallback { get; set; }

        /// <summary>
        /// The action to call after the current <see cref="MinesweeperPlayer"/> reveals a mine, losing the game.
        /// Passes the losing move coordinates.
        /// </summary>
        public Action<Point>? AfterRevealedMineCallback { get; set; }

        /// <summary>
        /// The action to call after the current <see cref="MinesweeperPlayer"/> clears the entire field, 
        /// winning the game.
        /// </summary>
        public Action? AfterFieldClearedCallback { get; set; }

        /// <summary>
        /// Initializes an instance of the <see cref="MinesweeperPlayer"/> class with an instance of the
        /// <see cref="Core.Minesweeper"/> game class.
        /// </summary>
        /// <param name="minesweeper">The <see cref="Core.Minesweeper"/> game.</param>
        public MinesweeperPlayer(Minesweeper minesweeper) => Minesweeper = minesweeper;

        /// <summary>
        /// Plays the <see cref="Minesweeper"/> game till the end, that is, until the player
        /// either wins, loses, or quits the game.
        /// </summary>
        public abstract void PlayToEnd();
    }
}