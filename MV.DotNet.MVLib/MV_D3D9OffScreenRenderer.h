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

#ifndef __MV_D3D9OFFSCREENRENDERER
#define __MV_D3D9OFFSCREENRENDERER

#include <d3d9.h>

#pragma comment (lib, "d3d9.lib")

#include <d3d11.h>
#include "MV_LogBase.h"

#pragma managed(push, off)
#include "SDL.h"
#include "SDL_syswm.h"
#pragma managed(pop)

class MV_D3D9OffScreenRenderer
{
public:

	MV_D3D9OffScreenRenderer();

	~MV_D3D9OffScreenRenderer();

	bool CreateOffScreenD3D9Device();

	void ReleaseOffScreenD3D9Device();

	void* CreateSharedSurface(HANDLE sharedHandle, int w, int h);

	void UpdateTexture(uint8_t* buf1, uint8_t* buf2, uint8_t* buf3, int buf1Len, int buf2Len, int buf3Len);

	void ReleaseSharedSurface();

private:	

	IDirect3D9Ex* _p_d3d9;
	IDirect3DDevice9Ex* _p_d3d9_device;
	IDirect3DSurface9* _p_shared_surface;
	SDL_Window* _hiden_wnd;
	SDL_Renderer* _renderer;
	SDL_Texture* _render_target_texture;
	SDL_Texture* _software_video_texture;
	int _w, _h;
};

#endif