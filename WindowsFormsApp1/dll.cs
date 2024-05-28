using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace WindowsFormsApp1
{
    public class dll
    {
        //C++ Algorithm Import
        [DllImport(@"DotAnalyzerCPP.dll", EntryPoint = "MeasVector")]
        public static extern unsafe float MeasVec(float* dcs, int iterN, float initPixelSize, float rotAngle, float sx, float sy, float step);

        [DllImport(@"DotAnalyzerCPP.dll", EntryPoint = "GetDotCenters")]
        public static extern unsafe float* GetDotCenters(int height, int width, byte* ptr, ref float x, ref float y);

        [DllImport(@"CvtImage.dll")]
        public unsafe static extern int Distortion(IntPtr ImageBuffer, float[] vectorX, float[] vectorY, int width, int height, int numCore = 0);
    }
}
