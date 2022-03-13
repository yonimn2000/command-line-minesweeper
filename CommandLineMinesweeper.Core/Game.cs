namespace YonatanMankovich.CommandLineMinesweeper.Core
{
    public class Game
    {
        public Board Board { get; }

        public Game() : this(new BoardOptions()) { }

        public Game(BoardOptions options)
        {
            Board = new Board(new BoardOptions());
        }
    }
}