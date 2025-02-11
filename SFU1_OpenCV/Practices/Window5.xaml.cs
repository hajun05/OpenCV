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
using System.Windows.Shapes;

namespace SFU1_OpenCV.Practices
{
    /// <summary>
    /// Interaction logic for Window5.xaml
    /// </summary>
    public partial class Window5 : System.Windows.Window
    {
        Mat frame;

        public Window5()
        {
            InitializeComponent();

            // (n) : 연결된 디바이스 id, 윈도우에서 지정. 문자열 : 영상 파일 경로
            VideoCapture capture = new VideoCapture(0); 
            frame = new Mat(); // 한 프레임
            Mat gray = new Mat(); // 흑백 이미지
            Mat hist = new Mat(); // 히스토그램
            int[] histSize = { 256 }; // 히스토그램 크기 (256단계)
            Rangef[] ranges = { new Rangef(0, 256) }; // 픽셀 값 범위 (0~255)

            // 카메라 영상 해상도 설정
            capture.Set(VideoCaptureProperties.FrameWidth, 640);
            capture.Set(VideoCaptureProperties.FrameHeight, 480);

            // 마우스 이벤트 등록
            MouseCallback cvMouseCallback = new MouseCallback(MyMouseEvent);

            while (true)
            {
                if (capture.IsOpened())
                {
                    capture.Read(frame);

                    Cv2.CvtColor(frame, gray, ColorConversionCodes.BGR2GRAY);
                    Cv2.CalcHist(new Mat[] { gray }, new int[] { 0 }, null, hist, 1, histSize, ranges);
                    Mat histImage = DrawHistogram(hist, new Scalar(255, 255, 255));

                    // 세로로 이미지 붙이기
                    Cv2.VConcat(new Mat[] { frame, histImage }, frame);

                    Cv2.ImShow("Camera View", frame);
                    Cv2.SetMouseCallback("Camera View", cvMouseCallback, frame.CvPtr);

                    // q 버튼을 누르면 출력 종료
                    if (Cv2.WaitKey(33) == 'q') break;
                }
            }

            // 카메라 장치와 연결 해제
            capture.Release();
            Cv2.DestroyAllWindows();
        }

        private bool mousePosCircle = false;
        private void MyMouseEvent(MouseEventTypes @event, int x, int y, MouseEventFlags flags, IntPtr userdate)
        {
            //Mat image = new Mat(userdate); // userdate로 이미지 참조하기 방법 없나?
            if (@event == MouseEventTypes.LButtonDown)
                mousePosCircle = true;
            else if (@event == MouseEventTypes.LButtonUp)
                mousePosCircle = false;

            if (mousePosCircle == true)
            {
                //MessageBox.Show($"마우스 좌표 {x}, {y}");
                Cv2.Circle(frame, x, y, 10, Scalar.Red, -1, LineTypes.AntiAlias);

                Cv2.ImShow("Camera View", frame);
            }
        }

        static Mat DrawHistogram(Mat hist, Scalar color)
        {
            int histWidth = 640, histHeight = 120;
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
    }
}
