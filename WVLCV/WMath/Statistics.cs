using System;
using System.Collections.Generic;
using System.Linq;

namespace WVLCV.WMath
{
    /// <summary>
    /// 통계 관련 편의 메서드들의 집합
    /// </summary>
    public static class Statistics
    {
        /// <summary>
        /// float 수치 Data의 평균을 계산합니다.
        /// </summary>
        /// <param name="data">주어진 Data</param>
        /// <returns>Data의 평균</returns>
        public static float Mean(IEnumerable<float> data)
        {
            return data.Average();
        }

        /// <summary>
        /// float 수치 Data의 분산을 계산합니다.
        /// </summary>
        /// <param name="data">주어진 Data</param>
        /// <returns>Data의 분산</returns>
        public static float Variance(IEnumerable<float> data)
        {
            float m = Mean(data);

            // 편차의 제곱을 계산한 IEnumerable<float>
            var v = from d in data
                    select (float)Math.Pow(d - m, 2);

            return Mean(v);
        }

        /// <summary>
        /// float 수치 Data의 표준 편차를 계산합니다.
        /// </summary>
        /// <param name="data">주어진 Data</param>
        /// <returns>Data의 표준 편차</returns>
        public static float StdDeviation(IEnumerable<float> data)
        {
            return (float)Math.Sqrt(Variance(data));
        }

        /// <summary>
        /// float 수치 Data의 표본 분산을 계산합니다.
        /// </summary>
        /// <param name="data">주어진 Data</param>
        /// <returns>Data의 표본 분산</returns>
        public static float SampleVariance(IEnumerable<float> data)
        {
            float m = Mean(data);

            // 편차의 제곱을 계산한 IEnumerable<float>
            var v = from d in data
                    select Math.Pow(d - m, 2);

            var s = (float)v.Sum();

            return s / (data.Count() - 1f);
        }

        /// <summary>
        /// float 수치 Data의 표본 표준 편차을 계산합니다.
        /// </summary>
        /// <param name="data">주어진 Data</param>
        /// <returns>Data의 표본 표준 편차</returns>
        public static float SampleStdDeviation(IEnumerable<float> data)
        {
            return (float)Math.Sqrt(SampleVariance(data));
        }

        /// <summary>
        /// 두 개의 float 수치 Data의 공분산을 계산합니다.
        /// </summary>
        /// <param name="data1">첫번째 Data</param>
        /// <param name="data2">두번째 Data</param>
        /// <returns>두 Data의 공분산</returns>
        public static float Covariance(IEnumerable<float> data1, IEnumerable<float> data2)
        {
            int n1 = data1.Count();
            int n2 = data2.Count();

            if (n1 != n2)
            {
                throw new InvalidOperationException("두 Data의 크기가 다릅니다.");
            }

            float m1 = Mean(data1);
            float m2 = Mean(data2);

            // 편차를 계산한 IEnumerable<float>
            var v1 = from d in data1
                     select d - m1;
            var v2 = from d in data2
                     select d - m2;

            float[] mult = new float[n1];
            for (int i = 0; i < n1; i++)
            {
                mult[i] = v1.ElementAt(i) * v2.ElementAt(i);
            }

            return Mean(mult);
        }

        /// <summary>
        /// 두 개의 float 수치 Data의 표본 공분산을 계산합니다.
        /// </summary>
        /// <param name="data1">첫번째 Data</param>
        /// <param name="data2">두번째 Data</param>
        /// <returns>두 Data의 표본 공분산</returns>
        public static float SampleCovariance(IEnumerable<float> data1, IEnumerable<float> data2)
        {
            int n1 = data1.Count();
            int n2 = data2.Count();

            if (n1 != n2)
            {
                throw new InvalidOperationException("두 Data의 크기가 다릅니다.");
            }

            float m1 = Mean(data1);
            float m2 = Mean(data2);

            // 편차를 계산한 IEnumerable<float>
            var v1 = from d in data1
                     select d - m1;
            var v2 = from d in data2
                     select d - m2;

            float[] mult = new float[n1];
            for (int i = 0; i < n1; i++)
            {
                mult[i] = v1.ElementAt(i) * v2.ElementAt(i);
            }

            float s = mult.Sum();

            return s / (n1 - 1);
        }
    }
}