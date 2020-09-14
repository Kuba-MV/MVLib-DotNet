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
using System.Collections.Generic;

namespace MV.DotNet.Common
{
    /// <summary>
    /// Descirbes subtitle line
    /// </summary>
    public struct MV_SubtitleLine
    {
        /// <summary>
        /// UTF8 string with subtitle line
        /// </summary>
        public string Line { get; internal set; }

        /// <summary>
        /// Format marker if text should be bolded
        /// </summary>
        public bool isBold { get; internal set; }

        /// <summary>
        /// Format marker if text should be italic
        /// </summary>
        public bool isItalic { get; internal set; }
    }

    /// <summary>
    /// Collection of subtitle items
    /// </summary>
    public class MV_SubtitleLines : List<MV_SubtitleLine>
    {
    }
    
    /// <summary>
    /// Subtitles event arguments
    /// </summary>
    public class MV_SubtitleEventArgs : EventArgs
    {
        /// <summary>
        /// True if subtitle items are new set of text
        /// </summary>
        public bool IsNew { get; internal set; }

        /// <summary>
        /// True when there is not text in subtitle item
        /// </summary>
        public bool IsEmpty { get; internal set; }

        /// <summary>
        /// Collection of subtitle lines
        /// </summary>
        public MV_SubtitleLines Lines { get; internal set; }
    }
}
