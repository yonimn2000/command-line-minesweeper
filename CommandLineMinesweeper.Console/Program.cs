using System.Drawing;
using YonatanMankovich.CommandLineMinesweeper.Console;
using YonatanMankovich.CommandLineMinesweeper.Core;
using YonatanMankovich.CommandLineMinesweeper.Core.Enums;

Console.CursorVisible = false;
Console.WriteLine("Welcome to Yonatan's command line Minesweeper game!\n");

Minesweeper game = new Minesweeper(new BoardOptions());
MinesweeperConsoleDrawer drawer = new MinesweeperConsoleDrawer(game);
drawer.ConsoleLine = Console.CursorTop;
drawer.Draw(MinesweeperDrawOption.ShowEverything);
Thread.Sleep(1000);

GameMoveResult lastMoveResult = GameMoveResult.Playing;
Point lastMoveCoordinates = default;
//drawer.SelectedCoordinates = new Point(5, 5); // TODO
while (lastMoveResult != GameMoveResult.RevealedMine && lastMoveResult != GameMoveResult.AllClear)
{
    if (!game.GetSureMineCells().Any() && !game.GetSureClearCells().Any())
    {
        Cell randomUntouchedCell = game.GetUntouchedCells().OrderBy(c => Guid.NewGuid()).First();
        lastMoveCoordinates = randomUntouchedCell.Coordinates;
        lastMoveResult = game.RevealCell(lastMoveCoordinates.X, lastMoveCoordinates.Y);
        drawer.Draw();
    }

    foreach (Point point in game.GetSureMineCells().Select(c => c.Coordinates))
    {
        game.ToggleCellFlag(point.X, point.Y);
    }
    drawer.Draw();

    foreach (Point point in game.GetSureClearCells().Select(c => c.Coordinates))
    {
        lastMoveResult = game.RevealCell(point.X, point.Y);
    }
    drawer.Draw();
}

if (lastMoveResult == GameMoveResult.RevealedMine)
{
    drawer.HitMineCoordinates = lastMoveCoordinates;
    drawer.Draw(MinesweeperDrawOption.ShowHitMine);
    Console.WriteLine("BOOM! Game over...");
}

if (lastMoveResult == GameMoveResult.AllClear)
{
    drawer.Draw(MinesweeperDrawOption.AllClear);
    Console.WriteLine("Field cleared!");
}