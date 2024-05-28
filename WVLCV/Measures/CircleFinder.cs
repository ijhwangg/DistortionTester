using OpenCvSharp;
using System;
using System.Collections.Generic;
using WVLCV.WMath;

namespace WVLCV.Measures
{
    /// <summary>
    /// 이미지의 정해진 영역에서 원을 찾는 방법을 감싼 클래스입니다.
    /// </summary>
    public class CircleFinder
    {
        private Point2f Center { get; }

        private float Radius { get; }

        private float AngleStart { get; }

        private float AngleEnd { get; }

        private float UnitLength { get; }

        private int UnitWidth { get; }

        private float SearchInterval { get; }

        /// <summary>
        /// 완전한 원형 범위에서 원을 찾는 CircleFinder의 인스턴스를 생성합니다.
        /// </summary>
        /// <param name="center">탐색 범위의 중심</param>
        /// <param name="radius">탐색 범위의 반지름</param>
        public CircleFinder(Point2f center, float radius)
        {
            Center = center;
            Radius = radius;
            AngleStart = 0;
            AngleEnd = 360;

            UnitLength = 100;
            UnitWidth = 5;
            SearchInterval = 1;
        }

        /// <summary>
        /// 완전한 원형 범위에서 원을 찾는 CircleFinder의 인스턴스를 생성합니다. 탐색 인자를 설정할 수 있습니다.
        /// </summary>
        /// <param name="center">탐색 범위의 중심</param>
        /// <param name="radius">탐색 범위의 반지름</param>
        /// <param name="unitLength"><see cref="MeasurementUnit"/>의 길이</param>
        /// <param name="unitWidth"><see cref="MeasurementUnit"/>의 폭</param>
        /// <param name="searchInterval"><see cref="MeasurementUnit"/>을 설정할 간격 (단위: deg)</param>
        public CircleFinder(Point2f center, float radius, float unitLength, int unitWidth, float searchInterval)
        {
            Center = center;
            Radius = radius;
            AngleStart = 0;
            AngleEnd = 360;

            UnitLength = unitLength;
            UnitWidth = unitWidth;
            SearchInterval = searchInterval;
        }

        /// <summary>
        /// Arc 형태의 범위에서 원을 찾는 CircleFinder의 인스턴스를 생성합니다.
        /// </summary>
        /// <param name="center">탐색 범위의 중심</param>
        /// <param name="radius">탐색 범위의 반지름</param>
        /// <param name="angleStart">Arc가 시작되는 각도 (단위: deg)</param>
        /// <param name="angleEnd">Arc가 끝나는 각도 (단위: deg)</param>
        public CircleFinder(Point2f center, float radius, float angleStart, float angleEnd)
        {
            Center = center;
            Radius = radius;
            AngleStart = angleStart;
            AngleEnd = angleEnd;

            UnitLength = 100;
            UnitWidth = 5;
            SearchInterval = 1;
        }

        /// <summary>
        /// Arc 형태의 범위에서 원을 찾는 CircleFinder의 인스턴스를 생성합니다.  탐색 인자를 설정할 수 있습니다.
        /// </summary>
        /// <param name="center">탐색 범위의 중심</param>
        /// <param name="radius">탐색 범위의 반지름</param>
        /// <param name="angleStart">Arc가 시작되는 각도 (단위: deg)</param>
        /// <param name="angleEnd">Arc가 끝나는 각도 (단위: deg)</param>
        /// <param name="unitLength"><see cref="MeasurementUnit"/>의 길이</param>
        /// <param name="unitWidth"><see cref="MeasurementUnit"/>의 폭</param>
        /// <param name="searchInterval"><see cref="MeasurementUnit"/>을 설정할 간격 (단위: deg)</param>
        public CircleFinder(Point2f center, float radius, float angleStart, float angleEnd, float unitLength, int unitWidth, float searchInterval)
        {
            Center = center;
            Radius = radius;
            AngleStart = angleStart;
            AngleEnd = angleEnd;

            UnitLength = unitLength;
            UnitWidth = unitWidth;
            SearchInterval = searchInterval;
        }

        /// <summary>
        /// 흑백 이미지의 지정한 범위 내에서 원을 찾습니다.
        /// </summary>
        /// <param name="grayImage">흑백 이미지</param>
        /// <returns>Fitting된 원</returns>
        public Circle2 Find(Mat grayImage)
        {
            var pts = new List<Point2f>();
            for (float deg = AngleStart; deg <= AngleEnd; deg += SearchInterval)
            {
                float radian = deg * (float)Math.PI / 180.0f;

                var vec = new Point2f((float)Math.Cos(radian), (float)Math.Sin(radian));
                var beg = Center + vec * (Radius - UnitLength / 2.0);
                var end = Center + vec * (Radius + UnitLength / 2.0);

                var mu = new MeasurementUnit(beg, end, UnitWidth);
                var p = mu.Get(grayImage);

                if (!float.IsNaN(p.X) && !float.IsNaN(p.Y))
                {
                    pts.Add(p);
                }
            }

            var fit = Fittings.FitCircle(pts);

            return fit;
        }
    }
}