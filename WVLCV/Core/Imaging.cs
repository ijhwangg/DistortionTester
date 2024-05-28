using OpenCvSharp;
using System;

namespace WVLCV.Core
{
    /// <summary>
    /// 이미지 생성관련 함수가 정의된 클래스입니다.
    /// </summary>
    public static class Imaging
    {
        /// <summary>
        /// byte[] 형식의 ImageData를 Mat으로 반환합니다.
        /// </summary>
        /// <param name="data">ImageData</param>
        /// <param name="width">이미지의 가로길이</param>
        /// <param name="height">이미지의 세로길이</param>
        /// <param name="stride">ImageData에서 한 행의 길이</param>
        /// <returns></returns>
        public static Mat ConstructMatFromGrabData(byte[] data, int width, int height, int stride)
        {
            Mat img;

            if (width * 4 <= stride)
            {
                using (var temp = new Mat(height, width, MatType.CV_8UC4, data, stride))
                {
                    img = temp.Clone();
                }
            }
            else if (width * 3 <= stride)
            {
                using (var temp = new Mat(height, width, MatType.CV_8UC3, data, stride))
                {
                    img = temp.Clone();
                }
            }
            else if (width * 2 <= stride)
            {
                using (var temp = new Mat(height, width, MatType.CV_8UC2, data, stride))
                {
                    img = temp.Clone();
                }
            }
            else
            {
                using (var temp = new Mat(height, width, MatType.CV_8UC1, data, stride))
                {
                    img = temp.Clone();
                }
            }

            return img;
        }

        /// <summary>
        /// IntPtr 형식의 ImageData를 Mat으로 반환합니다.
        /// </summary>
        /// <param name="data">ImageData</param>
        /// <param name="width">이미지의 가로길이</param>
        /// <param name="height">이미지의 세로길이</param>
        /// <param name="stride">ImageData에서 한 행의 길이</param>
        /// <returns></returns>
        public static Mat ConstructMatFromGrabData(IntPtr data, int width, int height, int stride)
        {
            Mat img;

            if (width * 4 <= stride)
            {
                using (var temp = new Mat(height, width, MatType.CV_8UC4, data, stride))
                {
                    img = temp.Clone();
                }
            }
            else if (width * 3 <= stride)
            {
                using (var temp = new Mat(height, width, MatType.CV_8UC3, data, stride))
                {
                    img = temp.Clone();
                }
            }
            else if (width * 2 <= stride)
            {
                using (var temp = new Mat(height, width, MatType.CV_8UC2, data, stride))
                {
                    img = temp.Clone();
                }
            }
            else
            {
                using (var temp = new Mat(height, width, MatType.CV_8UC1, data, stride))
                {
                    img = temp.Clone();
                }
            }

            return img;
        }
    }
}