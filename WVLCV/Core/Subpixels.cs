using OpenCvSharp;
using System;

namespace WVLCV.Core
{
    /// <summary>
    /// Subpixel 취득 관련 메서드들의 집합
    /// </summary>
    public static class Subpixels
    {
        /// <summary>
        /// 쌍선형 방식으로 Subpixel Gray Value를 취득합니다.
        /// <para>
        /// [예외] 예외를 방출하는 대신에 float.NaN을 반환합니다.
        /// </para>
        /// </summary>
        /// <param name="grayImage">CV_8UC1 OpenCV Mat 이미지</param>
        /// <param name="x">취득할 X좌표</param>
        /// <param name="y">취득할 Y좌표</param>
        /// <returns>쌍선형 방식으로 보간된 Gray Value</returns>
        public static float GrayValueBilinear(Mat grayImage, float x, float y)
        {
            try
            {
                int r = (int)Math.Floor(y);
                int c = (int)Math.Floor(x);

                if (0 < r && r + 1 < grayImage.Rows &&
                    0 < c && c + 1 < grayImage.Cols)
                {
                    float a = y - r;
                    float b = x - c;

                    using (var gMat = new Mat<byte>(grayImage))
                    {
                        var iter = gMat.GetIndexer();

                        byte g00 = iter[r, c], g01 = iter[r, c + 1];
                        byte g10 = iter[r + 1, c], g11 = iter[r + 1, c + 1];

                        float c0 = g00 + b * (g01 - g00);
                        float c1 = g10 + b * (g11 - g10);

                        return c0 + a * (c1 - c0);
                    }
                }
                else
                {
                    return float.NaN;
                }
            }
            catch
            {
                return float.NaN;
            }
        }
    }
}