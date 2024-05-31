using Microsoft.Win32;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using V8Predict;
using Image = System.Drawing.Image;

namespace YoloV8Test2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //onnx模型，从文件载入
        private Yolov8? predictor;
        //图像地址
        private string? ImageFilePath;
        //onnx模型地址
        private string? modelPath;
        //onnx中的labels信息
        private string[]? resultArray = null;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Btn_ModelPath_Click(object sender, RoutedEventArgs e)
        {
            //判断是否使用GPU
            bool isGpU = CB_Gpu.IsChecked.Value;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            {
                if (openFileDialog.ShowDialog() == true)
                {
                    modelPath = openFileDialog.FileName;
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();

                    //载入onnx文件
                    predictor = new Yolov8(modelPath, isGpU);
                    stopwatch.Stop();

                    var timespan = stopwatch.ElapsedMilliseconds;
                    TB_LoadModelTime.Dispatcher.Invoke(new Action(() =>
                    {
                        TB_LoadModelTime.Text = timespan.ToString();
                        TB_ModelPath.Text = modelPath;
                    }));

                    //从Onnx文件中获取Labels信息
                    GetLabels getLabels = new GetLabels();
                    resultArray = getLabels.GetOnnxLabels(modelPath);
                }
            }

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

                //使用Yolo推理
                var predictions = predictor.Predict(image, useNumpy: false);
                stopwatch.Stop();

                var timespan = stopwatch.ElapsedMilliseconds;
                TB_InferTime.Dispatcher.Invoke(new Action(() =>
                {
                    TB_InferTime.Text = timespan.ToString();
                    ;
                }));

                //画识别框
                Image _imageField = DrawBoxes.DrawBox(image, predictions);
                BitmapImage bitmapImage = ConvertToBitmapImage(_imageField);

                //界面显示结果
                img.Source = bitmapImage;


            }
        }

        //界面显示图像转换
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
    }
}