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
    /// Enumeration defines render mode for Media Vault Library
    /// </summary>
    public enum MV_RenderMode : int
    {
        /// <summary>
        /// Player is bounded to display vsync and frame alignment is on.\n
        /// Expected screen redraw is equal to display refresh rate.\n
        /// This mode gives best playback quality, but has a little performance impact.\n
        /// </summary>
        Composition = 0,

        /// <summary>
        /// Player is not bounded to any vsync composition or game loop.\n
        /// There is no any frame alignment and playback is based only on decoded position time stamps.\n
        /// This mode requires screen redraw only when new video frame is available.\n
        /// It has better performance but there is a little video playback quality penalty.\n
        /// </summary>
        Immediate = 1
    }
}
