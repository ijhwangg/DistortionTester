using System;
using System.Collections.Generic;

namespace WVLCV.WMath
{
    /// <summary>
    /// 이산 Data f(i) = y_i로부터 보간된 함수를 나타내는 클래스입니다.
    /// </summary>
    public class Interpolation1D
    {
        private List<float> Y { get; }

        /// <summary>
        /// 이산 Data f(i) = y_i로부터 보간된 함수를 생성합니다.
        /// </summary>
        /// <param name="y">0부터 시작하는 인덱스를 가진 Data</param>
        public Interpolation1D(IEnumerable<float> y)
        {
            Y = new List<float>(y);
        }

        /// <summary>
        /// 선형 보간을 이용하여 인덱스 x에서의 보간된 y값을 계산합니다.
        /// </summary>
        /// <param name="x">목표 인덱스</param>
        /// <returns>x에서의 y값</returns>
        public float Of(float x)
        {
            if (x <= 0)
            {
                return Y[0];
            }
            else if (Y.Count - 1 <= x)
            {
                return Y[Y.Count - 1];
            }
            else
            {
                int i = (int)Math.Floor(x);

                float alpha = x - i;

                float y1 = Y[i];
                float y2 = Y[i + 1];

                return y1 + alpha * (y2 - y1);
            }
        }

        /// <summary>
        /// 선형 보간을 이용하여 주어진 y값을 갖는 보간된 인덱스 x를 계산합니다.
        /// </summary>
        /// <param name="y">목표 값</param>
        /// <returns>y값을 갖는 첫번째 x</returns>
        public float Arg(float y)
        {
            for (int i = 0; i < Y.Count - 1; i++)
            {
                float y1 = Y[i];
                if (y == y1)
                {
                    return i;
                }

                float y2 = Y[i + 1];
                if ((y - y1) * (y - y2) < 0)
                {
                    return (y - y1) / (y2 - y1) + i;
                }
            }

            return Y.Count;
        }
    }
}