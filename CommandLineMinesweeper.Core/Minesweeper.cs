using YonatanMankovich.CommandLineMinesweeper.Core.Enums;

namespace YonatanMankovich.CommandLineMinesweeper.Core
{
    public class Minesweeper
    {
        public int TotalMines { get; }
        public CellsGrid Grid => Board.Grid;
        private Board Board { get; }

        public Minesweeper() : this(new BoardOptions()) { }

        public Minesweeper(BoardOptions options)
        {
            TotalMines = options.GetNumberOfMines();
            Board = new Board(options);
        }

        public GameMoveResult RevealCell(int x, int y)
        {
            CellRevealResult cellRevealResult = Board.RevealCell(x, y);
            if (cellRevealResult == CellRevealResult.Mine)
                return GameMoveResult.RevealedMine;
            if (cellRevealResult == CellRevealResult.Invalid)
                return GameMoveResult.InvalidMove;
            if (cellRevealResult == CellRevealResult.Clear && Board.IsFieldClear())
                return GameMoveResult.AllClear;
            return GameMoveResult.Playing;
        }

        public void ToggleCellFlag(int x, int y)
        {
            Board.ToggleCellFlag(x, y);
        }

        public int GetNumberOfRemainingMines() => TotalMines - Board.CountFlags();

        public IEnumerable<Cell> GetUntouchedCells()
        {
            return Grid.GetAllCells().Where(c => c.State == CellState.Untouched);
        }

        public ISet<Cell> GetSureMineCells()
        {
            ISet<Cell> mineCells = new HashSet<Cell>();

            foreach (Cell cell in GetAllRevealedNumberedCells())
            {
                List<Cell> neighbors = Grid.GetNeighborsOfCell(cell);
                IList<Cell> untouchedNeighbors = neighbors.Where(n => n.State == CellState.Untouched).ToList();
                int flaggedNeighbors = neighbors.Count(n => n.State == CellState.Flagged);
                if (untouchedNeighbors.Count + flaggedNeighbors == cell.MinesAround)
                    foreach (Cell neighbor in untouchedNeighbors)
                        mineCells.Add(neighbor);
            }

            return mineCells;
        }

        public ISet<Cell> GetSureClearCells()
        {
            ISet<Cell> clearCells = new HashSet<Cell>();

            foreach (Cell cell in GetAllRevealedNumberedCells())
            {
                List<Cell> neighbors = Grid.GetNeighborsOfCell(cell);
                if (neighbors.Count(n => n.State == CellState.Flagged) == cell.MinesAround)
                    foreach (Cell neighbor in neighbors.Where(n => n.State == CellState.Untouched))
                        clearCells.Add(neighbor);
            }

            return clearCells;
        }

        private IEnumerable<Cell> GetAllRevealedNumberedCells()
            => Grid.GetAllCells().Where(c => c.State == CellState.Revealed && c.MinesAround > 0);
    }
}