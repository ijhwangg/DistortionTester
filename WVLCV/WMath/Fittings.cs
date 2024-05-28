using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WVLCV.WMath
{
    /// <summary>
    /// Fitting 관련 메서드들의 집합
    /// </summary>
    public static class Fittings
    {
        /// <summary>
        /// 2차원 데이터에 대한 PCA를 수행합니다.
        /// </summary>
        /// <param name="data1">첫번째 차원의 Data</param>
        /// <param name="data2">두번째 차원의 Data</param>
        /// <param name="lambda1">첫번째 주성분의 고윳값</param>
        /// <param name="lambda2">두번째 주성분의 고윳값</param>
        /// <param name="vector1">첫번째 주성분의 고유벡터</param>
        /// <param name="vector2">두번째 주성분의 고유벡터</param>
        public static void Pca2dim(IEnumerable<float> data1, IEnumerable<float> data2, out float lambda1, out float lambda2, out Point2f vector1, out Point2f vector2)
        {
            // 표본 공분산 행렬의 각 성분을 계산
            float var1 = Statistics.SampleVariance(data1);
            float var2 = Statistics.SampleVariance(data2);
            float cov = Statistics.SampleCovariance(data1, data2);

            if (cov == 0)
            {
                lambda1 = var1;
                lambda2 = var2;

                vector1 = new Point2f(1, 0);
                vector2 = new Point2f(0, 1);
            }
            else
            {
                // 분산은 양수이므로 두 고윳값 중 큰 것이 결정된다
                float tr = var1 + var2;
                float tm = var1 - var2;
                float root = (float)Math.Sqrt(tm * tm + 4 * cov * cov);

                lambda1 = (tr + root) / 2;
                lambda2 = (tr - root) / 2;

                // 고유벡터를 구하고 Normalize
                // 1
                float v1x = cov;
                float v1y = lambda1 - var1;

                if (v1x < 0)
                {
                    v1x *= -1;
                    v1y *= -1;
                }

                float len1 = (float)Math.Sqrt(v1x * v1x + v1y * v1y);
                vector1 = new Point2f(v1x / len1, v1y / len1);

                // 2
                float v2x = cov;
                float v2y = lambda2 - var1;

                if (0 < v2y)
                {
                    v2x *= -1;
                    v2y *= -1;
                }

                float len2 = (float)Math.Sqrt(v2x * v2x + v2y * v2y);
                vector2 = new Point2f(v2x / len2, v2y / len2);
            }
        }

        /// <summary>
        /// 점과 직선 사이의 부호있는 거리를 계산합니다.
        /// </summary>
        /// <param name="l">직선</param>
        /// <param name="p">점</param>
        /// <returns>점과 직선 사이의 부호있는 거리</returns>
        public static float Distance(Line2D l, Point2f p)
        {
            float len = (float)Math.Sqrt(l.Vx * l.Vx + l.Vy * l.Vy);

            // Vy(X - X0) - Vx(Y - Y0)
            float abs = (float)(l.Vy * (p.X - l.X1) - l.Vx * (p.Y - l.Y1));

            return abs / len;
        }

        /// <summary>
        /// Point2f의 집합에 최적 직선을 Fitting합니다.
        /// </summary>
        /// <param name="points">Point2f의 집합</param>
        /// <param name="maxIteration">최대 반복 횟수</param>
        /// <returns>최적 직선</returns>
        public static Line2D FitLine(IEnumerable<Point2f> points, int maxIteration = 5)
        {
            int c = points.Count();
            if (c < 2)
            {
                throw new ArgumentException("점의 수가 부족합니다.", nameof(points));
            }
            else if (c == 2)
            {
                var p0 = points.ElementAt(0);
                var p1 = points.ElementAt(1);
                var vec = p1 - p0;
                float veclen = (float)Math.Sqrt(vec.X * vec.X + vec.Y * vec.Y);
                vec *= (1 / veclen);
                return new Line2D(vec.X, vec.Y, p0.X, p0.Y);
            }
            else
            {
                // Data를 X좌표와 Y좌표로 분해하여 PCA를 수행
                var x = new List<float>();
                var y = new List<float>();
                foreach (var p in points)
                {
                    x.Add(p.X);
                    y.Add(p.Y);
                }

                Pca2dim(x, y, out float lambda1, out float lambda2, out Point2f vec1, out Point2f vec2);

                var centerX = Statistics.Mean(x);
                var centerY = Statistics.Mean(y);

                var lineNow = new Line2D(vec1.X, vec1.Y, centerX, centerY);

                if (maxIteration <= 0)
                {
                    return lineNow;
                }
                else
                {
                    int iter = 0;
                    int countNow = c;

                    IEnumerable<Point2f> pts = points;
                    do
                    {
                        // 각 점과 직선사이의 거리를 계산
                        var dist = from p in pts
                                   select Distance(lineNow, p);
                        float m = Statistics.Mean(dist);
                        float sig = Statistics.StdDeviation(dist);

                        // 거리가 표준편차 99% 이내에 들어오는 점만 다시 선택
                        pts = from p in pts
                              where Math.Abs(Distance(lineNow, p) - m) < 2.58 * sig
                              select p;

                        int countNew = pts.Count();
                        if (countNew == countNow)
                        {
                            // 모두 다시 선택되었다면 반환
                            return lineNow;
                        }
                        else
                        {
                            // PCA를 재수행
                            x.Clear();
                            y.Clear();
                            foreach (var p in pts)
                            {
                                x.Add(p.X);
                                y.Add(p.Y);
                            }

                            Pca2dim(x, y, out float l1, out float l2, out Point2f v1, out Point2f v2);

                            centerX = Statistics.Mean(x);
                            centerY = Statistics.Mean(y);

                            lineNow = new Line2D(v1.X, v1.Y, centerX, centerY);

                            countNow = countNew;

                            iter++;
                        }
                    } while (iter < maxIteration);

                    return lineNow;
                }
            }
        }

        /// <summary>
        /// Point2f의 집합에 최적 원을 Fitting합니다.
        /// </summary>
        /// <param name="points">Point2f의 집합</param>
        /// <param name="maxIteration">최대 반복 횟수</param>
        /// <returns>최적 원</returns>
        public static Circle2 FitCircle(IEnumerable<Point2f> points, int maxIteration = 20)
        {
            int c = points.Count();
            if (c < 3)
            {
                throw new ArgumentException("점의 수가 부족합니다.", nameof(points));
            }
            else if (c == 3)
            {
                var p1 = points.ElementAt(0);
                var p2 = points.ElementAt(1);
                var p3 = points.ElementAt(2);
                return Circle2.PassingThreePoints(p1, p2, p3);
            }
            else
            {
                var circleNow = FitCirclePart(points);

                if (maxIteration <= 0)
                {
                    return circleNow;
                }
                else
                {
                    int iter = 0;
                    int countNow = c;

                    IEnumerable<Point2f> pts = points;
                    do
                    {
                        // 각 점과 원 사이의 거리를 계산
                        var dist = from p in pts
                                   select circleNow.Distance(p);
                        float m = Statistics.Mean(dist);
                        float sig = Statistics.StdDeviation(dist);

                        // 거리가 표준편차 99% 이내에 들어오는 점만 다시 선택
                        pts = new List<Point2f>(from p in pts
                                                where Math.Abs(circleNow.Distance(p) - m) < (2.58 * sig)
                                                select p);

                        int countNew = pts.Count();
                        if (countNew == countNow)
                        {
                            // 모두 다시 선택되었다면 반환
                            return circleNow;
                        }
                        else
                        {
                            // 원 피팅을 재수행
                            circleNow = FitCirclePart(pts);

                            countNow = countNew;

                            iter++;
                        }
                    } while (iter < maxIteration);

                    return circleNow;
                }
            }
        }

        /// <summary>
        /// SVD를 이용해 Algebraic Distance를 최소화합니다.
        /// </summary>
        /// <param name="points">Point2f의 집합</param>
        /// <returns>Algebraic Distance를 최소화하는 원</returns>
        private static Circle2 FitCirclePart(IEnumerable<Point2f> points)
        {
            int n = points.Count();

            using (var bMat = new Mat<float>(n, 4))
            {
                var bIter = bMat.GetIndexer();
                for (int i = 0; i < n; i++)
                {
                    var p = points.ElementAt(i);
                    bIter[i, 0] = p.X * p.X + p.Y * p.Y;
                    bIter[i, 1] = p.X;
                    bIter[i, 2] = p.Y;
                    bIter[i, 3] = 1;
                }

                Mat u = new Mat();
                try
                {
                    SVD.SolveZ(bMat, u);

                    using (var uMat = new Mat<float>(u))
                    {
                        var uIter = uMat.GetIndexer();
                        float a = uIter[0, 0];
                        float b1 = uIter[1, 0];
                        float b2 = uIter[2, 0];
                        float c = uIter[3, 0];

                        // Center = (-b1/2a, -b2/2a), Radius = Sqrt((b1^2+b2^2)/4a^2 - c/a)
                        return new Circle2(new Point2f(-b1 / (2 * a), -b2 / (2 * a)),
                            (float)Math.Sqrt((b1 * b1 + b2 * b2) / (4 * a * a) - c / a));
                    }
                }
                finally
                {
                    u?.Dispose();
                }
            }
        }
    }
}