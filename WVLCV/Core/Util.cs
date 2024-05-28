using OpenCvSharp;
using System;

namespace WVLCV.Core
{
    /// <summary>
    /// 편의 기능을 모아놓은 메서드 집합
    /// </summary>
    public static class Util
    {
        /// <summary>
        /// 지정한 배율로 확대/축소한 이미지를 화면에 표시하고 키 입력을 기다립니다.
        /// </summary>
        /// <param name="img">표시할 이미지</param>
        /// <param name="fx">X 배율</param>
        /// <param name="fy">Y 배율</param>
        public static void ShowImageZoom(Mat img, float fx, float fy)
        {
            using (var zoom = img.Resize(Size.Zero, fx, fy))
            using (var wind = new Window())
            {
                wind.Image = zoom;
                wind.Move(300, 300);
                Cv2.WaitKey();
            }
        }

        /// <summary>
        /// <see cref="Mat"/> 이미지에 <see cref="Circle2"/>을 그립니다.
        /// </summary>
        /// <param name="img">대상 이미지</param>
        /// <param name="circle">그릴 원</param>
        /// <param name="color">그릴 색상</param>
        /// <param name="thickness">그릴 두께 (-1로 지정하면 원을 꽉 채움)</param>
        public static void DrawCircle2(Mat img, Circle2 circle, Scalar color, int thickness = 1)
        {
            int fx = (int)(circle.Center.X * 16);
            int fy = (int)(circle.Center.Y * 16);
            int fr = (int)(circle.Radius * 16);
            img.Circle(fx, fy, fr, color, thickness, LineTypes.AntiAlias, 4);
        }

        /// <summary>
        /// <see cref="Mat"/> 이미지에 십자가를 그립니다.
        /// </summary>
        /// <param name="img">대상 이미지</param>
        /// <param name="position">그릴 위치</param>
        /// <param name="color">그릴 색상</param>
        /// <param name="size">십자가의 크기</param>
        /// <param name="degree">십자가의 각도</param>
        /// <param name="thickness">그릴 두께</param>
        public static void DrawCross(Mat img, Point2f position, Scalar color,
            float size, float degree, int thickness = 1)
        {
            float angle = degree * (float)Math.PI / 180.0f;
            float cos = (float)Math.Cos(angle);
            float sin = (float)Math.Sin(angle);

            var d1 = new Point2f(cos, sin);
            var d2 = new Point2f(sin, -cos);

            d1 *= size;
            d2 *= size;

            var p11 = position + d1;
            var p12 = position - d1;

            var p21 = position + d2;
            var p22 = position - d2;

            int p11x = (int)(p11.X * 16);
            int p11y = (int)(p11.Y * 16);
            int p12x = (int)(p12.X * 16);
            int p12y = (int)(p12.Y * 16);
            img.Line(p11x, p11y, p12x, p12y, new Scalar(color.Val0, color.Val1, color.Val2),
                thickness, LineTypes.AntiAlias, 4);

            int p21x = (int)(p21.X * 16);
            int p21y = (int)(p21.Y * 16);
            int p22x = (int)(p22.X * 16);
            int p22y = (int)(p22.Y * 16);
            img.Line(p21x, p21y, p22x, p22y, new Scalar(color.Val0, color.Val1, color.Val2),
                thickness, LineTypes.AntiAlias, 4);
        }
    }
}