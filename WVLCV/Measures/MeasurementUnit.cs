using OpenCvSharp;
using System.Collections.Generic;
using System.Linq;
using WVLCV.Core;
using WVLCV.WMath;

namespace WVLCV.Measures
{
    /// <summary>
    /// Foreground와 Backgoround만을 가지는 영역에서 Half Maximum Subpixel Point를 검색하는 불변 구조체입니다.
    /// </summary>
    public struct MeasurementUnit
    {
        private Point2f Begin { get; }

        private Point2f End { get; }

        private int Width { get; }

        /// <summary>
        /// 시작점과 끝점, 그리고 폭으로 측정 Unit을 설정합니다.
        /// </summary>
        /// <param name="begin">시작점</param>
        /// <param name="end">끝점</param>
        /// <param name="width">0 또는 그 이상의 폭</param>
        public MeasurementUnit(Point2f begin, Point2f end, int width)
        {
            Begin = begin;
            End = end;
            Width = width <= 0 ? 0 : width;
        }

        /// <summary>
        /// 이미지에서 Half Maximum Subpixel Point를 검색합니다.
        /// </summary>
        /// <param name="grayImage">CV_8UC1 Open CV Mat 이미지</param>
        /// <returns>측정 범위 내의 Half Maximum Subpixel Point</returns>
        public Point2f Get(Mat grayImage)
        {
            var vec = End - Begin;
            var len = End.DistanceTo(Begin);
            vec *= 1 / len;

            var perp = new Point2f(vec.Y, -vec.X);

            // 시작점 부터 1 pixel 거리 단위로 Gray Profile 생성
            List<float> y = new List<float>();
            for (float i = 0; i < len; i += 1)
            {
                var at = Begin + vec * i;
                float grayVal = 0;
                int count = 0;

                // 수직 방향으로 Width 만큼의 폭으로 Gray Level을 조사하여 평균을 계산한 것이 현재 위치에서의 Gray Profile
                for (int j = 0; j <= Width; j++)
                {
                    if (j == 0)
                    {
                        grayVal += Subpixels.GrayValueBilinear(grayImage, at.X, at.Y);
                        count++;
                    }
                    else
                    {
                        var at1 = at + perp * j;
                        var at2 = at + perp * (-j);
                        grayVal += Subpixels.GrayValueBilinear(grayImage, at1.X, at1.Y);
                        grayVal += Subpixels.GrayValueBilinear(grayImage, at2.X, at2.Y);
                        count += 2;
                    }
                }
                grayVal /= count;
                y.Add(grayVal);
            }

            // Half Maximum의 기준이 되는 Maximum(yTop)을 계산하기 위해
            // 최댓값과 중앙값의 1:9 분할값보다 큰 값들의 평균을 선택
            float max = y.Max();
            float min = y.Min();
            float center = (max + min) / 2;
            float bound = (center + 9 * max) / 10;

            var yTop = from yVal in y
                       where bound < yVal
                       select yVal;
            float target = Statistics.Mean(yTop) / 2;

            // Gray Profile을 선형 보간된 함수로 만들어서 목표 위치를 계산
            var f = new Interpolation1D(y);
            float t = f.Arg(target);

            return Begin + vec * t;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="grayImage"></param>
        /// <returns></returns>
        public Point2f GetHalfMax(Mat grayImage)
        {
            var vec = End - Begin;
            var len = End.DistanceTo(Begin);
            vec *= 1 / len;

            var perp = new Point2f(vec.Y, -vec.X);

            // 시작점 부터 1 pixel 거리 단위로 Gray Profile 생성
            List<float> y = new List<float>();
            for (float i = 0; i < len; i += 1)
            {
                var at = Begin + vec * i;
                float grayVal = 0;
                int count = 0;

                // 수직 방향으로 Width 만큼의 폭으로 Gray Level을 조사하여 평균을 계산한 것이 현재 위치에서의 Gray Profile
                for (int j = 0; j <= Width; j++)
                {
                    if (j == 0)
                    {
                        grayVal += Subpixels.GrayValueBilinear(grayImage, at.X, at.Y);
                        count++;
                    }
                    else
                    {
                        var at1 = at + perp * j;
                        var at2 = at + perp * (-j);
                        grayVal += Subpixels.GrayValueBilinear(grayImage, at1.X, at1.Y);
                        grayVal += Subpixels.GrayValueBilinear(grayImage, at2.X, at2.Y);
                        count += 2;
                    }
                }
                grayVal /= count;
                y.Add(grayVal);
            }

            // Half Maximum의 기준이 되는 Maximum(yTop)을 계산하기 위해
            // 최댓값과 중앙값의 1:9 분할값보다 큰 값들의 평균을 선택
            float max = y.Max();
            float min = y.Min();
            float center = max / 2;
            float bound = (center + 9 * max) / 10;

            var yTop = from yVal in y
                       where bound < yVal
                       select yVal;
            float target = Statistics.Mean(yTop) / 2;

            // Gray Profile을 선형 보간된 함수로 만들어서 목표 위치를 계산
            var f = new Interpolation1D(y);
            float t = f.Arg(target);

            return Begin + vec * t;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="grayImage"></param>
        /// <returns></returns>
        public Point2f GetQuarter(Mat grayImage)
        {
            var vec = End - Begin;
            var len = End.DistanceTo(Begin);
            vec *= 1 / len;

            var perp = new Point2f(vec.Y, -vec.X);

            // 시작점 부터 1 pixel 거리 단위로 Gray Profile 생성
            List<float> y = new List<float>();
            for (float i = 0; i < len; i += 1)
            {
                var at = Begin + vec * i;
                float grayVal = 0;
                int count = 0;

                // 수직 방향으로 Width 만큼의 폭으로 Gray Level을 조사하여 평균을 계산한 것이 현재 위치에서의 Gray Profile
                for (int j = 0; j <= Width; j++)
                {
                    if (j == 0)
                    {
                        grayVal += Subpixels.GrayValueBilinear(grayImage, at.X, at.Y);
                        count++;
                    }
                    else
                    {
                        var at1 = at + perp * j;
                        var at2 = at + perp * (-j);
                        grayVal += Subpixels.GrayValueBilinear(grayImage, at1.X, at1.Y);
                        grayVal += Subpixels.GrayValueBilinear(grayImage, at2.X, at2.Y);
                        count += 2;
                    }
                }
                grayVal /= count;
                y.Add(grayVal);
            }

            // Half Maximum의 기준이 되는 Maximum(yTop)을 계산하기 위해
            // 최댓값과 중앙값의 1:9 분할값보다 큰 값들의 평균을 선택
            float max = y.Max();
            float min = y.Min();
            float center = max / 4;
            float bound = (center + 9 * max) / 10;

            var yTop = from yVal in y
                       where bound < yVal
                       select yVal;
            float target = Statistics.Mean(yTop) / 2;

            // Gray Profile을 선형 보간된 함수로 만들어서 목표 위치를 계산
            var f = new Interpolation1D(y);
            float t = f.Arg(target);

            return Begin + vec * t;
        }
    }
}