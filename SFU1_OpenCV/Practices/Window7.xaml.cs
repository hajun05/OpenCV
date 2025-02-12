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
            Mat gray = new Mat();
            Mat binary = new Mat();
            Mat morp= new Mat();
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

            Cv2.WaitKey(0);
            Cv2.DestroyAllWindows();
        }
    }
}
