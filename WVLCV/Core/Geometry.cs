using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using WVLCV.WMath;

namespace WVLCV.Core
{
    /// <summary>
    /// 기하학 관련 메서드들의 집합
    /// </summary>
    public static class Geometry
    {
        /// <summary>
        /// 두 직선 <see cref="Line2D"/>의 교점을 계산합니다.
        /// </summary>
        /// <param name="l1">첫번째 직선</param>
        /// <param name="l2">두번째 직선</param>
        /// <param name="intersection">두 직선의 교점이 존재한다면 반환되고, 존재하지 않으면 (0, 0)을 반환합니다.</param>
        /// <returns>교점이 존재하면 0, 불능이면 -1, 부정이면 1을 반환합니다.</returns>
        public static int IntersectionLines(Line2D l1, Line2D l2, out Point2f intersection)
        {
            float a = (float)l1.Vy, b = (float)(-l1.Vx), e = (float)(l1.Vy * l1.X1 - l1.Vx * l1.Y1);
            float c = (float)l2.Vy, d = (float)(-l2.Vx), f = (float)(l2.Vy * l2.X1 - l2.Vx * l2.Y1);

            float det = a * d - b * c;
            if (det == 0)
            {
                intersection = new Point2f(0, 0);
                if (a * f - c * e == 0 && b * f - c * e == 0)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                float x = (e * d - b * f) / det;
                float y = (a * f - e * c) / det;
                intersection = new Point2f(x, y);

                return 0;
            }
        }

        /// <summary>
        /// 주어진 모든 점을 포함하는 가장 작은 원을 계산합니다.
        /// </summary>
        /// <param name="points">주어진 점</param>
        /// <returns>주어진 모든 점을 포함하는 가장 작은 원</returns>
        public static Circle2 SmallestEnclosingCircle(IEnumerable<Point2f> points)
        {
            // 1. Convex Hull을 계산
            var pt2f = from p in points
                       select new Point2f((float)p.X, (float)p.Y);
            var convex = Cv2.ConvexHull(pt2f);
            var convPt = from p in convex
                         select new Point2f(p.X, p.Y);

            // 2. 무게중심 G를 계산
            //    G로부터 가장 먼 점 P0을 선택
            //    단위벡터 G->P0와 직선 L을 기록
            int n = convPt.Count();
            float sX = 0;
            float sY = 0;
            for (int i = 0; i < n; i++)
            {
                var temp = convPt.ElementAt(i);
                sX += temp.X;
                sY += temp.Y;
            }
            sX /= n;
            sY /= n;
            var ptG = new Point2f(sX, sY);

            float maxDist = float.NegativeInfinity;
            int pt0Index = 0;
            var pt0 = convPt.ElementAt(0);
            for (int i = 0; i < n; i++)
            {
                var temp = convPt.ElementAt(i);
                float d = (float)ptG.DistanceTo(temp);
                if (maxDist < d)
                {
                    pt0Index = i;
                    pt0 = temp;
                    maxDist = d;
                }
            }

            var vec = (pt0 - ptG) * (1 / maxDist);
            var line = new Line2D(vec.X, vec.Y, ptG.X, ptG.Y);

            // 3. P0과 다른 점을 잇는 선분의 수직 이등분선 T를 계산
            //    L과 T의 교점 I와 반지름 R_i = |I->P0|을 계산
            //    원 (I, R_i)가 모든 점을 포함하면 기록
            //    R_i중 가장 작은 것과 그때의 점 P_i를 기록
            //    원 (I, R_i)가 점을 3개 포함하고 있으면 다음 단계 생략
            float err = (float)Math.Sqrt(2) / 2;
            float minRad = float.PositiveInfinity;
            Point2f minPt = new Point2f();
            Point2f minInter = new Point2f();
            Point2f minCenter = new Point2f();
            Circle2 minCircle = new Circle2();

            for (int i = 0; i < n; i++)
            {
                if (i != pt0Index)
                {
                    var other = convPt.ElementAt(i);
                    var ovec = other - pt0;
                    float ovecL = (float)other.DistanceTo(pt0);
                    ovec *= (1 / ovecL);
                    var operp = new Point2f(ovec.Y, -ovec.X);
                    var ocent = (other + pt0) * 0.5;

                    var lineT = new Line2D(operp.X, operp.Y, ocent.X, ocent.Y);

                    if (IntersectionLines(line, lineT, out Point2f ointer) == 0)
                    {
                        float orad = (float)ointer.DistanceTo(pt0);
                        var testC = new Circle2(ointer, orad);

                        if (orad < minRad)
                        {
                            var conta = from p in convPt
                                        where testC.Distance(p) < err
                                        select p;
                            if (conta.Count() == n)
                            {
                                minRad = orad;
                                minPt = other;
                                minInter = ointer;
                                minCenter = ocent;
                                minCircle = testC;
                            }
                        }
                    }
                }
            }

            // 4. 선분 P0~P_i의 이등분선 방향으로 3개의 점에 접할 때까지 검색
            //    없으면 원 (P0<->P_i)가 구하려는 원
            var onCircle = from p in convPt
                           where Math.Abs(minCircle.Distance(p)) < err
                           select p;

            var direcBi = minCenter - minInter;
            float s = 0;
            Point2f iShift = minInter;
            Circle2 shiftC = new Circle2(minInter, minRad);

            while (onCircle.Count() < 3 && s < 1)
            {
                s += 0.01f;

                iShift = minInter + direcBi * s;
                float radShift = (float)iShift.DistanceTo(pt0);
                shiftC = new Circle2(iShift, radShift);

                onCircle = from p in convPt
                           where Math.Abs(shiftC.Distance(p)) < err
                           select p;
            }

            if (onCircle.Count() < 3)
            {
                var tempCr = Circle2.PassingTwoPoints(pt0, minPt);
                return new Circle2(tempCr.Center, tempCr.Radius + 0.5f);
            }
            else
            {
                // 5. 3개 이상의 원 위의 점을 얻었다면 반시계방향으로 정렬
                //    인접하는 두 점 사이의 각도가 모두 180도보다 작거나 같으면 종료
                var ptAngle = from p in onCircle
                              select new { Point = p, Angle = 0 < Math.Atan2(p.Y, p.X) ? Math.Atan2(p.Y, p.X) : Math.Atan2(p.Y, p.X) + 2 * Math.PI };
                var sorted = ptAngle.OrderBy((pa) => { return pa.Angle; });
                int sn = sorted.Count();

                Point2f p01 = new Point2f();
                Point2f p02 = new Point2f();
                bool isSelected = false;
                for (int i = 0; i < sn; i++)
                {
                    int j = i + 1 != sn ? i + 1 : 0;
                    float t1 = (float)sorted.ElementAt(j).Angle;
                    float t2 = (float)sorted.ElementAt(j).Angle;

                    if (Math.PI < t1 - t2)
                    {
                        p01 = sorted.ElementAt(i).Point;
                        p02 = sorted.ElementAt(j).Point;
                        isSelected = true;
                        break;
                    }
                }

                if (!isSelected)
                {
                    return new Circle2(shiftC.Center, shiftC.Radius + 0.5f);
                }
                else
                {
                    // 6. 180도보다 큰 점의 쌍이 있다면 그 두 점에 대해 4번을 수행
                    var acent = (p01 + p02) * 0.5;
                    var aperp = acent - iShift;
                    float alen = (float)acent.DistanceTo(iShift);

                    float iter = 2 * err / alen;
                    var ainter = iShift + aperp * iter;
                    float arad = (float)ainter.DistanceTo(p01);

                    var aCir = new Circle2(ainter, arad);

                    onCircle = from p in convPt
                               where Math.Abs(aCir.Distance(p)) < err
                               select p;

                    while (onCircle.Count() < 3 && iter < 1)
                    {
                        iter += 0.01f;

                        ainter = iShift + aperp * iter;
                        arad = (float)ainter.DistanceTo(p01);
                        aCir = new Circle2(ainter, arad);

                        onCircle = from p in convPt
                                   where Math.Abs(aCir.Distance(p)) < err
                                   select p;
                    }

                    if (onCircle.Count() < 3)
                    {
                        var tempCr = Circle2.PassingTwoPoints(p01, p02);
                        return new Circle2(tempCr.Center, tempCr.Radius + 0.5f);
                    }
                    else
                    {
                        return new Circle2(aCir.Center, aCir.Radius + 0.5f);
                    }
                }
            }
        }

        /// <summary>
        /// 주어진 Contour 안에 포함되는 가장 큰 원을 계산합니다.
        /// </summary>
        /// <param name="contour">점의 집합으로 표현된 Contour</param>
        /// <param name="imageWidth">검색 영역의 폭</param>
        /// <param name="imageHeight">검색 영역의 높이</param>
        /// <returns>주어진 Contour 안에 포함되는 가장 큰 원</returns>
        public static Circle2 LargestInnerCircle(IEnumerable<Point> contour, int imageWidth, int imageHeight)
        {
            using (Subdiv2D subdiv = new Subdiv2D())
            {
                subdiv.InitDelaunay(new Rect(0, 0, imageWidth, imageHeight));
                var cont2f = from p in contour
                             select new Point2f(p.X, p.Y);
                subdiv.Insert(cont2f);
                var trs = subdiv.GetTriangleList();

                float maxArea2 = float.NegativeInfinity;
                Circle2 maxIncircle = new Circle2();
                for (int i = 0; i < trs.Length; i++)
                {
                    var vec6 = trs[i];

                    var p1 = new Point2f(vec6[0], vec6[1]);
                    var p2 = new Point2f(vec6[2], vec6[3]);
                    var p3 = new Point2f(vec6[4], vec6[5]);

                    float area = (p1.X * p2.Y + p2.X * p3.Y + p3.X * p1.Y) - (p2.X * p1.Y + p3.X * p2.Y + p1.X * p3.Y);
                    if (maxArea2 < area)
                    {
                        var outCircle = Circle2.PassingThreePoints(
                            new Point2f(p1.X, p1.Y),
                            new Point2f(p2.X, p2.Y),
                            new Point2f(p3.X, p3.Y));

                        var cvpt = from p in contour
                                   select new Point(p.X, p.Y);

                        if (0 <= outCircle.Center.X && outCircle.Center.X < imageWidth &&
                            0 <= outCircle.Center.Y && outCircle.Center.Y < imageHeight &&
                                Cv2.PointPolygonTest(cvpt, new Point2f((float)outCircle.Center.X, (float)outCircle.Center.Y), false) == 1)
                        {
                            maxArea2 = area;
                            maxIncircle = outCircle;
                        }
                    }
                }

                return maxIncircle;
            }
        }

        /// <summary>
        /// 각의 이등분선을 계산합니다.
        /// </summary>
        /// <param name="center">끼인각의 중심점</param>
        /// <param name="left">끼인각에서 안쪽을 향했을 때 왼쪽에 있는 점</param>
        /// <param name="right">끼인각에서 안쪽을 향했을 때 오른쪽에 있는 점</param>
        /// <returns>각의 이등분선</returns>
        public static Line2D BisectAngle(Point2f center, Point2f left, Point2f right)
        {
            // 각 점으로의 벡터를 계산
            var vec1 = left - center;
            var vec2 = right - center;

            // 두 벡터의 동경을 계산
            float t1 = (float)Math.Atan2(vec1.Y, vec1.X);
            float t2 = (float)Math.Atan2(vec2.Y, vec2.X);

            // 동경의 중심을 계산
            // 항상 각의 안쪽을 향하도록 보정
            float t = (t1 + t2) / 2;
            if (Math.PI / 2 < Math.Abs(t - t1))
            {
                t += (float)Math.PI;
            }

            return new Line2D(Math.Cos(t), Math.Sin(t), center.X, center.Y);
        }

        /// <summary>
        /// 삼각형의 내접원을 계산합니다.
        /// <para>
        /// [입력 조건] 삼각형의 세 점을 시계 반대방향 순서로 입력해야 합니다.
        /// </para>
        /// <para>
        /// [예외] 예외가 발생하는 대신에 중심이 (0, 0)이고 반지름이 0인 원을 반환합니다.
        /// </para>
        /// </summary>
        /// <param name="p1">삼각형의 첫번째 꼭짓점</param>
        /// <param name="p2">삼각형의 두번째 꼭짓점</param>
        /// <param name="p3">삼각형의 세번째 꼭짓점</param>
        /// <returns>삼각형의 내접원</returns>
        public static Circle2 InscribedCircleOfTriangle(Point2f p1, Point2f p2, Point2f p3)
        {
            var bi1 = BisectAngle(p1, p3, p2);
            var bi2 = BisectAngle(p2, p1, p3);

            if (IntersectionLines(bi1, bi2, out var inter) == 0)
            {
                var v = p2 - p1;
                float len = (float)Math.Sqrt(v.X * v.X + v.Y * v.Y);
                v *= (1 / len);
                var l = new Line2D(v.X, v.Y, p1.X, p1.Y);

                float rad = Math.Abs(Fittings.Distance(l, inter));

                return new Circle2(inter, rad);
            }
            else
            {
                return new Circle2();
            }
        }

        /// <summary>
        /// 두 점으로 나타낸 선분의 수직이등분선을 계산합니다.
        /// <para>
        /// [입력 조건] 두 점이 달라야 합니다.
        /// </para>
        /// <para>
        /// [예외] 예외가 발생하는 대신에 null을 반환합니다.
        /// </para>
        /// </summary>
        /// <param name="p1">선분의 첫번째 점</param>
        /// <param name="p2">선분의 두번째 점</param>
        /// <returns>수직이등분선</returns>
        public static Line2D PerpendicularBisector(Point2f p1, Point2f p2)
        {
            try
            {
                var center = (p1 + p2) * 0.5;
                var d = p2 - p1;
                float len = (float)p2.DistanceTo(p1);
                d *= (1 / len);

                var perp = new Point2f(-d.Y, d.X);

                return new Line2D(perp.X, perp.Y, center.X, center.Y);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 지정한 직선에 수직하고 주어진 한 점을 지나는 직선을 계산합니다.
        /// </summary>
        /// <param name="line">지정한 직선</param>
        /// <param name="pass">지나가야 하는 점</param>
        /// <returns>지정한 직선에 수직하고 주어진 한 점을 지나는 직선</returns>
        public static Line2D PerpendicularLine(Line2D line, Point2f pass)
        {
            var direc = new Point2f((float)line.Vx, (float)line.Vy);
            var perp = new Point2f(-direc.Y, direc.X);

            return new Line2D(perp.X, perp.Y, pass.X, pass.Y);
        }
    }
}