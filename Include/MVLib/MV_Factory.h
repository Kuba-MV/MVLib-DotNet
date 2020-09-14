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

/*
This is factory for all MVLib components.
Use this functions to instantinate MVLib objects.
*/

#include "MV_DLLExport.h"
#include "IMV_Decoder.h"
#ifdef MV_INCLUDE_SDL_PLAYER
#include "IMV_PlayerSDL.h"
#else
#include "IMV_PlayerD3D.h"
#endif
#include "IMV_SubtitleParser.h"
#include "IMV_SubtitleService.h"


/*
Instantinates IMV_Decoder class.
packetsBufferSize - defines the size of demuxer buffer. You can increase this buffer if you expect unstable to read source for. ex internet streams.
packetsFramesSize - defines how many decoded frames are buffered in memory.
*/
MVLIB_API IMV_Decoder* MV_CreateDecoder(int packetsBufferSize, int packetsFramesSize);

/*
Instantinates IMV_PlayerBase class.
*/

MVLIB_API IMV_PlayerBase* MV_CreatePlayerBase();

/*
Instantinates IMV_PlayerD3D class.
*/

MVLIB_API IMV_PlayerD3D* MV_CreatePlayerD3D();

#ifdef MV_INCLUDE_SDL_PLAYER
/*
Instantinates IMV_PlayerSDL class.
*/

MVLIB_API IMV_PlayerSDL* MV_CreatePlayerSDL(SDL_Renderer* renderer);

#endif // MV_INCLUDE_SDL_PLAYER

/*
Instantinates MPL2 subtitle parser
*/
MVLIB_API IMV_SubtitleParser* MV_GetSubtitleMPL2Parser();

/*
Instantinates MicroDVD subtitle parser
*/
MVLIB_API IMV_SubtitleParser* MV_GetSubtitleMicroDVDParser();

/*
Instantinates SubRip(Srt) subtitle parser
*/
MVLIB_API IMV_SubtitleParser* MV_GetSubtitleSrtParser();

/*
Instantinates high level subtitle service
*/
MVLIB_API IMV_SubtitleService* MV_CreateSubtitleService();
