namespace YonatanMankovich.CommandLineMinesweeper.Core
{
    public class Game
    {
        public int TotalMines { get; }
        private Board Board { get; }

        public Game() : this(new BoardOptions()) { }

        public Game(BoardOptions options)
        {
            TotalMines = options.GetNumberOfMines();
            Board = new Board(options);
        }

        public bool RevealCell(int x, int y) => Board.RevealCell(x, y);

        public int GetNumberOfRemainingMines() => TotalMines - Board.CountFlags();
    }
}