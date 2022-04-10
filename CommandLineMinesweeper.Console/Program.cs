using System.Diagnostics;
using System.Runtime.InteropServices;
using YonatanMankovich.CommandLineMinesweeper.Console;
using YonatanMankovich.CommandLineMinesweeper.Core;
using YonatanMankovich.ConsoleDiffWriter.Data;
using YonatanMankovich.SimpleConsoleMenus;

const string boardOptionsPath = "MinesweeperOptions.xml";

Console.Title = "Yonatan's Command Line Minesweeper";
Console.CursorVisible = false;

BoardOptions boardOptions = ReadBoardOptions();

do
{
    Console.WriteLine("Welcome to Yonatan's command line Minesweeper game!" + Environment.NewLine);

    Console.WriteLine($"(Current board: {boardOptions.Height}x{boardOptions.Width} with {boardOptions.Mines} mines)" + Environment.NewLine);

    SimpleConsoleMenu mainMenu
        = new SimpleConsoleMenu("Select game mode:", "Play", "Auto-play", "Options", "Exit");

    mainMenu.Show();
    Console.Clear();

    switch (mainMenu.SelectedIndex)
    {
        case 0: HumanPlay(boardOptions); break;
        case 1: AutoPlay(boardOptions); break;
        case 2: SetSettings(); break;
        case 3: Environment.Exit(0); break;
        default: throw new NotImplementedException();
    }

    Console.Clear();
} while (true);


// ------------------------- Helper methods below ------------------------------

BoardOptions ReadBoardOptions()
{
    BoardOptions? readOptions = BoardOptions.ReadFromXmlFile(boardOptionsPath);
    if (readOptions == null)
    {
        readOptions = BoardOptions.Beginner;
        readOptions.SaveToXmlFile(boardOptionsPath);
    }
    return readOptions;
}

void SetSettings()
{
    SimpleConsoleMenu modeSelectionMenu =
        new SimpleConsoleMenu("Select the level of difficulty:", "Beginner", "Intermediate", "Expert", "Custom");
    modeSelectionMenu.Show();

    switch (modeSelectionMenu.SelectedIndex)
    {
        case 0: boardOptions = BoardOptions.Beginner; break;
        case 1: boardOptions = BoardOptions.Intermediate; break;
        case 2: boardOptions = BoardOptions.Expert; break;
        case 3:
            {
                Console.WriteLine("Please edit the options file and hit ENTER whenever you are done.");

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    Process.Start("explorer", boardOptionsPath);
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    Process.Start("nano", boardOptionsPath);
                else
                    Console.WriteLine("Please edit the file at " + boardOptionsPath);

                Console.WriteLine(Environment.NewLine + "Press ENTER to go to main menu . . .");
                Console.ReadLine();
                boardOptions = ReadBoardOptions();
            }
            break;
        default: throw new NotImplementedException();
    }

    boardOptions.SaveToXmlFile(boardOptionsPath);
}

void HumanPlay(BoardOptions options)
{
    Console.WriteLine("Use the arrow keys to select a cell; " +
        "F to toggle a flag; R to reveal the cell; Q to return to main menu." + Environment.NewLine);

    Minesweeper game = new Minesweeper(options);
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

    Console.WriteLine(Environment.NewLine + "Press ENTER to go to main menu . . .");
    Console.ReadLine();
}

void AutoPlay(BoardOptions options)
{
    Minesweeper game = new Minesweeper(options);
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

    Console.WriteLine(Environment.NewLine + "Press ENTER to go to main menu . . .");
    Console.ReadLine();
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