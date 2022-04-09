using YonatanMankovich.CommandLineMinesweeper.Console;
using YonatanMankovich.CommandLineMinesweeper.Core;
using YonatanMankovich.ConsoleDiffWriter.Data;

Console.Title = "Yonatan's Command Line Minesweeper";
Console.CursorVisible = false;

Console.WriteLine("Welcome to Yonatan's command line Minesweeper game!\n");

Minesweeper game = new Minesweeper(new BoardOptions());
MinesweeperConsoleDrawer drawer = new MinesweeperConsoleDrawer(game, Console.CursorTop);

MinesweeperAutoPlayer autoPlayer = new MinesweeperAutoPlayer(game);
int msDelayBetweenMoves = 50;

drawer.Draw();
autoPlayer.AfterMoveCallback = () =>
{
    drawer.SelectedCoordinates = autoPlayer.LastMoveCoordinates;
    drawer.Draw();
    Thread.Sleep(msDelayBetweenMoves);
};
autoPlayer.AfterRevealedMineCallback = () =>
{
    drawer.Draw(MinesweeperDrawOption.ShowMines);
    new ConsoleString("BOOM! Game over...", ConsoleColor.Black, ConsoleColor.Red).WriteLine();
};
autoPlayer.AfterFieldClearedCallback = () =>
{
    drawer.Draw(MinesweeperDrawOption.AllClear);
    new ConsoleString("Field cleared!", ConsoleColor.Black, ConsoleColor.Green).WriteLine();
};
autoPlayer.PlayToEnd();