/*

MIT License

Media Vault Library DotNet 

Copyright (C) 2020 Jakub Gluszkiewicz (kubabrt@gmail.com)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

#pragma once

#define MV_INCLUDE_SDL_PLAYER 1

#include "IMV_PlayerSDL.h"
#include "IMV_SubtitleService.h"
#include "MV_Factory.h"

#include "MV.DotNet.MVLibManager.h"
#include "MV_D3D11HwndRenderer.h"
#include "MV_D3D9OffScreenRenderer.h"

#include <vector>

namespace MVDotNetMVLib 
{
	public ref class MVLibWrapper
	{

	public:

		/// <summary>
		/// Creates MVLibWrapper instance
		/// </summary>
		MVLibWrapper();

		/// <summary>
		/// Creates renderer which will be rendering to provided hwnd
		/// </summary>
		void CreateD3DHwndRenderer(System::IntPtr wndHandle);

		/// <summary>
		/// Creates offscreen renderer
		/// </summary>
		void CreateD3DOffScreenRenderer();

		/// <summary>
		/// Renders current frame into d3d scene.
		/// Use it only with HWND renderer.
		/// You should call this method periodically to refresh screen.
		/// </summary>
		bool RenderScene(int sizeMode, bool forceRender);

		/// <summary>
		/// Copy offscreen video buffer to surface.
		/// Use it only with offscreen renderer.
		/// You should call this method periodically in your game loop.
		/// </summary>
		bool RenderOffScreen();

		/// <summary>
		/// Copy offscreen video buffer to surface.
		/// Use it only with offscreen renderer.
		/// You should call this method periodically in your game loop.
		/// Warning!! It will render textures internally. Call GetOffScreenSharedSurface() to obtain shared texture handle
		/// </summary>
		bool RenderOffScreenShared();

		/// <summary>
		/// Open media from provided url or path.
		/// Use it onlt with HWND renderer.
		/// </summary>
		void OpenMediaHwnd(System::String ^ PathOrUrl, bool asyncOpen, int packetsBufferSize, int framesBufferSize);

		/// <summary>
		/// Open media from provided url or path.
		/// Use it only with offscreen renderer.
		/// </summary>
		void OpenMediaOffScreen(System::String ^ PathOrUrl, int packetsBufferSize, int framesBufferSize);

		/// <summary>
		/// Open subtitles file.\n
		/// Subtitle file must be in UTF-8 format.
		/// </summary>
		bool OpenSubtitles(System::String ^ Path);

		/// <summary>
		/// Open subtitles file.
		/// Subtitle file must be in UTF-8 format.
		/// Use this method to perform subtitle rendering with HWND renderer.
		/// </summary>
		bool OpenSubtitlesHwnd(System::String ^ path, System::String ^ fontPath, int fontSize, int outlineSize, int margin, int lineSpace,
			System::Drawing::Color fontColor, System::Drawing::Color backColor, System::Drawing::Color outlineColor);

		/// <summary>
		/// Close subtitles.
		/// </summary>
		void CloseSubtitles();

		/// <summary>
		/// Get subtitles item from current video position.
		/// </summary>
		void GetSubtitles(bool% isNew, bool% isEmpty, System::Collections::Generic::List<System::String^>^ linesText, System::Collections::Generic::List<bool>^ linesBold, System::Collections::Generic::List<bool>^ linesItalic);

		/// <summary>
		/// Get pointer to video offscreen surface.
		/// Use it only with offscreen renderer.
		/// Returns D3D9Surface pointer.
		/// It can be used with any decoding type.
		/// WARNING! This is not shared surface, caller is responsible for incorporating this pointer into rendering pipeline.
		/// </summary>
		System::IntPtr GetOffScreenSurface();

		/// <summary>
		/// Get pointer to video offscreen shared surface.
		/// Use it only with offscreen renderer.
		/// Use it only with DXVA or D3D11VA decoding types.
		/// </summary>
		System::IntPtr GetOffScreenSharedSurface();

		/// <summary>
		/// Release shared surface.
		/// </summary>
		void ReleaseSharedSurface();

		/// <summary>
		/// Start playback.
		/// </summary>
		void Play();

		/// <summary>
		/// Pause playback.
		/// </summary>
		void Pause();

		/// <summary>
		/// Stop playback.
		/// </summary>
		void Stop();

		/// <summary>
		/// Close media.
		/// </summary>
		void Close();
		
		/// <summary>
		/// Set media volume.\n
		/// Value must be between 0.0 and 1.0
		/// </summary>
		void SetVolume(float volume);

		/// <summary>
		/// Get current media volume.\m
		/// Value is in range between 0.o and 1.0
		/// </summary>
		float GetVolume();

		/// <summary>
		/// Get media duration in seconds.\n
		/// </summary>
		double GetDuration();

		/// <summary>
		/// Get media position in seconds.\n
		/// </summary>
		double GetPosition();

		/// <summary>
		/// Set media position in seconds
		/// </summary>
		void SetPosition(double position);

		/// <summary>
		/// Get current player state.
		/// </summary>
		int GetPlayerState();

		/// <summary>
		/// Set decoding type.\n
		/// Decoding type can be set only in Unitilizaed and Closed states.
		/// </summary>
		void SetDecodingType(int type);

		/// <summary>
		/// Switch off/on vsync frame pooling for video decoding.
		/// </summary>
		void SetRenderImmediateMode(bool mode);

		/// <summary>
		/// Returns true if media has audio stream.
		/// </summary>
		bool HasAudio();

		/// <summary>
		/// Returns true if media has video stream.
		/// </summary>
		bool HasVideo();

		/// <summary>
		/// Get video width.
		/// </summary>
		int GetVideoWidth();

		/// <summary>
		/// Get video height
		/// </summary>
		int GetVideoHeight();

		/// <summary>
		/// Release all memory associated with this class.\n
		/// Caller is reponsible for calling this method before giving this class under GC collector.
		/// </summary>
		void Release();
		
	private:

		IMV_PlayerD3D* _p_player;
		IMV_SubtitleService* _p_sub_service;

		MV_D3D11HwndRenderer* _p_d3d_hwnd_renderer;
		MV_D3D9OffScreenRenderer* _p_d3d_offscreen_renderer;
		void * _sharedHandle;

		System::IntPtr _offscreen_surface;

		float _volume;
		int _decoding_type;
		bool _render_immediate_mode;
		MV_Color* _fontColor;
		MV_Color* _backColor;
		MV_Color* _outlineColor;
	};
}
