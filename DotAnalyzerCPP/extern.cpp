#include "extern.h"
#include <opencv2/opencv.hpp>
#include "DCF.cpp"

DOTANALYZERCPP_API float MeasVector(float* dcs, int iterN, float initPixelSize, float rotAngle, float sx, float sy, float step)
{
	DCF dcf;
	int size = (int)dcs[0];
	std::vector<cv::Point2f> dotCenters(size);
	float x = 0;
	float y = 0;

	for (size_t i = 0; i < size; i++)
	{
		x = dcs[2 * i + 1];
		y = dcs[2 * i + 2];
		dotCenters.push_back(cv::Point2f(x, y));
	}

	initPixelSize += step / 2;

	cv::Point2f sp(sx, sy);

	float result = dcf.DifferenceCalculator(dotCenters, initPixelSize, rotAngle, sp);
	float resultL = dcf.DifferenceCalculator(dotCenters, initPixelSize - step, rotAngle, sp);
	float resultR = dcf.DifferenceCalculator(dotCenters, initPixelSize + step, rotAngle, sp);

	std::cout << resultL << "," << result << "," << resultR << "\n";

	float dp = step;

	for (int i = 0; i < iterN; i++)
	{
		dp /= (float)2;
		if (resultL > resultR)
		{
			initPixelSize += dp;
			resultL = result;
			result = dcf.DifferenceCalculator(dotCenters, initPixelSize, rotAngle, sp);
		}
		else
		{
			initPixelSize -= dp;
			result = dcf.DifferenceCalculator(dotCenters, initPixelSize, rotAngle, sp);
			resultR = result;
		}
	}
	return initPixelSize;
}
DOTANALYZERCPP_API float* GetDotCenters(int height, int width, unsigned char* ptr, float &x, float &y)
{
	DCF dcf;
	//ptr --> Mat
	auto img = cv::Mat(height, width, CV_8UC1, ptr);
	//cv::imwrite(R"(C:\Users\WTA\Desktop\[WTA]Project\DistortionTeat\WindowsFormsApp1\bin\x64\Debug)", img);
	std::vector<cv::Point2f> dcs;

	//이미지상에 보이는 모든 DotCenter찾기
	dcf.DotCenterFinder(img, 1.0, x, y, dcs);

	//dot
	auto num = dcs.size();

	float* res = new float[num * 2 + 1];
	res[0] = num;

	for (size_t i = 0; i < num; i++)
	{
		res[2 * i + 1] = dcs[i].x;
		res[2 * i + 2] = dcs[i].y;
	}

	return res;
}
DOTANALYZERCPP_API void NativeFree(void * nativePtr)
{
	if (nativePtr != nullptr)
	{
		delete[] nativePtr;
	}
}