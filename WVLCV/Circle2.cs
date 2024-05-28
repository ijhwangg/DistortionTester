using OpenCvSharp;
using System;

namespace WVLCV
{
    /// <summary>
    /// 평면상의 원을 나타내는 불변 구조체입니다.
    /// </summary>
    public struct Circle2
    {
        /// <summary>
        /// 원의 중심입니다.
        /// </summary>
        public Point2f Center { get; }

        /// <summary>
        /// 원의 반지름입니다.
        /// </summary>
        public float Radius { get; }

        /// <summary>
        /// 주어진 중심과 반지름을 가지는 원을 생성합니다.
        /// </summary>
        /// <param name="center">주어진 중심</param>
        /// <param name="radius">주어진 반지름</param>
        public Circle2(Point2f center, float radius)
        {
            Center = center;
            Radius = radius;
        }

        /// <summary>
        /// 두 점이 만드는 선분을 지름으로 하는 원을 생성합니다.
        /// </summary>
        /// <param name="p1">첫번째 점</param>
        /// <param name="p2">두번째 점</param>
        /// <returns>주어진 두 점이 만드는 선분을 지름으로 하는 원</returns>
        public static Circle2 PassingTwoPoints(Point2f p1, Point2f p2)
        {
            var center = (p1 + p2) * 0.5;
            float rad = (float)center.DistanceTo(p1);
            return new Circle2(center, rad);
        }

        /// <summary>
        /// 세 점을 지나는 원을 생성합니다.
        /// </summary>
        /// <param name="p1">첫번째 점</param>
        /// <param name="p2">두번째 점</param>
        /// <param name="p3">세번째 점</param>
        /// <returns>주어진 세 점을 지나는 원</returns>
        public static Circle2 PassingThreePoints(Point2f p1, Point2f p2, Point2f p3)
        {
            using (var left = new Mat(3, 3, MatType.CV_32FC1))
            using (var right = new Mat(3, 1, MatType.CV_32FC1))
            using (var leftF = new Mat<float>(left))
            using (var rightF = new Mat<float>(right))
            {
                var iter1 = leftF.GetIndexer();
                var iter2 = rightF.GetIndexer();

                iter1[0, 0] = p1.X; iter1[0, 1] = p1.Y; iter1[0, 2] = 1; iter2[0, 0] = -p1.X * p1.X - p1.Y * p1.Y;
                iter1[1, 0] = p2.X; iter1[1, 1] = p2.Y; iter1[1, 2] = 1; iter2[1, 0] = -p2.X * p2.X - p2.Y * p2.Y;
                iter1[2, 0] = p3.X; iter1[2, 1] = p3.Y; iter1[2, 2] = 1; iter2[2, 0] = -p3.X * p3.X - p3.Y * p3.Y;

                using (var ans = left.Inv() * right)
                using (var x = ans.ToMat())
                using (var xd = new Mat<float>(x))
                {
                    var iter3 = xd.GetIndexer();
                    float a = iter3[0, 0];
                    float b = iter3[1, 0];
                    float c = iter3[2, 0];

                    float x0 = -0.5f * a;
                    float y0 = -0.5f * b;
                    float r = (float)Math.Sqrt(x0 * x0 + y0 * y0 - c);

                    return new Circle2(new Point2f(x0, y0), r);
                }
            }
        }

        /// <summary>
        /// 원과 점 사이의 부호있는 거리를 계산합니다. 원 내부에 있으면 음수, 원 외부에 있으면 양수입니다.
        /// </summary>
        /// <param name="p">주어진 점</param>
        /// <returns>원과 점 사이의 부호있는 거리</returns>
        public float Distance(Point2f p)
        {
            var d = (float)p.DistanceTo(Center);

            return d - Radius;
        }
    }
}