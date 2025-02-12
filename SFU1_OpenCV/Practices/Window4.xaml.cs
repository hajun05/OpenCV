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
    /// Interaction logic for Window4.xaml
    /// </summary>
    public partial class Window4 : System.Windows.Window
    {
        private Mat image;

        public Window4()
        {
            InitializeComponent();

            //_MouseCallback();
            MouseCallbackDraw();

            Cv2.WaitKey(0);
            Cv2.DestroyAllWindows();
        }

        private void MouseCallbackDraw()
        {
            // 이미지 읽기, 생성
            string imageFilePath = "C:/Users/Hajun/source/repos/OpenCV/SFU1_OpenCV/Resources/cat.jpg";
            image = Cv2.ImRead(imageFilePath);


            Cv2.NamedWindow("image", WindowFlags.Normal);
            Cv2.ResizeWindow("image", 700, 500);
            Cv2.ImShow("image", image);

            MouseCallback cvMouseCallback = new MouseCallback(MyMouseEvent);
            Cv2.SetMouseCallback("image", cvMouseCallback, image.CvPtr);
        }

        public void _MouseCallback()
        {
            Mat src = new Mat(new OpenCvSharp.Size(500, 500), MatType.CV_8UC3, new Scalar(255, 255, 255));

            Cv2.ImShow("white board", src);

            MouseCallback cvMouseCallback = new MouseCallback(MyMouseEvent);
            Cv2.SetMouseCallback("white board", cvMouseCallback, src.CvPtr);
        }

        private void MyMouseEvent(MouseEventTypes @event, int x, int y, MouseEventFlags flags, IntPtr userdate)
        {
            //Mat image = new Mat(userdate); // userdate로 이미지 참조하기 방법 없나?
            if (flags == MouseEventFlags.LButton)
            {
                //MessageBox.Show($"마우스 좌표 {x}, {y}");
                Cv2.Circle(image, x, y, 10, Scalar.Red, -1, LineTypes.AntiAlias);
                Cv2.ImShow("image", image);
            }

        }
    }
}
