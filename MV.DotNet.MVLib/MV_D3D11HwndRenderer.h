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

#ifndef __MV_D3D11HWNDRENDERER
#define __MV_D3D11HWNDRENDERER

//#include <Windows.h>

#include "MV_LogBase.h"

#pragma managed(push, off)
#include "SDL.h"
#include "SDL_syswm.h"
#pragma managed(pop)

#include "IMV_SubtitleService.h"

using namespace std;

class MV_D3D11HwndRenderer
{
public:

	MV_D3D11HwndRenderer();

	~MV_D3D11HwndRenderer();

	void RenderScene(SDL_Texture* p_video_tex, int sizeMode, double mediaPosition);
	
	int CreateRenderer(HWND parent);

	bool AddSubService(IMV_SubtitleService* p_sub_sevice, int lineSpace, int margin, 
		const char* fontPath, int fontSize, int outlineSize, 
		MV_Color * fontColor, MV_Color * backColor, MV_Color * outlineColor);

	void RemoveSubService();

	void ClearSubTextures();

	SDL_Renderer* GetRenderer();

private:

	SDL_Window* _p_wnd;
	SDL_Renderer* _p_renderer;
	IMV_SubtitleService* _p_sub_service;
	MV_Color* _fontColor;
	MV_Color* _outlineColor;
	MV_Color* _backColor;
	int _margin, _lineSpace, _outlineSize, _fontSize;
	vector<SDL_Texture*> _subTextures;
};

#endif