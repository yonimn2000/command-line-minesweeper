using System.Drawing;
using YonatanMankovich.CommandLineMinesweeper.Core;
using YonatanMankovich.CommandLineMinesweeper.Core.Enums;

namespace YonatanMankovich.CommandLineMinesweeper.Console
{
    public class MinesweeperConsolePlayer : MinesweeperPlayer
    {
        private Point LastSelectedPoint { get; set; } = default;
        private ConsoleKey[] AllowedKeys { get; } = new ConsoleKey[]
        {
            ConsoleKey.F, ConsoleKey.R,
            ConsoleKey.LeftArrow, ConsoleKey.RightArrow,
            ConsoleKey.DownArrow, ConsoleKey.UpArrow
        };

        public MinesweeperConsolePlayer(Minesweeper minesweeper) : base(minesweeper) { }

        public override void PlayToEnd()
        {
            MinesweeperMoveResult lastMoveResult = MinesweeperMoveResult.Playing;
            Point lastMoveCoordinates = default;
            while (lastMoveResult != MinesweeperMoveResult.RevealedMine && lastMoveResult != MinesweeperMoveResult.AllClear)
            {
                MinesweeperMove move = GetUserMove();
                lastMoveCoordinates = move.Coordinates;
                lastMoveResult = Minesweeper.MakeMove(move);

                switch (lastMoveResult)
                {
                    case MinesweeperMoveResult.RevealedMine: AfterRevealedMineCallback?.Invoke(move.Coordinates); break;
                    case MinesweeperMoveResult.AllClear: AfterFieldClearedCallback?.Invoke(); break;
                    case MinesweeperMoveResult.Playing: AfterMoveCallback?.Invoke(move.Coordinates); break;
                    case MinesweeperMoveResult.InvalidMove: AfterInvalidMoveCallback?.Invoke(move.Coordinates, "Invalid move..."); break;
                    default: throw new NotImplementedException();
                }
            }
        }

        public MinesweeperMove GetUserMove()
        {
            do
            {
                Point selectedPoint = LastSelectedPoint;

                AfterSelectCallback?.Invoke(selectedPoint);
                switch (GetValidUserKey())
                {
                    case ConsoleKey.F:
                        return new MinesweeperMove(MinesweeperMoveType.ToggleFlag, selectedPoint);

                    case ConsoleKey.R:
                        return new MinesweeperMove(MinesweeperMoveType.RevealCell, selectedPoint);

                    case ConsoleKey.RightArrow: selectedPoint = MoveSelectedPoint(selectedPoint, new Point(1, 0)); break;
                    case ConsoleKey.LeftArrow: selectedPoint = MoveSelectedPoint(selectedPoint, new Point(-1, 0)); break;
                    case ConsoleKey.UpArrow: selectedPoint = MoveSelectedPoint(selectedPoint, new Point(0, -1)); break;
                    case ConsoleKey.DownArrow: selectedPoint = MoveSelectedPoint(selectedPoint, new Point(0, 1)); break;

                    default: throw new NotImplementedException();
                }
                LastSelectedPoint = selectedPoint;
            } while (true);
        }

        private ConsoleKey GetValidUserKey()
        {
            ConsoleKey lastKey;
            do
            {
                lastKey = System.Console.ReadKey(intercept: true).Key;
            } while (Array.IndexOf(AllowedKeys, lastKey) < 0); // While key not allowed.
            return lastKey;
        }

        private Point MoveSelectedPoint(Point selectedPoint, Point offset)
        {
            Point? possibleCoordinates = null;

            if (offset.Y > 0)      // Moving down.
                possibleCoordinates = GetPossibleCoordinates(selectedPoint, c => c.Coordinates.Y > selectedPoint.Y);
            else if (offset.Y < 0) // Moving up.
                possibleCoordinates = GetPossibleCoordinates(selectedPoint, c => c.Coordinates.Y < selectedPoint.Y);
            else if (offset.X > 0) // Moving right.
                possibleCoordinates = GetPossibleCoordinates(selectedPoint, c => c.Coordinates.X > selectedPoint.X);
            else if (offset.X < 0) // Moving left.
                possibleCoordinates = GetPossibleCoordinates(selectedPoint, c => c.Coordinates.X < selectedPoint.X);

            return possibleCoordinates ?? selectedPoint; // If no move available, return the same point.
        }

        /// <summary>
        /// Gets the closest point to the given point.
        /// </summary>
        /// <param name="point">A point.</param>
        /// <param name="cellLimitingPredicate">A predicate that limits the looked-up cells.</param>
        /// <returns></returns>
        private Point? GetPossibleCoordinates(Point point, Func<Cell, bool> cellLimitingPredicate)
        {
            return Minesweeper.Grid.GetAllCells()
                    .Where(c => c.IsValidForMove())
                    .Where(cellLimitingPredicate)
                    .MinBy(c => Math.Sqrt( // Order by the smallest hypotenuse between the points.
                        Math.Pow(point.X - c.Coordinates.X, 2) +
                        Math.Pow(point.Y - c.Coordinates.Y, 2)))?.Coordinates;
        }
    }
}