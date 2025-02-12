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
    /// Interaction logic for Window6.xaml
    /// </summary>
    public partial class Window6 : System.Windows.Window
    {
        public Window6()
        {
            InitializeComponent();

            EdgeDetection();

            Cv2.WaitKey(0);
            Cv2.DestroyAllWindows();
        }

        public void EdgeDetection()
        {
            string imagePath = "C:\\Users\\Hajun\\source\\repos\\OpenCV\\SFU1_OpenCV\\Resources\\cat.jpg";
            Mat image = Cv2.ImRead(imagePath);
            Cv2.Resize(image, image, new OpenCvSharp.Size(image.Size().Width / 4, image.Size().Height / 4));

            // 1. Sobel 에지 검출.
            // Sobel 필터는 가로(X)와 세로(Y) 방향에서 미분을 수행하여 엣지를 검출.
            // 부드러운 엣지를 잡아내는 데 효과적이며 노이즈에 강함.
            // 작은 커널(마스크) 사용시 미세한 엣지를 검출하지 못함.
            Mat sobelX = new Mat(), sobelY = new Mat(), sobel = new Mat(); 
            Cv2.Sobel(image, sobelX, MatType.CV_64F, 1, 0, ksize: 3); 
            Cv2.Sobel(image, sobelY, MatType.CV_64F, 0, 1, ksize: 3); 
            Cv2.ConvertScaleAbs(sobelX, sobelX); 
            Cv2.ConvertScaleAbs(sobelY, sobelY);
            Cv2.AddWeighted(sobelX, 0.5, sobelY, 0.5, 0, sobel);

            // 2. Scharr 에지 검출
            // Sobel보다 더 강한 엣지 강조.
            // 작은 커널 크기에서도 선명한 엣지 검출 가능.
            // Sobel보다 노이즈에 민감
            Mat scharrX = new Mat(), scharrY = new Mat(), scharr = new Mat();
            Cv2.Sobel(image, scharrX, MatType.CV_64F, 1, 0);
            Cv2.Sobel(image, scharrY, MatType.CV_64F, 0, 1);
            Cv2.ConvertScaleAbs(scharrX, scharrX);
            Cv2.ConvertScaleAbs(scharrY, scharrY);
            Cv2.AddWeighted(scharrX, 0.5, scharrY, 0.5, 0, scharr);

            // 3. Laplacian 에지 검출
            // 2차 미분을 사용하여 엣지의 변화가 급격한 부분을 강조
            // 엣지의 강도를 강조하지만 노이즈에 매우 민감.
            // 주로 Gaussian Blur와 함께 사용하여 노이즈를 줄이고 엣지를 강조.
            Mat laplacian = new Mat();
            Cv2.Laplacian(image, laplacian, MatType.CV_64F);
            Cv2.ConvertScaleAbs(laplacian, laplacian);

            // 4. Canny 에지 검출
            // 가장 정교한 엣지 검출을 수행.
            // Sobel 필터 + 비최대 억제 + 히스트리시스 에지 트래킹(Hysteresis Thresholding) 결합
            // 비최대 억제 : 특정 에지 픽셀과 그래디언트 벡터의 방향이 같은 인접 에지의 그래디언트 크기 비교, 최대인 픽셀만을 에지로 설정.
            // 히스트리시스 에지 트래킹 : 상위, 하위 2개 임계값 설정. 두 임계값 사이의 픽셀값만 에지로 설정.
            // Gaussian Blur와 결합하여 노이즈 제거 후 사용.
            // 임계값을 조절하여 원하는 강도의 엣지를 검출 가능.
            // 검출이 정교하지만 그만큼 많은 시간 소요.
            Mat canny = new Mat();
            Cv2.Canny(image, canny, 50, 150);

            // 5. 결과 출력
            Cv2.ImShow("Original Image", image);
            Cv2.NamedWindow("Sobel Edge", WindowFlags.Normal);
            Cv2.ImShow("Sobel Edge", sobel);
            Cv2.ImShow("Scharr Edge", scharr);
            Cv2.ImShow("Laplacian Edge", laplacian);
            Cv2.ImShow("Canny Edge", canny);
        }
    }
}
