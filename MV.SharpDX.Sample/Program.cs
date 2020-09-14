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
using System.Threading;

using SharpDX.Windows;

using MVDotNetMVLib;

//This sample demonstrates how to use MVLibDotNet to bring mediaplayback into DirectX11 environment.
//Sample is build based on DriectX11 API but if you understand logic, you can use MVLibDotNet with DirectX9 or Direct12
//Bear in mind this sample uses Hadrware Acceleration during decoding movies. It workd only with DXVA2.0 or DXVA11 decodint types.
//It is possible to support or decoding types like YUV12 but it will require some additional work.
//To support additional times you should use another pair of OffScreen rendering option without using shared textures.
//Use full ofscreen rendering and get not shared D3D9Surface. It supports all decoding types.
//However you will be resposnible to make additional copy to create shared texture by your own and utulize it in your project.
//Considering that this sample will give you best possible performance it should be enoug to bring absolutly perfect playback into your multimedia environment without any other modyfications.
//To build sample with x64 architecture copy all needed libraries from Dependencies forlder X64 and reference to x64 MVLibDotnet.

//To run sample pass as first argument path or url to your media stream.

namespace MV.SharpDX.Sample
{
    static class Program
    {
        /// <summary>
        /// Possible player states enumeration.
        /// </summary>
        public enum MV_PlayerStateEnum : int
        {
            /// <summary>
            /// Initial player state.
            /// </summary>
            NotInitialized = 0,
            /// <summary>
            /// Player has loaded source and is paused.
            /// </summary>
            Paused = 1,
            /// <summary>
            /// Player released media resources and is in closed states.
            /// </summary>
            Closed = 2,
            /// <summary>
            /// Player is in fault state. See log for details.
            /// </summary>
            Error = 3,
            /// <summary>
            /// Player is playing media.
            /// </summary>
            Playing = 4,
            /// <summary>
            /// Player currently is during seeking operation.
            /// </summary>
            Seeking = 5,
            /// <summary>
            /// Player is stopped.
            /// </summary>
            Stopped = 6,
            /// <summary>
            /// Player is during loading source. Source is not ready at this stage.
            /// </summary>
            Loading = 7,
            /// <summary>
            /// Player finished playing media stream stream.
            /// </summary>
            Finished = 8
        }

        /// <summary>
        /// Media Vault decoding type available options.\n
        /// Please read enum notes for details.\n
        /// </summary>
        public enum MV_DecodingTypeEnum : int
        {
            /// <summary>
            /// MV Engine will choose best decoding type automatically.
            /// </summary>
            MV_Auto = 0,
            /// <summary>
            /// Video frame is decoded into YUV420P format. This is software decoding type.
            /// </summary>
            MV_YUV420P = 1,

            /// <summary>
            /// Video frame is decoded and converted into RGBA format. This is software decoding type.
            /// </summary>
            MV_RGBA = 2,

            /// <summary>
            /// Video frame is decoded via DXVA 2.0 hardware accelerator. This is hardware decoding type.
            /// </summary>
            MV_DXVA = 3,

            /// <summary>
            /// Video frame is decoded via DXVA11 hardware accelerator. This is hardware decoding type.
            /// </summary>
            MV_D3D11VA = 4
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length < 1)
                return;

            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");

            //initialize MVLib
            MVLibWrapperManager.InitializeMediaVault();

            var videoForm = new RenderForm("MVLib - SharpDX Video Player");

            videoForm.Width = 1280;
            videoForm.Height = 720;

            D3D11Renderer renderer = new D3D11Renderer();

            renderer.InitializeD3D11(videoForm.Handle, videoForm.ClientSize.Width, videoForm.ClientSize.Height);

            //create MVLib player
            MVLibWrapper mvPlayer = new MVLibWrapper();

            //first initialize offscreen renderer
            mvPlayer.CreateD3DOffScreenRenderer();

            //this sample supports only hw decoding. It can be DX11VA or DXVA2
            mvPlayer.SetDecodingType((int) MV_DecodingTypeEnum.MV_D3D11VA);

            //now you can open your stream
            //pass 0 as buffer sizef - they will be interpreted as default
            mvPlayer.OpenMediaOffScreen(args[0], 0, 0);

            bool initialized = false;

            RenderLoop.Run(videoForm, () =>
            {
                renderer.Clear();
                renderer.Render();
                renderer.Present();

                //process our stream
                mvPlayer.RenderOffScreenShared();

                //lets wait for ready state
                if (!initialized && (MV_PlayerStateEnum) mvPlayer.GetPlayerState() == MV_PlayerStateEnum.Paused)
                {
                    //if we have frame ready and shared surface wasnt initialized
                    if (mvPlayer.GetOffScreenSharedSurface()  != renderer.GetSharedHandle())
                        renderer.OpenSharedResource(mvPlayer.GetOffScreenSharedSurface());

                    mvPlayer.SetVolume(0.8f);
                    mvPlayer.Play();
                }
            });

            mvPlayer.Close();
            mvPlayer.Release();
            mvPlayer = null;

            renderer.CloseD3D11();
            renderer = null;

            //do not forget to close mv lib before exit
            MVLibWrapperManager.CloseMediaVault();

        }
    }
}
