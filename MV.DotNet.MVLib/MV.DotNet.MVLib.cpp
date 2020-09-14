#include "pch.h"

#include "MV.DotNet.MVLib.h"

MVDotNetMVLib::MVLibWrapper::MVLibWrapper() :
	_p_player(NULL),
	_p_d3d_hwnd_renderer(NULL),
	_volume(0.0f),
	_render_immediate_mode(false),
	_decoding_type(0),
	_p_d3d_offscreen_renderer(NULL),
	_offscreen_surface(System::IntPtr::Zero),
	_p_sub_service(NULL),
	_backColor(NULL),
	_fontColor(NULL),
	_outlineColor(NULL),
	_sharedHandle(NULL)
{

}

void MVDotNetMVLib::MVLibWrapper::CreateD3DHwndRenderer(System::IntPtr wndHandle)
{
	_p_d3d_hwnd_renderer = new MV_D3D11HwndRenderer();

	_p_d3d_hwnd_renderer->CreateRenderer((HWND)wndHandle.ToPointer());
}

void MVDotNetMVLib::MVLibWrapper::CreateD3DOffScreenRenderer()
{
	_p_d3d_offscreen_renderer = new MV_D3D9OffScreenRenderer();

	bool result = _p_d3d_offscreen_renderer->CreateOffScreenD3D9Device();

	if (!result)
	{
		delete _p_d3d_offscreen_renderer;
		_p_d3d_offscreen_renderer = NULL;
	}

}

void MVDotNetMVLib::MVLibWrapper::OpenMediaHwnd(System::String ^ PathOrUrl, bool asyncOpen, int packetsBufferSize, int framesBufferSize)
{
	if (!_p_d3d_hwnd_renderer)
		throw gcnew System::NullReferenceException("Native D3DRenderer is not initialized!");

	if (!_p_d3d_hwnd_renderer->GetRenderer())
		throw gcnew System::NullReferenceException("Native D3DRenderer doesn't have proper renderer!");

	if (!_p_player)
		_p_player = MV_CreatePlayerSDL(_p_d3d_hwnd_renderer->GetRenderer());

	if (!_p_player)
		throw gcnew System::NullReferenceException("Unable to create native MV_Player!");

	if (System::String::IsNullOrEmpty(PathOrUrl))
		return;

	_p_player->SetVolume(_volume);
	_p_player->SetD3DDecodingType((MV_PlayerD3D_DecodingType)_decoding_type);
	_p_player->SetVSyncAlignment(!_render_immediate_mode);

	System::IntPtr ptr = System::Runtime::InteropServices::Marshal::StringToHGlobalAnsi(PathOrUrl);

	IMV_PlayerSDL* p_sdl_player = (IMV_PlayerSDL*)_p_player;

	if (!asyncOpen)
		p_sdl_player->OpenMedia((char*)ptr.ToPointer(), packetsBufferSize, framesBufferSize);
	else
		p_sdl_player->OpenMediaAsync((char*)ptr.ToPointer(), packetsBufferSize, framesBufferSize);

	p_sdl_player = NULL;

	System::Runtime::InteropServices::Marshal::FreeHGlobal(ptr);

}

void MVDotNetMVLib::MVLibWrapper::OpenMediaOffScreen(System::String ^ PathOrUrl, int packetsBufferSize, int framesBufferSize)
{
	if (!_p_d3d_offscreen_renderer)
		throw gcnew System::NullReferenceException("Native D3DOffScreenRenderer is not initialized!");

	if (!_p_player)
		_p_player = MV_CreatePlayerD3D();

	if (!_p_player)
		throw gcnew System::NullReferenceException("Unable to create native MV_Player!");

	if (System::String::IsNullOrEmpty(PathOrUrl))
		return;

	_p_player->SetVolume(_volume);
	_p_player->SetD3DDecodingType((MV_PlayerD3D_DecodingType)_decoding_type);
	_p_player->SetVSyncAlignment(!_render_immediate_mode);
	
	System::IntPtr ptr = System::Runtime::InteropServices::Marshal::StringToHGlobalAnsi(PathOrUrl);

	_p_player->OpenMedia((char*)ptr.ToPointer(), packetsBufferSize, framesBufferSize);

	System::Runtime::InteropServices::Marshal::FreeHGlobal(ptr);

}

bool MVDotNetMVLib::MVLibWrapper::OpenSubtitles(System::String ^ Path)
{
	CloseSubtitles();

	_p_sub_service = MV_CreateSubtitleService();

	if (!_p_sub_service)
		return false;

	System::IntPtr ptr = System::Runtime::InteropServices::Marshal::StringToHGlobalAnsi(Path);

	bool result = _p_sub_service->OpenSubtitle((char*)ptr.ToPointer(), MV_SubtitleFormatAuto);

	System::Runtime::InteropServices::Marshal::FreeHGlobal(ptr);

	return result;
}

bool MVDotNetMVLib::MVLibWrapper::OpenSubtitlesHwnd(System::String ^ path, System::String ^ fontPath,  int fontSize, int outlineSize, int margin, int lineSpace, System::Drawing::Color fontColor, System::Drawing::Color backColor, System::Drawing::Color outlineColor)
{
	if (!_p_d3d_hwnd_renderer)
	{
		MV_WriteLog(MV_ERROR, "HWND renderer doesn't exist.");
		return false;
	}

	if (_backColor)
	{
		delete _backColor;
		_backColor = NULL;
	}

	if (_fontColor)
	{
		delete _fontColor;
		_fontColor = NULL;
	}

	if (_outlineColor)
	{
		delete _outlineColor;
		_outlineColor = NULL;
	}

	if (fontColor == System::Drawing::Color::Empty)
	{
		MV_WriteLog(MV_ERROR, "Font color can not be empty!");
		return false;
	}

	_fontColor = new MV_Color();
	_fontColor->a = fontColor.A;
	_fontColor->b = fontColor.B;
	_fontColor->g = fontColor.G;
	_fontColor->r = fontColor.R;

	if (outlineColor != System::Drawing::Color::Empty)
	{
		_outlineColor = new MV_Color();
		_outlineColor->a = outlineColor.A;
		_outlineColor->b = outlineColor.B;
		_outlineColor->g = outlineColor.G;
		_outlineColor->r = outlineColor.R;
	}

	if (backColor != System::Drawing::Color::Empty)
	{
		_backColor = new MV_Color();
		_backColor->a = backColor.A;
		_backColor->b = backColor.B;
		_backColor->g = backColor.G;
		_backColor->r = backColor.R;
	}

	CloseSubtitles();

	_p_sub_service = MV_CreateSubtitleService();

	if (!_p_sub_service)
	{
		MV_WriteLog(MV_ERROR, "Unable to create subtitle service!");
		return false;
	}

	System::IntPtr ptr1 = System::Runtime::InteropServices::Marshal::StringToHGlobalAnsi(path);
	System::IntPtr ptr2 = System::Runtime::InteropServices::Marshal::StringToHGlobalAnsi(fontPath);

	bool result = _p_sub_service->OpenSubtitle((char*)ptr1.ToPointer(), MV_SubtitleFormatAuto);

	if (!result)
	{
		MV_WriteLog(MV_ERROR, "Unable to open subtitles!");
		System::Runtime::InteropServices::Marshal::FreeHGlobal(ptr1);
		System::Runtime::InteropServices::Marshal::FreeHGlobal(ptr2);

		return false;
	}

	result = _p_d3d_hwnd_renderer->AddSubService(_p_sub_service, lineSpace, margin, (char*)ptr2.ToPointer(), fontSize, outlineSize, _fontColor, _backColor, _outlineColor);

	System::Runtime::InteropServices::Marshal::FreeHGlobal(ptr1);
	System::Runtime::InteropServices::Marshal::FreeHGlobal(ptr2);

	if (!result)
	{
		MV_WriteLog(MV_ERROR, "Unable to add sub service to hwnd renderer. Probably font path is not valid, or font is not ttf compilant file.");
		return false;
	}

	return true;
}

void MVDotNetMVLib::MVLibWrapper::CloseSubtitles()
{
	if (_p_sub_service)
	{
		_p_sub_service->Release();
		_p_sub_service = NULL;
	}

	if (_p_d3d_hwnd_renderer)
		_p_d3d_hwnd_renderer->RemoveSubService();
}

void  MVDotNetMVLib::MVLibWrapper::GetSubtitles(bool% isNew, bool% isEmpty, System::Collections::Generic::List<System::String^>^ linesText, System::Collections::Generic::List<bool>^ linesBold, System::Collections::Generic::List<bool>^ linesItalic)
{
	if (!_p_player)
		return;

	if (!_p_sub_service)
		return;

	if (linesText == nullptr)
		return;

	if (linesBold == nullptr)
		return;

	if (linesItalic == nullptr)
		return;

	MV_SubtitleServiceItem item = _p_sub_service->GetItem(_p_player->GetPosition());

	isNew = item.isNew;
	isEmpty = item.isEmpty;

	if (!isNew || isEmpty || !item.item.lines)
		return;

	for (size_t a = 0; a < item.item.lines->size(); a++)
	{
		linesText->Add(gcnew System::String(item.item.lines->at(a).line.c_str(), 0, item.item.lines->at(a).line.length(), System::Text::Encoding::UTF8));
		linesBold->Add(item.item.lines->at(a).isBold);
		linesItalic->Add(item.item.lines->at(a).isItalic);
	}
}

System::IntPtr MVDotNetMVLib::MVLibWrapper::GetOffScreenSurface()
{
	return _offscreen_surface;
}

System::IntPtr MVDotNetMVLib::MVLibWrapper::GetOffScreenSharedSurface()
{
	if (!_p_d3d_offscreen_renderer)
		return System::IntPtr::Zero;

	if (!_sharedHandle)
		return System::IntPtr::Zero;

	return System::IntPtr(_sharedHandle);
}

void MVDotNetMVLib::MVLibWrapper::ReleaseSharedSurface()
{
	if (!_p_d3d_offscreen_renderer)
		return;

	_p_d3d_offscreen_renderer->ReleaseSharedSurface();

	_sharedHandle = NULL;

	_offscreen_surface = System::IntPtr::Zero;
}

void MVDotNetMVLib::MVLibWrapper::Play()
{
	if (!_p_player)
		return;

	_p_player->Play();

}

void MVDotNetMVLib::MVLibWrapper::Pause()
{
	if (!_p_player)
		return;

	_p_player->Pause();
}

void MVDotNetMVLib::MVLibWrapper::Stop()
{
	if (!_p_player)
		return;

	_p_player->Stop();
}

void MVDotNetMVLib::MVLibWrapper::Close()
{
	CloseSubtitles();

	ReleaseSharedSurface();

	_offscreen_surface = System::IntPtr::Zero;

	if (!_p_player)
		return;

	_p_player->Close();
}

void MVDotNetMVLib::MVLibWrapper::SetVolume(float volume)
{
	if (volume > 1.0f || volume < 0.0f)
		throw gcnew System::ArgumentOutOfRangeException("Volume can be value in range between 0.0 and 1.0");

	_volume = volume;

	if (!_p_player)
		return;

	_p_player->SetVolume(volume);
}

float MVDotNetMVLib::MVLibWrapper::GetVolume()
{
	if (!_p_player)
		return 0.0f;

	return _p_player->GetVolume();
}

double MVDotNetMVLib::MVLibWrapper::GetDuration()
{
	if (!_p_player)
		return 0.0f;

	return _p_player->GetDuration();
}

double MVDotNetMVLib::MVLibWrapper::GetPosition()
{
	if (!_p_player)
		return 0.0f;

	return _p_player->GetPosition();
}

void MVDotNetMVLib::MVLibWrapper::SetPosition(double position)
{
	if (!_p_player)
		return;

	return _p_player->SeekTo(position);
}

int MVDotNetMVLib::MVLibWrapper::GetPlayerState()
{
	if (!_p_player)
		return 0;

	return (int)_p_player->GetPlayerState();
}

void MVDotNetMVLib::MVLibWrapper::SetDecodingType(int type)
{
	_decoding_type = type;

	if (_decoding_type < 0 || _decoding_type > 4)
		_decoding_type = 0;

	if (_p_player)
		_p_player->SetD3DDecodingType((MV_PlayerD3D_DecodingType)type);
}

void MVDotNetMVLib::MVLibWrapper::SetRenderImmediateMode(bool mode)
{
	_render_immediate_mode = mode;

	if (!_p_player)
		return;

	_p_player->SetVSyncAlignment(mode);
}

bool MVDotNetMVLib::MVLibWrapper::HasAudio()
{
	if (!_p_player)
		return false;

	return _p_player->HasAudio();
}

bool MVDotNetMVLib::MVLibWrapper::HasVideo()
{
	if (!_p_player)
		return false;

	return _p_player->HasVideo();
}

int MVDotNetMVLib::MVLibWrapper::GetVideoWidth()
{
	if (!_p_player)
		return 0;

	return _p_player->GetVideoWidth();
}

int MVDotNetMVLib::MVLibWrapper::GetVideoHeight()
{
	if (!_p_player)
		return 0;

	return _p_player->GetVideoHeight();
}

bool MVDotNetMVLib::MVLibWrapper::RenderScene(int sizeMode, bool forceRender)
{
	if (sizeMode < 0 || sizeMode > 2)
		return false;

	if (!MVLibWrapperManager::IsInitialized)
	{
		if (_p_player && _p_player->GetPlayerState() != MV_PlayerState_Closed)
			_p_player->Close();
	}

	if (!_p_d3d_hwnd_renderer)
		return false;

	IMV_PlayerSDL* p_sdl_player = (IMV_PlayerSDL*)_p_player;

	SDL_Texture* p_video_texture = NULL;
	
	if (_p_player && (_p_player->GetPlayerState() != MV_PlayerState_Error 
		&& _p_player->GetPlayerState() != MV_PlayerState_NotInitialized 
		&& _p_player->GetPlayerState() != MV_PlayerState_Loading)	)
			p_video_texture = p_sdl_player->GetVideoTexture();

	if (!_render_immediate_mode || (_render_immediate_mode && _p_player && _p_player->HasNewVideoFrame()) || forceRender)
	{
		double mediaPosition = -1.0f;

		if (_p_player)
			mediaPosition = _p_player->GetPosition();

		_p_d3d_hwnd_renderer->RenderScene(p_video_texture, sizeMode, mediaPosition);
		return true;
	}

	return false;
}

bool MVDotNetMVLib::MVLibWrapper::RenderOffScreen()
{
	if (!MVLibWrapperManager::IsInitialized)
	{
		if (_p_player && _p_player->GetPlayerState() != MV_PlayerState_Closed)
			_p_player->Close();
	}

	if (!_p_d3d_offscreen_renderer)
		return false;

	MV_PlayerD3D_Frame fb = MV_PlayerD3D_Frame::GetEmptyFrame();

	if (_p_player && (_p_player->GetPlayerState() != MV_PlayerState_Error
		&& _p_player->GetPlayerState() != MV_PlayerState_NotInitialized
		&& _p_player->GetPlayerState() != MV_PlayerState_Loading))
		fb = _p_player->GetVideoBuffer();

	if (fb.isEmpty)
		return false;
	else
	{
		if (_offscreen_surface == System::IntPtr::Zero)
		{
			_sharedHandle = fb.sharedHandle;

			void* p_hw_frame = _p_d3d_offscreen_renderer->CreateSharedSurface(fb.sharedHandle, GetVideoWidth(), GetVideoHeight());

			if (p_hw_frame)
				_offscreen_surface = System::IntPtr(p_hw_frame);
		}

		//if we have yuv or rgb decoding we need to render buffers
		if (_p_player->GetDecodingType() == MV_DecodingType_RGB || _p_player->GetDecodingType() == MV_DecodingType_YUV420)
			_p_d3d_offscreen_renderer->UpdateTexture(fb.buf1, fb.buf2, fb.buf3, fb.buf1Len, fb.buf2Len, fb.buf3Len);

		return true;
	}

}

bool MVDotNetMVLib::MVLibWrapper::RenderOffScreenShared()
{
	if (!MVLibWrapperManager::IsInitialized)
	{
		if (_p_player && _p_player->GetPlayerState() != MV_PlayerState_Closed)
			_p_player->Close();
	}

	if (!_p_d3d_offscreen_renderer)
		return false;

	MV_PlayerD3D_Frame fb = MV_PlayerD3D_Frame::GetEmptyFrame();

	if (_p_player && (_p_player->GetPlayerState() != MV_PlayerState_Error
		&& _p_player->GetPlayerState() != MV_PlayerState_NotInitialized
		&& _p_player->GetPlayerState() != MV_PlayerState_Loading))
		fb = _p_player->GetVideoBuffer();

	if (fb.isEmpty)
		return false;
	else
	{
		_sharedHandle = fb.sharedHandle;

		return true;
	}

}

void MVDotNetMVLib::MVLibWrapper::Release()
{
	CloseSubtitles();

	if (_p_player)
	{
		_p_player->Release();
		_p_player = NULL;
	}

	if (_backColor)
	{
		delete _backColor;
		_backColor = NULL;
	}

	if (_fontColor)
	{
		delete _fontColor;
		_fontColor = NULL;
	}

	if (_outlineColor)
	{
		delete _outlineColor;
		_outlineColor = NULL;
	}

	_offscreen_surface = System::IntPtr::Zero;

	ReleaseSharedSurface();

	if (_p_d3d_hwnd_renderer)
	{
		delete _p_d3d_hwnd_renderer;
		_p_d3d_hwnd_renderer = NULL;
	}

	if (_p_d3d_offscreen_renderer)
	{
		delete _p_d3d_offscreen_renderer;
		_p_d3d_offscreen_renderer = NULL;
	}
}

