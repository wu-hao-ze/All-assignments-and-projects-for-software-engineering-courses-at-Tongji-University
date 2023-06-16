using UnityEngine;

namespace Unity.VisualScripting
{
    public class StickyNote : GraphElement<IGraph>
    {
        [DoNotSerialize] public static readonly Color defaultColor = new Color(.969f, .91f, .624f);

        public StickyNote() : base() { }

        [Serialize] public Rect position { get; set; }

        [Serialize] public string title { get; set; } = "Sticky Note";

        [Serialize]
        [InspectorTextArea(minLines = 1)]
        public string body { get; set; }

        [Serialize] [Inspectable] public ColorEnum colorTheme { get; set; }

        public enum ColorEnum
        {
            Classic,
            Black,
            Dark,
            Orange,
            Green,
            Blue,
            Red,
            Purple,
            Teal,
        }

        public static Color GetStickyColor(ColorEnum enumValue)
        {
            switch (enumValue)
            {
                case ColorEnum.Black:
                    return new Color(.122f, .114f, .09f);
                case ColorEnum.Dark:
                    return new Color(.184f, .145f, .024f);
                case ColorEnum.Orange:
                    return new Color(.988f, .663f, .275f);
                case ColorEnum.Green:
                    return new Color(.376f, .886f, .655f);
                case ColorEnum.Blue:
                    return new Color(.518f, .725f, .855f);
                case ColorEnum.Red:
                    return new Color(1f, .502f, .502f);
                case ColorEnum.Purple:
                    return new Color(.98f, .769f, .949f);
                case ColorEnum.Teal:
                    return new Color(.475f, .878f, .89f);
                default:
                    //Classic
                    return new Color(.969f, .91f, .624f);
            }
        }

        public static Color GetFontColor(ColorEnum enumValue)
        {
            switch (enumValue)
            {
                case ColorEnum.Black:
                case ColorEnum.Dark:
                    return Color.white;
                default:
                    return Color.black;
            }
        }
    }
}
