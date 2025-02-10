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

namespace SFU1_OpenCV
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : System.Windows.Window
    {
        public Window1()
        {
            InitializeComponent();

            string imageFilePath = "C:/Users/Hajun/source/repos/OpenCV/SFU1_OpenCV/Resources/cat.jpg";
            // Mat = Matrix = 2차원 배열(행렬)
            // 컴퓨터 비전에선 주로 행렬을 통해 2차원 이미지를 데이터화
            Mat src = Cv2.ImRead(imageFilePath); // 원본 이미지
            Mat gray = new Mat(); // 흑백 이미지
            Mat hist = new Mat(); // 히스토그램

            // 가로 256, 세로 이미지 크기만큼 1로만 가득 차 있는 행렬 생성
            Mat result = Mat.Ones(new OpenCvSharp.Size(256, src.Height), MatType.CV_8UC1); // CV_8UC1 : 8bit 1channel 이미지
            Mat dst = new Mat();

            Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);

            // 히스토그램 값 생성
            // 원본 행렬, 채널 수, 이미지 마스크(이미지 출력 틀), 출력 행렬, 차원 수, 히스토그램 크기(가로), 각 차원에 대한 색상값 범위
            Cv2.CalcHist(new Mat[] { gray }, new int[] { 0 }, null, hist, 1, new int[] { 256 },
                new Rangef[] { new Rangef(0, 256) });

            // 값의 범위를 정규화(최소 n ~ 최대 m -> 최소 0 ~ 최대 255로 변환)
            // 원본 행렬, 출력 행렬, 최소값, 최대값, 변환 방식
            Cv2.Normalize(hist, hist, 0, 255, NormTypes.MinMax);

            // 히스토그램 값을 갖고 값의 크기만큼 선의 길이를 정하고 선을 그림
            for(int i = 0; i < hist.Rows; i++)
            {
                Cv2.Line(result, new OpenCvSharp.Point(i, src.Height),
                    new OpenCvSharp.Point(i, src.Height - hist.Get<float>(i) * src.Height * 0.5), Scalar.White);
                // * src.Height * 0.5 : 히스토그램 크기를 이미지의 절반 크기로 확대. 없으면 너무 작아서 안보임
            }

            // 가로로 원본 이미지, 히스토그램 행렬을 붙이기(총 높이가 같아야 가능)
            Cv2.HConcat(new Mat[] { gray, result }, dst);
            Cv2.NamedWindow("dst", WindowFlags.Normal); // pdf에 없는 코드.
            // WindowFlags.Normal : "dst"의 이미지 크기를 창 크기에 맞게 변경.
            Cv2.ImShow("dst", dst);

            // 사용자 입력이 들어오면 창을 모두 닫기
            Cv2.WaitKey(0);
            Cv2.DestroyAllWindows();
        }
    }
}
