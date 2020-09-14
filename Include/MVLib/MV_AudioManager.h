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

#include "MV_LogBase.h"

/*
Prints to log all available audio devices.
*/
MVLIB_API void MV_list_audio_devices();

/*
Returns true if audio subsystem was initialized.
*/
MVLIB_API bool MV_is_audio_device_initialized();

/*
Opens default audio device.
You dont have to call this method, any IMV_Player will call this method if needed for you.
If you will decide to call this method you should do it only once not per player instance.
*/
MVLIB_API bool MV_open_audio_device();

/*
Closes audio device. 
You should call this method before quiting from your application.
*/
MVLIB_API void MV_close_audio_device();

/*
Resolves OPEN_AL int error to string.
*/
MVLIB_API const char* MV_openAlErrorToString(int err);


