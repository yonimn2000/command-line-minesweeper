using System.Drawing;
using YonatanMankovich.CommandLineMinesweeper.Core.Enums;
using YonatanMankovich.CommandLineMinesweeper.Core.Exceptions;

namespace YonatanMankovich.CommandLineMinesweeper.Core
{
    public class Minesweeper
    {
        public int TotalMines { get; }
        public CellsGrid Grid { get; }

        public Minesweeper() : this(new BoardOptions()) { }

        public Minesweeper(BoardOptions options)
        {
            TotalMines = options.GetNumberOfMines();
            Grid = new CellsGrid(options.Width, options.Height);
            PlaceRandomMines(options);
        }

        private void PlaceRandomMines(BoardOptions options)
        {
            // Shuffle all cells and take as many as the number of mines to place.
            Random random = options.RandomSeed == null ? new Random() : new Random((int)options.RandomSeed);
            foreach (Cell cell in Grid.GetAllCells().OrderBy(c => random.Next()).Take(options.GetNumberOfMines()))
            {
                cell.IsMine = true;
                foreach (Cell neighbor in Grid.GetNeighborsOfCell(cell))
                    neighbor.IncrementMinesAround();
            }
        }

        public MinesweeperMoveResult MakeMove(MinesweeperMove move) => MakeMove(move.MoveType, move.Coordinates);
        public MinesweeperMoveResult MakeMove(MinesweeperMoveType moveType, int x, int y) => MakeMove(moveType, new Point(x, y));
        public MinesweeperMoveResult MakeMove(MinesweeperMoveType moveType, Point coordinates)
        {
            try
            {
                switch (moveType)
                {
                    case MinesweeperMoveType.PlaceFlag: PlaceCellFlag(coordinates); break;
                    case MinesweeperMoveType.RemoveFlag: RemoveCellFlag(coordinates); break;
                    case MinesweeperMoveType.ToggleFlag: ToggleCellFlag(coordinates); break;
                    case MinesweeperMoveType.Reveal: RevealCell(coordinates); break;
                    default: throw new NotImplementedException();
                }
            }
            catch (RevealedMineException)
            {
                return MinesweeperMoveResult.RevealedMine;
            }
            catch (CellException ce) when (ce is FlaggedCellRevealException || ce is RevealRevealedCellException)
            {
                return MinesweeperMoveResult.InvalidMove;
            }

            if (IsFieldClear())
                return MinesweeperMoveResult.AllClear;

            return MinesweeperMoveResult.Playing;
        }

        private void PlaceCellFlag(Point point)
        {
            Grid.GetCell(point).PlaceFlag();
        }

        private void RemoveCellFlag(Point point)
        {
            Grid.GetCell(point).RemoveFlag();
        }

        private void ToggleCellFlag(Point point)
        {
            Grid.GetCell(point).ToggleFlag();
        }

        private void RevealCell(Point point)
        {
            Cell selectedCell = Grid.GetCell(point);
            selectedCell.Reveal();

            // Expand blank area around blank cell.
            if (selectedCell.MinesAround == 0)
            {
                Queue<Cell> cellsToReveal = new Queue<Cell>();
                cellsToReveal.Enqueue(selectedCell);

                while (cellsToReveal.Count > 0)
                {
                    Cell cell = cellsToReveal.Dequeue();

                    // For each untouched neighbor of the current cell...
                    foreach (Cell neighbor in Grid.GetNeighborsOfCell(cell)
                        .Where(n => n.State == CellState.Untouched))
                    {
                        // Reveal neighbor and enqueue if blank.
                        neighbor.Reveal();

                        if (neighbor.MinesAround == 0)
                            cellsToReveal.Enqueue(neighbor);
                    }
                }
            }
        }

        public int CountFlags()
        {
            return Grid.GetAllCells().Count(c => c.State == CellState.Flagged);
        }

        public bool IsFieldClear()
        {
            return !Grid.GetAllCells().Any(c => c.State != CellState.Revealed && !c.IsMine);
        }

        public int GetNumberOfRemainingMines() => IsFieldClear() ? 0 : TotalMines - CountFlags();

        public double GetGameCompleteness()
            => IsFieldClear() ? 1 : 1 - ((double)GetUntouchedCells().Count() / (Grid.Width * Grid.Height));

        public IEnumerable<Cell> GetUntouchedCells() => Grid.GetAllCells().Where(c => c.State == CellState.Untouched);

        public void Reset()
        {
            foreach (Cell cell in Grid.GetAllCells())
                cell.ClearState();
        }
    }
}