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

namespace MV.DotNet.Common
{
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
}