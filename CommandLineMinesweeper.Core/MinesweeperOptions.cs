namespace YonatanMankovich.CommandLineMinesweeper.Core
{
    /// <summary>
    /// Represents Minesweeper game options.
    /// </summary>
    public class MinesweeperOptions
    {
        private int height;
        private int width;
        private int mines;

        /// <summary>
        /// Gets or sets the height of the game board (each unit is one cell).
        /// </summary>
        public int Height
        {
            get => height;
            set
            {
                if (value < 1)
                    throw new ArgumentException($"Board height cannot be less than 1.", nameof(value));

                height = value;
            }
        }

        /// <summary>
        /// Gets or sets the width of the game board (each unit is one cell).
        /// </summary>
        public int Width
        {
            get => width;
            set
            {
                if (value < 1)
                    throw new ArgumentException("Board width cannot be less than 1.", nameof(value));

                width = value;
            }
        }

        /// <summary>
        /// Gets or sets the number of mines on the game board.
        /// </summary>
        public int Mines
        {
            get => mines; set
            {
                if (value < 0)
                    throw new ArgumentException("The number of mines on the board cannot be negative.", nameof(value));

                if (value > height * width)
                    throw new ArgumentException("The number of mines on the board cannot exceed the number of cells.", nameof(value));

                mines = value;
            }
        }

        /// <summary>
        /// Gets or sets a seed for the random number generator of the game (for placing mines on the board randomly).
        /// </summary>
        public int? RandomSeed { get; set; }

        /// <summary>
        /// Initializes an instance of the <see cref="MinesweeperOptions"/> class to the <see cref="Beginner"/> options.
        /// </summary>
        public MinesweeperOptions()
        {
            MinesweeperOptions boardOptions = Beginner;
            Height = boardOptions.Height;
            Width = boardOptions.Width;
            Mines = boardOptions.Mines;
            RandomSeed = boardOptions.RandomSeed;
        }

        /// <summary>
        /// Initializes an instance of the <see cref="MinesweeperOptions"/> class to the specified options.
        /// </summary>
        /// <param name="height">The board hight in cells.</param>
        /// <param name="width">The board width in cells.</param>
        /// <param name="mines">The number of mines to place on the board.</param>
        /// <param name="randomSeed">The optional seed for the game random number generator.</param>
        public MinesweeperOptions(int height, int width, int mines, int? randomSeed = null)
        {
            Height = height;
            Width = width;
            Mines = mines;
            RandomSeed = randomSeed;
        }

        /// <summary>
        /// Saves the current options to an file at the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        public void SaveToFile(string path)
        {
            File.WriteAllText(path, $"Height:{Height}\nWidth:{Width}\nMines:{Mines}\nRandom:{RandomSeed}");
        }

        /// <summary>
        /// Reads the saved game options from a specified file path into a new <see cref="MinesweeperOptions"/> object.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <returns>A new <see cref="MinesweeperOptions"/> object with the read game options.</returns>
        public static MinesweeperOptions? ReadFromFile(string path)
        {
            if (!File.Exists(path))
                return null;

            IDictionary<string, string> options = new Dictionary<string, string>();

            foreach (string line in File.ReadLines(path))
            {
                string[] tokens = line.Split(':');
                options.Add(tokens[0], tokens[1]);
            }

            int height = int.Parse(options["Height"]);
            int width = int.Parse(options["Width"]);
            int mines = int.Parse(options["Mines"]);
            int? randomSeed = string.IsNullOrWhiteSpace(options["Random"]) ? null : int.Parse(options["Random"]);
            return new MinesweeperOptions(height, width, mines, randomSeed);
        }

        /// <summary>
        /// Creates an instance of <see cref="MinesweeperOptions"/> with
        /// the beginner Minesweeper options (9X9 grid with 10 mines).
        /// </summary>
        public static MinesweeperOptions Beginner => new MinesweeperOptions(height: 9, width: 9, mines: 10);

        /// <summary>
        /// Creates an instance of <see cref="MinesweeperOptions"/> with
        /// the intermediate Minesweeper options (16X16 grid with 40 mines).
        /// </summary>
        public static MinesweeperOptions Intermediate => new MinesweeperOptions(height: 16, width: 16, mines: 40);

        /// <summary>
        /// Creates an instance of <see cref="MinesweeperOptions"/> with
        /// the expert Minesweeper options (16X30 grid with 99 mines).
        /// </summary>
        public static MinesweeperOptions Expert => new MinesweeperOptions(height: 16, width: 30, mines: 99);
    }
}