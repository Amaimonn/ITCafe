namespace ITCafe.Data.Settings
{
    public struct ScreenResolution
    {
        public int Width;
        public int Height;
        
        public override string ToString() => $"{Width}x{Height}";
    }
}