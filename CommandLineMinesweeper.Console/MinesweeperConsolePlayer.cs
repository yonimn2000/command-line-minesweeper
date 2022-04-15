using System.Drawing;
using YonatanMankovich.CommandLineMinesweeper.Core;
using YonatanMankovich.CommandLineMinesweeper.Core.Enums;

namespace YonatanMankovich.CommandLineMinesweeper.Console
{
    /// <summary>
    /// Represents a console-based <see cref="Minesweeper"/> game player.
    /// </summary>
    public class MinesweeperConsolePlayer : MinesweeperPlayer
    {
        private enum MoveDirection { Left, Right, Down, Up }

        /// <summary>
        /// The array of keys the user can press for actions.
        /// </summary>
        private ConsoleKey[] AllowedKeys { get; } = new ConsoleKey[]
        {
            ConsoleKey.F, ConsoleKey.R, ConsoleKey.Q, // F = toggle flag; R = Reveal; Q = quit;
            ConsoleKey.LeftArrow, ConsoleKey.RightArrow, // Arrows are for selecting grid cells.
            ConsoleKey.DownArrow, ConsoleKey.UpArrow
        };

        /// <summary>
        /// Initializes an instance of the <see cref="MinesweeperConsolePlayer"/> class with an instance of the
        /// <see cref="Minesweeper"/> game class.
        /// </summary>
        /// <param name="minesweeper">The <see cref="Minesweeper"/> game.</param>
        public MinesweeperConsolePlayer(Minesweeper minesweeper) : base(minesweeper) { }

        /// <inheritdoc/>
        public override void PlayToEnd()
        {
            MinesweeperMoveResult lastMoveResult = MinesweeperMoveResult.Playing;
            Point lastSelectedPoint = default;
            while (lastMoveResult != MinesweeperMoveResult.RevealedMine && lastMoveResult != MinesweeperMoveResult.AllClear)
            {
                MinesweeperMove? move = GetUserMove(lastSelectedPoint);

                if (move == null) // Player quit.
                    break;

                lastMoveResult = Minesweeper.MakeMove(move);
                lastSelectedPoint = move.Coordinates;

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

        /// <summary>
        /// Gets a move from a user starting at a <paramref name="selectedPoint"/>.
        /// </summary>
        /// <param name="selectedPoint">The currently selected point on the grid.</param>
        /// <returns>A <see cref="MinesweeperMove"/> that corresponds to the user action.</returns>
        /// <exception cref="NotImplementedException"></exception>
        private MinesweeperMove? GetUserMove(Point selectedPoint)
        {
            do
            {
                AfterCellSelectedCallback?.Invoke(selectedPoint);
                switch (GetValidUserKey())
                {
                    case ConsoleKey.F:
                        return new MinesweeperMove(MinesweeperMoveType.ToggleFlag, selectedPoint);

                    case ConsoleKey.R:
                        return new MinesweeperMove(MinesweeperMoveType.RevealCell, selectedPoint);

                    case ConsoleKey.Q:
                        return null;

                    case ConsoleKey.RightArrow: selectedPoint = MoveSelectedPoint(selectedPoint, MoveDirection.Right); break;
                    case ConsoleKey.LeftArrow: selectedPoint = MoveSelectedPoint(selectedPoint, MoveDirection.Left); break;
                    case ConsoleKey.UpArrow: selectedPoint = MoveSelectedPoint(selectedPoint, MoveDirection.Up); break;
                    case ConsoleKey.DownArrow: selectedPoint = MoveSelectedPoint(selectedPoint, MoveDirection.Down); break;

                    default: throw new NotImplementedException();
                }
            } while (true);
        }

        /// <summary>
        /// Gets a valid <see cref="ConsoleKey"/> from the user. Validity is based on the <see cref="AllowedKeys"/> array.
        /// </summary>
        /// <returns>A valid <see cref="ConsoleKey"/> from the user.</returns>
        private ConsoleKey GetValidUserKey()
        {
            ConsoleKey lastKey;
            do
            {
                lastKey = System.Console.ReadKey(intercept: true).Key;
            } while (Array.IndexOf(AllowedKeys, lastKey) < 0); // While key not allowed.
            return lastKey;
        }

        /// <summary>
        /// Moves the <paramref name="selectedPoint"/> to the next available cell to the direction of 
        /// the <paramref name="offset"/>. Returns the same point if no move is available in the chosen direction.
        /// </summary>
        /// <param name="selectedPoint">The currently selected point on the grid.</param>
        /// <param name="offset">The direction to which to move the selected point.</param>
        /// <returns>A new <see cref="Point"/> which is valid for the next user move.</returns>
        private Point MoveSelectedPoint(Point selectedPoint, MoveDirection direction)
        {
            Point? possibleCoordinates = GetPossibleCoordinates(selectedPoint, direction switch
            {
                MoveDirection.Left => c => c.Coordinates.X < selectedPoint.X,
                MoveDirection.Right => c => c.Coordinates.X > selectedPoint.X,
                MoveDirection.Down => c => c.Coordinates.Y > selectedPoint.Y,
                MoveDirection.Up => c => c.Coordinates.Y < selectedPoint.Y,
                _ => throw new NotImplementedException() // Default case.
            });

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