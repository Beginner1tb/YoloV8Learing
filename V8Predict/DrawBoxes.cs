
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;


namespace V8Predict
{
    public static class DrawBoxes
    {
        public static Image DrawBox(Image image, List<YoloPrediction> predictions)
        {
            using var graphics = Graphics.FromImage(image);
            foreach (var prediction in predictions) // iterate predictions to draw results
            {
                double score = Math.Round(prediction.Score, 2);
                graphics.DrawRectangles(new Pen(prediction.Label.Color, 1), new[] { prediction.Rectangle });
                var (x, y) = (prediction.Rectangle.X - 3, prediction.Rectangle.Y - 23);
                graphics.DrawString($"{prediction.Label.Name} ({score})",
                                new Font("Consolas", 16, GraphicsUnit.Pixel), new SolidBrush(prediction.Label.Color),
                                new PointF(x, y));
            }
            return image;
        }

    }
}
