using YonatanMankovich.CommandLineMinesweeper.Console;
using YonatanMankovich.CommandLineMinesweeper.Core;
using YonatanMankovich.SimpleColorConsole;
using YonatanMankovich.SimpleConsoleMenus;

const string gameOptionsPath = "MinesweeperOptions.txt";

Console.Title = "Yonatan's Command Line Minesweeper";
Console.CursorVisible = false;

MinesweeperOptions minesweeperOptions = ReadBoardOptions();
SimpleConsoleMenu mainMenu = new SimpleConsoleMenu("Select game mode:", "Play", "Auto-play", "Options", "Exit");
do
{
    Console.WriteLine("Welcome to Yonatan's command line Minesweeper game!" + Environment.NewLine);

    Console.WriteLine($"(Current board: {minesweeperOptions.Height}x{minesweeperOptions.Width} " +
        $"with {minesweeperOptions.Mines} mines" +
        $"{(minesweeperOptions.RandomSeed.HasValue ? $" [Random seed: {minesweeperOptions.RandomSeed}]" : "")})" + Environment.NewLine);

    mainMenu.Show();
    Console.Clear();
    switch (mainMenu.SelectedIndex)
    {
        case 0: HumanPlay(minesweeperOptions); break;
        case 1: AutoPlay(minesweeperOptions); break;
        case 2: SetSettings(); break;
        case 3: Environment.Exit(0); break;
        default: throw new NotImplementedException();
    }

    Console.Clear();
} while (true);


// ------------------------- Helper methods below ------------------------------

MinesweeperOptions ReadBoardOptions()
{
    MinesweeperOptions? readOptions = MinesweeperOptions.ReadFromFile(gameOptionsPath);
    if (readOptions == null)
    {
        readOptions = MinesweeperOptions.Beginner;
        readOptions.SaveToFile(gameOptionsPath);
    }
    return readOptions;
}

void SetSettings()
{
    SimpleConsoleMenu modeSelectionMenu =
        new SimpleConsoleMenu("Select the level of difficulty:",
        "Beginner", "Intermediate", "Expert", "Custom", "Go back");
    modeSelectionMenu.Show();

    switch (modeSelectionMenu.SelectedIndex)
    {
        case 0: minesweeperOptions = MinesweeperOptions.Beginner; break;
        case 1: minesweeperOptions = MinesweeperOptions.Intermediate; break;
        case 2: minesweeperOptions = MinesweeperOptions.Expert; break;
        case 3: HandleSettingCustomSettings(); break;
        case 4: break;
        default: throw new NotImplementedException();
    }

    minesweeperOptions.SaveToFile(gameOptionsPath);
}

void HandleSettingCustomSettings()
{
    Console.WriteLine();
    if (OperatingSystem.IsWindows())
        Console.CursorVisible = true;

    bool isCorrect = false;

    do
    {
        try
        {
            Console.Write($"Enter board height [{minesweeperOptions.Height}]: ");
            string? readHeight = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(readHeight))
                minesweeperOptions.Height = int.Parse(readHeight);

            Console.Write($"Enter board width [{minesweeperOptions.Width}]: ");
            string? readWidth = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(readWidth))
                minesweeperOptions.Width = int.Parse(readWidth);

            Console.Write($"Enter number of mines [{minesweeperOptions.Mines}]: ");
            string? readMines = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(readMines))
                minesweeperOptions.Mines = int.Parse(readMines);

            Console.Write($"(Optional) Enter random seed [{minesweeperOptions.RandomSeed}] (space to clear): ");
            string? readRandom = Console.ReadLine();
            minesweeperOptions.RandomSeed = string.IsNullOrWhiteSpace(readRandom) ? null : int.Parse(readRandom);

            if (OperatingSystem.IsWindows())
                Console.CursorVisible = false;

            isCorrect = true;
        }
        catch (Exception e) when (e is ArgumentException || e is FormatException)
        {
            isCorrect = false;
            Console.WriteLine(e.Message);
        }
    } while (!isCorrect);
}

void HumanPlay(MinesweeperOptions options)
{
    Console.WriteLine("Use the arrow keys to select a cell; " +
        "F to toggle a flag; R to reveal the cell; Q to return to main menu." + Environment.NewLine);

    Minesweeper game = new Minesweeper(options);
    MinesweeperPlayer player = new MinesweeperConsolePlayer(game);
    MinesweeperConsoleDrawer drawer = new MinesweeperConsoleDrawer(game);

    drawer.Draw();
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

void AutoPlay(MinesweeperOptions options)
{
    Minesweeper game = new Minesweeper(options);
    MinesweeperAutoPlayer player = new MinesweeperAutoPlayer(game);
    player.MoveInBatches = true;
    MinesweeperConsoleDrawer drawer = new MinesweeperConsoleDrawer(game);
    //int msDelayBetweenMoves = 50;

    drawer.Draw();
    SetCommonCallbacks(player, drawer);
    player.AfterMoveCallback = (lastMoveCoordinates) =>
    {
        drawer.SelectedCoordinates = lastMoveCoordinates;
        drawer.Draw();
        // Thread.Sleep(msDelayBetweenMoves);
    };
    player.PlayToEnd();

    Console.WriteLine("Guess moves made: " + player.GuessMoves);
    Console.WriteLine(Environment.NewLine + "Press ENTER to go to main menu . . .");
    Console.ReadLine();
}

void SetCommonCallbacks(MinesweeperPlayer player, MinesweeperConsoleDrawer drawer)
{
    player.AfterCellSelectedCallback = (lastMoveCoordinates) =>
    {
        drawer.SelectedCoordinates = lastMoveCoordinates;
        drawer.Draw();
    };
    player.AfterInvalidMoveCallback = (lastMoveCoordinates, message) =>
    {
        drawer.AdditionalText = new ColorLines().AddLine(message.TextWhite().BackDarkYellow());
        drawer.Draw();
    };

    player.AfterRevealedMineCallback = (lastMoveCoordinates) =>
    {
        drawer.SelectedCoordinates = lastMoveCoordinates;
        drawer.AdditionalText = new ColorLines().AddLine("BOOM! Game over...".TextBlack().BackRed());
        drawer.Draw(MinesweeperDrawOption.ShowMines);
    };
    player.AfterFieldClearedCallback = () =>
    {
        drawer.AdditionalText = new ColorLines().AddLine("Field cleared!".TextBlack().BackGreen());
        drawer.Draw(MinesweeperDrawOption.AllClear);
    };
}