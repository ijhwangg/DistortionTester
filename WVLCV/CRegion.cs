using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using WVLCV.WMath;

namespace WVLCV
{
    /// <summary>
    /// 영역을 나타내는 클래스입니다.
    /// </summary>
    public class CRegion
    {
        /// <summary>
        /// 영역을 구성하는 Contour 정보
        /// </summary>
        public Point[][] ContInfo { get; private set; }

        /// <summary>
        /// 영역을 구성하는 Contour의 Hierachy 정보
        /// </summary>
        public HierarchyIndex[] HInfo { get; private set; }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        protected CRegion()
        {
            ContInfo = null;
            HInfo = null;
        }

        /// <summary>
        /// 축과 평행한 직사각형으로부터 영역을 생합니다.
        /// </summary>
        /// <param name="rectangle">축과 평행한 직사각형</param>
        /// <returns>축과 평행한 직사각형으로부터 생성된 영역</returns>
        public static CRegion FromRectangle(Rect rectangle)
        {
            var tl = rectangle.TopLeft;
            var br = rectangle.BottomRight;

            var bl = new Point(rectangle.Left, rectangle.Bottom);
            var tr = new Point(rectangle.Right, rectangle.Top);

            var result = new CRegion()
            {
                ContInfo = new Point[1][] { new Point[4] { tl, bl, br, tr } },
                HInfo = new HierarchyIndex[1] { new HierarchyIndex(-1, -1, -1, -1) },
            };

            return result;
        }

        /// <summary>
        /// 원으로부터 영역을 생성합니다.
        /// </summary>
        /// <param name="circle">원</param>
        /// <returns>원으로부터 생성된 영역</returns>
        public static CRegion FromCircle(Circle2 circle)
        {
            var w = circle.Center.X + circle.Radius + 1;
            var h = circle.Center.Y + circle.Radius + 1;

            using (var temp = new Mat((int)h, (int)w, MatType.CV_8UC1, Scalar.Black))
            {
                temp.Circle((int)circle.Center.X, (int)circle.Center.Y, (int)circle.Radius,
                    Scalar.White, -1, LineTypes.Link8, 1);
                temp.FindContours(out Point[][] cont, out HierarchyIndex[] hi,
                    RetrievalModes.Tree, ContourApproximationModes.ApproxNone);

                return FromContour(cont, hi);
            }
        }

        /// <summary>
        /// Contour와 Hierachy 정보로부터 영역을 생성합니다.
        /// </summary>
        /// <param name="contour">Contour 정보</param>
        /// <param name="hierachy">Hierachy 정보</param>
        /// <returns>Contour와 Hierachy 정보로부터 생성된 영역</returns>
        public static CRegion FromContour(
            IEnumerable<IEnumerable<Point>> contour,
            IEnumerable<HierarchyIndex> hierachy)
        {
            int n = contour.Count();
            int h = hierachy.Count();

            var result = new CRegion()
            {
                ContInfo = new Point[n][],
                HInfo = new HierarchyIndex[h]
            };

            for (int i = 0; i < n; i++)
            {
                var pts = contour.ElementAt(i);
                int m = pts.Count();

                result.ContInfo[i] = new Point[m];
                for (int j = 0; j < m; j++)
                {
                    result.ContInfo[i][j] = pts.ElementAt(j);
                }
            }

            for (int k = 0; k < h; k++)
            {
                var hi = hierachy.ElementAt(k);
                result.HInfo[k] = new HierarchyIndex(hi.Next, hi.Previous, hi.Child, hi.Parent);
            }

            return result;
        }

        private Point GetTopLeft()
        {
            int left = int.MaxValue;
            int top = int.MaxValue;

            for (int i = 0; i < ContInfo.Length; i++)
            {
                for (int j = 0; j < ContInfo[i].Length; j++)
                {
                    var p = ContInfo[i][j];
                    if (p.X < left)
                    {
                        left = p.X;
                    }

                    if (p.Y < top)
                    {
                        top = p.Y;
                    }
                }
            }

            return new Point(left, top);
        }

        private Point GetBottomRight()
        {
            int right = 0;
            int bottom = 0;

            for (int i = 0; i < ContInfo.Length; i++)
            {
                for (int j = 0; j < ContInfo[i].Length; j++)
                {
                    var p = ContInfo[i][j];
                    if (right < p.X)
                    {
                        right = p.X;
                    }

                    if (bottom < p.Y)
                    {
                        bottom = p.Y;
                    }
                }
            }

            return new Point(right, bottom);
        }

        /// <summary>
        /// 두 영역의 합집합을 구합니다.
        /// </summary>
        /// <param name="lhs">첫번째 영역</param>
        /// <param name="rhs">두번째 영역</param>
        /// <returns>두 영역의 합집합</returns>
        public static CRegion operator |(CRegion lhs, CRegion rhs)
        {
            var p1 = lhs.GetBottomRight();
            var p2 = rhs.GetBottomRight();

            int w = p2.X < p1.X ? p1.X : p2.X;
            int h = p2.Y < p1.Y ? p1.Y : p2.Y;

            using (var m1 = lhs.GetMask(w + 5, h + 5))
            using (var m2 = rhs.GetMask(w + 5, h + 5))
            using (var or = new Mat())
            {
                Cv2.BitwiseOr(m1, m2, or);
                or.FindContours(out Point[][] cont, out HierarchyIndex[] hi,
                    RetrievalModes.Tree, ContourApproximationModes.ApproxNone);

                return FromContour(cont, hi);
            }
        }

        /// <summary>
        /// 두 영역의 교집합을 구합니다.
        /// </summary>
        /// <param name="lhs">첫번째 영역</param>
        /// <param name="rhs">두번째 영역</param>
        /// <returns>두 영역의 교집합</returns>
        public static CRegion operator &(CRegion lhs, CRegion rhs)
        {
            var p1 = lhs.GetBottomRight();
            var p2 = rhs.GetBottomRight();

            int w = p2.X < p1.X ? p1.X : p2.X;
            int h = p2.Y < p1.Y ? p1.Y : p2.Y;

            using (var m1 = lhs.GetMask(w + 5, h + 5))
            using (var m2 = rhs.GetMask(w + 5, h + 5))
            using (var and = new Mat())
            {
                Cv2.BitwiseAnd(m1, m2, and);
                and.FindContours(out Point[][] cont, out HierarchyIndex[] hi,
                    RetrievalModes.Tree, ContourApproximationModes.ApproxNone);

                return FromContour(cont, hi);
            }
        }

        /// <summary>
        /// 두 영역의 차집합을 구합니다.
        /// </summary>
        /// <param name="lhs">첫번째 영역</param>
        /// <param name="rhs">두번째 영역</param>
        /// <returns>두 영역의 차집합</returns>
        public static CRegion operator -(CRegion lhs, CRegion rhs)
        {
            var p1 = lhs.GetBottomRight();
            var p2 = rhs.GetBottomRight();

            int w = p2.X < p1.X ? p1.X : p2.X;
            int h = p2.Y < p1.Y ? p1.Y : p2.Y;

            using (var m1 = lhs.GetMask(w + 5, h + 5))
            using (var m2 = rhs.GetMask(w + 5, h + 5))
            using (var m2c = new Mat())
            {
                Cv2.BitwiseNot(m2, m2c);
                using (var and = new Mat())
                {
                    Cv2.BitwiseAnd(m1, m2c, and);
                    and.FindContours(out Point[][] cont, out HierarchyIndex[] hi, RetrievalModes.Tree, ContourApproximationModes.ApproxNone);

                    return FromContour(cont, hi);
                }
            }
        }

        /// <summary>
        /// Mask로 사용하기 위한 이미지를 생성합니다.
        /// </summary>
        /// <param name="width">Mask 이미지의 폭</param>
        /// <param name="height">Mask 이미지의 높이</param>
        /// <returns>Mask로 사용하기 위한 이미지</returns>
        public Mat GetMask(int width, int height)
        {
            var mask = new Mat(height, width, MatType.CV_8UC1, Scalar.Black);
            mask.DrawContours(ContInfo, -1, Scalar.White, -1, LineTypes.Link8, HInfo);

            return mask;
        }

        /// <summary>
        /// 해당 영역에서의 Gray Value 부피와 무게중심을 계산합니다.
        /// </summary>
        /// <param name="grayImage">Gray Value 이미지</param>
        /// <param name="volume">Gray Value 부피</param>
        /// <param name="center">Gray Value 무게중심</param>
        /// <param name="power">Gray Value에 Powered Weight를 줍니다. w.g. = pow(g, power)</param>
        public void GrayVolumeCenter(Mat grayImage, out float volume, out Point2f center, float power = 1)
        {
            var tl = GetTopLeft();
            var br = GetBottomRight();

            using (var mob = new Mat<byte>(grayImage))
            using (var bin = GetMask(br.X + 2, br.Y + 2))
            using (var binMat = new Mat<byte>(bin))
            {
                var iter = mob.GetIndexer();
                var biter = binMat.GetIndexer();

                float vol = 0;
                float x = 0;
                float y = 0;

                for (int r = tl.Y - 1; r <= br.Y + 1; r++)
                {
                    if (r < 0) r = 0;
                    for (int c = tl.X - 1; c <= br.X + 1; c++)
                    {
                        if (c < 0) c = 0;
                        if (biter[r, c] == 255)
                        {
                            byte g = iter[r, c];
                            float wg = g;
                            if (power != 1)
                            {
                                wg = (float)Math.Pow(wg, power);
                            }

                            vol += wg;
                            x += wg * c;
                            y += wg * r;
                        }
                    }
                }

                if (vol != 0)
                {
                    x /= vol;
                    y /= vol;
                }

                volume = vol;
                center = new Point2f(x, y);
            }
        }

        /// <summary>
        /// 영역의 넓이를 계산합니다.
        /// </summary>
        /// <returns>영역의 넓이</returns>
        public int Area()
        {
            var br = GetBottomRight();
            using (var bin = GetMask(br.X + 2, br.Y + 2))
            {
                return bin.CountNonZero();
            }
        }

        /// <summary>
        /// 영역 안에 있는 Gray Value의 평균 구합니다.
        /// </summary>
        /// <param name="grayImage">Gray Value 이미지</param>
        /// <returns>Gray Value 평균</returns>
        public float GrayAverage(Mat grayImage)
        {
            var br = GetBottomRight();
            using (var mob = new Mat<byte>(grayImage))
            using (var bin = GetMask(br.X + 2, br.Y + 2))
            using (var binMat = new Mat<byte>(bin))
            {
                var iter = mob.GetIndexer();
                var biter = binMat.GetIndexer();

                var values = new List<float>();
                for (int r = 0; r < br.Y + 2; r++)
                {
                    for (int c = 0; c < br.X + 2; c++)
                    {
                        if (biter[r, c] == 255)
                        {
                            values.Add(iter[r, c]);
                        }
                    }
                }

                return (float)Statistics.Mean(values);
            }
        }

        /// <summary>
        /// 영역 안에 있는 Gray Value의 표준편차를 구합니다.
        /// </summary>
        /// <param name="grayImage">Gray Value 이미지</param>
        /// <returns>Gray Value 표준편차</returns>
        public float GrayDeviation(Mat grayImage)
        {
            var br = GetBottomRight();
            using (var mob = new Mat<byte>(grayImage))
            using (var bin = GetMask(br.X + 2, br.Y + 2))
            using (var binMat = new Mat<byte>(bin))
            {
                var iter = mob.GetIndexer();
                var biter = binMat.GetIndexer();

                var values = new List<float>();
                for (int r = 0; r < br.Y + 2; r++)
                {
                    for (int c = 0; c < br.X + 2; c++)
                    {
                        if (biter[r, c] == 255)
                        {
                            values.Add(iter[r, c]);
                        }
                    }
                }

                return Statistics.StdDeviation(values);
            }
        }

        /// <summary>
        /// 영역을 축소시킵니다.
        /// </summary>
        /// <param name="radius">축소할 반경</param>
        /// <returns>축소된 영역</returns>
        public CRegion Erode(float radius)
        {
            var br = GetBottomRight();

            int s = (int)(radius * 2);
            using (var bin = GetMask(br.X + 2 * s, br.Y + 2 * s))
            using (var el = Cv2.GetStructuringElement(MorphShapes.Ellipse, new Size(s, s)))
            using (var erd = bin.Erode(el))
            {
                erd.FindContours(out Point[][] cont, out HierarchyIndex[] hi, RetrievalModes.Tree, ContourApproximationModes.ApproxNone);

                return FromContour(cont, hi);
            }
        }

        /// <summary>
        /// 영역을 팽창시킵니다.
        /// </summary>
        /// <param name="radius">팽창할 반경</param>
        /// <returns>팽창한 영역</returns>
        public CRegion Dilate(float radius)
        {
            var br = GetBottomRight();

            int s = (int)(radius * 2);
            using (var bin = GetMask(br.X + 2 * s, br.Y + 2 * s))
            using (var el = Cv2.GetStructuringElement(MorphShapes.Ellipse, new Size(s, s)))
            using (var dil = bin.Dilate(el))
            {
                dil.FindContours(out Point[][] cont, out HierarchyIndex[] hi, RetrievalModes.Tree, ContourApproximationModes.ApproxNone);

                return FromContour(cont, hi);
            }
        }

        /// <summary>
        /// 영역을 Open시킵니다.
        /// </summary>
        /// <param name="radius">Open할 반경</param>
        /// <returns>Open된 영역</returns>
        public CRegion Open(float radius)
        {
            var br = GetBottomRight();

            int s = (int)(radius * 2);
            using (var bin = GetMask(br.X + 2 * s, br.Y + 2 * s))
            using (var el = Cv2.GetStructuringElement(MorphShapes.Ellipse, new Size(s, s)))
            using (var opn = bin.MorphologyEx(MorphTypes.Open, el))
            {
                opn.FindContours(out Point[][] cont, out HierarchyIndex[] hi, RetrievalModes.Tree, ContourApproximationModes.ApproxNone);

                return FromContour(cont, hi);
            }
        }

        /// <summary>
        /// 영역을 Close시킵니다.
        /// </summary>
        /// <param name="radius">Close할 반경</param>
        /// <returns>Close된 영역</returns>
        public CRegion Close(float radius)
        {
            var br = GetBottomRight();

            int s = (int)(radius * 2);
            using (var bin = GetMask(br.X + 2 * s, br.Y + 2 * s))
            using (var el = Cv2.GetStructuringElement(MorphShapes.Ellipse, new Size(s, s)))
            using (var cls = bin.MorphologyEx(MorphTypes.Close, el))
            {
                cls.FindContours(out Point[][] cont, out HierarchyIndex[] hi, RetrievalModes.Tree, ContourApproximationModes.ApproxNone);

                return FromContour(cont, hi);
            }
        }

        /// <summary>
        /// 영역을 연결된 성분별로 분리합니다.
        /// </summary>
        /// <returns>연결 영역의 배열</returns>
        public CRegion[] SplitConnectedComponets()
        {
            var sign = new bool[ContInfo.Length];
            var outIndex = new List<int>();

            for (int i = 0; i < ContInfo.Length; i++)
            {
                if (HInfo[i].Parent == -1)
                {
                    sign[i] = true;
                    outIndex.Add(i);
                }
                else
                {
                    sign[i] = sign[HInfo[i].Parent] ^ true;
                    if (sign[i])
                    {
                        outIndex.Add(i);
                    }
                }
            }

            var result = new CRegion[outIndex.Count];

            for (int i = 0; i < outIndex.Count; i++)
            {
                int nDirectChild = 0;
                int cindex = HInfo[outIndex[i]].Child;
                if (cindex != -1)
                {
                    do
                    {
                        nDirectChild++;
                        cindex = HInfo[cindex].Next;
                    } while (cindex != -1);
                }

                var scont = new Point[nDirectChild + 1][];
                scont[0] = new Point[ContInfo[outIndex[i]].Length];
                Array.Copy(ContInfo[outIndex[i]], scont[0], ContInfo[outIndex[i]].Length);

                cindex = HInfo[outIndex[i]].Child;
                for (int j = 1; j <= nDirectChild; j++)
                {
                    scont[j] = new Point[ContInfo[cindex].Length];
                    Array.Copy(ContInfo[cindex], scont[j], ContInfo[cindex].Length);

                    cindex = HInfo[cindex].Next;
                }

                var shi = new HierarchyIndex[nDirectChild + 1];
                shi[0] = new HierarchyIndex(-1, -1, 1, -1);
                for (int j = 1; j <= nDirectChild; j++)
                {
                    int next = (j + 1) == nDirectChild + 1 ? -1 : j + 1;
                    int prev = (j - 1) <= 0 ? -1 : j - 1;
                    shi[j] = new HierarchyIndex(next, prev, -1, 0);
                }

                result[i] = FromContour(scont, shi);
            }

            return result;
        }
    }
}