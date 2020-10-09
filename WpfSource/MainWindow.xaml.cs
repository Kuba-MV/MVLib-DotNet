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
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;

using MV.DotNet.Common;

namespace MV.WPF.PlayerSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool setupSlider = false;

        private DateTime lastSeekOperation = DateTime.Now;

        bool _hold_position_updated = true;

        private string currentMediaFile = string.Empty;

        public MainWindow()
        {
            InitializeComponent();

            mvPlayer.ShowFPS = true;

            sliderVolume.Value = (double)mvPlayer.Volume * 100.0f;


            txtPlayerState.Text = mvPlayer.MediaState.ToString();

            mvPlayer.MediaStateChanged += MvPlayer_MediaStateChanged;

            mvPlayer.MediaPositionChanged += MvPlayer_MediaPositionChanged;

            cmbDecodingType.ItemsSource = Enum.GetValues(typeof(MV_DecodingTypeEnum)).Cast<MV_DecodingTypeEnum>();
            cmbDecodingType.SelectedIndex = 0;

            cmbSizeMode.ItemsSource = Enum.GetNames(typeof(MV_SizeMode));
            cmbSizeMode.SelectedIndex = 0;

            mvPlayer.SubtitleChanged += MvPlayer_SubtitleChanged;
        }

        private void MvPlayer_SubtitleChanged(object sender, MV_SubtitleEventArgs e)
        {
            ClearSubtitleLines();

            if (e.Lines == null || e.Lines.Count == 0)
                return;

            //not elegant solution to set subtitle lines
            //but its only example
            //in true environment it should be more flexible collection management with visual items

            int idx = e.Lines.Count - 1;

            if (e.Lines.Count > 0)
            {
                subLine1.Text = e.Lines[idx].Line;

                if (e.Lines[idx].isBold)
                    subLine1.FontWeight = FontWeights.Bold;

                if (e.Lines[idx].isItalic)
                    subLine1.FontStyle = FontStyles.Italic;

                idx--;
            }

            if (e.Lines.Count > 1)
            {
                subLine2.Text = e.Lines[idx].Line;

                if (e.Lines[idx].isBold)
                    subLine2.FontWeight = FontWeights.Bold;

                if (e.Lines[idx].isItalic)
                    subLine2.FontStyle = FontStyles.Italic;

                idx--;
            }

            if (e.Lines.Count > 2)
            {
                subLine3.Text = e.Lines[idx].Line;

                if (e.Lines[idx].isBold)
                    subLine3.FontWeight = FontWeights.Bold;

                if (e.Lines[idx].isItalic)
                    subLine3.FontStyle = FontStyles.Italic;

                idx--;
            }

            if (e.Lines.Count > 3)
            {
                subLine4.Text = e.Lines[idx].Line;

                if (e.Lines[idx].isBold)
                    subLine4.FontWeight = FontWeights.Bold;

                if (e.Lines[idx].isItalic)
                    subLine4.FontStyle = FontStyles.Italic;
            }

        }

        private void MvPlayer_MediaPositionChanged(object sender, MV_MediaPositionChangedEventArgs e)
        {
            TimeSpan position_t = TimeSpan.FromSeconds(mvPlayer.MediaPosition);
            TimeSpan duration_t = TimeSpan.FromSeconds(mvPlayer.MediaDuration);

            int position = (int)e.Position;

            if (position > mvPlayer.MediaDuration)
                position = (int)Math.Floor(mvPlayer.MediaDuration);

            if (position < 0)
                position = 0;

            //hold slider update if user can still seek
            if (DateTime.Now.Subtract(lastSeekOperation).TotalSeconds > 2)
                sliderPosition.Value = position;

            txtPosition.Text = string.Format("{0:00}:{1:00}:{2:00} / {3:00}:{4:00}:{5:00}", position_t.Hours, position_t.Minutes, position_t.Seconds, duration_t.Hours, duration_t.Minutes, duration_t.Seconds);
        }

        private void MvPlayer_MediaStateChanged(object sender, MV_MediaStateChangedEventArgs e)
        {
            //check if we are in MVAuto mode
            //if so, we can use fallback if hw decoding will fail
            if (mvPlayer.MediaState == MV_PlayerStateEnum.Error && mvPlayer.DecodingType == MV_DecodingTypeEnum.MV_Auto)
            {
                //close failed source
                mvPlayer.Close();

                //change decoding type into sw yuv decoding
                //mvPlayer1.DecodingType = MV_DecodingTypeEnum.MV_YUV420P;
                cmbDecodingType.SelectedIndex = (int)MV_DecodingTypeEnum.MV_YUV420P;

                mvPlayer.OpenMedia(currentMediaFile);
            }

            txtPlayerState.Text = mvPlayer.MediaState.ToString();

            if (e.State != MV_PlayerStateEnum.Error &&
                e.State != MV_PlayerStateEnum.NotInitialized &&
                e.State != MV_PlayerStateEnum.Closed &&
                e.State != MV_PlayerStateEnum.Loading)
            {
                sliderPosition.IsEnabled = true;

                if (setupSlider)
                {
                    setupSlider = false;

                    TimeSpan position_t = TimeSpan.FromSeconds(mvPlayer.MediaPosition);
                    TimeSpan duration_t = TimeSpan.FromSeconds(mvPlayer.MediaDuration);

                    sliderPosition.Value = 0;
                    sliderPosition.Maximum = (int)Math.Floor(mvPlayer.MediaDuration);
                    txtPosition.Text = string.Format("{0:00}:{1:00}:{2:00} / {3:00}:{4:00}:{5:00}", position_t.Hours, position_t.Minutes, position_t.Seconds, duration_t.Hours, duration_t.Minutes, duration_t.Seconds);

                    txtMediaSize.Text = string.Format("Width: {0} Height: {1}", mvPlayer.VideoWidth, mvPlayer.VideoHeight);

                }
            }
            else
            {
                sliderPosition.IsEnabled = false;

            }

            if (e.State == MV_PlayerStateEnum.Seeking)
                lastSeekOperation = DateTime.Now;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            mvPlayer.Release();
        }

        private void BtnOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            Nullable<bool> result = ofd.ShowDialog();

            if (result == true)
            {
                currentMediaFile = ofd.FileName;
                mvPlayer.OpenMedia(ofd.FileName);
            }

            setupSlider = true;
        }

        private void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            mvPlayer.Play();
        }

        private void BtnPause_Click(object sender, RoutedEventArgs e)
        {
            mvPlayer.Pause();
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            mvPlayer.Stop();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            mvPlayer.Close();
            GC.Collect();
        }

        private void CmbDecodingType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            mvPlayer.DecodingType = (MV_DecodingTypeEnum)cmbDecodingType.SelectedIndex;
        }

        private void MvPlayer_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (this.WindowStyle == WindowStyle.SingleBorderWindow)
            {
                this.WindowStyle = WindowStyle.None;
                this.WindowState = WindowState.Maximized;
                grdMain.RowDefinitions[1].Height = new GridLength(0);
                grdMain.RowDefinitions[2].Height = new GridLength(0);
                grdMain.RowDefinitions[3].Height = new GridLength(0);

            }
            else
            {
                this.WindowStyle = WindowStyle.SingleBorderWindow;
                this.WindowState = WindowState.Normal;
                grdMain.RowDefinitions[1].Height = new GridLength(50);
                grdMain.RowDefinitions[2].Height = new GridLength(50);
                grdMain.RowDefinitions[3].Height = new GridLength(50);
            }
        }

        private void SliderVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mvPlayer.Volume = (float)sliderVolume.Value / 100.0f;
        }

        private void SliderPosition_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            _hold_position_updated = true;
        }

        private void SliderPosition_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!_hold_position_updated)
                mvPlayer.MediaPosition = sliderPosition.Value;
        }

        private void SliderPosition_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            _hold_position_updated = false;
        }

        private void CmbSizeMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            mvPlayer.SizeMode = (MV_SizeMode)cmbSizeMode.SelectedIndex;
        }

        private void BtnOpenSUb_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            Nullable<bool> result = ofd.ShowDialog();

            if (result == true)
            {
                mvPlayer.OpenSubtitles(ofd.FileName);
            }
        }

        private void ClearSubtitleLines()
        {
            subLine1.Text = string.Empty;
            subLine2.Text = string.Empty;
            subLine3.Text = string.Empty;
            subLine4.Text = string.Empty;

            subLine1.FontStyle = FontStyles.Normal;
            subLine2.FontStyle = FontStyles.Normal;
            subLine3.FontStyle = FontStyles.Normal;
            subLine4.FontStyle = FontStyles.Normal;

            subLine1.FontWeight = FontWeights.Normal;
            subLine2.FontWeight = FontWeights.Normal;
            subLine3.FontWeight = FontWeights.Normal;
            subLine4.FontWeight = FontWeights.Normal;
        }
    }
}
