namespace YonatanMankovich.CommandLineMinesweeper.Core
{
    public class BoardOptions
    {
        public int Height { get; }
        public int Width { get; }
        public double MinesRatio { get; }
        public int? RandomSeed { get; }

        public BoardOptions() : this(height: 20, width: 20, minesRatio: 0.15) { }

        public BoardOptions(int height, int width, double minesRatio, int? randomSeed = null)
        {
            if (height < 1)
                throw new ArgumentException($"Board height cannot be less than 1.", nameof(height));

            if (width < 1)
                throw new ArgumentException("Board width cannot be less than 1.", nameof(width));

            if (minesRatio < 0)
                throw new ArgumentException("Board mines ratio cannot be negative.", nameof(minesRatio));

            Height = height;
            Width = width;
            MinesRatio = minesRatio;
            RandomSeed = randomSeed;
        }

        public int GetNumberOfMines() => (int)Math.Round(Height * Width * MinesRatio);
    }
}