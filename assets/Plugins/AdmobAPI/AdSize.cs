namespace admob
{
    public class AdSize
    {
        public static readonly AdSize Banner = new(320, 50);
        public static readonly AdSize MediumRectangle = new(300, 250);
        public static readonly AdSize IABBanner = new(468, 60);
        public static readonly AdSize Leaderboard = new(728, 90);
        public static readonly AdSize WideSkyscraper = new(120, 600);
        public static readonly AdSize SmartBanner = new(-1, -2);

        public int Width { get; }

        public int Height { get; }

        public AdSize(int width, int height)
        {
            Width = width;
            Height = height;
        }
    }
}