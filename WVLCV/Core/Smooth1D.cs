using System;
using System.Collections.Generic;
using System.Linq;

namespace WVLCV.Core
{
    /// <summary>
    /// 1차원 Smoothing 메서드들의 집합
    /// </summary>
    public static class Smooth1D
    {
        /// <summary>
        /// 1차원 Gaussian Smoothing을 수행합니다. Kernel의 크기는 3, 5, 7, 9, 또는 11로 제한됩니다.
        /// </summary>
        /// <param name="data">입력 Data</param>
        /// <param name="kernelSize">Gaussian Filter의 크기: 3, 5, 7, 9, 또는 11</param>
        /// <returns>Smoothing 처리된 Data</returns>
        public static IEnumerable<double> Gaussian(IEnumerable<double> data, int kernelSize)
        {
            // Kernel 크기를 범위내로 제한
            int k = kernelSize;
            if (k < 3)
            {
                k = 3;
            }
            else if (11 < k)
            {
                k = 1;
            }
            else if (k % 2 == 0)
            {
                k = k - 1;
            }

            double sigma = (k - 1) / 4.0;

            // Kernel 계산
            // G(x) = (e^(-x^2/(2 sigma^2))) / (sigma sqrt(2 pi))
            double[] kernel = new double[k];
            double denom = 2 * sigma * sigma;
            double con = sigma * Math.Sqrt(2 * Math.PI);

            int center = (k - 1) / 2;
            for (int i = 0; i < k; i++)
            {
                double x = i - center;
                double numer = -x * x;

                kernel[i] = Math.Pow(Math.E, numer / denom) / con;
            }

            // Kernel의 합이 1이 되도록 Normalize
            double sum = kernel.Sum();
            for (int i = 0; i < k; i++)
            {
                kernel[i] /= sum;
            }

            // Smooth 계산
            int n = data.Count();
            double[] result = new double[n];
            for (int i = 0; i < n; i++)
            {
                int[] references = new int[k];
                for (int j = 0; j < k; j++)
                {
                    references[j] = i + (j - center);
                    if (references[j] < 0)
                    {
                        references[j] = 0;
                    }
                    else if (n <= references[j])
                    {
                        references[j] = n - 1;
                    }
                }

                double smoothValue = 0;
                for (int j = 0; j < k; j++)
                {
                    smoothValue += (data.ElementAt(references[j]) * kernel[j]);
                }
                result[i] = smoothValue;
            }

            return result;
        }
    }
}