using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using System.IO;
using WVLCV.Core;

namespace WindowsFormsApp1
{
    public class VectorMapGenerator
    {
        public float AverageDiff { get; set; }

        public float InitPixelSize { get; set; }

        public float PixelLimitation { get; set; } = 3.0f;

        public float DotToDotDistance { get; set; } = 500.0f;

        public List<Point2f> IdealCentersFin { get; set; } = new List<Point2f>();

        public List<Point2f> RealCentersFin { get; set; } = new List<Point2f>();

        public List<Point2f> IdealCenters { get; set; } = new List<Point2f>();

        public List<Point2f> RealCenters { get; set; } = new List<Point2f>();

        public List<Point2f> VectorPointCenters { get; set; } = new List<Point2f>();

        public Mat ShowImg { get; set; }

        public float ZoomFactor { get; set; } = 1F;

        public Point2f[,] VMap { get; set; }

        public int ImageHeight = 3000;
        public int ImageWidth = 4080;
        public int ImageCH = 3;

        public void VectorMapShow(Mat img, List<Point2f> points,
            float rot, float ps, float zoomFactor,
            List<VectorAndPosition> vectorPoints,
            Point2f StartPoint, float MaximumDiff, float FOVDiameter, float ratio)
        {
            IdealCentersFin.Clear();
            RealCentersFin.Clear();
            IdealCenters.Clear();
            RealCenters.Clear();

            var zoomedPoints = ZoomPoints(points, StartPoint, zoomFactor, 1F);

            var rotatedPoints = CommonFunctions.RotatePoints(zoomedPoints, StartPoint, rot);

            var dtd = DotToDotDistance / ps;
            var tmpMax = default(float);

            MaximumDiff = 0F;
            AverageDiff = 0F;
            PixelLimitation = 10.0F;

            if (File.Exists("item.txt"))
            {
                File.Delete("item.txt");
            }
            StreamWriter sw1 = new StreamWriter("item.txt");
            foreach (Point2f f in rotatedPoints)
            {
                sw1.WriteLine(string.Format("{0},{1}", f.X, f.Y));
            }
            sw1.Close();

            var count = 0;

            // 수정 2024-05-27
            foreach (var item in rotatedPoints)
            {
                var nX = (float)Math.Round((item.X - StartPoint.X) / dtd);
                var nY = (float)Math.Round((item.Y - StartPoint.Y) / dtd);

                var diff = (float)Math.Sqrt(
                    Math.Pow(StartPoint.X + dtd * nX - item.X, 2)
                    + Math.Pow(StartPoint.Y + dtd * nY - item.Y, 2));

                if (diff < PixelLimitation)
                {
                    var tmpVect = new VectorAndPosition(
                        item,
                        new Point2f((StartPoint.X + dtd * nX - item.X) * -1, (StartPoint.Y + dtd * nY - item.Y) * -1));

                    vectorPoints.Add(tmpVect);

                    VectorPointCenters.Add(tmpVect.Position);

                    AverageDiff += diff;
                    count += 1;

                    IdealCenters.Add(new Point2f(StartPoint.X + dtd * nX, StartPoint.Y + dtd * nY));
                    RealCenters.Add(item);
                }

                if (tmpMax < diff)
                {
                    tmpMax = diff;
                    MaximumDiff = diff;
                }
            }

            // 기존
            /*
            foreach (var item in rotatedPoints)
            {
                var nX = (float)Math.Round((item.X - StartPoint.X) / dtd);
                var nY = (float)Math.Round((item.Y - StartPoint.Y) / dtd);

                var diff = (float)Math.Sqrt(
                    Math.Pow(StartPoint.X + dtd * nX - item.X, 2)
                    + Math.Pow(StartPoint.Y + dtd * nY - item.Y, 2));

                if (diff < PixelLimitation)
                {
                    var tmpVect = new VectorAndPosition(
                        item,
                        new Point2f(StartPoint.X + dtd * nX - item.X, StartPoint.Y + dtd * nY - item.Y));

                    vectorPoints.Add(tmpVect);

                    VectorPointCenters.Add(tmpVect.Position);

                    AverageDiff += diff;
                    count += 1;

                    IdealCenters.Add(new Point2f(StartPoint.X + dtd * nX, StartPoint.Y + dtd * nY));
                    RealCenters.Add(item);
                }

                if (tmpMax < diff)
                {
                    tmpMax = diff;
                    MaximumDiff = diff;
                }
            }*/

            if (File.Exists("vector_point.txt"))
            {
                File.Delete("vector_point.txt");
            }
            StreamWriter vp = new StreamWriter("vector_point.txt");
            foreach (Point2f f in VectorPointCenters)
            {
                vp.WriteLine(string.Format("{0},{1}", f.X, f.Y));
            }
            vp.Close();


            if (File.Exists("ideal_cssharp2.txt"))
            {
                File.Delete("ideal_cssharp2.txt");
            }
            StreamWriter sw = new StreamWriter("ideal_cssharp2.txt");
            foreach (Point2f f in IdealCenters)
            {
                sw.WriteLine(string.Format("{0},{1}", f.X, f.Y));
            }
            sw.Close();

            if (File.Exists("real_point.txt"))
            {
                File.Delete("real_point.txt");
            }
            StreamWriter re = new StreamWriter("real_point.txt");
            foreach (Point2f f in RealCenters)
            {
                re.WriteLine(string.Format("{0},{1}", f.X, f.Y));
            }
            re.Close();

            IdealCentersFin = CommonFunctions.RotatePoints(IdealCenters, StartPoint, -rot);
            RealCentersFin = CommonFunctions.RotatePoints(RealCenters, StartPoint, -rot);


            Mat IdealDotImg;
            IdealDotImg = new Mat(ImageHeight, ImageWidth, MatType.CV_8UC3);
            for (int i = 0; i < IdealCentersFin.Count; i++)
            {
                Cv2.Circle(IdealDotImg, new Point(IdealCentersFin[i].X, IdealCentersFin[i].Y), 5, Scalar.Red, -1);  // 빨간색 점으로 표시
            }
            Cv2.ImWrite("Ideal_Dot_Image.bmp", IdealDotImg);

            Mat RealDotImg;
            RealDotImg = new Mat(ImageHeight, ImageWidth, MatType.CV_8UC3);
            for (int i = 0; i < RealCentersFin.Count; i++)
            {
                Cv2.Circle(RealDotImg, new Point(RealCentersFin[i].X, RealCentersFin[i].Y), 5, Scalar.Red, -1);  // 빨간색 점으로 표시
            }
            Cv2.ImWrite("Real_Dot_Image.bmp", RealDotImg);


            for (int i = 0; i < count; i++)
            {
                // 기존
                /*
                 DrawVector(img,
                    IdealCentersFin[i].X, IdealCentersFin[i].Y,
                    RealCentersFin[i].X, RealCentersFin[i].Y,
                    ratio, Scalar.Green, 2);
                    */

                // 수정 2024-05-27
                DrawVector(img,
                    RealCentersFin[i].X, RealCentersFin[i].Y,
                    IdealCentersFin[i].X, IdealCentersFin[i].Y,
                    ratio, Scalar.Green, 2);

                Util.DrawCross(img, IdealCentersFin[i], Scalar.Red, 10, 45);
                Util.DrawCross(img, RealCentersFin[i], Scalar.Blue, 10, 45);
            }

            AverageDiff /= count;

            Cv2.Circle(img,
                new Point(img.Width / 2, img.Height / 2), (int)((FOVDiameter / 2) / ps),
                Scalar.Yellow, 2, LineTypes.AntiAlias);

        }

        public Point2f[,] VectorMapGenerate(List<VectorAndPosition> vectorStructures)
        {
            var blockSize = ImageHeight / 8;

            var result = new Point2f[ImageHeight, ImageWidth];
            // VectorMapGeneratorFor(0, ImageHeight, vectorStructures, result, 100);
            Parallel.Invoke(
                () => { VectorMapGeneratorFor(0, blockSize, vectorStructures, result, 100); },
                () => { VectorMapGeneratorFor(blockSize, blockSize * 2, vectorStructures, result, 100); },
                () => { VectorMapGeneratorFor(blockSize * 2, blockSize * 3, vectorStructures, result, 100); },
                () => { VectorMapGeneratorFor(blockSize * 3, blockSize * 4, vectorStructures, result, 100); },
                () => { VectorMapGeneratorFor(blockSize * 4, blockSize * 5, vectorStructures, result, 100); },
                () => { VectorMapGeneratorFor(blockSize * 5, blockSize * 6, vectorStructures, result, 100); },
                () => { VectorMapGeneratorFor(blockSize * 6, blockSize * 7, vectorStructures, result, 100); },
                () => { VectorMapGeneratorFor(blockSize * 7, ImageHeight, vectorStructures, result, 100); }
                );

            return result;
        }

        public void VectorMapGeneratorFor(int iStart, int iEnd,
            List<VectorAndPosition> vectorStructures, Point2f[,] result, int range = 100)
        {
            // Range(0:Width, iStart:iEnd) 반복
            for (int i = iStart; i < iEnd; i++)
            {
                for (int j = 0; j < ImageWidth; j++)
                {
                    // 현재 점 (i, j)로 부터 각 도트 중심까지의 거리를 계산해서 모음
                    var distance = new List<VectorSort>();
                    for (int k = 0; k < vectorStructures.Count; k++)
                    {
                        if (Math.Abs(vectorStructures[k].Position.X - j) < range &&
                            Math.Abs(vectorStructures[k].Position.Y - i) < range)
                        {
                            distance.Add(
                                new VectorSort(
                                    (float)Math.Sqrt(
                                        Math.Pow(vectorStructures[k].Position.X - j, 2)
                                        + Math.Pow(vectorStructures[k].Position.Y - i, 2)),
                                    k));
                        }
                    }

                    // 거리를 정렬해서 가까운 순으로 최대 3개까지 선택
                    var orderedVector = distance.OrderBy(a => a.Distance);
                    var pts = new Point2f[3];
                    var vectors = new Point2f[3];
                    for (int k = 0; k < 3 && k < orderedVector.Count(); k++)
                    {
                        pts[k] = vectorStructures[orderedVector.ElementAt(k).Index].Position;
                        vectors[k] = vectorStructures[orderedVector.ElementAt(k).Index].Vector;
                    }

                    // (i, j)에서의 벡터를 가장 가까운 세 점으로 선형보간
                    var ptx0 = new Point3f(pts[0].X, pts[0].Y, vectors[0].X);
                    var ptx1 = new Point3f(pts[1].X, pts[1].Y, vectors[1].X);
                    var ptx2 = new Point3f(pts[2].X, pts[2].Y, vectors[2].X);
                    var pty0 = new Point3f(pts[0].X, pts[0].Y, vectors[0].Y);
                    var pty1 = new Point3f(pts[1].X, pts[1].Y, vectors[1].Y);
                    var pty2 = new Point3f(pts[2].X, pts[2].Y, vectors[2].Y);

                    result[i, j] = new Point2f(
                        FindTriangleHeight(i, j, ptx0, ptx1, ptx2),
                        FindTriangleHeight(i, j, pty0, pty1, pty2));
                }
            }
        }

        public float FindTriangleHeight(int i, int j, Point3f pt0, Point3f pt1, Point3f pt2)
        {
            var n1 = pt2 - pt0;
            var n2 = pt1 - pt0;

            var cross = new Point3f(
                n1.Y * n2.Z - n1.Z * n2.Y,
                n1.Z * n2.X - n1.X * n2.Z,
                n1.X * n2.Y - n1.Y * n2.X);

            if (0 == cross.Z)
            {
                return 0;
            }

            return pt0.Z + (pt0.X - j) * cross.X / cross.Z + (pt0.Y - i) * cross.Y / cross.Z;
        }

        public void DrawVector(Mat src, float initX, float initY, float finX, float finY,
            float ratio, Scalar color, int linewidth)
        {
            finX = finX + (finX - initX) * ratio;
            finY = finY + (finY - initY) * ratio;

            var angle = (float)Math.Atan2(finY - initY, finX - initX);

            var length = (float)Math.Sqrt(Math.Pow(finX - initX, 2) + Math.Pow(finY - initY, 2));

            var arrowHeadLength = length / 8F;
            var arrPoint1 = new Point(
                arrowHeadLength * (float)Math.Cos(angle + Math.PI / 6.0),
                arrowHeadLength * (float)Math.Sin(angle + Math.PI / 6.0));
            var arrPoint2 = new Point(
                arrowHeadLength * (float)Math.Cos(angle - Math.PI / 6.0),
                arrowHeadLength * (float)Math.Sin(angle - Math.PI / 6.0));

            Cv2.Line(src, new Point(initX, initY), new Point(finX, finY),
                color, linewidth, LineTypes.AntiAlias);
            Cv2.Line(src, new Point(finX, finY), new Point(finX, finY) - arrPoint1,
                color, linewidth, LineTypes.AntiAlias);
            Cv2.Line(src, new Point(finX, finY), new Point(finX, finY) - arrPoint2,
                color, linewidth, LineTypes.AntiAlias);
        }

        public void DrawVectorMap(List<Point2f> dc, float ps, float rot,
            float zoomFactor, Point2f StartPoint, float MaximumDiff, float FOVDiameter,
            Point2f TiltFactor, float ratio)
        {
            ShowImg = new Mat(ImageHeight, ImageWidth, MatType.CV_8UC3);

            // 기존
            /*
            DrawVector(ShowImg, ImageWidth / 2F, ImageHeight / 2F,
                ImageWidth / 2F + TiltFactor.X, ImageHeight / 2F + TiltFactor.Y,
                ratio, Scalar.Yellow, 10);
            */

            // 수정 2024-05-27
            DrawVector(ShowImg, ImageWidth / 2F + TiltFactor.X, ImageHeight / 2F + TiltFactor.Y,
                ImageWidth / 2F, ImageHeight / 2F,
                ratio, Scalar.Yellow, 10);

            Cv2.ImWrite("DrawVectorMap.bmp", ShowImg);

            var tmp = new List<VectorAndPosition>();
            VectorMapShow(ShowImg, dc, rot, ps, zoomFactor,
                tmp, StartPoint, MaximumDiff, FOVDiameter, ratio);
        }

        public float[,] SolveUsingLU(float[,] matrix, float[,] rightPart, int n)
        {
            // decomposition of matrix
            var lu = new float[n, n];
            var sum = default(float);

            for (int i = 0; i < n; i++)
            {
                for (int j = i; j < n; j++)
                {
                    sum = 0;
                    for (int k = 0; k < i; k++)
                    {
                        sum += lu[i, k] * lu[k, j];
                    }

                    lu[i, j] = matrix[i, j] - sum;
                }

                for (int j = i + 1; j < n; j++)
                {
                    sum = 0;
                    for (int k = 0; k < i; k++)
                    {
                        sum += lu[j, k] * lu[k, i];
                    }

                    if (lu[i, i] == 0)
                    {
                        lu[i, i] = 0.0001F;
                    }

                    lu[j, i] = (1 / lu[i, i]) * (matrix[j, i] - sum);
                }
            }

            // find solution of Ly = b
            var y = new float[n];
            for (int i = 0; i < n; i++)
            {
                sum = 0;
                for (int k = 0; k < i; k++)
                {
                    sum += lu[i, k] * y[k];
                }

                y[i] = rightPart[i, 0] - sum;
            }

            // find solution of Ux = y
            var x = new float[n, 1];
            for (int i = n - 1; 0 <= i; i--)
            {
                sum = 0;
                for (int k = i + 1; k < n; k++)
                {
                    sum += lu[i, k] * x[k, 0];
                }

                if (lu[i, i] == 0)
                {
                    lu[i, i] = 0.0001f;
                }

                x[i, 0] = (y[i] - sum) / lu[i, i];
            }

            return x;
        }

        public void DistortionCorrection(List<Point2f> DotCenters,
            ref Point2f StartPoint, float RotationAngle, float PixelSize,
            float MaximumDiff, float FOVDiameter, float ratio)
        {
            //ShowImg = new Mat(Cam.ImageHeight, Cam.ImageWidth, MatType.CV_8UC3);
            //DotCenters = CommonFunctions.DotCenterFinder(ShowImg, 1.0f, ref StartPoint);

            var res = new List<VectorAndPosition>();

            List<Point2f> sort_vect = DotCenters.OrderBy(p => p.X).ThenBy(p => p.Y).ToList();
            DotCenters = sort_vect;

            VectorMapShow(ShowImg, DotCenters, RotationAngle, PixelSize, ZoomFactor,
                res, StartPoint, MaximumDiff, FOVDiameter, ratio);

            Cv2.ImWrite("DistortionCorrection.bmp", ShowImg);

            VMap = VectorMapGenerate(res);

            if (File.Exists("VMap.txt"))
            {
                File.Delete("VMap.txt");
            }
            StreamWriter sw1 = new StreamWriter("VMap.txt");
            foreach (Point2f f in VMap)
            {
                sw1.WriteLine(string.Format("{0},{1}", f.X, f.Y));
            }
            sw1.Close();

            var bw = new BinaryWriter(
                File.Open(@"_TopCorrection.dat", FileMode.Create));

            for (int i = 0; i < ImageHeight; i++)
            {
                for (int j = 0; j < ImageWidth; j++)
                {
                    bw.Write(VMap[i, j].X);
                    bw.Write(VMap[i, j].Y);
                }
            }

            bw.Close();
            bw.Dispose();
        }

        public List<Point2f> ZoomPoints(List<Point2f> points, Point2f zoomCenter,
            float zoomFactorX, float zoomFactorY)
        {
            var result = new List<Point2f>();

            foreach (var item in points)
            {
                var newX = zoomCenter.X + (item.X - zoomCenter.X) * zoomFactorX;
                var newY = zoomCenter.Y + (item.Y - zoomCenter.Y) * zoomFactorY;

                result.Add(new Point2f(newX, newY));
            }

            return result;
        }
    }
}
