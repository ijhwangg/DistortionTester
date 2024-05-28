using OpenCvSharp;
using System.Collections.Generic;
using WVLCV.WMath;

namespace WVLCV.Measures
{
    /// <summary>
    /// 이미지의 정해진 영역에서 직선을 찾는 방법을 감싼 클래스입니다.
    /// </summary>
    public class LineFinder
    {
        private Point2f P1 { get; }

        private Point2f P2 { get; }

        private float UnitLength { get; }

        private int UnitWidth { get; }

        private float SearchInterval { get; }

        /// <summary>
        /// 기본값으로 LineFinder의 인스턴스를 생성합니다.
        /// </summary>
        /// <param name="p1">검색 범위 시작점</param>
        /// <param name="p2">검색 범위 끝점</param>
        public LineFinder(Point2f p1, Point2f p2)
        {
            P1 = p1;
            P2 = p2;
            UnitLength = 100;
            UnitWidth = 5;
            SearchInterval = 10;
        }

        /// <summary>
        /// 인자를 설정하여 LineFinder의 인스턴스를 생성합니다.
        /// </summary>
        /// <param name="p1">검색 범위 시작점</param>
        /// <param name="p2">검색 범위 끝점</param>
        /// <param name="unitLength"><see cref="MeasurementUnit"/>의 길이</param>
        /// <param name="unitWidth"><see cref="MeasurementUnit"/>의 폭</param>
        /// <param name="searchInterval"><see cref="MeasurementUnit"/>을 설정할 간격</param>
        public LineFinder(Point2f p1, Point2f p2, float unitLength, int unitWidth, float searchInterval)
        {
            P1 = p1;
            P2 = p2;
            UnitLength = unitLength;
            UnitWidth = unitWidth;
            SearchInterval = searchInterval;
        }

        /// <summary>
        /// 흑백 이미지의 지정한 범위 내에서 직선을 찾습니다.
        /// </summary>
        /// <param name="grayImage">흑백 이미지</param>
        /// <returns>Fitting된 직선</returns>
        public Line2D Find(Mat grayImage)
        {
            var vec = P2 - P1;
            var len = P2.DistanceTo(P1);
            vec *= 1 / len;

            var perp = new Point2f(vec.Y, -vec.X);

            List<Point2f> pts = new List<Point2f>();
            for (float i = 0; i < len; i += SearchInterval)
            {
                var begin = P1 + vec * i - perp * (UnitLength / 2.0);
                var end = begin + perp * UnitLength;
                var mu = new MeasurementUnit(begin, end, UnitWidth);
                var newPt = mu.GetQuarter(grayImage);
                if (!float.IsNaN(newPt.X) && !float.IsNaN(newPt.Y) &&
                    !float.IsInfinity(newPt.X) && !float.IsInfinity(newPt.Y))
                {
                    pts.Add(newPt);
                }
            }

            var line = Fittings.FitLine(pts);

            return line;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="grayImage"></param>
        /// <returns></returns>
        public Line2D FindHalfMax(Mat grayImage)
        {
            var vec = P2 - P1;
            var len = P2.DistanceTo(P1);
            vec *= 1 / len;

            var perp = new Point2f(vec.Y, -vec.X);

            List<Point2f> pts = new List<Point2f>();
            for (float i = 0; i < len; i += SearchInterval)
            {
                var begin = P1 + vec * i - perp * (UnitLength / 2.0);
                var end = begin + perp * UnitLength;
                var mu = new MeasurementUnit(begin, end, UnitWidth);
                var newPt = mu.GetHalfMax(grayImage);
                if (!float.IsNaN(newPt.X) && !float.IsNaN(newPt.Y) &&
                    !float.IsInfinity(newPt.X) && !float.IsInfinity(newPt.Y))
                {
                    pts.Add(newPt);
                }
            }

            var line = Fittings.FitLine(pts);

            return line;
        }

        public Line2D FindQuarter(Mat grayImage)
        {
            var vec = P2 - P1;
            var len = P2.DistanceTo(P1);
            vec *= 1 / len;

            var perp = new Point2f(vec.Y, -vec.X);

            List<Point2f> pts = new List<Point2f>();
            for (float i = 0; i < len; i += SearchInterval)
            {
                var begin = P1 + vec * i - perp * (UnitLength / 2.0);
                var end = begin + perp * UnitLength;
                var mu = new MeasurementUnit(begin, end, UnitWidth);
                var newPt = mu.GetQuarter(grayImage);
                if (!float.IsNaN(newPt.X) && !float.IsNaN(newPt.Y) &&
                    !float.IsInfinity(newPt.X) && !float.IsInfinity(newPt.Y))
                {
                    pts.Add(newPt);
                }
            }

            var line = Fittings.FitLine(pts);

            return line;
        }
    }
}