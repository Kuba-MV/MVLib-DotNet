#include "pch.h"
#include "MV_D3D9OffScreenRenderer.h"

MV_D3D9OffScreenRenderer::MV_D3D9OffScreenRenderer() :
	_p_d3d9(NULL),
	_p_d3d9_device(NULL),
	_p_shared_surface(NULL),
	_w(0),
	_h(0),
	_renderer(NULL),
	_render_target_texture(NULL),
	_hiden_wnd(NULL),
	_software_video_texture(NULL)
{
}

MV_D3D9OffScreenRenderer::~MV_D3D9OffScreenRenderer()
{
	ReleaseOffScreenD3D9Device();
}

bool MV_D3D9OffScreenRenderer::CreateOffScreenD3D9Device()
{
	HRESULT hr = Direct3DCreate9Ex(D3D_SDK_VERSION, (IDirect3D9Ex **)&_p_d3d9);

	if (FAILED(hr))
	{
		MV_WriteLog(MV_ERROR, "Unable to create d3d9 ex interface. Error %i", hr);
		return false;
	}

	D3DPRESENT_PARAMETERS d3dpp;
	ZeroMemory(&d3dpp, sizeof(d3dpp));
	d3dpp.Windowed = TRUE;
	d3dpp.BackBufferCount = 1;
	d3dpp.SwapEffect = D3DSWAPEFFECT_DISCARD;
	d3dpp.BackBufferWidth = 1;
	d3dpp.BackBufferHeight = 1;
	d3dpp.BackBufferFormat = D3DFMT_A8R8G8B8;
	d3dpp.hDeviceWindow = GetDesktopWindow();
	d3dpp.PresentationInterval = D3DPRESENT_INTERVAL_IMMEDIATE;

	hr = _p_d3d9->CreateDeviceEx(0,
		D3DDEVTYPE_HAL,
		d3dpp.hDeviceWindow,
		D3DCREATE_FPU_PRESERVE | D3DCREATE_HARDWARE_VERTEXPROCESSING,
		&d3dpp,
		0,
		&_p_d3d9_device);

	if (FAILED(hr))
	{
		MV_WriteLog(MV_ERROR, "Unable to create d3d9ex device. Error %i", hr);
		return false;
	}

	MV_WriteLog(MV_INFO, "D3D9Ex device created.");

	return true;
}

void MV_D3D9OffScreenRenderer::ReleaseOffScreenD3D9Device()
{
	ReleaseSharedSurface();

	if (_p_d3d9_device)
	{
		_p_d3d9_device->Release();
		_p_d3d9_device = NULL;
	}

	if (_p_d3d9)
	{
		_p_d3d9->Release();
		_p_d3d9 = NULL;
	}
}

void * MV_D3D9OffScreenRenderer::CreateSharedSurface(HANDLE sharedHandle, int w, int h)
{
	ReleaseSharedSurface();

	if (w == 0 || h == 0)
	{
		MV_WriteLog(MV_ERROR, "Proposed size is invalid!");
		return NULL;
	}

	if (!sharedHandle)
	{
		MV_WriteLog(MV_INFO, "Shared handle is not present. Assuming conversion will be needed!");
	}

	if (!_p_d3d9_device)
		return NULL;

	_w = w;
	_h = h;

	//prepare for color conversions
	if (!sharedHandle)
	{
		//create wnd far away to make it not visible
		//its hackish but wnd has to be visible to be renderable

		_hiden_wnd = SDL_CreateWindow("Media Vault", 20000, 0, 1, 1, 
			SDL_WINDOW_SKIP_TASKBAR | 
			SDL_WINDOW_BORDERLESS
			);

		if (!_hiden_wnd)
		{
			MV_WriteLog(MV_ERROR, "Unable to create hiden window. Error %s!", SDL_GetError());
			return NULL;
		}

		_renderer = SDL_CreateRenderer(_hiden_wnd, 1, SDL_RENDERER_ACCELERATED | SDL_RENDERER_TARGETTEXTURE);

		if (!_renderer)
		{
			MV_WriteLog(MV_ERROR, "Unable to create off screen renderer. Error %s!", SDL_GetError());
			return NULL;
		}
	}
	
	//decide if we use shared handle or we create new one
	//if this yuv or rgb
	HANDLE handle = sharedHandle;

	IDirect3DTexture9* p_shared_texture = NULL;

	HRESULT hr = _p_d3d9_device->CreateTexture(w, h,
		1, D3DUSAGE_RENDERTARGET, D3DFMT_A8R8G8B8, D3DPOOL_DEFAULT,
		&p_shared_texture, &handle);

	if (FAILED(hr))
	{
		MV_WriteLog(MV_ERROR, "Unable to create shared texture. Error %i", hr);
		return NULL;
	}

	hr = p_shared_texture->GetSurfaceLevel(0, &_p_shared_surface);

	if (FAILED(hr))
	{
		MV_WriteLog(MV_ERROR, "Unable to get surface from shared texture. Error %i", hr);
		return NULL;
	}

	p_shared_texture->Release();
	p_shared_texture = NULL;

	MV_WriteLog(MV_INFO, "Shared surface created!");

	//if we have yuv or rgb decoding keep allocating resources
	if (sharedHandle)
		return (void*)_p_shared_surface;

	_render_target_texture = SDL_CreateTextureWithSharedHandle_11(_renderer, _w, _h, &handle);

	if (!_render_target_texture)
	{
		MV_WriteLog(MV_ERROR, "Unable to create shared render target texture for yuv/color space conversion. Error %i", hr);
		return NULL;
	}

	MV_WriteLog(MV_INFO, "Offscreen color space conversion resources created!");

	int result = SDL_SetRenderTarget(_renderer, _render_target_texture);

	if (result < 0)
	{
		MV_WriteLog(MV_ERROR, "Cant set offscreen render target! Error: %s", SDL_GetError());
		return NULL;
	}

	return (void*)_p_shared_surface;
}

void MV_D3D9OffScreenRenderer::UpdateTexture(uint8_t* buf1, uint8_t* buf2, uint8_t* buf3, int buf1Len, int buf2Len, int buf3Len)
{
	if (!_renderer)
		return;

	if (_w == 0 || _h == 0)
		return;

	//create yuv texture if needed
	if (!_software_video_texture && buf1 && buf2 && buf3)
	{
		_software_video_texture = SDL_CreateTexture(_renderer, SDL_PIXELFORMAT_IYUV, SDL_TEXTUREACCESS_STREAMING, _w, _h);

		if (!_software_video_texture)
		{
			MV_WriteLog(MV_ERROR, "Cant create yuv offscreen texture! Error: %s", SDL_GetError());
			return;
		}
	}

	//assuming we need rgb texture
	if (!_software_video_texture && !buf2)
	{
		_software_video_texture = SDL_CreateTexture(_renderer, SDL_PIXELFORMAT_RGB24, SDL_TEXTUREACCESS_STREAMING, _w, _h);

		if (!_software_video_texture)
		{
			MV_WriteLog(MV_ERROR, "Cant create rgba offscreen texture! Error: %s", SDL_GetError());
			return;
		}
	}

	if (!_software_video_texture)
		return;

	if (buf1 && buf2 && buf3)
	{
		int result = SDL_UpdateYUVTexture(_software_video_texture, 0, buf1, buf1Len, buf2, buf2Len, buf3, buf3Len);

		if (result < 0)
		{
			MV_WriteLog(MV_ERROR, "Cant update yuv texture! Error: %s", SDL_GetError());
			return;
		}
	}
	else
	{
		if (buf1)
		{
			int result = SDL_UpdateTexture(_software_video_texture, 0, buf1, buf1Len);

			if (result < 0)
			{
				MV_WriteLog(MV_ERROR, "Cant create rgba offscreen texture! Error: %s", SDL_GetError());
				return;
			}
		}
	}

//	SDL_Event event;

	/*while (SDL_PollEvent(&event))
	{

	}*/

	SDL_Rect rect;
	rect.x = 0;
	rect.y = 0;
	rect.w = _w;
	rect.h = _h;

	int result = SDL_RenderCopy(_renderer, _software_video_texture, 0, &rect);
	
	if (result < 0)
	{
		MV_WriteLog(MV_ERROR, "Cant render offscreen texture! Error: %s", SDL_GetError());
		return;
	}


	//now we have to assure rendering operation is completed to fill shared surface with pixels

	ID3D11Device* p_d3d11_device = (ID3D11Device*) SDL_RenderGetD3D11Device1(_renderer);
	ID3D11DeviceContext* p_d3d11_device_ctx = (ID3D11DeviceContext*)SDL_RenderGetD3D11DeviceContext1(_renderer);

	ID3D11Query* pEventQuery = NULL;
	D3D11_QUERY_DESC queryDesc;
	queryDesc.MiscFlags = 0;
	queryDesc.Query = D3D11_QUERY_EVENT;

	HRESULT hr = p_d3d11_device->CreateQuery(&queryDesc, &pEventQuery);

	if (FAILED(hr))
	{
		p_d3d11_device->Release();
		p_d3d11_device = NULL;
		p_d3d11_device_ctx->Release();
		p_d3d11_device_ctx = NULL;
		MV_WriteLog(MV_ERROR, "Unable to create d3d11 query for flushing for offscreen rendering. HRESULT: %i", hr);
		return;
	}

	p_d3d11_device_ctx->End(pEventQuery);

	if (pEventQuery != NULL)
	{
		while (p_d3d11_device_ctx->GetData(pEventQuery, NULL, 0, 0) == S_FALSE)
		{
			SDL_Delay(1);
		};
	}

	pEventQuery->Release();
	pEventQuery = NULL;

	p_d3d11_device_ctx->Release();
	p_d3d11_device_ctx = NULL;

	p_d3d11_device->Release();
	p_d3d11_device = NULL;
}

void MV_D3D9OffScreenRenderer::ReleaseSharedSurface()
{
	if (_p_shared_surface)
	{
		_p_shared_surface->Release();
		_p_shared_surface = NULL;
	}

	if (_render_target_texture)
	{
		if (_renderer)
			SDL_SetRenderTarget(_renderer, NULL);

		SDL_DestroyTexture(_render_target_texture);
		_render_target_texture = NULL;
	}

	if (_software_video_texture)
	{
		SDL_DestroyTexture(_software_video_texture);
		_software_video_texture = NULL;
	}

	if (_renderer)
	{
		SDL_DestroyRenderer(_renderer);
		_renderer = NULL;
	}

	if (_hiden_wnd)
	{
		SDL_DestroyWindow(_hiden_wnd);
		_hiden_wnd = NULL;
	}

	_w = 0;
	_h = 0;

}
