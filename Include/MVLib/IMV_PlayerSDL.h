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

#include "IMV_PlayerD3D.h"
#include "SDL.h"

/*
This player implementation desinged for SDL2 framework.
It derives all methods from IMV_PlayerBase and IMV_PlayerD3D clasess.
Instead of raw frame buffers it returns ready to use SDL_Texture.
Note! If you want to use video accelerated decoding you have to link to specialy builded SDL2 library provided with this project!
*/
class IMV_PlayerSDL : public IMV_PlayerD3D
{

public:

	/*
	Open AVStream in separate thread.
	pathOrUrl - it can be file location or url to network stream
	returns true on success, otherwise check log for error details
	*/
	virtual void OpenMediaAsync(const char* pathOrUrl) = 0;

	/*
	Open AVStream in separate thread.
	pathOrUrl - it can be file location or url to network stream
	returns true on success, otherwise check log for error details
	*/
	virtual void OpenMediaAsync(const char* pathOrUrl, int packets_buffer_size, int frames_buffer_size) = 0;
	
	/*
	Returns ready to display SDL_Texture.
	You have to call this method periodically to unqueue video buffers.
	Not calling this method if source has video stream will cause memory growth.
	Note! Do not call GetVideoBuffer from MV_PlayerBase or GetVideoFrame() from MV_PlayerD3D derived classes if you are using this method.
	*/
	virtual SDL_Texture* GetVideoTexture() = 0;

};
