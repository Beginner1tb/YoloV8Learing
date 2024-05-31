using System.Drawing;

namespace YoloV8Test1
{
    public class YoloLabel
    {
        public int Id { get; set; }

        public string? Name { get; set; }
        
        public YoloLabelKind Kind { get; set; }
        
        public Color Color { get; set; }

        public YoloLabel() => Color = Color.Yellow;
    }
}
