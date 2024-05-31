using Microsoft.ML.OnnxRuntime;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Color = System.Drawing.Color;
using Image = System.Drawing.Image;
using Pen = System.Drawing.Pen;



namespace YoloV8Test1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Yolov8? predictor;
        private string? ImageFilePath;
        private string? modelPath;
        private string[]? resultArray = null;
        public MainWindow()
        {
            InitializeComponent();
            predictor = null;
        }

        private void Btn_ModelPath_Click(object sender, RoutedEventArgs e)
        {

            bool isGpU = CB_Gpu.IsChecked.Value;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            {

                if (openFileDialog.ShowDialog() == true)
                {
                    modelPath = openFileDialog.FileName;
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    predictor = new Yolov8(modelPath, isGpU);
                    stopwatch.Stop();

                    var timespan = stopwatch.ElapsedMilliseconds;
                    TB_LoadModelTime.Dispatcher.Invoke(new Action(() =>
                    {
                        TB_LoadModelTime.Text = timespan.ToString();
                        TB_ModelPath.Text = modelPath;
                    }));

                    using (var session = new InferenceSession(modelPath))
                    {
                        // 获取模型元数据
                        var modelMetadata = session.ModelMetadata;


                        if (modelMetadata.CustomMetadataMap.ContainsKey("names"))
                        {
                            string labels = modelMetadata.CustomMetadataMap["names"];

                            // 正则表达式匹配单引号中的内容
                            Regex regex = new Regex("'([^']*)'");
                            MatchCollection matches = regex.Matches(labels);

                            // 创建一个列表来存储匹配的结果
                            List<string> resultList = new List<string>();

                            // 遍历所有的匹配项
                            foreach (Match match in matches)
                            {
                                // 将匹配的内容添加到结果列表中
                                resultList.Add(match.Groups[1].Value);
                            }

                            // 将结果列表转换为数组
                            resultArray = resultList.ToArray();

                            // 打印结果数组
                            foreach (string item in resultArray)
                            {
                                Debug.WriteLine(item);
                            }

                        }
                        else
                        {
                            Debug.WriteLine("No label information found in the model metadata.");
                        }
                    }
                }
            }

        }

        private Image DrawBox(Image image, List<YoloPrediction> predictions)
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

        private void Btn_ImgPath_Click(object sender, RoutedEventArgs e)
        {

            OpenFileDialog openFileDialog = new OpenFileDialog();
            {

                if (openFileDialog.ShowDialog() == true)
                {
                    ImageFilePath = openFileDialog.FileName;

                    TB_LoadModelTime.Dispatcher.Invoke(new Action(() => { TB_ImagePath.Text = ImageFilePath; }));
                }
            }
        }

        private void Btn_Infer_Click(object sender, RoutedEventArgs e)
        {
            if (predictor != null && ImageFilePath != null && modelPath != null)
            {
         
               



                predictor.SetupYoloDefaultLabels(resultArray);
                using var image = Image.FromFile(ImageFilePath);
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                var predictions = predictor.Predict(image, useNumpy: false);
                stopwatch.Stop();

                var timespan = stopwatch.ElapsedMilliseconds;
                TB_InferTime.Dispatcher.Invoke(new Action(() =>
                {
                    TB_InferTime.Text = timespan.ToString();
                    ;
                }));
                Image _imageField = DrawBox(image, predictions);
                BitmapImage bitmapImage = ConvertToBitmapImage(_imageField);

                img.Source = bitmapImage;


            }
        }

        public static BitmapImage ConvertToBitmapImage(Image image)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                image.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }

        private void Btn_LabelInfo_Click(object sender, RoutedEventArgs e)
        {
            if (modelPath != null)
            {
                using (var session = new InferenceSession(modelPath))
                {
                    // 获取模型元数据
                    var modelMetadata = session.ModelMetadata;

                    // 打印模型元数据中的信息
                    Debug.WriteLine("Model Metadata:");
                    Debug.WriteLine($"Producer Name: {modelMetadata.ProducerName}");
                    Debug.WriteLine($"Graph Name: {modelMetadata.GraphName}");
                    Debug.WriteLine($"Domain: {modelMetadata.Domain}");
                    Debug.WriteLine($"Description: {modelMetadata.Description}");
                    Debug.WriteLine($"Version: {modelMetadata.Version}");

                    // 检查模型是否包含标签信息（假设标签信息存储在元数据的自定义属性中）
                    foreach (var customProperty in modelMetadata.CustomMetadataMap)
                    {
                        Debug.WriteLine($"Custom Metadata - {customProperty.Key}: {customProperty.Value}");
                        //Debug.WriteLine($"Custom Metadata - {customProperty.Value}");
                    }

                    // 假设标签信息存储在一个叫做"labels"的自定义属性中
                    if (modelMetadata.CustomMetadataMap.ContainsKey("names"))
                    {
                        string labels = modelMetadata.CustomMetadataMap["names"];
                        Debug.WriteLine("This label " + labels);
                        // 正则表达式匹配单引号中的内容
                        Regex regex = new Regex("'([^']*)'");
                        MatchCollection matches = regex.Matches(labels);

                        // 创建一个列表来存储匹配的结果
                        List<string> resultList = new List<string>();

                        // 遍历所有的匹配项
                        foreach (Match match in matches)
                        {
                            // 将匹配的内容添加到结果列表中
                            resultList.Add(match.Groups[1].Value);
                        }

                        // 将结果列表转换为数组
                        string[] resultArray = resultList.ToArray();

                        // 打印结果数组
                        foreach (string item in resultArray)
                        {
                            Debug.WriteLine(item);
                        }
                        var labelList = labels.Split(',').ToList();
                        Debug.WriteLine("Labels:");
                        foreach (var label in labelList)
                        {
                            Debug.WriteLine(label);
                        }
                    }
                    else
                    {
                        Debug.WriteLine("No label information found in the model metadata.");
                    }
                }
            }
        }
    }
}