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
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;
using System.Drawing;

using MV.DotNet.Common;

namespace MV.DotNet.WinForms.MVPlayer
{
    /// <summary>
    /// Media Vault Player Control for WinForms.
    /// </summary>
    [ToolboxBitmap(typeof(MVPlayer))]
    public partial class MVPlayer : UserControl
    {
        #region private classes
        internal class EventItem
        {
            internal int eventType;
            internal object eventData;
        }
        #endregion

        #region fields
        private const string MVLIB_WRAPPER_TYPE = "MVLibWrapper";

        private dynamic _media_wrapper = null;

        private Stopwatch _clock;
        private long _frameCount;

        private float _volume = 0.0f;

        private MV_PlayerStateEnum _lastState = MV_PlayerStateEnum.NotInitialized;
        private MV_DecodingTypeEnum _decodingType = MV_DecodingTypeEnum.MV_Auto;

        bool _show_fps = false;

        object _render_locker = new object();

        BackgroundWorker _renderThread = null;

        private volatile bool _signal_render_finish = false;

        private AutoResetEvent _resetEvent = new AutoResetEvent(false);

        private object _events_lock = new object();

        private Queue<EventItem> _events = new Queue<EventItem>();

        private double _position_changed_step = 1.0f;

        private double _last_position_value = 0.0f;

        private MV_RenderMode _render_mode = MV_RenderMode.Composition;

        private bool _force_render = false;

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
        /// <param name="sender">Deledate sender.</param>
        /// <param name="e">Current position arguments.</param>
        public delegate void MediaPositionChangedeHandler(object sender, MV_MediaPositionChangedEventArgs e);

        /// <summary>
        /// Event is fired when position of media will change.
        /// </summary>
        public event MediaPositionChangedeHandler MediaPositionChanged;
        #endregion

        #region constructor
        /// <summary>
        /// Create new instance of MVPlayer control.
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

                IsInDesignMode = false;

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
                    throw new InvalidOperationException("Unable to find MVLibWrapper class in loaded assemblies! Check if you dependencies are correct!\n Please visit https://github.com/Kuba-MV/MVLib-DotNet if you need to download additional dependencies!");

                _media_wrapper = Activator.CreateInstance(mvlibWrapperType);

                if (_media_wrapper == null)
                    throw new InvalidOperationException("Unable to create MVLibWrapper instance. Check if your dependencies are correct!");

                _media_wrapper.CreateD3DHwndRenderer(pnD3D.Handle);

            }
            else
                IsInDesignMode = true;

            if (!IsInDesignMode)
            {
                InstallInRenderLoop();
            }

        }
        #endregion

        #region private properties

        private bool IsInDesignMode { get; set; }

        private bool IsRenderLoopInitialized { get; set; }

        private bool RestorePlayingState { get; set; }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets video size mode.
        /// </summary>
        public MV_SizeMode SizeMode { get; set; }

        /// <summary>
        /// Defines ppsition change step.\n
        /// It defines period of time in seconds between media state changed events.\n
        /// Than higher than media position change events are fired less frequently.
        /// </summary>
        public double PositionChangedStep
        {
            get
            {
                return _position_changed_step;
            }
            set
            {
                if (value > 0.0f && value < 10.0f)
                    _position_changed_step = value;
            }
        }

        /// <summary>
        /// When switched on FPS counter will be shown in your window title.
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
        /// Value must be in range from 0.0 to 1.0.
        /// </summary>
        public virtual float Volume
        {
            get
            {
                return _volume;
            }
            set
            {
                lock (_render_locker)
                {
                    if (_media_wrapper != null)
                        _media_wrapper.SetVolume(value);
                }

                _volume = value;
            }
        }

        /// <summary>
        /// Gets current media position in seconds.
        /// </summary>
        public virtual double MediaPosition
        {
            get
            {
                lock (_render_locker)
                {

                    if (_media_wrapper == null)
                        return 0.0f;

                    return _media_wrapper.GetPosition();
                }
            }
            set
            {
                lock (_render_locker)
                {
                    if (_media_wrapper != null)
                        _media_wrapper.SetPosition(value);
                }
            }
        }

        /// <summary>
        /// Gets media duration in seconds.
        /// </summary>
        public virtual double MediaDuration
        {
            get
            {
                lock (_render_locker)
                {

                    if (_media_wrapper == null)
                        return 0.0f;

                    return _media_wrapper.GetDuration();
                }
            }
        }

        /// <summary>
        /// Gets player current media state.
        /// </summary>
        public virtual MV_PlayerStateEnum MediaState
        {
            get
            {
                lock (_render_locker)
                {

                    if (_media_wrapper == null)
                        return _lastState;

                    return (MV_PlayerStateEnum)_media_wrapper.GetPlayerState();
                }

            }
        }

        /// <summary>
        /// Gets or sets player decoding mode.\n
        /// See enumeration description for more informations.
        /// </summary>
        public virtual MV_DecodingTypeEnum DecodingType
        {
            get
            {
                return _decodingType;
            }
            set
            {
                lock (_render_locker)
                {
                    if (_media_wrapper != null)
                    {
                        _media_wrapper.SetDecodingType((int)value);
                        _decodingType = value;
                    }
                }
            }
        }

        /// <summary>
        /// Gets video panel child.\n
        /// You can attach events to this panel.
        /// </summary>
        public Control VideoPanel
        {
            get
            {
                return pnD3D;
            }
        }

        /// <summary>
        /// Gets or sets player render mode.\n
        /// See enumeration description for more details.
        /// </summary>
        public MV_RenderMode RenderMode
        {
            get
            {
                return _render_mode;
            }
            set
            {
                _render_mode = value;

                lock (_render_locker)
                {
                    if (_media_wrapper != null)
                    {
                        if (_render_mode == MV_RenderMode.Composition)
                            _media_wrapper.SetRenderImmediateMode(false);
                        else
                            _media_wrapper.SetRenderImmediateMode(true);
                    }
                }
            }
        }

        /// <summary>
        /// Returns true if media contains video stream.
        /// </summary>
        public bool HasVideo
        {
            get
            {
                lock (_render_locker)
                {

                    if (_media_wrapper == null)
                        return false;

                    return _media_wrapper.HasVideo();
                }
            }
        }

        /// <summary>
        /// Returns true if media contains audio stream.
        /// </summary>
        public bool HasAudio
        {
            get
            {
                lock (_render_locker)
                {

                    if (_media_wrapper == null)
                        return false;

                    return _media_wrapper.HasAudio();
                }
            }
        }

        /// <summary>
        /// Returns video width.
        /// </summary>
        public int VideoWidth
        {
            get
            {
                lock (_render_locker)
                {

                    if (_media_wrapper == null)
                        return 0;

                    return _media_wrapper.GetVideoWidth();
                }
            }
        }

        /// <summary>
        /// Returns video height.
        /// </summary>
        public int VideoHeight
        {
            get
            {
                lock (_render_locker)
                {

                    if (_media_wrapper == null)
                        return 0;

                    return _media_wrapper.GetVideoHeight();
                }
            }
        }


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
            lock (_render_locker)
            {

                if (_media_wrapper == null)
                    return;

                if (string.IsNullOrEmpty(PathOrUrl))
                    return;

                _media_wrapper.OpenMediaHwnd(PathOrUrl, false, packetsBufferSize, framesBufferSize);
            }
        }

        /// <summary>
        /// Opens media asynchronously.\n
        /// Wait for proper state following media state change event.
        /// </summary>
        /// <param name="PathOrUrl">It can be path to file or stream address.</param>
        public virtual void OpenMediaAsync(string PathOrUrl)
        {
            OpenMediaAsync(PathOrUrl, 0, 0);
        }

        /// <summary>
        /// Opens media asynchronously.\n
        /// Wait for proper state following media state change event.
        /// </summary>
        /// <param name="PathOrUrl">It can be path to file or stream address</param>
        /// <param name="packetsBufferSize">Defines size of demuxer buffer.</param>
        /// <param name="framesBufferSize">Defines size of frames buffer.</param>
        public virtual void OpenMediaAsync(string PathOrUrl, int packetsBufferSize, int framesBufferSize)
        {
            lock (_render_locker)
            {
                if (_media_wrapper == null)
                    return;

                if (string.IsNullOrEmpty(PathOrUrl))
                    return;

                _media_wrapper.OpenMediaHwnd(PathOrUrl, true, packetsBufferSize, framesBufferSize);
            }
        }

        /// <summary>
        /// Open subtitles for rendering.\n
        /// Warning! This method can be called when mv player is added to parent!
        /// </summary>
        /// <param name="SubtitlesPath">Path to subtitle file. File must be UTF-8 charset. Supported subtitles formats are: SubRip(srt), MicroDVD, MPL2</param>
        /// <param name="fontPath">Path to font file. Font must be TTF format.</param>
        /// <param name="fontSize">Size of the font</param>
        /// <param name="outlineSize">Size of outline. Pass 0 to avoid rendering outline</param>
        /// <param name="margin">Size in pixels bottom margin of first subtitle line</param>
        /// <param name="lineSpace">Size in pixels the space between lines</param>
        /// <param name="fontColor">Font color. Can not be empty.</param>
        /// <param name="backColor">Color of background. Pass color empty to make background transparent</param>
        /// <param name="outlineColor">Color of outline. Pass colort empty to make outline transparent</param>
        public virtual void OpenSubtitles(string SubtitlesPath, string fontPath, int fontSize, int outlineSize, int margin, int lineSpace, Color fontColor, Color backColor, Color outlineColor)
        {
            bool result = false;
            lock (_render_locker)
            {

                if (_media_wrapper == null)
                    return;

                result = _media_wrapper.OpenSubtitlesHwnd(SubtitlesPath, fontPath, fontSize, outlineSize, margin, lineSpace, fontColor, backColor, outlineColor);
            }

            if (!result)
                throw new InvalidOperationException("Unable to create subtitle service. Check mv_log for more details!");
        }

        public virtual void CloseSubtitles()
        {
            lock (_render_locker)
            {

                if (_media_wrapper == null)
                    return;

                _media_wrapper.CloseSubtitles();
            }
        }

        /// <summary>
        /// Starts playback.
        /// </summary>
        public virtual void Play()
        {
            lock (_render_locker)
            {

                if (_media_wrapper == null)
                    return;

                _media_wrapper.Play();
            }
        }

        /// <summary>
        /// Pauses playback.
        /// </summary>
        public virtual void Pause()
        {
            lock (_render_locker)
            {
                if (_media_wrapper == null)
                    return;

                _media_wrapper.Pause();
            }
        }

        /// <summary>
        /// Stops playback.
        /// </summary>
        public virtual void Stop()
        {
            lock (_render_locker)
            {
                if (_media_wrapper == null)
                    return;

                _media_wrapper.Stop();
            }
        }

        /// <summary>
        /// Closes media resources and releases memory.\
        /// Warning! Call dispose method to fully release memory when control is not needed anymore.
        /// </summary>
        public virtual void Close()
        {
            lock (_render_locker)
            {
                if (_media_wrapper == null)
                    return;

                _media_wrapper.CloseSubtitles();
                _media_wrapper.Close();
            }
        }

        #endregion

        #region private methods

        private void ReleaseNatives()
        {
            if (_media_wrapper != null)
            {
                _media_wrapper.Release();
                _media_wrapper = null;
            }
        }

        private void Render()
        {
            long fps = 0;
            bool show_fps = false;

            if (IsDisposed || _media_wrapper == null)
                return;

            lock (_render_locker)
            {

                bool rendered_frame = (bool) _media_wrapper.RenderScene((int) SizeMode, _force_render);

                _force_render = false;

                if (_render_mode == MV_RenderMode.Immediate)
                    System.Threading.Thread.Sleep(1);

                if (ShowFPS && rendered_frame)
                {
                    _frameCount++;

                    if (_clock != null && _clock.ElapsedMilliseconds >= 1000)
                    {

                        fps = _frameCount;
                        show_fps = true;

                        _frameCount = 0;
                        _clock.Restart();
                    }

                }
            }

            if (MediaState != _lastState)
            {
                MV_MediaStateChangedEventArgs args = new MV_MediaStateChangedEventArgs();
                args.State = MediaState;

                EventItem item = new EventItem();
                item.eventData = args;
                item.eventType = 0; //media state changed

                lock(_events_lock)
                {
                    _events.Enqueue(item);
                }

                _lastState = args.State;
            }

            if (show_fps)
            {
                EventItem item = new EventItem();
                item.eventData = fps;
                item.eventType = 1; //fps counter

                lock (_events_lock)
                {
                    _events.Enqueue(item);
                }
                
            }

            if (Math.Abs(MediaPosition - _last_position_value) > PositionChangedStep)
            {
                double position = MediaPosition;

                MV_MediaPositionChangedEventArgs args = new MV_MediaPositionChangedEventArgs();
                args.Position = position;

                EventItem item = new EventItem();
                item.eventData = args;
                item.eventType = 2; //media position changed

                lock (_events_lock)
                {
                    _events.Enqueue(item);
                }

                _last_position_value = position;
            }

        }

        private void DeInstallRenderLoop()
        {
            _signal_render_finish = true;

            if (IsRenderLoopInitialized)
                _resetEvent.WaitOne();

            if (_renderThread != null)
            {
                _renderThread.Dispose();
                _renderThread = null;
            }

            if (IsInDesignMode)
                return;

            IsRenderLoopInitialized = false;

            if (_clock != null)
            {
                _clock.Stop();
                _clock = null;
            }
        }

        private void InstallInRenderLoop()
        {
            _signal_render_finish = false;

            _renderThread = new BackgroundWorker();

            _renderThread.DoWork += (sender, e) =>
             {
                 while (!_signal_render_finish)
                 {
                     Render();

                     System.Threading.Thread.Sleep(0);
                 }

                 _resetEvent.Set();
             };

            IsRenderLoopInitialized = true;

            _renderThread.RunWorkerAsync();

        }

        #endregion

        #region overrides

        protected override void OnControlAdded(ControlEventArgs e)
        {
            lock (_render_locker)
            {
                base.OnControlAdded(e);
            }
        }

        protected override void OnControlRemoved(ControlEventArgs e)
        {
            lock (_render_locker)
            {
                base.OnControlRemoved(e);
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            eventsDispatcher.Enabled = false;
            eventsDispatcher.Dispose();
            eventsDispatcher = null;

            DeInstallRenderLoop();

            _events.Clear();

            ReleaseNatives();

            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        protected override void OnResize(EventArgs e)
        {
            lock (_render_locker)
            {
                //in case immediate mode we have to refresh d3d backbuffer
                //also it avoid mem leaks in immediate mode
                _force_render = true;
                base.OnResize(e);
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            lock (_render_locker)
            {
                base.OnSizeChanged(e);
            }
        }
        #endregion

        #region private events

        private void eventsDispatcher_Tick(object sender, EventArgs e)
        {
            List<EventItem> items = new List<EventItem>();

            lock(_events_lock)
            {
                while (_events.Count > 0)
                    items.Add(_events.Dequeue());
            }

            //dispatch events
            foreach (EventItem i in items)
            {
                //media changed
                if (i.eventType == 0)
                {
                    MV_MediaStateChangedEventArgs args = (MV_MediaStateChangedEventArgs)i.eventData;

                    if (MediaStateChanged != null)
                        MediaStateChanged(this, args);

                    continue;
                }

                //fps update
                if (i.eventType == 1)
                {
                    Form parent = this.TopLevelControl as Form;

                    if (parent != null)
                    {
                        parent.Text = string.Format("{0:F2} FPS ", i.eventData);
                        parent = null;
                    }

                    continue;
                }

                //position changed
                if (i.eventType == 2)
                {
                    MV_MediaPositionChangedEventArgs args = (MV_MediaPositionChangedEventArgs)i.eventData;

                    if (MediaPositionChanged != null)
                        MediaPositionChanged(this, args);

                    continue;
                }
            }

            items.Clear();
        }

        #endregion
    }
}
