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
using System.Windows.Forms;
using System.Drawing;

using MV.DotNet.Common;

namespace MV.WinForms.PlayerSample
{
    public partial class frmMain : Form
    {
        bool setupSlider = false;
        DateTime lastSeekOperation = DateTime.Now;

        private string currentFileName = string.Empty;

        public frmMain()
        {
            InitializeComponent();

            //set initial values and ui states

            //mvPlayer1.ShowFPS = true;
            mvPlayer1.Volume = 0.2f;
            mvPlayer1.PositionChangedStep = 0.1f;

            tbVolume.Value = (int) (mvPlayer1.Volume * 100.0f);

            lblPlayerState.Text = mvPlayer1.MediaState.ToString();

            mvPlayer1.MediaStateChanged += MvPlayer1_MediaStateChanged;
            mvPlayer1.MediaPositionChanged += MvPlayer1_MediaPositionChanged;

            cmbDecodingType.DataSource = Enum.GetNames(typeof(MV_DecodingTypeEnum));
            cmbDecodingType.SelectedIndex = 0;

            cmbSizeMode.DataSource = Enum.GetNames(typeof(MV_SizeMode));
            cmbSizeMode.SelectedIndex = 0;

            cmbRenderMode.DataSource = Enum.GetNames(typeof(MV_RenderMode));
            cmbRenderMode.SelectedIndex = 0;

            mvPlayer1.VideoPanel.DoubleClick += mvPlayer1_DoubleClick;
        }

        private void MvPlayer1_MediaPositionChanged(object sender, MV_MediaPositionChangedEventArgs e)
        {
            TimeSpan position_t = TimeSpan.FromSeconds(mvPlayer1.MediaPosition);
            TimeSpan duration_t = TimeSpan.FromSeconds(mvPlayer1.MediaDuration);

            int position = (int)e.Position;

            if (position > mvPlayer1.MediaDuration)
                position = (int) Math.Floor(mvPlayer1.MediaDuration);

            if (position < 0)
                position = 0;

            //hold slider update if user can still seek
            if (DateTime.Now.Subtract(lastSeekOperation).TotalSeconds > 2)
                tbPosition.Value = position;

            lblPositionData.Text = string.Format("{0:00}:{1:00}:{2:00} / {3:00}:{4:00}:{5:00}", position_t.Hours, position_t.Minutes, position_t.Seconds, duration_t.Hours, duration_t.Minutes, duration_t.Seconds);
        }

        private void MvPlayer1_MediaStateChanged(object sender, MV_MediaStateChangedEventArgs e)
        {
            //check if we are in MVAuto mode
            //if so, we can use fallback if hw decoding will fail
            if (mvPlayer1.MediaState == MV_PlayerStateEnum.Error && mvPlayer1.DecodingType == MV_DecodingTypeEnum.MV_Auto)
            {
                //close failed source
                mvPlayer1.Close();

                //change decoding type into sw yuv decoding
                //mvPlayer1.DecodingType = MV_DecodingTypeEnum.MV_YUV420P;
                cmbDecodingType.SelectedIndex = (int)MV_DecodingTypeEnum.MV_YUV420P;

                mvPlayer1.OpenMediaAsync(currentFileName);
            }

            lblPlayerState.Text = mvPlayer1.MediaState.ToString();

            if (e.State != MV_PlayerStateEnum.Error &&
                e.State != MV_PlayerStateEnum.NotInitialized &&
                e.State != MV_PlayerStateEnum.Closed &&
                e.State != MV_PlayerStateEnum.Loading)
            {
                tbPosition.Enabled = true;

                //prepare slider if media are ready
                if (setupSlider)
                {
                    setupSlider = false;

                    TimeSpan position_t = TimeSpan.FromSeconds(mvPlayer1.MediaPosition);
                    TimeSpan duration_t = TimeSpan.FromSeconds(mvPlayer1.MediaDuration);

                    tbPosition.Value = 0;
                    tbPosition.Maximum = (int) Math.Floor(mvPlayer1.MediaDuration);
                    lblPositionData.Text = string.Format("{0:00}:{1:00}:{2:00} / {3:00}:{4:00}:{5:00}", position_t.Hours, position_t.Minutes, position_t.Seconds, duration_t.Hours, duration_t.Minutes, duration_t.Seconds);

                    lblMediaSizeData.Text = string.Format("Width: {0} Height: {1}", mvPlayer1.VideoWidth, mvPlayer1.VideoHeight);

                }
            }
            else
            {
                tbPosition.Enabled = false;
                
            }

            if (e.State == MV_PlayerStateEnum.Seeking)
                lastSeekOperation = DateTime.Now;
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
           mvPlayer1.Play();
        }

        private void btnOpenMedia_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            currentFileName = ofd.FileName;

            mvPlayer1.OpenMedia(ofd.FileName);
            ofd.Dispose();

            setupSlider = true;
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            mvPlayer1.Pause();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            mvPlayer1.Stop();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            mvPlayer1.Close();

            lblPositionData.Text = "";
            lblMediaSizeData.Text = "";
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //its good habit to call dispose to release all native resources.
            mvPlayer1.Dispose();
        }

        private void tbVolume_ValueChanged(object sender, EventArgs e)
        {
            mvPlayer1.Volume = (float)tbVolume.Value / 100.0f;
        }

        private void cmbDecodingType_SelectedIndexChanged(object sender, EventArgs e)
        {
            mvPlayer1.DecodingType = (MV_DecodingTypeEnum)cmbDecodingType.SelectedIndex;
        }

        private void tbPosition_Scroll(object sender, EventArgs e)
        {
            mvPlayer1.MediaPosition = (float)tbPosition.Value;

        }

        private void cmbSizeMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            mvPlayer1.SizeMode = (MV_SizeMode)cmbSizeMode.SelectedIndex;
        }

        private void mvPlayer1_DoubleClick(object sender, EventArgs e)
        {
            if (this.FormBorderStyle == FormBorderStyle.Sizable)
            {
                this.FormBorderStyle = FormBorderStyle.None;
                this.WindowState = FormWindowState.Maximized;
                pnDown.Visible = false;
                pnSlider.Visible = false;
            }
            else
            {
                this.FormBorderStyle = FormBorderStyle.Sizable;
                this.WindowState = FormWindowState.Normal;
                pnDown.Visible = true;
                pnSlider.Visible = true;
            }
        }

        private void cmbRenderMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            mvPlayer1.RenderMode = (MV_RenderMode)cmbRenderMode.SelectedIndex;
        }

        private void btnOpenSubtitles_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            Color fontColor = Color.White;
            Color backColor = Color.Empty;
            Color outlineColor = Color.Black;

            string fontsfolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Fonts);

            string fontPath = fontsfolder + "\\" + "arial.ttf";

            mvPlayer1.OpenSubtitles(ofd.FileName, fontPath, 38,2,30,0, fontColor, backColor, outlineColor);

            ofd.Dispose();
        }
    }
}
