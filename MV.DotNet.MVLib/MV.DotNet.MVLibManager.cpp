#include "pch.h"
#include "MV.DotNet.MVLibManager.h"


int MVDotNetMVLib::MVLibWrapperManager::InitializeMediaVault()
{
	if (SDL_Init(SDL_INIT_VIDEO | SDL_INIT_TIMER | SDL_INIT_EVENTS))
	{
		MV_WriteLog(MV_ERROR, "Unable to initialize SDL subsystem!");

		return -1;
	}

	IsInitialized = true;

	return 0;
}

void MVDotNetMVLib::MVLibWrapperManager::CloseMediaVault()
{
	SDL_Quit();

	MV_close_audio_device();

	IsInitialized = false;
}

void MVDotNetMVLib::MVLibWrapperManager::InitializeLog(System::String ^ path)
{
	System::IntPtr ptr = System::Runtime::InteropServices::Marshal::StringToHGlobalAnsi(path);

	MV_InitializeLog((char*)ptr.ToPointer());
	
	System::Runtime::InteropServices::Marshal::FreeHGlobal(ptr);
}

void MVDotNetMVLib::MVLibWrapperManager::DisposeLog()
{
	MV_DisposeLog();
}