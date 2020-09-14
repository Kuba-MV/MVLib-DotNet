/*
  Media Vault Library
  Copyright (C) 2020 Jakub Gluszkiewicz (kubabrt@gmail.com)
  This software is provided 'as-is', without any express or implied
  warranty.  In no event will the authors be held liable for any damages
  arising from the use of this software.
  You may use this code under license agreements which can be found at www.libmediavault.com.
  You may use this code and referenced libraries for free without any limitations and any fees under following conditions:
  - your derative work is for non-commercial purposes
  - your derative work is for commercial purposes but your annual company income is not greater than 50K $ (american dollars)
  Otherwise please contact me directly to buy license for commmercial use.
*/

#pragma once

#include "MV_DLLExport.h"

/*
Defines possible log levels
*/
enum MV_LogLevel
{
	MV_INFO = 0,
	MV_ERROR = 1,
	MV_CRITICAL = 2
};

/*
Initializes log for debuging purposes
*/
MVLIB_API int MV_InitializeLog(const char* fileName);

/*
Write informations to log.
This method is thread safe.
*/
MVLIB_API int MV_WriteLog(int level, const char* msg, ...);

/*
Release all log resources.
*/
MVLIB_API void MV_DisposeLog();