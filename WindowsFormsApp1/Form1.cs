﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OpenCvSharp;
using System.Drawing.Imaging;
using System.Diagnostics;
using OpenCvSharp.Extensions;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory", SetLastError = false)]
        static extern void CopyMemory(IntPtr dest, IntPtr src, int count);

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
        private string SavePath = "왜곡보정후";

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
            if (!(Directory.Exists(ImagePath + "\\" + SavePath)))
            {
                Directory.CreateDirectory(ImagePath + "\\" + SavePath);
            }
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
                    string imgPath = ImagePath + "\\" + SelectImage;
                    var gn = SelectImage.LastIndexOf("Btm");
                    var ImgNum = SelectImage.Substring(0, gn);

                    var i_n = SelectImage.LastIndexOf("]");
                    var d_n = SelectImage.LastIndexOf(".");
                    var img_name = SelectImage.Substring(i_n + 1, (d_n - i_n) - 1);


                    var undistortion = CorrectDistortionForMeasure(imgPath, 1, VectorX, VectorY, ImageWidth, ImageHeight);
                    Mat undistortion_meas = BitmapConverter.ToMat(undistortion);

                    Mat undistortion_blu = CorrectDistortionForDefect(imgPath, 1, VectorX, VectorY, ImageWidth, ImageHeight);
                    Mat undistortion_bf = CorrectDistortionForDefect(imgPath, 3, VectorX, VectorY, ImageWidth, ImageHeight);
                    Mat undistortion_area = CorrectDistortionForDefect(imgPath, 2, VectorX, VectorY, ImageWidth, ImageHeight);
                    Mat undistortion_df = CorrectDistortionForDefect(imgPath, 4, VectorX, VectorY, ImageWidth, ImageHeight);

                    SaveBitmapsToTiff($@"{ImagePath}\{SavePath}\{ImgNum}{img_name}.tif", undistortion_meas, undistortion_blu, undistortion_area, undistortion_bf, undistortion_df);
                }
            }
            else
            {
                string[] img_list = GetImageFiles(ImagePath, new string[] { "*.tiff", "*.bmp", "*.jpg" });
                for (int i = 0; i < 10; i++)
                {
                    Console.WriteLine($@"{i} 이미지 왜곡 보정 시작");

                    var i_n = img_list[i].LastIndexOf("]");
                    var d_n = img_list[i].LastIndexOf(".");
                    var img_name = img_list[i].Substring(i_n + 1, (d_n - i_n) - 1);
                    var ImgNum = img_list[i].Substring(i_n - 5, 6);

                    var undistortion = CorrectDistortionForMeasure(img_list[i], 1, VectorX, VectorY, ImageWidth, ImageHeight);
                    Mat undistortion_meas = BitmapConverter.ToMat(undistortion);

                    Mat undistortion_blu = CorrectDistortionForDefect(img_list[i], 1, VectorX, VectorY, ImageWidth, ImageHeight);
                    Mat undistortion_bf = CorrectDistortionForDefect(img_list[i], 3, VectorX, VectorY, ImageWidth, ImageHeight);
                    Mat undistortion_area = CorrectDistortionForDefect(img_list[i], 2, VectorX, VectorY, ImageWidth, ImageHeight);
                    Mat undistortion_df = CorrectDistortionForDefect(img_list[i], 4, VectorX, VectorY, ImageWidth, ImageHeight);

                    SaveBitmapsToTiff($@"{ImagePath}\{SavePath}\{ImgNum}{img_name}.tif", undistortion_meas, undistortion_blu, undistortion_area, undistortion_bf, undistortion_df);
                    Console.WriteLine($@"{i} 이미지 왜곡 보정 완료");
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
                TargetFileDialog.Filters.Add(new CommonFileDialogFilter("Image Files", "*.bmp;*.jpg"));

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
                VectorMapFileDialog.Filters.Add(new CommonFileDialogFilter("DAT Files", "*.dat"));

                if (VectorMapFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    TextVectorMap.Text = Path.GetFileName(VectorMapFileDialog.FileName);

                    VectorMapPath = VectorMapFileDialog.FileName;

                    SelectVectorMap = true;
                }
            }
        }

        static string[] GetImageFiles(string directoryPath, string[] searchPatterns)
        {
            List<string> files = new List<string>();

            // 각 검색 패턴에 대해 파일을 가져옵니다.
            foreach (string pattern in searchPatterns)
            {
                files.AddRange(Directory.GetFiles(directoryPath, pattern));
            }

            return files.ToArray();
        }

        public static Mat CorrectDistortionForDefect(string img_path, int index, float[] vectorX, float[] vectorY, int imageWidth, int imageHeight)
        {
            using (Bitmap tiffImg = (Bitmap)Image.FromFile(img_path))
            {
                tiffImg.SelectActiveFrame(FrameDimension.Page, index);
                Mat te = BitmapConverter.ToMat(tiffImg);

                Cv2.ImWrite("te.jpg", te);
                Mat blueChannel, greenChannel, redChannel;
                SplitChannels(te, out blueChannel, out greenChannel, out redChannel);

                // Assuming dll.ChDistortion modifies the data in place
                dll.ChDistortion(blueChannel.Data, vectorX, vectorY, imageWidth, imageHeight);
                dll.ChDistortion(greenChannel.Data, vectorX, vectorY, imageWidth, imageHeight);
                dll.ChDistortion(redChannel.Data, vectorX, vectorY, imageWidth, imageHeight);

                // Save the undistorted channels to debug if needed
                Cv2.ImWrite("undistortedRImg.jpg", redChannel);
                Cv2.ImWrite("undistortedGImg.jpg", greenChannel);
                Cv2.ImWrite("undistortedBImg.jpg", blueChannel);

                // Merge the corrected channels back into a single 3-channel image
                Mat undistortedImg = MergeChannels(blueChannel, greenChannel, redChannel);

                // Save the final merged image
                Cv2.ImWrite("undistortedImg.jpg", undistortedImg);

                return undistortedImg;
            }
        }

        public static void SplitChannels(Mat image, out Mat blueChannel, out Mat greenChannel, out Mat redChannel)
        {
            // Create an array to hold the individual channel matrices
            Mat[] channels = new Mat[3];

            // Split the image into individual channels
            Cv2.Split(image, out channels);

            // Assign the channels to the output variables
            blueChannel = channels[0];
            greenChannel = channels[1];
            redChannel = channels[2];
        }

        public static Mat MergeChannels(Mat blueChannel, Mat greenChannel, Mat redChannel)
        {
            // Create an array to hold the individual channel matrices
            Mat[] channels = new Mat[] { blueChannel, greenChannel, redChannel };

            // Merge the channels into a single image
            Mat mergedImage = new Mat();
            Cv2.Merge(channels, mergedImage);

            return mergedImage;
        }

        private static Bitmap CorrectDistortionForMeasure(string img_path, int index, float[] vectorX, float[] vectorY, int imageWidth, int imageHeight)
        {
            using (Bitmap tiffImg = (Bitmap)Image.FromFile(img_path))
            {
                tiffImg.SelectActiveFrame(FrameDimension.Page, index);

                Bitmap te = new Bitmap(tiffImg);

                BitmapData tedata = te.LockBits(new Rectangle(0, 0, imageWidth, imageHeight), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                IntPtr tempData = tedata.Scan0;

                dll.Distortion(tempData, vectorX, vectorY, imageWidth, imageHeight);

                Bitmap undistorted_img = new Bitmap(imageWidth, imageHeight, PixelFormat.Format24bppRgb);
                BitmapData bluData = undistorted_img.LockBits(new Rectangle(0, 0, imageWidth, imageHeight), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

                CopyMemory(bluData.Scan0, tempData, imageWidth * imageHeight * 3);

                te.UnlockBits(tedata);
                undistorted_img.UnlockBits(bluData);
                te.Dispose();

                return undistorted_img;
            }
        }

        static void SaveBitmapsToTiff(string tiffFilePath, params Mat[] mats)
        {
            // 첫 번째 이미지를 TIFF로 저장
            using (Bitmap firstBitmap = mats[0].ToBitmap())
            {
                Encoder encoder = Encoder.SaveFlag;
                EncoderParameters encoderParams = new EncoderParameters(1);
                ImageCodecInfo codecInfo = GetEncoderInfo("image/tiff");

                encoderParams.Param[0] = new EncoderParameter(encoder, (long)EncoderValue.MultiFrame);
                firstBitmap.Save(tiffFilePath, codecInfo, encoderParams);

                // 나머지 이미지를 추가
                encoderParams.Param[0] = new EncoderParameter(encoder, (long)EncoderValue.FrameDimensionPage);
                for (int i = 1; i < mats.Length; i++)
                {
                    using (Bitmap bitmap = mats[i].ToBitmap())
                    {
                        firstBitmap.SaveAdd(bitmap, encoderParams);
                    }
                }

                // 마지막 이미지 추가
                encoderParams.Param[0] = new EncoderParameter(encoder, (long)EncoderValue.Flush);
                firstBitmap.SaveAdd(encoderParams);
            }
        }

        static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo encoder in encoders)
            {
                if (encoder.MimeType == mimeType)
                {
                    return encoder;
                }
            }
            throw new Exception("Encoder not found.");
        }
    }
}
