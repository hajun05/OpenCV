using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SFU1_OpenCV.Practices
{
    /// <summary>
    /// Interaction logic for Window3.xaml
    /// </summary>
    public partial class Window3 : System.Windows.Window
    {
        public Window3()
        {
            InitializeComponent();

            // 이미지 읽기, 생성
            //string imgPath = "C:/Users/Hajun/source/repos/OpenCV/SFU1_OpenCV/Resources/cat.jpg";
            //Mat image = Cv2.ImRead(imgPath);
            Mat image = new Mat(new OpenCvSharp.Size(640, 360), MatType.CV_8UC3, Scalar.All(255));

            // 선 그리기
            Cv2.Line(image, new OpenCvSharp.Point(10, 10), new OpenCvSharp.Point(630, 10), Scalar.Red, 10, LineTypes.AntiAlias);
            Cv2.Line(image, new OpenCvSharp.Point(10, 30), new OpenCvSharp.Point(630, 30), Scalar.Orange, 10, LineTypes.AntiAlias);

            // 원 그리기
            Cv2.Circle(image, new OpenCvSharp.Point(30, 70), 20, Scalar.Yellow, 10, LineTypes.AntiAlias);
            Cv2.Circle(image, new OpenCvSharp.Point(90, 70), 25, Scalar.Green, 10, LineTypes.AntiAlias);

            // 사각형 그리기
            Cv2.Rectangle(image, new OpenCvSharp.Rect(130, 50, 40, 40), Scalar.Blue, 10, LineTypes.AntiAlias);
            Cv2.Rectangle(image, new OpenCvSharp.Point(185, 45), new OpenCvSharp.Point(235, 95), Scalar.Navy, -1, LineTypes.AntiAlias);

            // 타원 그리기
            Cv2.Ellipse(image, new RotatedRect(new Point2f(290, 70), new Size2f(75.0, 50.0), 30), Scalar.Purple, 10, LineTypes.AntiAlias);

            // 결과 그리기
            Cv2.ImShow("Drawn Shapes", image);
            Cv2.WaitKey(0);
            Cv2.DestroyAllWindows();
        }
    }
}
