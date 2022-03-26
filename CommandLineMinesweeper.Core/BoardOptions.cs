namespace YonatanMankovich.CommandLineMinesweeper.Core
{
    public class BoardOptions
    {
        public int Height { get; set; } = 16;
        public int Width { get; set; } = 30;
        public double MinesRatio { get; set; } = 0.20625;
        public int? RandomSeed { get; set; } = null;
        public int GetNumberOfMines() => (int)(Height * Width * MinesRatio);
    }
}