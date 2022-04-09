using System.Drawing;
using YonatanMankovich.CommandLineMinesweeper.Core.Enums;

namespace YonatanMankovich.CommandLineMinesweeper.Core
{
    public class MinesweeperMove
    {
        public Point Coordinates { get; set; }
        public MinesweeperMoveType MoveType { get; set; }

        public MinesweeperMove(MinesweeperMoveType moveType, int x, int y) : this(moveType, new Point(x, y)) { }

        public MinesweeperMove(MinesweeperMoveType moveType, Point coordinates)
        {
            MoveType = moveType;
            Coordinates = coordinates;
        }
    }
}