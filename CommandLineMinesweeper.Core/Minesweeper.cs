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
    }
}