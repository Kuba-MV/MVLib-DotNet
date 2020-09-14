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

#include "MV_AudioManager.h"

#pragma managed(push, off)
#include "SDL.h"
#pragma managed(pop)
#include "MV_LogBase.h"

namespace MVDotNetMVLib 
{
	public ref class MVLibWrapperManager
	{

	public:

		/// <summary>
		/// Defines if MVLib was initialized.
		/// </summary>
		static property bool IsInitialized
		{
		public:
			bool get() { return _is_initialized; }
		internal:
			void set(bool val) { _is_initialized = val; }
		}

		/// <summary>
		/// Initialize all native resources.
		/// Should be called at program start.
		/// </summary>
		static int InitializeMediaVault();

		/// <summary>
		/// Closes native resources.
		/// Should be called before program exit.
		/// </summary>
		static void CloseMediaVault();

		/// <summary>
		/// Start loging with provided path.
		/// </summary>
		static void InitializeLog(System::String ^ path);

		/// <summary>
		/// Stop loging
		/// </summary>
		static void DisposeLog();

	private:
		
		static bool _is_initialized;
	};
}
