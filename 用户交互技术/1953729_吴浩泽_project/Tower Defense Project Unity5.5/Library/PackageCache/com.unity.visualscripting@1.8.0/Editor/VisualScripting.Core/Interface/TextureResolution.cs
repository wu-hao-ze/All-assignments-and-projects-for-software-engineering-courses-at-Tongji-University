namespace Unity.VisualScripting
{
    public struct TextureResolution
    {
        public int width;

        public int height;

        public TextureResolution(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        public TextureResolution(int side)
        {
            width = side;
            height = side;
        }

        public static implicit operator TextureResolution(int side)
        {
            return new TextureResolution(side);
        }

        public override string ToString()
        {
            return $"{width}x{height}";
        }
    }
}
