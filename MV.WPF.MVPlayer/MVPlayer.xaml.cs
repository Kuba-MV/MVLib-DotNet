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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Interop;
using System.Collections.Generic;

using MV.DotNet.Common;

namespace MV.DotNet.WPF.MVPlayer
{
    /// <summary>
    /// Media Vault Library Control for WPF
    /// </summary>
    public partial class MVPlayer : UserControl
    {

        #region fields
        private const string MVLIB_WRAPPER_TYPE = "MVLibWrapper";

        private dynamic _media_wrapper = null;

        private bool _show_fps = false;

        private Stopwatch _clock;
        private long _frameCount;

        private double _last_position_measure = 0D;

        private D3DImage _d3d_render = null;

        private IntPtr _d3d_surface = IntPtr.Zero;

        private bool _lastSubtitleNotEmpty = false;

        #endregion

        #region ctor
        /// <summary>
        /// Creates new instance of Media Vault Library Control for WPF
        /// </summary>
        public MVPlayer()
        {
            InitializeComponent();

            if ((LicenseManager.UsageMode == LicenseUsageMode.Runtime))
            {
                if (!MV_Manager.IsInitialized)
                    throw new InvalidOperationException("MediaVault library is not initialized! Call MV_Manager.InitilizeMediaVault() before using this control in runtime!");

                if (MV_Manager.GetMVLibDLL() == null)
                    throw new NullReferenceException("Unable to obtain loaded MVLib! Are you sure initialization was succeeded?");

                Type mvlibWrapperType = null;

                foreach (Type type in MV_Manager.GetMVLibDLL().GetExportedTypes())
                {
                    if (type.Name == MVLIB_WRAPPER_TYPE)
                    {
                        mvlibWrapperType = type;
                        break;
                    }
                }

                if (mvlibWrapperType == null)
                    throw new InvalidOperationException("Unable to find MVLibWrapper class in loaded assemblies! Check if you dependencies are correct!\nPlease visit https://bitbucket.org/MV_Kuba/mediavaultlibdotnet/wiki/Downloads if you need to download additional dependencies!");

                _media_wrapper = Activator.CreateInstance(mvlibWrapperType);

                if (_media_wrapper == null)
                    throw new InvalidOperationException("Unable to create MVLibWrapper instance. Check if your dependencies are correct!");

                _media_wrapper.CreateD3DOffScreenRenderer();

                _d3d_render = new D3DImage();
                _d3d_render.IsFrontBufferAvailableChanged += _d3d_render_IsFrontBufferAvailableChanged;

                videoImage.Source = _d3d_render;

                CompositionTarget.Rendering += CompositionTarget_Rendering;

            }
        }

        #endregion

        #region events
        /// <summary>
        /// Media state change handler.
        /// </summary>
        /// <param name="sender">Delegate sender.</param>
        /// <param name="e">State changed arguments.</param>
        public delegate void MediaStateChangedeHandler(object sender, MV_MediaStateChangedEventArgs e);

        /// <summary>
        /// Event is fired when player state will change.
        /// </summary>
        public event MediaStateChangedeHandler MediaStateChanged;

        /// <summary>
        /// Position changed handler.
        /// </summary>
        /// <param name="sender">Delegate sender.</param>
        /// <param name="e">Current position arguments.</param>
        public delegate void MediaPositionChangedeHandler(object sender, MV_MediaPositionChangedEventArgs e);

        /// <summary>
        /// Event is fired when position of media will change.
        /// </summary>
        public event MediaPositionChangedeHandler MediaPositionChanged;

        /// <summary>
        /// Subtitle changed handler.
        /// </summary>
        /// <param name="sender">Delegate sender</param>
        /// <param name="e">Subtitle data</param>
        public delegate void SubtitleChangedHandler(object sender, MV_SubtitleEventArgs e);

        /// <summary>
        /// Fired when subtitle changed
        /// </summary>
        public event SubtitleChangedHandler SubtitleChanged;
        #endregion

        #region private methods

        private void _d3d_render_IsFrontBufferAvailableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_d3d_render == null)
                return;

            if (!_d3d_render.IsFrontBufferAvailable)
            {
                _d3d_render.Lock();
                _d3d_render.SetBackBuffer(D3DResourceType.IDirect3DSurface9, IntPtr.Zero);
                _d3d_render.Unlock();
            }
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            if (ShowFPS)
            {
                _frameCount++;

                if (_clock != null && _clock.ElapsedMilliseconds >= 1000)
                {
                    Window wnd_parent = Window.GetWindow(this);

                    if (wnd_parent != null)
                        wnd_parent.Title = string.Format("FPS: {0}", _frameCount);

                    _frameCount = 0;
                    _clock.Restart();
                }

            }

            // Dirty hack to force WPF to refresh at vsync interval
            // let pretend we always have something to redraw
            // so funny tbh, who made that engine?????!!!???

            this.Background = new SolidColorBrush(Color.FromRgb(0x0, 0x0, 0x0));


            if (_media_wrapper == null)
                return;

            //event handling
            if (MediaPosition != _media_wrapper.GetPosition() && Math.Abs(_last_position_measure - MediaPosition) > PositionChangedStep)
            {
                _last_position_measure = _media_wrapper.GetPosition();
                MV_MediaPositionChangedEventArgs args = new MV_MediaPositionChangedEventArgs();

                args.Position = _media_wrapper.GetPosition();

                if (MediaPositionChanged != null)
                    MediaPositionChanged(this, args);
            }

            //update some media dependency properties
            if (_media_wrapper.GetPosition() != MediaPosition)
                this.SetValue(MediaPositionProperty, _media_wrapper.GetPosition());

            if (_media_wrapper.GetDuration() != MediaDuration)
                MediaDuration = _media_wrapper.GetDuration();

            if ((int)MediaState != _media_wrapper.GetPlayerState())
            {

                MediaState = (MV_PlayerStateEnum)_media_wrapper.GetPlayerState();

                MV_MediaStateChangedEventArgs args = new MV_MediaStateChangedEventArgs();

                args.State = MediaState;

                if (MediaStateChanged != null)
                    MediaStateChanged(this, args);
            }

            if (VideoWidth == 0 || VideoHeight == 0)
                return;

            if (HasOpenSubtitles)
            {
                bool IsNew = false;
                bool IsEmpty = false;
                List<string> textLines = new List<string>();
                List<bool> textBoolFmt = new List<bool>();
                List<bool> textItalicFmt = new List<bool>();

                _media_wrapper.GetSubtitles(ref IsNew, ref IsEmpty, textLines, textBoolFmt, textItalicFmt);

                if (SubtitleChanged != null && (IsNew || (IsEmpty && _lastSubtitleNotEmpty)))
                {
                    MV_SubtitleEventArgs args = new MV_SubtitleEventArgs();

                    _lastSubtitleNotEmpty = !IsEmpty;
                    args.IsEmpty = IsEmpty;
                    args.IsNew = IsNew;

                    MV_SubtitleLines lines = new MV_SubtitleLines();

                    for (int a=0;a<textLines.Count;a++)
                    {
                        MV_SubtitleLine line = new MV_SubtitleLine();
                        line.isBold = textBoolFmt[a];
                        line.isItalic = textItalicFmt[a];
                        line.Line = textLines[a];

                        lines.Add(line);
                    }

                    args.Lines = lines;

                    SubtitleChanged(this, args);
                }

            }

            bool hasVideoFrame = _media_wrapper.RenderOffScreen();

            if (hasVideoFrame && _d3d_surface == IntPtr.Zero)
            {
                _d3d_surface = _media_wrapper.GetOffScreenSurface();

                if (_d3d_surface != IntPtr.Zero)
                {
                    _d3d_render.Lock();
                    _d3d_render.SetBackBuffer(D3DResourceType.IDirect3DSurface9, _d3d_surface);
                    _d3d_render.Unlock();

                    //restore window focus
                    if (DecodingType == MV_DecodingTypeEnum.MV_RGBA || DecodingType == MV_DecodingTypeEnum.MV_YUV420P)
                    {
                        Window wnd = Window.GetWindow(this);

                        if (wnd != null)
                            wnd.Activate();
                    }
                }
            }

            if (_d3d_render.IsFrontBufferAvailable && hasVideoFrame && _d3d_surface != IntPtr.Zero)
            {
                // lock the D3DImage
                _d3d_render.Lock();

                // invalidate the updated region of the D3DImage (in this case, the whole image)
                _d3d_render.AddDirtyRect(new Int32Rect(0, 0, VideoWidth, VideoHeight));

                // unlock the D3DImage
                _d3d_render.Unlock();
            }
        }

        private void ReleaseD3DSurface()
        {
            if (_media_wrapper != null)
                _media_wrapper.ReleaseSharedSurface();

            if (_d3d_render == null)
                return;

            _d3d_render.Lock();
            _d3d_render.SetBackBuffer(D3DResourceType.IDirect3DSurface9, IntPtr.Zero);
            _d3d_render.Unlock();

            _d3d_surface = IntPtr.Zero;
        }

        #endregion

        #region depedency stattics

        public static readonly DependencyProperty DecodingTypeProperty = DependencyProperty.Register(
          "DecodingType", typeof(MV_DecodingTypeEnum), typeof(MVPlayer), new PropertyMetadata(MV_DecodingTypeEnum.MV_Auto));

        public static readonly DependencyProperty VolumeProperty = DependencyProperty.Register(
          "Volume", typeof(float), typeof(MVPlayer), new PropertyMetadata(0.2f));

        public static readonly DependencyProperty MediaStateProperty = DependencyProperty.Register(
            "MediaState", typeof(MV_PlayerStateEnum), typeof(MVPlayer), new PropertyMetadata(MV_PlayerStateEnum.NotInitialized));

        public static readonly DependencyProperty PositionChangedStepProperty = DependencyProperty.Register(
            "PositionChangedStep", typeof(double), typeof(MVPlayer), new PropertyMetadata(0.2D));

        public static readonly DependencyProperty MediaPositionProperty = DependencyProperty.Register(
            "MediaPosition", typeof(double), typeof(MVPlayer), new PropertyMetadata(0D));

        public static readonly DependencyProperty MediaDurationProperty = DependencyProperty.Register(
            "MediaDuration", typeof(double), typeof(MVPlayer), new PropertyMetadata(0D));

        public static readonly DependencyProperty SizeModeProperty = DependencyProperty.Register(
            "SizeMode", typeof(MV_SizeMode), typeof(MVPlayer), new PropertyMetadata(MV_SizeMode.AspectRatio));

        public static readonly DependencyProperty RenderModeProperty = DependencyProperty.Register(
            "RenderMode", typeof(MV_RenderMode), typeof(MVPlayer), new PropertyMetadata(MV_RenderMode.Immediate));

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets video size mode.
        /// </summary>
        public bool ShowFPS
        {
            get
            {
                return _show_fps;
            }
            set
            {
                _show_fps = value;

                if (_show_fps)
                    _clock = Stopwatch.StartNew();
                else
                {
                    if (_clock != null)
                    {
                        _clock.Stop();
                        _clock = null;
                    }
                }
            }
        }

        /// <summary>
        /// Set volume of media stream.\n
        /// Value must be in range from 0.0 to 1.0.\n
        /// This is dependency property.
        /// </summary>
        public virtual float Volume
        {
            get
            {
                return (float) this.GetValue(VolumeProperty);
            }
            set
            {
                if (_media_wrapper != null)
                    _media_wrapper.SetVolume(value);

                this.SetValue(VolumeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets player render mode.\n
        /// See enumeration description for more details.\n
        /// This is dependency property.
        /// </summary>
        public MV_RenderMode RenderMode
        {
            get
            {
                return (MV_RenderMode)this.GetValue(RenderModeProperty);
            }
            set
            {
                if (value == MV_RenderMode.Composition)
                    _media_wrapper.SetRenderImmediateMode(false);
                else
                    _media_wrapper.SetRenderImmediateMode(true);

                this.SetValue(RenderModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets video size mode.\n
        /// This is dependency property.
        /// </summary>
        public MV_SizeMode SizeMode
        {
            get
            {
                return (MV_SizeMode) this.GetValue(SizeModeProperty);
            }
            set
            {
                this.SetValue(SizeModeProperty, value);

                if (value == MV_SizeMode.AspectRatio)
                    videoImage.Stretch = Stretch.Uniform;

                if (value == MV_SizeMode.Natural)
                    videoImage.Stretch = Stretch.None;

                if (value == MV_SizeMode.Stretch)
                    videoImage.Stretch = Stretch.Fill;
            }
        }

        /// <summary>
        /// Gets or sets player decoding mode.\n
        /// See enumeration description for more informations.\n
        /// This is dependency property.
        /// </summary>
        public MV_DecodingTypeEnum DecodingType
        {
            get
            {
                return (MV_DecodingTypeEnum)this.GetValue(DecodingTypeProperty);
            }
            set {

                if (_media_wrapper != null)
                {
                    _media_wrapper.SetDecodingType((int)value);
                    this.SetValue(DecodingTypeProperty, value);
                }
                
            }
        }

        /// <summary>
        /// Gets player current media state.\n
        /// This is dependency property.
        /// </summary>
        public virtual MV_PlayerStateEnum MediaState
        {
            get
            {
                return (MV_PlayerStateEnum)this.GetValue(MediaStateProperty);
            }
            private set
            {
                this.SetValue(MediaStateProperty, value);
            }
        }

        /// <summary>
        /// Defines ppsition change step.\n
        /// It defines period of time in seconds between media state changed events.\n
        /// Than higher than media position change events are fired less frequently.\n
        /// This is dependency property.
        /// </summary>
        public double PositionChangedStep
        {
            get
            {
                return (double)this.GetValue(PositionChangedStepProperty);
            }
            set
            {
                this.SetValue(PositionChangedStepProperty, value);
            }
        }

        /// <summary>
        /// Gets current media position in seconds.\n
        /// This is dependency property.
        /// </summary>
        public virtual double MediaPosition
        {
            get
            {
                return (double)this.GetValue(MediaPositionProperty);

            }
            set
            {
                if (_media_wrapper != null)
                    _media_wrapper.SetPosition(value);

                this.SetValue(MediaPositionProperty, value);
                
            }
        }

        /// <summary>
        /// Gets media duration in seconds.\n
        /// This is dependency property.
        /// </summary>
        public virtual double MediaDuration
        {
            get
            {
                return (double)this.GetValue(MediaDurationProperty);

            }
            private set
            {
                this.SetValue(MediaDurationProperty, value);

            }
        }

        /// <summary>
        /// Returns true if media contains video stream.
        /// </summary>
        public bool HasVideo
        {
            get
            {
                if (_media_wrapper == null)
                    return false;

                return _media_wrapper.HasVideo();

            }
        }

        /// <summary>
        /// Returns true if media contains audio stream.
        /// </summary>
        public bool HasAudio
        {
            get
            {
                if (_media_wrapper == null)
                    return false;

                return _media_wrapper.HasAudio();
            }
        }

        /// <summary>
        /// Returns video width.
        /// </summary>
        public int VideoWidth
        {
            get
            {
                if (_media_wrapper == null)
                    return 0;

                return _media_wrapper.GetVideoWidth();
            }
        }

        /// <summary>
        /// Returns video height.
        /// </summary>
        public int VideoHeight
        {
            get
            {
                if (_media_wrapper == null)
                    return 0;

                return _media_wrapper.GetVideoHeight();
            }
        }

        /// <summary>
        /// Returns if media has open subtitles.
        /// </summary>
        public bool HasOpenSubtitles { get; private set; }

        #endregion

        #region public methods

        /// <summary>
        /// Opens media.\n
        /// Wait for proper state following media state change event.
        /// </summary>
        /// <param name="PathOrUrl">It can be path to file or stream address.</param>
        public virtual void OpenMedia(string PathOrUrl)
        {
            OpenMedia(PathOrUrl, 0, 0);

        }

        /// <summary>
        /// Opens media.\n
        /// Wait for proper state following media state change event.
        /// </summary>
        /// <param name="PathOrUrl">It can be path to file or stream address</param>
        /// <param name="packetsBufferSize">Defines size of demuxer buffer.</param>
        /// <param name="framesBufferSize">Defines size of frames buffer.</param>
        public virtual void OpenMedia(string PathOrUrl, int packetsBufferSize, int framesBufferSize)
        {
            ReleaseD3DSurface();

            if (_media_wrapper == null)
                return;

            if (string.IsNullOrEmpty(PathOrUrl))
                return;

            _media_wrapper.OpenMediaOffScreen(PathOrUrl, packetsBufferSize, framesBufferSize);

        }

        /// <summary>
        /// Opens susbtitles at given path.
        /// Supported formats: SubRip(srt), MicroDVD, MPL2
        /// </summary>
        /// <param name="PathOrUrl"></param>
        public virtual void OpenSubtitles(string PathOrUrl)
        {
            if (_media_wrapper == null)
                return;

            if (string.IsNullOrEmpty(PathOrUrl))
                return;

            bool result = _media_wrapper.OpenSubtitles(PathOrUrl);

            if (!result)
                throw new System.FormatException("Subtitle path is incorrect or unsupported subtitle format!");

            HasOpenSubtitles = result;

        }

        /// <summary>
        /// Closes susbtitle service.
        /// </summary>
        public virtual void CloseSubtitles()
        {
            HasOpenSubtitles = false;
            _lastSubtitleNotEmpty = false;
            if (_media_wrapper == null)
                return;

            _media_wrapper.CloseSubtitles();

        }

        /// <summary>
        /// Starts playback.
        /// </summary>
        public virtual void Play()
        {
            if (_media_wrapper == null)
                return;

            _media_wrapper.Play();
        }

        /// <summary>
        /// Pauses playback.
        /// </summary>
        public virtual void Pause()
        {
            if (_media_wrapper == null)
                return;

            _media_wrapper.Pause();

        }

        /// <summary>
        /// Stops playback.
        /// </summary>
        public virtual void Stop()
        {
            if (_media_wrapper == null)
                return;

            _media_wrapper.Stop();
        }

        /// <summary>
        /// Closes media resources and releases memory.\
        /// Warning! Call release method to fully free memory when control is not needed anymore.
        /// </summary>
        public virtual void Close()
        {
            HasOpenSubtitles = false;

            ReleaseD3DSurface();

            if (_media_wrapper == null)
                return;

            _media_wrapper.Close();
        }

        /// <summary>
        /// Closes source and releases all native resources.
        /// </summary>
        public void Release()
        {
            HasOpenSubtitles = false;

            CompositionTarget.Rendering -= CompositionTarget_Rendering;

            if (_clock != null)
            {
                _clock.Stop();
                _clock = null;
            }

            ReleaseD3DSurface();

            _d3d_render = null;

            if (_media_wrapper != null)
            {
                _media_wrapper.Release();
                _media_wrapper = null;
            }
        }

        #endregion
    }
}
