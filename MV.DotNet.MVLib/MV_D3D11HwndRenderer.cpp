#include "pch.h"
#include "MV_D3D11HwndRenderer.h"

#include <d3d11.h>

MV_D3D11HwndRenderer::MV_D3D11HwndRenderer() :
	_p_renderer(NULL),
	_p_wnd(NULL),
	_lineSpace(0),
	_margin(0),
	_fontColor(NULL),
	_backColor(NULL),
	_outlineColor(NULL),
	_p_sub_service(NULL),
	_outlineSize(0),
	_fontSize(0)
{
}

MV_D3D11HwndRenderer::~MV_D3D11HwndRenderer()
{
	RemoveSubService();

	if (_p_renderer)
	{
		SDL_DestroyRenderer(_p_renderer);
		_p_renderer = NULL;
	}

	if (_p_wnd)
	{
		SDL_DestroyWindow(_p_wnd);
		_p_wnd = NULL;
	}
}

int MV_D3D11HwndRenderer::CreateRenderer(HWND parent)
{
	if (!parent)
		return -1;

	_p_wnd = SDL_CreateWindowFrom(parent);

	if (!_p_wnd)
	{
		MV_WriteLog(MV_CRITICAL, "Error during creating D3D window. Error: ", SDL_GetError());
		return -3;
	}

	MV_WriteLog(MV_INFO, "Window created.");

	//change texture scaling mode to linear
	SDL_SetHint(SDL_HINT_RENDER_SCALE_QUALITY, "1");

	_p_renderer = SDL_CreateRenderer(_p_wnd, 1, SDL_RENDERER_ACCELERATED | SDL_RENDERER_TARGETTEXTURE | SDL_RENDERER_PRESENTVSYNC);

	if (!_p_renderer)
	{
		MV_WriteLog(MV_CRITICAL, "Error during creating renderer. Error: ", SDL_GetError());
		return -4;
	}

	SDL_RendererInfo rinfo;

	SDL_GetRendererInfo(_p_renderer, &rinfo);

	MV_WriteLog(0, "Renderer is: %s", rinfo.name);

	//SDL_Event event;

	//set blend mode for our renderer
	SDL_SetRenderDrawBlendMode(_p_renderer, SDL_BLENDMODE_BLEND);

	SDL_SetRenderDrawColor(_p_renderer, 0, 0, 0, 0);

	return 0;
}

bool MV_D3D11HwndRenderer::AddSubService(IMV_SubtitleService * p_sub_sevice, int lineSpace, int margin, const char * fontPath, int fontSize, int outlineSize, MV_Color * fontColor, MV_Color * backColor, MV_Color * outlineColor)
{
	_p_sub_service = p_sub_sevice;

	if (!_p_sub_service)
	{
		MV_WriteLog(MV_ERROR, "Subtitle service is not present.");
		return false;
	}
	
	_p_sub_service->AddRef();

	_lineSpace = lineSpace;

	if (_lineSpace < 0)
		_lineSpace = 0;

	_margin = margin;

	if (_margin < 0)
		_margin = 0;

	_outlineSize = outlineSize;

	if (_outlineSize < 0)
		_outlineSize = 0;

	_fontSize = fontSize;

	if (_fontSize < 1)
		_fontSize = 1;

	bool result = _p_sub_service->OpenFont(fontPath, _fontSize, _outlineSize);

	if (!result)
	{
		MV_WriteLog(MV_ERROR, "Unable to open font.");
		RemoveSubService();

		return false;
	}

	_backColor = backColor;
	_fontColor = fontColor;
	_outlineColor = outlineColor;

	return true;
}

void MV_D3D11HwndRenderer::RemoveSubService()
{
	ClearSubTextures();

	if (_p_sub_service)
	{
		_p_sub_service->Release();
		_p_sub_service = NULL;
	}

	_lineSpace = 0;
	_margin = 0;
	_fontColor = NULL;
	_backColor = NULL;
	_outlineColor = NULL;
	_outlineSize = 0;
	_fontSize = 0;
}

void MV_D3D11HwndRenderer::ClearSubTextures()
{
	for (size_t a = 0; a < _subTextures.size(); a++)
	{
		SDL_Texture* p_tex = _subTextures.at(a);

		if (p_tex)
			SDL_DestroyTexture(p_tex);
	}

	_subTextures.clear();
}

SDL_Renderer* MV_D3D11HwndRenderer::GetRenderer()
{
	return _p_renderer;
}

void MV_D3D11HwndRenderer::RenderScene(SDL_Texture* p_video_tex, int sizeMode, double mediaPosition)
{
	if (!_p_renderer)
		return;

	//SDL_Event event;

	int w, h;

	SDL_GetWindowSize(_p_wnd, &w, &h);

	SDL_Rect rect;

	rect.x = 0;
	rect.y = 0;
	rect.w = w;
	rect.h = h;

	if (sizeMode == 1)
	{
		int vw, vh;

		if (p_video_tex)
		{
			SDL_QueryTexture(p_video_tex, 0, 0, &vw, &vh);

			int sourceWidth = vw;
			int sourceHeight = vh;
			int sourceX = 0;
			int sourceY = 0;
			int destX = 0;
			int destY = 0;

			float nPercent = 0;
			float nPercentW = 0;
			float nPercentH = 0;

			nPercentW = ((float)rect.w / (float)sourceWidth);
			nPercentH = ((float)rect.h / (float)sourceHeight);

			if (nPercentH < nPercentW)
			{
				nPercent = nPercentH;
				destX = (int)(((float)rect.w - ((float)sourceWidth * nPercent)) / 2);
			}
			else
			{
				nPercent = nPercentW;
				destY = (int) (((float) rect.h -((float) sourceHeight * nPercent)) / 2);
			}

			int destWidth = (int)(sourceWidth * nPercent);
			int destHeight = (int)(sourceHeight * nPercent);

			rect.x = destX;
			rect.y = destY;
			rect.w = destWidth;
			rect.h = destHeight;
		}
		
	}

	if (sizeMode == 2)
	{
		int vw, vh;

		if (p_video_tex)
		{
			SDL_QueryTexture(p_video_tex, 0, 0, &vw, &vh);
			rect.w = vw;
			rect.h = vh;
		}
	}
	
	if (_p_sub_service)
	{
		MV_SubtitleServiceItem subItem = _p_sub_service->GetItem(mediaPosition);

		if (subItem.isEmpty)
		{
			ClearSubTextures();
		}
		else
		{
			if (!subItem.item.lines)
				ClearSubTextures();
			else
			{
				if (subItem.isNew)
				{
					int meause = SDL_GetTicks();

					ClearSubTextures();

					if (!subItem.isEmpty)
					{
						if (subItem.item.lines)
						{

							vector<MV_SubtitleServiceVisualItem>* visualItems = _p_sub_service->RenderVisualItems(subItem.item.lines, _fontColor, _backColor, _outlineColor);

							if (visualItems)
							{
								for (size_t a = 0; a < visualItems->size(); a++)
								{
									MV_SubtitleServiceVisualItem visualItem = visualItems->at(a);

									if (!visualItem.data)
										continue;

									SDL_Texture* subTex = SDL_CreateTexture(_p_renderer, SDL_PIXELFORMAT_ARGB8888, SDL_TEXTUREACCESS_STREAMING, visualItem.w, visualItem.h);

									SDL_SetTextureBlendMode(subTex, SDL_BLENDMODE_BLEND);

									if (!subTex)
										continue;

									SDL_UpdateTexture(subTex, 0, visualItem.data, visualItem.pitch);

									_subTextures.push_back(subTex);
								}
							}


						}
					}
				}
			}

		}
	}

	int result = SDL_RenderClear(_p_renderer);

	if (result < 0)
	{
		MV_WriteLog(MV_ERROR, "Unable to clear render target. Error: %s", SDL_GetError());
		return;
	}
	
	if (p_video_tex)
	{
		result = SDL_RenderCopy(_p_renderer, p_video_tex, NULL, &rect);
		
		if (result < 0)
		{
			MV_WriteLog(MV_ERROR, "Unable to render video texture. Error: %s", SDL_GetError());
			return;
		}
	}

	if (_subTextures.size() > 0)
	{
		int wndW, wndH, texW, texH;

		SDL_GetWindowSize(_p_wnd, &wndW, &wndH);

		int texY = 0;

		for (size_t a = _subTextures.size(); a > 0; a--)
		{
			SDL_Texture* subTex = _subTextures.at(a - 1);

			SDL_QueryTexture(subTex, 0, 0, &texW, &texH);

			int subX, subY, lineSpace, bottomMargin;

			bottomMargin = _margin;
			lineSpace = _lineSpace;

			texY += texH;

			subX = (wndW - texW) / 2;
			subY = wndH - bottomMargin - texY - ((int)a * lineSpace);

			SDL_Rect subPos;
			subPos.x = subX;
			subPos.y = subY;
			subPos.w = texW;
			subPos.h = texH;

			SDL_RenderCopy(_p_renderer, subTex, 0, &subPos);
		}
	}

	SDL_RenderPresent(_p_renderer);
}

