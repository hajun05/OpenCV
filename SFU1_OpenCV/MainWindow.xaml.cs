using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OpenCvSharp;

namespace SFU1_OpenCV
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        public MainWindow()
        {
            InitializeComponent();

            textBox1.Text = Cv2.GetVersionString();

            Mat image1 = Cv2.ImRead("C:/Users/Hajun/source/repos/OpenCV/SFU1_OpenCV/Resources/cat.jpg");
            Mat image2 = new Mat(image1.Size(), MatType.CV_8UC1);
            Mat image3 = new Mat(image1.Size(), MatType.CV_8UC3);
            Mat image4 = new Mat(image1.Size(), MatType.CV_8UC3);
            Mat image5 = new Mat(image1.Size(), MatType.CV_8UC3);
            Mat image6 = new Mat(image1.Size(), MatType.CV_8UC3);
            // 무슨 이유인지 상대경로 이미지 접근이 불가, OS 차이?
            // WPF의 약점. 이미지 넣기가 어렵고 오류가 많다!

            //Cv2.ImShow("Image Test", image);
            //imageBox1.Source = OpenCvSharp.WpfExtensions.
            //    BitmapSourceConverter.ToBitmapSource(image);

            Cv2.CvtColor(image1, image2, ColorConversionCodes.RGB2GRAY);
            Cv2.CvtColor(image1, image3, ColorConversionCodes.BGR2XYZ);
            Cv2.CvtColor(image1, image4, ColorConversionCodes.BGR2YCrCb);
            Cv2.CvtColor(image1, image5, ColorConversionCodes.BGR2HSV);
            Cv2.CvtColor(image1, image6, ColorConversionCodes.BGR2Lab);

            imageBox1.Source = OpenCvSharp.WpfExtensions.
                BitmapSourceConverter.ToBitmapSource(image1);
            imageBox2.Source = OpenCvSharp.WpfExtensions.
                BitmapSourceConverter.ToBitmapSource(image2);
            imageBox3.Source = OpenCvSharp.WpfExtensions.
                BitmapSourceConverter.ToBitmapSource(image3);
            imageBox4.Source = OpenCvSharp.WpfExtensions.
                BitmapSourceConverter.ToBitmapSource(image4);
            imageBox5.Source = OpenCvSharp.WpfExtensions.
                BitmapSourceConverter.ToBitmapSource(image5);
            imageBox6.Source = OpenCvSharp.WpfExtensions.
                BitmapSourceConverter.ToBitmapSource(image6);
        }
    }
}