using System.Drawing;

namespace V8Predict.Extentions
{
    public static class RectangleExtensions
    {
        public static float Area(this RectangleF source)
        {
            return source.Width * source.Height;
        }
    }
}
