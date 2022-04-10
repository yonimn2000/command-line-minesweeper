using YonatanMankovich.CommandLineMinesweeper.Console;
using YonatanMankovich.CommandLineMinesweeper.Core;
using YonatanMankovich.ConsoleDiffWriter.Data;
using YonatanMankovich.SimpleConsoleMenus;

Console.Title = "Yonatan's Command Line Minesweeper";
Console.CursorVisible = false;

do
{
    Console.WriteLine("Welcome to Yonatan's command line Minesweeper game!" + Environment.NewLine);

    SimpleConsoleMenu mainMenu = new SimpleConsoleMenu("Select game mode:", "Play", "Auto-play", "Exit");
    mainMenu.Show();
    Console.Clear();

    Minesweeper game = new Minesweeper(new BoardOptions());

    switch (mainMenu.SelectedIndex)
    {
        case 0: HumanPlay(game); break;
        case 1: AutoPlay(game); break;
        case 2: Environment.Exit(0); break;
        default: throw new NotImplementedException();
    }

    Console.WriteLine(Environment.NewLine + "Press ENTER to go to main menu . . .");
    Console.ReadLine();
    Console.Clear();
} while (true);

void HumanPlay(Minesweeper game)
{
    Console.WriteLine("Use the arrow keys to select a cell; " +
        "F to toggle a flag; R to reveal the cell; Q to return to main menu." + Environment.NewLine);

    MinesweeperPlayer player = new MinesweeperConsolePlayer(game);
    MinesweeperConsoleDrawer drawer = new MinesweeperConsoleDrawer(game);

    drawer.DrawBase();
    SetCommonCallbacks(player, drawer);
    player.AfterMoveCallback = (lastMoveCoordinates) =>
    {
        drawer.SelectedCoordinates = null; // Unselect.
        drawer.AdditionalText = null; // Clear additional text.
        drawer.Draw();
    };
    player.PlayToEnd();
}

void AutoPlay(Minesweeper game)
{
    MinesweeperPlayer player = new MinesweeperAutoPlayer(game);
    MinesweeperConsoleDrawer drawer = new MinesweeperConsoleDrawer(game);
    //int msDelayBetweenMoves = 50;

    drawer.DrawBase();
    SetCommonCallbacks(player, drawer);
    player.AfterMoveCallback = (lastMoveCoordinates) =>
    {
        drawer.SelectedCoordinates = lastMoveCoordinates;
        drawer.Draw();
        // Thread.Sleep(msDelayBetweenMoves);
    };
    player.PlayToEnd();
}

void SetCommonCallbacks(MinesweeperPlayer player, MinesweeperConsoleDrawer drawer)
{
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
}