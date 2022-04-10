using System.Xml.Serialization;

namespace YonatanMankovich.CommandLineMinesweeper.Core
{
    public class BoardOptions
    {
        private int height;
        private int width;
        private int mines;

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

        public int Mines
        {
            get => mines; set
            {
                if (value < 0)
                    throw new ArgumentException("The number of mines on the board cannot be negative.", nameof(mines));

                if (value > height * width)
                    throw new ArgumentException("The number of mines on the board cannot exceed the number of cells.", nameof(value));

                mines = value;
            }
        }
        public int? RandomSeed { get; set; }

        public BoardOptions()
        {
            BoardOptions boardOptions = Beginner;
            Height = boardOptions.Height;
            Width = boardOptions.Width;
            Mines = boardOptions.Mines;
            RandomSeed = boardOptions.RandomSeed;
        }

        public BoardOptions(int height, int width, int mines, int? randomSeed = null)
        {
            Height = height;
            Width = width;
            Mines = mines;
            RandomSeed = randomSeed;
        }

        public void SaveToXmlFile(string path)
        {
            TextWriter? writer = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(BoardOptions));
                writer = new StreamWriter(path);
                serializer.Serialize(writer, this);
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }

        public static BoardOptions? ReadFromXmlFile(string path)
        {
            TextReader? reader = null;
            try
            {
                if (!File.Exists(path))
                    return null;

                XmlSerializer serializer = new XmlSerializer(typeof(BoardOptions));
                reader = new StreamReader(path);
                return serializer.Deserialize(reader) as BoardOptions;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }

        public static BoardOptions Beginner => new BoardOptions(height: 9, width: 9, mines: 10);
        public static BoardOptions Intermediate => new BoardOptions(height: 16, width: 16, mines: 40);
        public static BoardOptions Expert => new BoardOptions(height: 16, width: 30, mines: 99);
    }
}