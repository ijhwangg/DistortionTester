using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OpenCvSharp;
using System.Drawing.Imaging;
using System.Diagnostics;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private float cameraSenSorSize = 3.45f;
        private float magnification = 0.367f;


        private const float deg = (float)(Math.PI / 180.0);

        private int ImageHeight = 3000;
        private int ImageWidth = 4080;
        private int ImageCH = 3;

        private float dp = 0.5f; //dot 간격 [mm]

        private List<Point2f> dotCenters = new List<Point2f>();
        private float zoomFactor = 1.0f;

        private Mat ShowImg;

        private VectorMapGenerator vMapGenerator = new VectorMapGenerator();

        public Form1()
        {
            InitializeComponent();
            //calDotarrary();
        }



        public unsafe void CalDotarrary()
        {
            float pixelSize = cameraSenSorSize / magnification;
            float rotationAngle = 0.0f;// float.Parse(RotationAngleValueBox.Text) * deg;

            //Center dot x,y 값
            float sx = 0;
            float sy = 0;
            IntPtr GrabPtr = IntPtr.Zero;
            // var temp = new Mat(@"C:\Users\진소미\Desktop\DotTarget\Top Macro.bmp");
            //GetDotCenters의 Input Image는 Gray(1 channel) Image가 되어야 함.
            var colorImg = new Mat(@"C:\Users\WTA\Desktop\[WTA]2024\#Issue\0510_왜곡보정_여백채우기\타겟2\top.bmp");
            var grayImg = colorImg.CvtColor(ColorConversionCodes.BGR2GRAY);
            var src = grayImg.DataPointer;

            //DotCenter를 찾고,            
            var dots = dll.GetDotCenters(ImageHeight, ImageWidth, src, ref sx, ref sy);


            //pixelSize = dll.MeasVec(dots, 20, pixelSize, -rotationAngle, sx, sy, dp);
            Point2f StartPoint = new Point2f(sx, sy);

            var num = dots[0];

            float x = 0, y = 0;

            for (int i = 0; i < num; i++)
            {
                x = dots[2 * i + 1];
                y = dots[2 * i + 2];
                dotCenters.Add(new Point2f(x, y));
            }



            //PixelSizeValueBox.Text = pixelSize.ToString("F3");


            Point2f tiltFactor = new Point2f();
            float maximumDiff = 0.0f;
            float vectorLength = 50.0f;

            List<Point2f> sort_point = dotCenters.OrderBy(p => p.X).ThenBy(p => p.Y).ToList();
            dotCenters = sort_point;

            vMapGenerator.DrawVectorMap(dotCenters, pixelSize, -rotationAngle, zoomFactor, StartPoint, maximumDiff, 36000, tiltFactor, vectorLength);

            vMapGenerator.DistortionCorrection(dotCenters, ref StartPoint, rotationAngle, pixelSize, maximumDiff, 36000, vectorLength);
            // var Bmps = ShowImg.ToBitmapSource();
            //vMapGenerator.ShowImg.Dispose();
            //Bmps.Freeze();
            colorImg.Dispose();
            grayImg.Dispose();
        }


        //VectorMap
        public float[] VectorX = null;

        public float[] VectorY = null;

        private void LoadVectorMap(string path)
        {
            if (!File.Exists(path))
            {
                VectorX = null;
                VectorY = null;
                return;
            }
            FileStream fs = null;
            BinaryReader br = null;
            VectorX = new float[ImageHeight * ImageWidth];
            VectorY = new float[ImageHeight * ImageWidth];

            try
            {
                fs = new FileStream(path, FileMode.Open);
                br = new BinaryReader(fs);

                for (int i = 0; i < ImageHeight; i++)
                {
                    for (int j = 0; j < ImageWidth; j++)
                    {
                        VectorX[i * ImageWidth + j] = br.ReadSingle();
                        VectorY[i * ImageWidth + j] = br.ReadSingle();
                    }
                }
            }
            catch (Exception ex)
            {
                //Global.logExcept.Write(ex.Message);
            }
            finally
            {
                if (br != null) { br.Close(); br.Dispose(); }
                if (fs != null) { fs.Close(); fs.Dispose(); }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            LoadVectorMap("_TopCorrection.dat");
            //LoadVectorMap(@"C:\Users\WTA\Desktop\[WTA]2024\#Issue\0510_왜곡보정_여백채우기\보간법 비교\Btm\_TopCorrection.dat");
            unsafe
            {
                //Bitmap te = new Bitmap(@"C:\Users\WTA\Desktop\[WTA]2024\#Issue\0510_왜곡보정_여백채우기\타겟2\btm.bmp");
                Bitmap te = new Bitmap(@"C:\Users\WTA\Desktop\[WTA]2024\#Issue\0510_왜곡보정_여백채우기\제품\하부\[0001]Btm.bmp");
                BitmapData tedata = te.LockBits(new Rectangle(0, 0, ImageWidth, ImageHeight), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                IntPtr temp_data = tedata.Scan0;

                dll.Distortion(temp_data, VectorX, VectorY, ImageWidth, ImageHeight);

                Bitmap distortion = new Bitmap(ImageWidth, ImageHeight, ImageWidth * 3, PixelFormat.Format24bppRgb, temp_data);

                te.UnlockBits(tedata);

                distortion.Save("correct_distortion.bmp", ImageFormat.Bmp);

                te.Dispose();

            }

            stopwatch.Stop();

            // 경과된 시간 가져오기
            TimeSpan elapsedTime = stopwatch.Elapsed;

            // 경과된 시간을 텍스트 박스에 표시
            correctionTime.Text = elapsedTime.TotalMilliseconds + " ms";

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            CalDotarrary();

            stopwatch.Stop();

            TimeSpan elapsedTime = stopwatch.Elapsed;

            extractTime.Text = elapsedTime.TotalMilliseconds + " ms";
        }
    }

}