using YonatanMankovich.CommandLineMinesweeper.Console;
using YonatanMankovich.CommandLineMinesweeper.Core;
using YonatanMankovich.ConsoleDiffWriter.Data;

Console.Title = "Yonatan's Command Line Minesweeper";
Console.CursorVisible = false;

Console.WriteLine("Welcome to Yonatan's command line Minesweeper game!\n");

Console.WriteLine("Use the arrow keys to select a cell; F to toggle a flag; R to reveal the cell.\n");

Minesweeper game = new Minesweeper(new BoardOptions());
MinesweeperConsoleDrawer drawer = new MinesweeperConsoleDrawer(game, Console.CursorTop);

MinesweeperPlayer player = new MinesweeperConsolePlayer(game);

drawer.Draw();
player.AfterSelectCallback = (lastMoveCoordinates) =>
{
    drawer.SelectedCoordinates = lastMoveCoordinates;
    drawer.Draw();
};
player.AfterInvalidMoveCallback = (lastMoveCoordinates, message) =>
{
    drawer.AdditionalText = new ConsoleLines().AddLine(new ConsoleString(message, ConsoleColor.White, ConsoleColor.DarkYellow));
    drawer.Draw();
};
player.AfterMoveCallback = (lastMoveCoordinates) =>
{
    drawer.SelectedCoordinates = null; // Unselect.
    drawer.AdditionalText = null; // Clear additional text.
    drawer.Draw();
};
player.AfterRevealedMineCallback = (lastMoveCoordinates) =>
{
    drawer.SelectedCoordinates = lastMoveCoordinates;
    drawer.AdditionalText = new ConsoleLines().AddLine(new ConsoleString("BOOM! Game over...", ConsoleColor.Black, ConsoleColor.Red));
    drawer.Draw(MinesweeperDrawOption.ShowMines);
};
player.AfterFieldClearedCallback = () =>
{
    drawer.AdditionalText = new ConsoleLines().AddLine(new ConsoleString("Field cleared!", ConsoleColor.Black, ConsoleColor.Green));
    drawer.Draw(MinesweeperDrawOption.AllClear);
};
player.PlayToEnd();
/*
game.Reset();
player = new MinesweeperAutoPlayer(game);
int msDelayBetweenMoves = 50;

drawer.Draw();
player.AfterMoveCallback = (lastMoveCoordinates) =>
{
    drawer.SelectedCoordinates = lastMoveCoordinates;
    drawer.Draw();
    Thread.Sleep(msDelayBetweenMoves);
};
player.PlayToEnd();*/