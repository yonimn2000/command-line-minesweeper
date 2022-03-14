using YonatanMankovich.CommandLineMinesweeper.Console;
using YonatanMankovich.CommandLineMinesweeper.Core;

Console.WriteLine("Welcome to Yonatan's command line Minesweeper game!\n");

Game game = new Game(new BoardOptions());
ConsoleDrawer.DrawBoard(game, true);

Random random = new Random((int)DateTime.Now.Ticks + 1);
while (true)
{
    ConsoleDrawer.DrawBoard(game);
    bool result = game.RevealCell(random.Next(20), random.Next(10));
    if (result)
    {
        ConsoleDrawer.DrawBoard(game, true);
        break;
    }

    Thread.Sleep(1000);
}