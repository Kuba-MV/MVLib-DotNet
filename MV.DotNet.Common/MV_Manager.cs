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

using System;
using System.IO;
using System.Reflection;

namespace MV.DotNet.Common
{
    /// <summary>
    /// MV_Manager is singleton class.\n
    /// Use it to initialize Media Vault subsystem.\n
    /// Should be used at application start (initialize) and application finish (dispose resources).\n
    /// </summary>
    public static class MV_Manager
    {
        private const string MVLIBMANAGER_WRAPPER_TYPE = "MVLibWrapperManager";
        
        private static Assembly _mvLib_dll = null;
        private static Type _mvlib_manager = null;

        internal static Assembly GetMVLibDLL()
        {
            return _mvLib_dll;
        }

        /// <summary>
        /// Get information if Media Vault engine is already initialized.
        /// </summary>
        public static bool IsInitialized
        {
            get
            {
                if (_mvlib_manager == null)
                    return false;

                return (bool) _mvlib_manager.GetProperty("IsInitialized").GetValue(null, null);
            }
        }

        /// <summary>
        /// Initialize Media Vault Engine and loads all necessary dependencies.\n
        /// It should be called once at application start.\n
        /// You must call this method before using any Media Vault controls.\n
        /// Preferable it should be called before any user interface object will be created.\n
        /// </summary>
        public static void InitializeMediaVault()
        {
#if DEBUGPROJ
            _mvLib_dll = Assembly.LoadFile(@"C:\MediaVaultProject\MediaVault.DotNet\MV.DotNet.MVLib\MVLibBin\Win32\Debug\MV.DotNet.MVLib.dll");
#else
            string basePath = AppDomain.CurrentDomain.BaseDirectory;

            string platform = @"\MVLib\x86\MV.DotNet.MVLib.dll";

            if (Environment.Is64BitProcess)
                platform = @"\MVLib\x64\MV.DotNet.MVLib.dll";

            try
            {
                _mvLib_dll = Assembly.LoadFile(basePath + platform);
            }
            catch
            {
                throw new FileNotFoundException("Unable to load MVLibWrapper. Please check if dependencies are placed in your appliaction folder. _" +
                    "Native dependencies should be placed in MVLib folder with x86 and x64 sub folders.\nPlease visit https://bitbucket.org/MV_Kuba/mediavaultlibdotnet/wiki/Downloads if you need to download additional dependencies!");
            }
#endif

            if (_mvLib_dll == null)
                throw new FileNotFoundException("Unable to load MVLibWrapper. Please check if dependencies are placed in your appliaction folder. _" +
                    "Native dependencies should be placed in MVLib folder with x86 and x64 sub folders.\nPlease visit https://bitbucket.org/MV_Kuba/mediavaultlibdotnet/wiki/Downloads if you need to download additional dependencies!");

            foreach (Type type in MV_Manager.GetMVLibDLL().GetExportedTypes())
            {
                if (type.Name == MVLIBMANAGER_WRAPPER_TYPE)
                {
                    _mvlib_manager = type;
                    break;
                }
            }

            if (_mvlib_manager == null)
                throw new InvalidOperationException("Unable to find MVLibManager class in loaded assemblies! Check if you dependencies are correct!");

             int result = (int) _mvlib_manager.GetMethod("InitializeMediaVault", BindingFlags.Public | BindingFlags.Static).Invoke(null, null);

            if (result < 0)
                throw new InvalidOperationException("Initialization failed. Returned value is " + result.ToString());

        }

        /// <summary>
        /// Close Media Vault Engine and release all allocated resources.\n
        /// It should be called once on application exit\n
        /// </summary>
        public static void CloseMediaVault()
        {
            if (_mvlib_manager == null)
                return;

            _mvlib_manager.GetMethod("CloseMediaVault", BindingFlags.Public | BindingFlags.Static).Invoke(null, null);
        }

        /// <summary>
        /// Initialize debug log with providen path.
        /// </summary>
        /// <param name="path"></param>
        public static void InitializeLog(string path)
        {
            if (_mvlib_manager == null)
                return;

            _mvlib_manager.GetMethod("InitializeLog", BindingFlags.Public | BindingFlags.Static).Invoke(null, new string[] { path });
        }

        /// <summary>
        /// Release log.
        /// </summary>
        public static void DisposeLog()
        {
            if (_mvlib_manager == null)
                return;

            _mvlib_manager.GetMethod("DisposeLog", BindingFlags.Public | BindingFlags.Static).Invoke(null, null);
        }

    }
}
