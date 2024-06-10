// DotAnalyzerCPP.cpp : DLL 응용 프로그램을 위해 내보낸 함수를 정의합니다.
//

#include <stdio.h>
#include <opencv2/opencv.hpp>
#include <opencv2/imgproc.hpp>
#include <cmath>
#include <vector>
#include <iostream>
#include <fstream>
#include <string>

class DCF {
public:
	cv::Point2f TiltFactor;
	float RotationAngle;
	float MaximumDiff;
	float PixelSize;
	std::vector<cv::Point2f> DotCenters;

	float DifferenceCalculator(std::vector<cv::Point2f> points, float pixelSize, float rotAngle, cv::Point2f StartPoint)
	{
		auto rotatedPoints = RotatePoints(points, StartPoint, rotAngle);

		float dotToDotDistance = 250;
		float dtd = dotToDotDistance / pixelSize;

		float count = 0;
		float tmpMax = 0;
		float aveDiff = 0;

		float nX = 0;
		float nY = 0;
		float difference = 0;

		std::ofstream text_file("dutpoint2.txt");
		if (text_file.is_open())
		{
			for (int i = 0; i < rotatedPoints.size(); i++)
			{
				text_file << rotatedPoints[i].x << "," << rotatedPoints[i].y << "\n";

			}

			text_file.close();
		}


		std::vector<cv::Point2f> ideal_point;

		for (auto& item : rotatedPoints)
		{
			nX = round((item.x - StartPoint.x) / dtd);
			nY = round((item.y - StartPoint.y) / dtd);
			difference = sqrt(pow(item.y - (StartPoint.y + dtd * nY), 2) + pow(item.x - (StartPoint.x + dtd * nX), 2));

			ideal_point.push_back(cv::Point2f(StartPoint.x + dtd * nX, StartPoint.y + dtd * nY));

			if (item.x < StartPoint.x + 50 && item.x > StartPoint.x - 50)
			{
				aveDiff += difference;
				count += 1;
				TiltFactor += cv::Point2f(item.x - (StartPoint.x + dtd * nX), item.y - (StartPoint.y + dtd * nY));

				if (difference > tmpMax)
				{
					tmpMax = difference;
					MaximumDiff = difference;
				}
			}
		}
		std::ofstream text_file2("idealPoint2.txt");
		if (text_file2.is_open())
		{
			for (int i = 0; i < ideal_point.size(); i++)
			{
				text_file2 << ideal_point[i].x << "," << ideal_point[i].y << "\n";

			}

			text_file2.close();
		}


		TiltFactor /= count;
		aveDiff /= count;

		return aveDiff;
	}
	std::vector<cv::Point2f> RotatePoints(const std::vector<cv::Point2f> &points, cv::Point2f rotationCenter, float angle)
	{
		std::vector<cv::Point2f> result;

		float newX = 0;
		float newY = 0;
		float resX = 0;
		float resY = 0;

		for (auto& item : points)
		{
			newX = item.x - rotationCenter.x;
			newY = item.y - rotationCenter.y;
			resX = cos(angle) * newX - sin(angle) * newY;
			resY = sin(angle) * newX + cos(angle) * newY;

			result.push_back(rotationCenter + cv::Point2f(resX, resY));
		}
		return result;
	}
	void DotCenterFinder(cv::Mat img, float weight, float &x, float &y, std::vector<cv::Point2f> &dotCenters)
	{
		cv::Mat thresImg, grayInv, mask;
		cv::Mat temp_closing;
		std::vector<std::vector<cv::Point> > contours;
		std::vector<cv::Vec4i> hierarchy;
		cv::Point2f center(0, 0);

		auto width = img.cols;
		auto height = img.rows;

		double thres = cv::mean(img)[0] + 10;

		cv::convertScaleAbs(img, grayInv, -1, 255);
		cv::threshold(img, thresImg, thres, 255, CV_8UC1);
		cv::imwrite("thresImg.bmp", thresImg);
		cv::morphologyEx(thresImg, temp_closing, cv::MORPH_CLOSE, cv::Mat(), cv::Point(-1, -1), 3);
		cv::imwrite("sclae.bmp", grayInv);
		cv::imwrite("temp_closing.bmp", temp_closing);

		// 기존 Contour 추출
		cv::findContours(temp_closing, contours, hierarchy, cv::RetrievalModes::RETR_TREE, cv::ContourApproximationModes::CHAIN_APPROX_NONE);
		
		// 수정 2024-05-27 Canny Edge로 Contour 추출
		//cv::Mat canny_img;
		//cv::Canny(temp_closing, canny_img, thres, 255);
		//cv::imwrite("canny_img.bmp", canny_img);

		//std::vector<cv::Vec3f> circles;
		//cv::HoughCircles(
		//	canny_img, circles, cv::HOUGH_GRADIENT, 1.2, 20, 50, 20, 10, 16
		//);

		//// Convert the image to BGR for color drawing
		//cv::Mat draw_circle = cv::Mat(height, width, CV_8UC1, cv::Scalar(0, 0, 0));

		//// Draw the circles
		//for (size_t i = 0; i < circles.size(); i++) {
		//	cv::Point center(cvRound(circles[i][0]), cvRound(circles[i][1]));
		//	int radius = cvRound(circles[i][2]);
		//	cv::circle(draw_circle, center, radius, cv::Scalar(255, 255, 255), -4);
		//}
		//cv::imwrite("draw_circle.bmp", draw_circle);
		//cv::Mat invert_circle_img = cv::Mat(height, width, CV_8UC1, cv::Scalar(0, 0, 0));
		//cv::bitwise_not(draw_circle, invert_circle_img);
		//
		//cv::imwrite("invert_circle_img.bmp", invert_circle_img);

		//cv::findContours(invert_circle_img, contours, hierarchy, cv::RetrievalModes::RETR_TREE, cv::ContourApproximationModes::CHAIN_APPROX_NONE);

		cv::Mat contours_img;
		cv::cvtColor(img, contours_img, cv::COLOR_GRAY2BGR);
		cv::drawContours(contours_img, contours, -1, cv::Scalar(0, 255, 0), 2);
		cv::imwrite("contours_img.bmp", contours_img);

		auto imgmof = cv::Mat_<unsigned char>{ grayInv };
		auto imgptr = imgmof.begin();

		cv::threshold(img, mask, thres + 10, 255, cv::ThresholdTypes::THRESH_BINARY_INV);
		cv::morphologyEx(mask, mask, cv::MORPH_CLOSE, cv::Mat(), cv::Point(-1, -1), 3);
		cv::imwrite("mask.bmp", mask);
		auto maskmof = cv::Mat_<unsigned char>{ mask };
		auto maskptr = maskmof.begin();

		for (size_t i = 0; i < contours.size(); i++)
		{
			if (contours[i].size() > 50)
			{
				GrayVolumeCenter(height, width, imgptr, maskptr, contours[i], center);
				dotCenters.push_back(center);
				if (center.x > width / 2 - 50 && center.x < width / 2 + 50 && center.y > height / 2 - 50 && center.y < height / 2 + 50)
				{
					x = center.x;
					y = center.y;
				}
			}
		}

		cv::Mat Center_Img;
		cv::cvtColor(img, Center_Img, cv::COLOR_GRAY2BGR);
		for (const auto& center : dotCenters) {
			// 좌표가 이미지 범위 내에 있는지 확인
			if (center.x >= 0 && center.x < Center_Img.cols && center.y >= 0 && center.y < Center_Img.rows) {
				cv::circle(Center_Img, center, 5, cv::Scalar(0, 0, 255), -1);  // 빨간색 점으로 표시
			}
			else {
				std::cerr << "Dot center out of bounds: " << center << std::endl;
			}
		}

		// 결과 이미지 저장
		std::string outputPath = "Center_Point_Image.bmp";
		cv::imwrite(outputPath, Center_Img);
	}
	void GrayVolumeCenter(int h, int w, cv::MatIterator_<uchar> imgptr, cv::MatIterator_<uchar> maskptr, std::vector<cv::Point> points, cv::Point2f &center)
	{
		auto tl = GetTopLeft(points);
		auto br = GetBottomRight(points);

		float vol = 0, x = 0, y = 0, wg = 0;

		unsigned char imgval, g;

		unsigned char maskval;

		for (size_t r = tl.y - 3; r < br.y + 3; r++)
		{
			for (size_t c = tl.x - 3; c < br.x + 3; c++)
			{
				maskval = *(maskptr + r * w + c);
				if (maskval == 255)
				{
					imgval = *(imgptr + r * w + c);
					g = imgval;
					wg = (float)g;

					vol += wg;
					x += wg * c;
					y += wg * r;
				}
			}
		}
		if (vol != 0)
		{
			x /= vol;
			y /= vol;
		}
		center.x = x;
		center.y = y;
	}
	cv::Point GetTopLeft(std::vector<cv::Point> points)
	{
		int left = 10000;
		int top = 10000;

		for (size_t j = 0; j < points.size(); j++)
		{
			auto p = points[j];
			if (p.x < left)
			{
				left = p.x;
			}

			if (p.y < top)
			{
				top = p.y;
			}
		}
		return cv::Point(left, top);
	}
	cv::Point GetBottomRight(std::vector<cv::Point> points)
	{
		int right = 0;
		int bottom = 0;

		for (size_t j = 0; j < points.size(); j++)
		{
			auto p = points[j];
			if (p.x > right)
			{
				right = p.x;
			}

			if (p.y > bottom)
			{
				bottom = p.y;
			}
		}
		return cv::Point(right, bottom);
	}
};