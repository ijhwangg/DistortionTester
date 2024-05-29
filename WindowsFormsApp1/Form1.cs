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
using Microsoft.WindowsAPICodePack.Dialogs;

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

        private bool isHandlingEvent = false;
        private bool IsSingleMode = false;
        private bool IsTotalMode = false;
        private bool SelectVectorMap = false;

        private string VectorMapPath = "";
        private string TargetPath = "";
        private string ImagePath = "";
        private string SelectImage = "";
        private string SavePath = "DistortionResult";

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
            //var colorImg = new Mat(@"C:\Users\WTA\Desktop\[WTA]2024\#Issue\0510_왜곡보정_여백채우기\Top Macro.bmp");
            var colorImg = new Mat(TargetPath);
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

        private void BtnExtraction_Click(object sender, EventArgs e)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            if (SelectVectorMap)
            {
                LoadVectorMap(VectorMapPath);
            }
            else
            {
                LoadVectorMap("_TopCorrection.dat");
            }

            if (!IsSingleMode && !IsTotalMode)
            {
                MessageBox.Show($"검사 모드를 선택해주세요.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (IsSingleMode)
            {
                if (SelectImage == "")
                {
                    MessageBox.Show($"이미지가 선택되지 않았습니다. 이미지를 선택해주세요!.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    var gn = SelectImage.LastIndexOf("Back");
                    var ImgNum = SelectImage.Substring(0, gn);

                    var backList = ImagePath + "\\" + ImgNum + "Back.jpg";
                    var colorList = ImagePath + "\\" + ImgNum + "Color.jpg";
                    var AreaList = ImagePath + "\\" + ImgNum + "ColorArea.jpg";
                    var DFList = ImagePath + "\\" + ImgNum + "Df.jpg";

                    {
                        Bitmap te = new Bitmap(colorList);
                        BitmapData tedata = te.LockBits(new Rectangle(0, 0, ImageWidth, ImageHeight), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                        IntPtr temp_data = tedata.Scan0;

                        dll.Distortion(temp_data, VectorX, VectorY, ImageWidth, ImageHeight);

                        Bitmap distortion = new Bitmap(ImageWidth, ImageHeight, ImageWidth * 3, PixelFormat.Format24bppRgb, temp_data);
                        distortion.Save($@"{SavePath}\{ImgNum}" + "Color.bmp", ImageFormat.Bmp);
                        te.UnlockBits(tedata);
                        te.Dispose();
                    }
                    {
                        Bitmap te = new Bitmap(AreaList);
                        BitmapData tedata = te.LockBits(new Rectangle(0, 0, ImageWidth, ImageHeight), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                        IntPtr temp_data = tedata.Scan0;

                        dll.Distortion(temp_data, VectorX, VectorY, ImageWidth, ImageHeight);

                        Bitmap distortion = new Bitmap(ImageWidth, ImageHeight, ImageWidth * 3, PixelFormat.Format24bppRgb, temp_data);
                        distortion.Save($@"{SavePath}\{ImgNum}" + "ColorArea.bmp", ImageFormat.Bmp);
                        te.UnlockBits(tedata);
                        te.Dispose();
                    }
                    {
                        Bitmap te = new Bitmap(DFList);
                        BitmapData tedata = te.LockBits(new Rectangle(0, 0, ImageWidth, ImageHeight), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                        IntPtr temp_data = tedata.Scan0;

                        dll.Distortion(temp_data, VectorX, VectorY, ImageWidth, ImageHeight);

                        Bitmap distortion = new Bitmap(ImageWidth, ImageHeight, ImageWidth * 3, PixelFormat.Format24bppRgb, temp_data);
                        distortion.Save($@"{SavePath}\{ImgNum}" + "Df.bmp", ImageFormat.Bmp);
                        te.UnlockBits(tedata);
                        te.Dispose();
                    }
                }
            }
            else
            {
                var backList = Directory.GetFiles(ImagePath, "*Back*.jpg");
                var colorList = Directory.GetFiles(ImagePath, "*Color*.jpg");
                var AreaList = Directory.GetFiles(ImagePath, "*ColorArea*.jpg");
                var DFList = Directory.GetFiles(ImagePath, "*Df*.jpg");

                for (int i = 0; i < backList.Length; i++)
                {
                    var gn = backList[i].LastIndexOf("Back");
                    var ImgNum = backList[i].Substring(gn - 9, 9);

                    {
                        Bitmap te = new Bitmap(backList[i]);
                        BitmapData tedata = te.LockBits(new Rectangle(0, 0, ImageWidth, ImageHeight), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                        IntPtr temp_data = tedata.Scan0;

                        dll.Distortion(temp_data, VectorX, VectorY, ImageWidth, ImageHeight);

                        Bitmap distortion = new Bitmap(ImageWidth, ImageHeight, ImageWidth * 3, PixelFormat.Format24bppRgb, temp_data);
                        distortion.Save($@"{SavePath}\{ImgNum}" + "Back.bmp", ImageFormat.Bmp);
                        te.UnlockBits(tedata);
                        te.Dispose();
                    }
                    {
                        Bitmap te = new Bitmap(colorList[i]);
                        BitmapData tedata = te.LockBits(new Rectangle(0, 0, ImageWidth, ImageHeight), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                        IntPtr temp_data = tedata.Scan0;

                        dll.Distortion(temp_data, VectorX, VectorY, ImageWidth, ImageHeight);

                        Bitmap distortion = new Bitmap(ImageWidth, ImageHeight, ImageWidth * 3, PixelFormat.Format24bppRgb, temp_data);
                        distortion.Save($@"{SavePath}\{ImgNum}" + "Color.bmp", ImageFormat.Bmp);
                        te.UnlockBits(tedata);
                        te.Dispose();
                    }
                    {
                        Bitmap te = new Bitmap(AreaList[i]);
                        BitmapData tedata = te.LockBits(new Rectangle(0, 0, ImageWidth, ImageHeight), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                        IntPtr temp_data = tedata.Scan0;

                        dll.Distortion(temp_data, VectorX, VectorY, ImageWidth, ImageHeight);

                        Bitmap distortion = new Bitmap(ImageWidth, ImageHeight, ImageWidth * 3, PixelFormat.Format24bppRgb, temp_data);
                        distortion.Save($@"{SavePath}\{ImgNum}" + "ColorArea.bmp", ImageFormat.Bmp);
                        te.UnlockBits(tedata);
                        te.Dispose();
                    }
                    {
                        Bitmap te = new Bitmap(DFList[i]);
                        BitmapData tedata = te.LockBits(new Rectangle(0, 0, ImageWidth, ImageHeight), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                        IntPtr temp_data = tedata.Scan0;

                        dll.Distortion(temp_data, VectorX, VectorY, ImageWidth, ImageHeight);

                        Bitmap distortion = new Bitmap(ImageWidth, ImageHeight, ImageWidth * 3, PixelFormat.Format24bppRgb, temp_data);
                        distortion.Save($@"{SavePath}\{ImgNum}" + "Df.bmp", ImageFormat.Bmp);
                        te.UnlockBits(tedata);
                        te.Dispose();
                    }
                }

            }

            stopwatch.Stop();

            // 경과된 시간 가져오기
            TimeSpan elapsedTime = stopwatch.Elapsed;

            // 경과된 시간을 텍스트 박스에 표시
            correctionTime.Text = elapsedTime.TotalMilliseconds + " ms";
        }

        private void BtnDistortionRun_Click(object sender, EventArgs e)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            if (!(Directory.Exists(SavePath)))
            {
                Directory.CreateDirectory(SavePath);
            }

            CalDotarrary();

            stopwatch.Stop();

            TimeSpan elapsedTime = stopwatch.Elapsed;

            extractTime.Text = elapsedTime.TotalMilliseconds + " ms";
        }

        private void LoadFileBtn_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog folderDialog = new CommonOpenFileDialog()
            { InitialDirectory = "", IsFolderPicker = true };
            {
                if (folderDialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    FilePathText.Text = folderDialog.FileName;

                    ImagePath = folderDialog.FileName;
                    ImageListBox.Items.Clear();

                    string[] files = Directory.GetFiles(FilePathText.Text);

                    foreach (string file in files)
                    {
                        ImageListBox.Items.Add(Path.GetFileName(file));
                    }
                }
            }
        }

        private void ChkSingle_CheckedChanged(object sender, EventArgs e)
        {
            IsSingleMode = true;
            if (ChkSingle.Checked)
            {
                ChkTotal.Checked = false;
                IsTotalMode = false;
            }        
        }

        private void ChkTotal_CheckedChanged(object sender, EventArgs e)
        {
            IsTotalMode = true;
            if (ChkTotal.Checked)
            {
                ChkSingle.Checked = false;
                IsSingleMode = false;
            }
        }

        private void BtnTarget_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog TargetFileDialog = new CommonOpenFileDialog()
            { InitialDirectory = "", IsFolderPicker = false };
            {
                if (TargetFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    TextTargetPath.Text = Path.GetFileName(TargetFileDialog.FileName);

                    TargetPath = TargetFileDialog.FileName;
                }
            }
        }

        private void ImageListBox_Select(object sender, EventArgs e)
        {
            if (ImageListBox.SelectedItem != null)
            {
                SelectImage = ImageListBox.SelectedItem.ToString();
            }
        }

        private void BtnVectorMap_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog VectorMapFileDialog = new CommonOpenFileDialog()
            { InitialDirectory = "", IsFolderPicker = false };
            {
                if (VectorMapFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    TextVectorMap.Text = Path.GetFileName(VectorMapFileDialog.FileName);

                    VectorMapPath = VectorMapFileDialog.FileName;

                    SelectVectorMap = true;
                }
            }
        }
    }
}
