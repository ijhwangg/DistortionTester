#pragma once
#include "header.h"
#include <stdio.h>

EXTERN_C_START

DOTANALYZERCPP_API float MeasVector(float* dcs, int iterN, float initPixelSize, float rotAngle, float sx, float sy, float step);
DOTANALYZERCPP_API float* GetDotCenters(int height, int width, unsigned char* ptr, float &x, float &y);
DOTANALYZERCPP_API void NativeFree(void* nativePtr);

EXTERN_C_END