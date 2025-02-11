using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
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

            //Draw();
            //TextandDraw();
            ImageHistogram();

            Cv2.WaitKey(0);
            Cv2.DestroyAllWindows();
        }

        public void ImageHistogram()
        {
            string imgPath = "C:/Users/Hajun/source/repos/OpenCV/SFU1_OpenCV/Resources/Lenna.png";
            Mat image = Cv2.ImRead(imgPath);

            // 1. 그레이스케일 변환 후 히스토그램 선언
            Mat gray = new Mat();
            Cv2.CvtColor(image, gray, ColorConversionCodes.BGR2GRAY);
            Mat grayHist = new Mat();

            // 1. 이미지 equalized(평활화, 평탄화) 후 히스토그램 선언
            Mat equalized = new Mat();
            Cv2.EqualizeHist(gray, equalized);
            Mat equalizedHist = new Mat();

            // 2. 히스토그램 초기화
            int[] histSize = { 256 }; // 히스토그램 크기 (256단계)
            Rangef[] ranges = { new Rangef(0, 256) }; // 픽셀 값 범위 (0~255)
            Cv2.CalcHist(new Mat[] { gray }, new int[] { 0 }, null, grayHist, 1, histSize, ranges);
            Cv2.CalcHist(new Mat[] { equalized }, new int[] { 0 }, null, equalizedHist, 1, histSize, ranges);

            // 3. 히스토그램 시각화 (그레이스케일)
            Mat histImage1 = DrawHistogram(grayHist, new Scalar(255, 255, 255));
            Mat histImage2 = DrawHistogram(equalizedHist, new Scalar(255, 255, 255));

            // 4. 결과 이미지 및 히스토그램 출력
            //Cv2.ImShow("Original Image", image);
            Cv2.ImShow("Grayscale Image", gray);
            Cv2.ImShow("Equalized image", equalized);
            Cv2.ImShow("Grayscale Histogram", histImage1);
            Cv2.ImShow("Equalizedscale Histogram", histImage2);
        }
        static Mat DrawHistogram(Mat hist, Scalar color)
        {
            int histWidth = 512, histHeight = 400;
            Mat histImage = new Mat(histHeight, histWidth, MatType.CV_8UC3, Scalar.Black);
            // 히스토그램 정규화 (0~histHeight 범위)
            Cv2.Normalize(hist, hist, 0, histImage.Rows, NormTypes.MinMax);
            // 히스토그램 그리기
            int binWidth = histWidth / hist.Rows;
            for (int i = 1; i < hist.Rows; i++)
            {
                int x1 = (i - 1) * binWidth;
                int y1 = histHeight - (int)hist.At<float>(i - 1);
                int x2 = i * binWidth;
                int y2 = histHeight - (int)hist.At<float>(i);
                Cv2.Line(histImage, new OpenCvSharp.Point(x1, y1), new OpenCvSharp.Point(x2, y2), color, 2);
            }
            return histImage;
        }

        public void TextandDraw()
        {
            // 이미지 읽기, 생성
            string imageFilePath = "C:/Users/Hajun/source/repos/OpenCV/SFU1_OpenCV/Resources/cat.jpg";
            Mat image = Cv2.ImRead(imageFilePath);

            Mat target0 = new Mat();
            Cv2.CvtColor(image, target0, ColorConversionCodes.BGRA2BGR);

            string text = "Hello, OpenCV"; // 이미지 위에 작성할 텍스트
            OpenCvSharp.Point textPosition = new OpenCvSharp.Point(50, 100);
            Cv2.PutText(target0, text, textPosition, HersheyFonts.HersheySimplex, 3, new Scalar(0, 255, 0), 4, LineTypes.AntiAlias);

            imageBox1.Source = OpenCvSharp.WpfExtensions.BitmapSourceConverter.ToBitmapSource(target0);
            Cv2.NamedWindow("target0", WindowFlags.Normal); // pdf에 없는 코드.
            // WindowFlags.Normal : "dst"의 이미지 크기를 창 크기에 맞게 변경.
            Cv2.ResizeWindow("target0", 600, 500);
            Cv2.ImShow("target0", target0);

        }

        public void Draw()
        {
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
        }
    }
}
