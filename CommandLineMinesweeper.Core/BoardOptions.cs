namespace YonatanMankovich.CommandLineMinesweeper.Core
{
    public class BoardOptions
    {
        public int Height { get; set; } = 10;
        public int Width { get; set; } = 20;
        public double MinesRatio { get; set; } = 0.25;
        public int GetNumberOfMines() => (int)(Height * Width * MinesRatio);
    }
}