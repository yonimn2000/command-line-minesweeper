namespace YonatanMankovich.CommandLineMinesweeper.Core
{
    public class BoardOptions
    {
        public int Height { get; set; } = 20;
        public int Width { get; set; } = 20;
        public double MinesRatio { get; set; } = 0.15;
        public int? RandomSeed { get; set; } = null;
        public int GetNumberOfMines() => (int)(Height * Width * MinesRatio);
    }
}