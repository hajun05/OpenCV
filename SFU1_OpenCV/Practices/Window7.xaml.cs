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
using OpenCvSharp;

namespace SFU1_OpenCV.Practices
{
    /// <summary>
    /// Interaction logic for Window7.xaml
    /// </summary>
    public partial class Window7 : System.Windows.Window
    {
        // 이미지 윤곽선 그리기
        // 윤곽선(Contour) : Edge를 모아서 선을 이루고, 선들이 모여 특정 도형을 이루는 것으로 분류된 것
        public Window7()
        {
            InitializeComponent();

            string imagePath = "C:\\Users\\Hajun\\source\\repos\\OpenCV\\SFU1_OpenCV\\Resources\\cat.jpg";
            Mat src = Cv2.ImRead(imagePath);
            Cv2.Resize(src, src, new OpenCvSharp.Size(src.Size().Width / 4, src.Size().Height / 4));

            //Contour1(src);
            //Contour2(src);
            Contour3(src);

            Cv2.WaitKey(0);
            Cv2.DestroyAllWindows();
        }

        public void Contour1(Mat src)
        {
            Mat gray = new Mat();
            Mat binary = new Mat();
            Mat morp = new Mat();
            Mat image = new Mat();
            Mat dst = src.Clone();

            Mat kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(3, 3));

            OpenCvSharp.Point[][] controus;
            HierarchyIndex[] hierarchy;

            Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);
            Cv2.Threshold(gray, binary, 230, 255, ThresholdTypes.Binary);
            Cv2.MorphologyEx(binary, morp, MorphTypes.Close, kernel, new OpenCvSharp.Point(-1, -1), 2);
            Cv2.BitwiseNot(morp, image);

            Cv2.FindContours(image, out controus, out hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxTC89KCOS);
            Cv2.DrawContours(dst, controus, -1, new Scalar(255, 0, 0), 2, LineTypes.AntiAlias, hierarchy, 3);

            for (int i = 0; i < controus.Length; i++)
            {
                for (int j = 0; j < controus[i].Length; j++)
                {
                    Cv2.Circle(dst, controus[i][j], 1, new Scalar(0, 0, 255), 3);
                }
            }

            Cv2.ImShow("dst", dst);
        }

        public void Contour2(Mat image)
        {
            // 그레이스케일 변환 (CV_8UC1 이미지만 허용)
            Mat gray = new Mat();
            Cv2.CvtColor(image, gray, ColorConversionCodes.BGR2GRAY);
            // 이미지 이진화 (Thresholding)
            Mat binary = new Mat();
            Cv2.Threshold(gray, binary, 100, 255, ThresholdTypes.Binary);
            // 윤곽선 검출 (CV_8UC1 이미지 사용)
            OpenCvSharp.Point[][] contours;
            HierarchyIndex[] hierarchy;
            Cv2.FindContours(binary, out contours, out hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxSimple);
            // 윤곽선 그리기 (컬러 이미지 위에 그리기)
            Mat contourImage = image.Clone();
            Cv2.DrawContours(contourImage, contours, -1, new Scalar(0, 0, 255), 2);
            // 결과 출력
            Cv2.ImShow("Original Image", image);
            Cv2.ImShow("Binary Image", binary);
            Cv2.ImShow("Contour Detection", contourImage);
        }

        public void Contour3(Mat src)
        {
            Mat gray = new Mat();
            Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY); // 그레이스케일 변환
            Mat imageCanny = new Mat(), imageThresh = new Mat(), imageAdaptive = new Mat();
            // Canny 엣지 검출 적용
            Cv2.Canny(gray, imageCanny, 100, 200);
            // 기본 Threshold 적용
            Cv2.Threshold(gray, imageThresh, 127, 255, ThresholdTypes.Binary);
            // Adaptive Threshold 적용
            Cv2.AdaptiveThreshold(gray, imageAdaptive, 255,
                        AdaptiveThresholdTypes.GaussianC, ThresholdTypes.Binary, 11, 2);
            // Contour 검출
            Mat cannyContours = DrawContoursOnImage(src.Clone(), imageCanny, "Canny");
            Mat threshContours = DrawContoursOnImage(src.Clone(), imageThresh, "Threshold");
            Mat adaptiveContours = DrawContoursOnImage(src.Clone(), imageAdaptive, "Adaptive Threshold");
            // 5. 결과 출력
            Cv2.ImShow("Original Image", src);
            Cv2.ImShow("Canny Edge Detection", cannyContours);
            Cv2.ImShow("Binary Threshold", threshContours);
            Cv2.ImShow("Adaptive Threshold", adaptiveContours);
        }
        static Mat DrawContoursOnImage(Mat image, Mat binary, string title)
        {
            OpenCvSharp.Point[][] contours;
            HierarchyIndex[] hierarchy;
            // 컨투어 찾기
            Cv2.FindContours(binary, out contours, out hierarchy,
                        RetrievalModes.External, ContourApproximationModes.ApproxSimple);
            // 컨투어 그리기 (초록색)
            Cv2.DrawContours(image, contours, -1, new Scalar(0, 255, 0), 2);
            // 컨투어의 좌표값을 이용하여 가장 왼쪽, 오른쪽, 위쪽, 아래쪽 점 찾기
            foreach (var contour in contours)
            {
                OpenCvSharp.Point extLeft = contour[0], extRight = contour[0],
                                extTop = contour[0], extBottom = contour[0];
                foreach (var pt in contour)
                {
                    if (pt.X < extLeft.X) extLeft = pt;
                    if (pt.X > extRight.X) extRight = pt;
                    if (pt.Y < extTop.Y) extTop = pt;
                    if (pt.Y > extBottom.Y) extBottom = pt;
                }
                // 빨간색 (왼쪽), 초록색 (오른쪽), 파란색 (위쪽), 노란색 (아래쪽) 점 표시
                Cv2.Circle(image, extLeft, 8, new Scalar(0, 0, 255), -1);
                Cv2.Circle(image, extRight, 8, new Scalar(0, 255, 0), -1);
                Cv2.Circle(image, extTop, 8, new Scalar(255, 0, 0), -1);
                Cv2.Circle(image, extBottom, 8, new Scalar(255, 255, 0), -1);
            }
            return image;
        }
    }
}
