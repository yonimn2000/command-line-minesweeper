using System.Drawing;

namespace YonatanMankovich.CommandLineMinesweeper.Core
{
    public abstract class MinesweeperPlayer
    {
        public Minesweeper Minesweeper { get; set; }
        public Action<Point>? AfterMoveCallback { get; set; }
        public Action<Point, string>? AfterInvalidMoveCallback { get; set; }
        public Action<Point>? AfterSelectCallback { get; set; }
        public Action<Point>? AfterRevealedMineCallback { get; set; }
        public Action? AfterFieldClearedCallback { get; set; }

        public MinesweeperPlayer(Minesweeper minesweeper)
        {
            Minesweeper = minesweeper;
        }

        public abstract void PlayToEnd();
    }
}