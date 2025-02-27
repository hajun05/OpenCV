﻿using System;
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
using System.Drawing;
using OpenCvSharp.Extensions;
using Tesseract;
using OpenCvSharp;
using System.IO;
using System.Net.Http;

namespace SFU1_OpenCV.Practices
{
    /// <summary>
    /// Interaction logic for Window8.xaml
    /// </summary>
    public partial class Window8 : System.Windows.Window
    {
        public Window8()
        {
            InitializeComponent();

            InitializeAsync();
        }
        private async void InitializeAsync()
        {
            try
            {
                string imgPath = "C:\\Users\\Hajun\\source\\repos\\OpenCV\\SFU1_OpenCV\\Resources\\card.png";
                Mat src = Cv2.ImRead(imgPath);

                OpenCvSharp.Point[] squares = Square(src);
                Mat square = DrawSquare(src, squares);

                Cv2.ImShow("square", square);

                Mat dst = PerspectiveTransform(src, squares);
                // OCR 메서드를 비동기적으로 호출
                string texts = await OCR(dst);

                Console.WriteLine(texts);

                textBox.Text = texts;

                Cv2.ImShow("dst", dst);
                Cv2.WaitKey(0);
                Cv2.DestroyAllWindows();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error during initialization: " + ex.Message);
            }
        }

        static double Angle(OpenCvSharp.Point pt1, OpenCvSharp.Point pt0, OpenCvSharp.Point pt2)
        {
            double u1 = pt1.X - pt0.X;
            double u2 = pt1.Y - pt0.Y;
            double v1 = pt2.X - pt0.X;
            double v2 = pt2.Y - pt0.Y;

            return ((u1 * v1 + u2 * v2) / (Math.Sqrt(u1 * u1 + u2 * u2) * Math.Sqrt(v1 * v1 + v2 * v2)));
        }

        public static OpenCvSharp.Point[] Square(Mat src)
        {
            Mat[] split = Cv2.Split(src);
            Mat blur = new Mat();
            Mat binary = new Mat();

            OpenCvSharp.Point[] squares = new OpenCvSharp.Point[4];

            int N = 10;
            double max = src.Size().Width * src.Size().Height * 0.9;
            double min = src.Size().Width * src.Size().Height * 0.1;

            for (int channel = 0; channel < 3; channel++)
            {
                Cv2.GaussianBlur(split[channel], blur, new OpenCvSharp.Size(5, 5), 1);
                for (int i = 0; i < N; i++)
                {
                    Cv2.Threshold(blur, binary, i * 255 / N, 255, ThresholdTypes.Binary);

                    OpenCvSharp.Point[][] contours;
                    HierarchyIndex[] hierarchy;
                    Cv2.FindContours(binary, out contours, out hierarchy, RetrievalModes.List, ContourApproximationModes.ApproxTC89KCOS);

                    Mat test = src.Clone();
                    Cv2.DrawContours(test, contours, -1, new Scalar(0, 0, 255), 3);

                    for (int j = 0; j < contours.Length; j++)
                    {
                        double perimeter = Cv2.ArcLength(contours[j], true);
                        OpenCvSharp.Point[] result = Cv2.ApproxPolyDP(contours[j], perimeter * 0.02, true);

                        double area = Cv2.ContourArea(result);
                        bool convex = Cv2.IsContourConvex(result);

                        if (result.Length == 4 && area > min && area < max && convex)
                        {
                            double cos = 0;
                            for (int k = 1; k < 5; k++)
                            {
                                double t = Math.Abs(Angle(result[(k - 1) % 4], result[k % 4], result[(k + 1) % 4]));
                                cos = cos > t ? cos : t;
                            }
                            if (cos < 0.15)
                            {
                                squares = result;
                            }
                        }
                    }
                }
            }
            return (squares);
        }
        public static Mat DrawSquare(Mat src, OpenCvSharp.Point[] squares)
        {
            Mat drawsquare = src.Clone();

            OpenCvSharp.Point[][] pts = new OpenCvSharp.Point[][] { squares };
            Cv2.Polylines(drawsquare, pts, true, Scalar.Yellow, 3, LineTypes.AntiAlias, 0);
            return (drawsquare);
        }

        public static Mat PerspectiveTransform(Mat src, OpenCvSharp.Point[] squares)
        {
            Mat dst = new Mat();
            Moments moments = Cv2.Moments(squares);
            double cX = moments.M10 / moments.M00;
            double cY = moments.M01 / moments.M00;

            Point2f[] src_pts = new Point2f[4];
            for (int i = 0; i < squares.Length; i++)
            {
                if (cX > squares[i].X && cY > squares[i].Y)
                {
                    src_pts[0] = squares[i];
                }
                if (cX > squares[i].X && cY < squares[i].Y)
                {
                    src_pts[1] = squares[i];
                }
                if (cX < squares[i].X && cY > squares[i].Y)
                {
                    src_pts[2] = squares[i];
                }
                if (cX < squares[i].X && cY < squares[i].Y)
                {
                    src_pts[3] = squares[i];
                }
            }

            Point2f[] dst_pts = new Point2f[4]
            {
                new Point2f(0, 0),
                new Point2f(0, src.Height),
                new Point2f(src.Width, 0),
                new Point2f(src.Width, src.Height)
            };

            Mat matrix = Cv2.GetPerspectiveTransform(src_pts, dst_pts);

            Cv2.WarpPerspective(src, dst, matrix, new OpenCvSharp.Size(src.Width, src.Height));
            return (dst);
        }

        public static async Task<string> OCR(Mat src)
        {
            try
            {
                Bitmap bitmap = src.ToBitmap();
                string tessdataFolderPath = @"C:/workspace/tessdata";
                string filePath = System.IO.Path.Combine(tessdataFolderPath, "eng.traineddata");

                if (File.Exists(filePath))
                {
                    Console.WriteLine("File exists.");
                }
                else
                {
                    Console.WriteLine("File does not exist. Downloading...");

                    // tessdata 폴더가 존재하지 않으면 생성
                    if (!Directory.Exists(tessdataFolderPath))
                    {
                        Directory.CreateDirectory(tessdataFolderPath);
                    }

                    // Trained data 파일 다운로드
                    await DownloadTrainedData(tessdataFolderPath);
                }

                using (var ocr = new TesseractEngine(tessdataFolderPath, "eng", EngineMode.Default))
                {
                    using (var texts = ocr.Process(bitmap))
                    {
                        string sentence = texts.GetText();
                        return sentence;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: ", e);
                return e.Message;
            }
        }

        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }

        private const string AppDataFolderName = "YourAppName";
        private const string TessdataFolderName = "tessdata";

        /// <summary>
        /// Gets the path to Blitz's directory in the AppData folder.
        /// </summary>
        /// <returns>The application directory path.</returns>
        public string GetAppDataFolderPath()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return System.IO.Path.Combine(appDataPath, AppDataFolderName);
        }

        /// <summary>
        /// Gets the path to Blitz's tessdata directory.
        /// </summary>
        /// <returns>The tessdata path.</returns>
        public string GetTessdataFolderPath()
        {
            return System.IO.Path.Combine(GetAppDataFolderPath(), TessdataFolderName);
        }

        private static async Task DownloadTrainedData(string tessdataFolderPath)
        {
            const string tessdataEngFileName = "eng.traineddata";
            const string tessdataEngUrl = "https://github.com/tesseract-ocr/tessdata_fast/raw/main/eng.traineddata";

            using (var client = new HttpClient())
            {
                var stream = await client.GetStreamAsync(tessdataEngUrl);
                var filePath = System.IO.Path.Combine(tessdataFolderPath, tessdataEngFileName);

                using (var fs = new FileStream(filePath, FileMode.OpenOrCreate))
                {
                    await stream.CopyToAsync(fs);
                }
            }
        }
    }
}
