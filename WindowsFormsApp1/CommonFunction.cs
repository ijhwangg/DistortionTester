using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using WVLCV;

namespace WindowsFormsApp1
{
    internal static class CommonFunctions
    {
        /// <summary>
        /// Dot Array측정 시, Dot Array가 카메라에 대해 항상 수평 혹은 수직으로 놓일 수는 없기 때문에,
        /// 이를 보정시켜주기 위한 함수.
        /// </summary>
        /// <param name="points">DotCenterFinder에서 구한, 실측정된 Dot를 모아놓은 Point2f List 데이터</param>
        /// <param name="rotationCenter">회전 중심 좌표</param>
        /// <param name="angle">회전 각도</param>
        public static List<Point2f> RotatePoints(List<Point2f> points, Point2f rotationCenter, float angle)
        {
            var ret = from p in points
                      select RotatePointLocal(p, rotationCenter, angle);

            return ret.ToList();
        }

        public static Point2f RotatePointLocal(Point2f p, Point2f center, float angle)
        {
            var shift = p - center;

            var cos = (float)Math.Cos(angle);
            var sin = (float)Math.Sin(angle);

            var rotX = cos * shift.X - sin * shift.Y;
            var rotY = sin * shift.X + cos * shift.Y;

            var rot = new Point2f(rotX, rotY);

            return rot + center;
        }

        /// <summary>
        /// Dot의 중심을 찾는 함수.
        /// Auto Threshold 후, Gray값으로 Weighted된 Moment를 Dot의 중심이라고 함.
        /// </summary>
        /// <param name="img">흑백 이미지</param>
        /// <param name="weight">Weight줄 때 Gray값의 n제곱을 Weight로 준다</param>
        public static List<Point2f> DotCenterFinder(Mat img, float weight, ref Point2f startPoint)
        {
            var gray = img.CvtColor(ColorConversionCodes.BGR2GRAY);

            var width = gray.Width;
            var height = gray.Height;

            var hw = width / 2F;
            var hh = height / 2F;
            var margin = 50F;

            var dotCenters = new List<Point2f>();

            var thres = (float)gray.Mean() * 0.8F;

            using (var thresImg = gray.Threshold(thres, 255, ThresholdTypes.BinaryInv))
            using (var grayInv = gray.ConvertScaleAbs(-1, 255))
            {
                Cv2.FindContours(thresImg,
                    out Point[][] contours, out HierarchyIndex[] hierarchy,
                    RetrievalModes.Tree, ContourApproximationModes.ApproxNone);

                for (int i = 0; i < contours.Length; i++)
                {
                    if (contours[i].Length > 80)
                    {
                        var region = CRegion.FromContour(
                            new Point[][] { contours[i] },
                            new HierarchyIndex[] { new HierarchyIndex(-1, -1, -1, -1) });

                        region.GrayVolumeCenter(grayInv,
                            out float volume, out Point2f center,
                            weight);

                        dotCenters.Add(center);

                        UpdateStartPoint(center, hw, hh, margin, ref startPoint);
                    }
                }
            }

            gray.Dispose();

            return dotCenters;
        }

        public static void UpdateStartPoint(
            Point2f center, float halfWidth, float halfHeight, float margin,
            ref Point2f startPoint)
        {
            if (center.X > halfWidth - margin &&
                center.X < halfWidth + margin &&
                center.Y > halfHeight - margin &&
                center.Y < halfHeight + margin)
            {
                startPoint = center;
            }
        }
    }
}
