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
    /// Interaction logic for Window2.xaml
    /// </summary>
    public partial class Window2 : System.Windows.Window
    {
        public Window2()
        {
            InitializeComponent();
            string imgPath = "C:/Users/Hajun/source/repos/OpenCV/SFU1_OpenCV/Resources/cat.jpg";
            Mat image = Cv2.ImRead(imgPath);
            int width = 100;
            int height = 100;

            // 빈 이미지 생성 (3채널, BGR)
            Mat randomImage = new Mat(height, width, MatType.CV_8UC3);
            Random rand = new Random();

            // 모든 픽셀에 랜덤한 BGR 값 넣기
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    byte b = (byte)rand.Next(256);  // 0~255 랜덤값
                    byte g = (byte)rand.Next(256);
                    byte r = (byte)rand.Next(256);
                    randomImage.Set(y, x, new Vec3b(b, g, r));
                }
            }

            // 저장할 파일 경로
            string outputPath = "C:/Users/Hajun/source/repos/OpenCV/SFU1_OpenCV/Resources/random_image.png";
            
            // 이미지 저장
            Cv2.ImWrite(outputPath, randomImage);
            Console.WriteLine($"이미지가 저장되었습니다: {outputPath}");
            
            // 이미지 출력 (창으로 확인)
            Cv2.ImShow("Random Image", randomImage);
            Cv2.WaitKey(0);
            Cv2.DestroyAllWindows();
        }
    }
}
