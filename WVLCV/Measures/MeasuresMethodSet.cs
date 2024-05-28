using OpenCvSharp;
using System;
using WVLCV.Core;

namespace WVLCV.Measures
{
    /// <summary>
    /// 측정용 메서드들의 집합
    /// </summary>
    public static class MeasuresMethodSet
    {
        /// <summary>
        /// 실루엣 이미지에서 인서트 영역을 추출합니다.
        /// <para>
        /// [예외] 예외를 던지지 않고, 그 대신에 blobMask를 null로 반환합니다.
        /// </para>
        /// <para>
        /// [메모리] blobMask에 할당된 메모리를 해제하는 것은 Caller의 책임입니다. blobMask를 해제하기 위해 Dispose()를 호출하세요. 이 메서드가 요구하는 다른 메모리는 메서드가 끝나면 즉시 반환됩니다.
        /// </para>
        /// </summary>
        /// <param name="inputImage">실루엣 이미지, CV_8UC3으로 간주합니다.</param>
        /// <param name="blobMask">선택된 인서트 영역, CV_8UC1인 흑백 마스크 이미지입니다.</param>
        public static void SelectInsertRegionBlob(Mat inputImage, out Mat blobMask)
        {
            try
            {
                var roi = new Rect(10, 10, inputImage.Cols - 20, inputImage.Rows - 20);

                using (var gray = inputImage.CvtColor(ColorConversionCodes.BGR2GRAY))
                using (var imgBound = new Mat(gray.Rows, gray.Cols, gray.Type(), Scalar.White))
                using (var grayInRoi = gray[roi])
                {
                    imgBound[roi] = grayInRoi;
                    using (var thre = grayInRoi.Threshold(90.0, 255, ThresholdTypes.BinaryInv))
                    {
                        thre.FindContours(out Point[][] cont, out HierarchyIndex[] hiearchy, RetrievalModes.External, ContourApproximationModes.ApproxNone);

                        // 최대 넓이 Contour 선택
                        int maxAreaIndex = 0;
                        double maxArea = double.NegativeInfinity;
                        for (int i = 0; i < cont.Length; i++)
                        {
                            double areaNow = Cv2.ContourArea(cont[i]);
                            if (maxArea < areaNow)
                            {
                                maxArea = areaNow;
                                maxAreaIndex = i;
                            }
                        }

                        var selectedCont = cont[maxAreaIndex];
                        blobMask = new Mat(inputImage.Rows, inputImage.Cols, MatType.CV_8UC1, Scalar.Black);
                        blobMask.DrawContours(new Point[][] { selectedCont }, 0, Scalar.White, -1);

                        Util.ShowImageZoom(blobMask, 0.25f, 0.25f);
                    }
                }
            }
            catch (Exception)
            {
                blobMask = null;
            }
        }
    }
}