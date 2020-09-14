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

#ifndef __MV_COMMON_
#define __MV_COMMON_

typedef unsigned __int8 uint8_t;

/*
Defines available INV_Decoder states
*/
enum MV_DecoderState
{
	MV_DeocderState_NotInitialized = 0,
	MV_DeocderState_Opened = 1,
	MV_DeocderState_Closed = 2,
	MV_DeocderState_Finished = 3,
	MV_DeocderState_Error = 4

};

/*
Defines available decoding types for IMV_Decoder.
*/
enum MV_DecodingType
{
	//software decoding - output is planar uint8_t* buffers
	MV_DecodingType_YUV420 = 0,
	//software decoding - ouput is uint8_t* buffer rgb
	MV_DecodingType_RGB = 1,
	//hardware decoding using DXVA - output is ID3DSurface9*
	MV_DecodingType_DXVA = 2,
	//hardware decoding using DXVA11 - output is ID3D11Texture2D*
	MV_DecodingType_DX11VA = 3
};

/*
Defines frame buffer data returned by IMV_Decoder or IMV_PlayerBase
Use GetEmptyFrame to initialize default frame buffer
WARNING! Do not modify data of this structure!
*/

struct MV_FrameBuffer
{
	/*
	 - in case of audio uint8_t* buffer;
	- in case of video yuv420 is uint8_t* buffer to y_plane
	- in case of video rgb is uint8_t* buffer to rgb raw pixel data
	- in case of video dxva is void pointer to ID3DSurface9*
	- in case of video dxva11 is void pointer to ID3D11Texture2D
	*/
	void* buf1;
	
	// - in case of video yuv420 is uint8_t* buffer to u_plane
	void* buf2;

	// - in case of video yuv is uint8_t* buffer to v_plane
	void* buf3;

	/*
	- in case of audio is buffer length
	- in case of video yuv420 is y plane length
	- in case of video rgb is buffer length
	- in case of video dxva11 is number of index of d3d11 subresource in d3d11texture2d
	*/
	int buflen1;
	// - in case of video yuv420 is u plane length
	
	int buflen2;
	// -in case of video yuv420 is v plane length
	
	int buflen3;

	// - true if this is audio frame/ false if this is video frame
	bool isAudio;
	
	// - true if this is hardware decoded video frame
	bool isHwAcc;

	// - true if frame has no data
	bool isEmpty;

	// - value when frame should be presented in seconds returned only by MV_Deocder
	double presentation_time;

	// - value when next frame should be presented in seconds returned only by MV_Decoder
	double next_frame_presentation_time;

	// - true when new frame was generated.Set only by IMV_PlayerBase(not by decoder)
	bool isNew;

	static MV_FrameBuffer GetEmptyFrame()
	{
		MV_FrameBuffer fb;

		fb.buf1 = nullptr;
		fb.buf2 = nullptr;
		fb.buf3 = nullptr;
		fb.buflen1 = 0;
		fb.buflen2 = 0;
		fb.buflen3 = 0;
		fb.isAudio = false;
		fb.isEmpty = true;
		fb.isHwAcc = false;
		fb.presentation_time = -1.0f;
		fb.next_frame_presentation_time = -1.0f;
		fb.isNew = false;

		return fb;
	}
};

/*
Defines possible player states
*/
enum MV_PlayerState
{
	MV_PlayerState_NotInitialized = 0,
	MV_PlayerState_Paused = 1,
	MV_PlayerState_Closed = 2,
	MV_PlayerState_Error = 3,
	MV_PlayerState_Playing = 4,
	MV_PlayerState_Seeking = 5,
	MV_PlayerState_Stopped = 6,
	MV_PlayerState_Loading = 7,
	MV_PlayerState_Finished = 8

};

/*
Defines possible player decoding types
*/
enum MV_PlayerD3D_DecodingType
{
	MV_PlayerD3D_DecodingType_Auto = 0,
	MV_PlayerD3D_DecodingType_YUV420P = 1,
	MV_PlayerD3D_DecodingType_RGBA = 2,
	MV_PlayerD3D_DecodingType_DXVA = 3,
	MV_PlayerD3D_DecodingType_D3D11VA = 4
};

/*
Defines frame buffer data returned by IMV_PlayerD3D
Use GetEmptyFrame to initialize default frame buffer
WARNING! Do not modify data of this struct
*/
struct MV_PlayerD3D_Frame
{
	/*
	- in case of video yuv420 is uint8_t* buffer to y_plane
	- in case of video rgb is uint8_t* buffer to rgb raw pixel data
	*/
	uint8_t* buf1;

	// - in case of video yuv420 is uint8_t* buffer to u_plane
	uint8_t* buf2;

	// -in case of video yuv is uint8_t* buffer to v_plane
	uint8_t* buf3;

	/*
	- in case of video yuv420 is y plane length
	- in case of video rgb is buffer length
	*/
	int buf1Len;

	// - in case of video yuv420 is u plane length
	int buf2Len;

	// -in case of video yuv420 is v plane length
	int buf3Len;

	/*
	null if this is software decoded frame. 
	Shared handle if this is hardware decoded frame. 
	Shared hanlde is ready to utilize by your application and IMV_PlayerD3D is resposnsible to sync GPU complete operations so you dont have to worry about it.
	*/
	void * sharedHandle;
	bool isEmpty;

	static MV_PlayerD3D_Frame GetEmptyFrame()
	{
		MV_PlayerD3D_Frame empty;

		empty.buf1 = nullptr;
		empty.buf2 = nullptr;
		empty.buf3 = nullptr;
		empty.buf1Len = 0;
		empty.buf1Len = 1;
		empty.buf1Len = 2;

		empty.sharedHandle = nullptr;

		empty.isEmpty = true;

		return empty;
	}

};

#endif