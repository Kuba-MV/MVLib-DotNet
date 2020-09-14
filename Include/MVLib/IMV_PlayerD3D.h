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

#include "IMV_PlayerBase.h"

/*
This is player implementation designed for DirectX environment.
It derives all methods from IMV_PlayerBase.
However this class provides ready to display buffers depending of decoding type.
*/
class IMV_PlayerD3D: public IMV_PlayerBase
{
public:

	/*
	Sets decoding type.
	See MV_PlayerD3D_DecodingType for more details.
	You can set decoding type only at NotInitialized and Closed player states.
	*/

	virtual void SetD3DDecodingType(MV_PlayerD3D_DecodingType decode_type) = 0;

	/*
	Returns ready to display video frame.
	See MV_PlayerD3D_Frame for more detailed information.
	You have to call this method periodically to unqueue video buffers.
	Not calling this method if source has video stream will cause memory growth.
	Note! Do not call GetVideoBuffer from MV_PlayerBase derived class if you are using this method.
	*/
	virtual MV_PlayerD3D_Frame GetVideoBuffer() = 0;

};
