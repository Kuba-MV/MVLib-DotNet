namespace MV.WinForms.PlayerSample
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnOpenMedia = new System.Windows.Forms.Button();
            this.btnPlay = new System.Windows.Forms.Button();
            this.pnDown = new System.Windows.Forms.Panel();
            this.lblMediaSizeData = new System.Windows.Forms.Label();
            this.lblMediaSize = new System.Windows.Forms.Label();
            this.cmbRenderMode = new System.Windows.Forms.ComboBox();
            this.lblRenderMode = new System.Windows.Forms.Label();
            this.cmbSizeMode = new System.Windows.Forms.ComboBox();
            this.lblSizeMode = new System.Windows.Forms.Label();
            this.lblPositionData = new System.Windows.Forms.Label();
            this.lblPosition = new System.Windows.Forms.Label();
            this.cmbDecodingType = new System.Windows.Forms.ComboBox();
            this.lblDecodingType = new System.Windows.Forms.Label();
            this.lblPlayerState = new System.Windows.Forms.Label();
            this.lblState = new System.Windows.Forms.Label();
            this.lblVolume = new System.Windows.Forms.Label();
            this.tbVolume = new System.Windows.Forms.TrackBar();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnPause = new System.Windows.Forms.Button();
            this.pnMiddle = new System.Windows.Forms.Panel();
            this.mvPlayer1 = new MV.DotNet.WinForms.MVPlayer.MVPlayer();
            this.pnSlider = new System.Windows.Forms.Panel();
            this.tbPosition = new System.Windows.Forms.TrackBar();
            this.btnOpenSubtitles = new System.Windows.Forms.Button();
            this.pnDown.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbVolume)).BeginInit();
            this.pnMiddle.SuspendLayout();
            this.pnSlider.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbPosition)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOpenMedia
            // 
            this.btnOpenMedia.Location = new System.Drawing.Point(10, 17);
            this.btnOpenMedia.Margin = new System.Windows.Forms.Padding(1);
            this.btnOpenMedia.Name = "btnOpenMedia";
            this.btnOpenMedia.Size = new System.Drawing.Size(80, 41);
            this.btnOpenMedia.TabIndex = 1;
            this.btnOpenMedia.Text = "OpenMedia";
            this.btnOpenMedia.UseVisualStyleBackColor = true;
            this.btnOpenMedia.Click += new System.EventHandler(this.btnOpenMedia_Click);
            // 
            // btnPlay
            // 
            this.btnPlay.Location = new System.Drawing.Point(103, 17);
            this.btnPlay.Margin = new System.Windows.Forms.Padding(1);
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(93, 41);
            this.btnPlay.TabIndex = 2;
            this.btnPlay.Text = "Play";
            this.btnPlay.UseVisualStyleBackColor = true;
            this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);
            // 
            // pnDown
            // 
            this.pnDown.Controls.Add(this.btnOpenSubtitles);
            this.pnDown.Controls.Add(this.lblMediaSizeData);
            this.pnDown.Controls.Add(this.lblMediaSize);
            this.pnDown.Controls.Add(this.cmbRenderMode);
            this.pnDown.Controls.Add(this.lblRenderMode);
            this.pnDown.Controls.Add(this.cmbSizeMode);
            this.pnDown.Controls.Add(this.lblSizeMode);
            this.pnDown.Controls.Add(this.lblPositionData);
            this.pnDown.Controls.Add(this.lblPosition);
            this.pnDown.Controls.Add(this.cmbDecodingType);
            this.pnDown.Controls.Add(this.lblDecodingType);
            this.pnDown.Controls.Add(this.lblPlayerState);
            this.pnDown.Controls.Add(this.lblState);
            this.pnDown.Controls.Add(this.lblVolume);
            this.pnDown.Controls.Add(this.tbVolume);
            this.pnDown.Controls.Add(this.btnClose);
            this.pnDown.Controls.Add(this.btnStop);
            this.pnDown.Controls.Add(this.btnOpenMedia);
            this.pnDown.Controls.Add(this.btnPause);
            this.pnDown.Controls.Add(this.btnPlay);
            this.pnDown.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnDown.Location = new System.Drawing.Point(0, 530);
            this.pnDown.Name = "pnDown";
            this.pnDown.Size = new System.Drawing.Size(1264, 151);
            this.pnDown.TabIndex = 4;
            // 
            // lblMediaSizeData
            // 
            this.lblMediaSizeData.AutoSize = true;
            this.lblMediaSizeData.Location = new System.Drawing.Point(73, 114);
            this.lblMediaSizeData.Name = "lblMediaSizeData";
            this.lblMediaSizeData.Size = new System.Drawing.Size(0, 13);
            this.lblMediaSizeData.TabIndex = 21;
            // 
            // lblMediaSize
            // 
            this.lblMediaSize.AutoSize = true;
            this.lblMediaSize.Location = new System.Drawing.Point(13, 114);
            this.lblMediaSize.Name = "lblMediaSize";
            this.lblMediaSize.Size = new System.Drawing.Size(60, 13);
            this.lblMediaSize.TabIndex = 20;
            this.lblMediaSize.Text = "Media size:";
            this.lblMediaSize.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cmbRenderMode
            // 
            this.cmbRenderMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRenderMode.FormattingEnabled = true;
            this.cmbRenderMode.Location = new System.Drawing.Point(546, 111);
            this.cmbRenderMode.Name = "cmbRenderMode";
            this.cmbRenderMode.Size = new System.Drawing.Size(142, 21);
            this.cmbRenderMode.TabIndex = 19;
            this.cmbRenderMode.SelectedIndexChanged += new System.EventHandler(this.cmbRenderMode_SelectedIndexChanged);
            // 
            // lblRenderMode
            // 
            this.lblRenderMode.AutoSize = true;
            this.lblRenderMode.Location = new System.Drawing.Point(455, 114);
            this.lblRenderMode.Name = "lblRenderMode";
            this.lblRenderMode.Size = new System.Drawing.Size(69, 13);
            this.lblRenderMode.TabIndex = 18;
            this.lblRenderMode.Text = "RenderMode";
            // 
            // cmbSizeMode
            // 
            this.cmbSizeMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSizeMode.FormattingEnabled = true;
            this.cmbSizeMode.Location = new System.Drawing.Point(302, 111);
            this.cmbSizeMode.Name = "cmbSizeMode";
            this.cmbSizeMode.Size = new System.Drawing.Size(142, 21);
            this.cmbSizeMode.TabIndex = 17;
            this.cmbSizeMode.SelectedIndexChanged += new System.EventHandler(this.cmbSizeMode_SelectedIndexChanged);
            // 
            // lblSizeMode
            // 
            this.lblSizeMode.AutoSize = true;
            this.lblSizeMode.Location = new System.Drawing.Point(213, 114);
            this.lblSizeMode.Name = "lblSizeMode";
            this.lblSizeMode.Size = new System.Drawing.Size(54, 13);
            this.lblSizeMode.TabIndex = 16;
            this.lblSizeMode.Text = "SizeMode";
            // 
            // lblPositionData
            // 
            this.lblPositionData.AutoSize = true;
            this.lblPositionData.Location = new System.Drawing.Point(609, 65);
            this.lblPositionData.Name = "lblPositionData";
            this.lblPositionData.Size = new System.Drawing.Size(0, 13);
            this.lblPositionData.TabIndex = 15;
            // 
            // lblPosition
            // 
            this.lblPosition.AutoSize = true;
            this.lblPosition.Location = new System.Drawing.Point(555, 66);
            this.lblPosition.Name = "lblPosition";
            this.lblPosition.Size = new System.Drawing.Size(47, 13);
            this.lblPosition.TabIndex = 14;
            this.lblPosition.Text = "Position:";
            // 
            // cmbDecodingType
            // 
            this.cmbDecodingType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDecodingType.FormattingEnabled = true;
            this.cmbDecodingType.Location = new System.Drawing.Point(302, 80);
            this.cmbDecodingType.Name = "cmbDecodingType";
            this.cmbDecodingType.Size = new System.Drawing.Size(142, 21);
            this.cmbDecodingType.TabIndex = 11;
            this.cmbDecodingType.SelectedIndexChanged += new System.EventHandler(this.cmbDecodingType_SelectedIndexChanged);
            // 
            // lblDecodingType
            // 
            this.lblDecodingType.AutoSize = true;
            this.lblDecodingType.Location = new System.Drawing.Point(213, 83);
            this.lblDecodingType.Name = "lblDecodingType";
            this.lblDecodingType.Size = new System.Drawing.Size(83, 13);
            this.lblDecodingType.TabIndex = 10;
            this.lblDecodingType.Text = "Decoding Type:";
            // 
            // lblPlayerState
            // 
            this.lblPlayerState.AutoSize = true;
            this.lblPlayerState.Location = new System.Drawing.Point(86, 83);
            this.lblPlayerState.Name = "lblPlayerState";
            this.lblPlayerState.Size = new System.Drawing.Size(0, 13);
            this.lblPlayerState.TabIndex = 9;
            // 
            // lblState
            // 
            this.lblState.AutoSize = true;
            this.lblState.Location = new System.Drawing.Point(13, 83);
            this.lblState.Name = "lblState";
            this.lblState.Size = new System.Drawing.Size(67, 13);
            this.lblState.TabIndex = 8;
            this.lblState.Text = "Player State:";
            // 
            // lblVolume
            // 
            this.lblVolume.AutoSize = true;
            this.lblVolume.Location = new System.Drawing.Point(646, 31);
            this.lblVolume.Name = "lblVolume";
            this.lblVolume.Size = new System.Drawing.Size(42, 13);
            this.lblVolume.TabIndex = 7;
            this.lblVolume.Text = "Volume";
            // 
            // tbVolume
            // 
            this.tbVolume.Location = new System.Drawing.Point(694, 17);
            this.tbVolume.Maximum = 100;
            this.tbVolume.Name = "tbVolume";
            this.tbVolume.Size = new System.Drawing.Size(152, 45);
            this.tbVolume.TabIndex = 6;
            this.tbVolume.ValueChanged += new System.EventHandler(this.tbVolume_ValueChanged);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(431, 17);
            this.btnClose.Margin = new System.Windows.Forms.Padding(1);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(93, 41);
            this.btnClose.TabIndex = 5;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(324, 17);
            this.btnStop.Margin = new System.Windows.Forms.Padding(1);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(93, 41);
            this.btnStop.TabIndex = 4;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnPause
            // 
            this.btnPause.Location = new System.Drawing.Point(216, 17);
            this.btnPause.Margin = new System.Windows.Forms.Padding(1);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(93, 41);
            this.btnPause.TabIndex = 3;
            this.btnPause.Text = "Pause";
            this.btnPause.UseVisualStyleBackColor = true;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // pnMiddle
            // 
            this.pnMiddle.Controls.Add(this.mvPlayer1);
            this.pnMiddle.Controls.Add(this.pnSlider);
            this.pnMiddle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnMiddle.Location = new System.Drawing.Point(0, 0);
            this.pnMiddle.Name = "pnMiddle";
            this.pnMiddle.Size = new System.Drawing.Size(1264, 530);
            this.pnMiddle.TabIndex = 5;
            // 
            // mvPlayer1
            // 
            this.mvPlayer1.BackColor = System.Drawing.Color.Black;
            this.mvPlayer1.DecodingType = MV.DotNet.Common.MV_DecodingTypeEnum.MV_Auto;
            this.mvPlayer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mvPlayer1.Location = new System.Drawing.Point(0, 0);
            this.mvPlayer1.MediaPosition = 0D;
            this.mvPlayer1.Name = "mvPlayer1";
            this.mvPlayer1.PositionChangedStep = 1D;
            this.mvPlayer1.RenderMode = MV.DotNet.Common.MV_RenderMode.Composition;
            this.mvPlayer1.ShowFPS = false;
            this.mvPlayer1.Size = new System.Drawing.Size(1264, 491);
            this.mvPlayer1.SizeMode = MV.DotNet.Common.MV_SizeMode.Stretch;
            this.mvPlayer1.TabIndex = 3;
            this.mvPlayer1.Volume = 0F;
            this.mvPlayer1.DoubleClick += new System.EventHandler(this.mvPlayer1_DoubleClick);
            // 
            // pnSlider
            // 
            this.pnSlider.Controls.Add(this.tbPosition);
            this.pnSlider.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnSlider.Location = new System.Drawing.Point(0, 491);
            this.pnSlider.Name = "pnSlider";
            this.pnSlider.Size = new System.Drawing.Size(1264, 39);
            this.pnSlider.TabIndex = 4;
            // 
            // tbPosition
            // 
            this.tbPosition.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbPosition.Enabled = false;
            this.tbPosition.Location = new System.Drawing.Point(0, 0);
            this.tbPosition.Name = "tbPosition";
            this.tbPosition.Size = new System.Drawing.Size(1264, 39);
            this.tbPosition.SmallChange = 50;
            this.tbPosition.TabIndex = 0;
            this.tbPosition.Scroll += new System.EventHandler(this.tbPosition_Scroll);
            // 
            // btnOpenSubtitles
            // 
            this.btnOpenSubtitles.Location = new System.Drawing.Point(546, 17);
            this.btnOpenSubtitles.Margin = new System.Windows.Forms.Padding(1);
            this.btnOpenSubtitles.Name = "btnOpenSubtitles";
            this.btnOpenSubtitles.Size = new System.Drawing.Size(93, 41);
            this.btnOpenSubtitles.TabIndex = 22;
            this.btnOpenSubtitles.Text = "Open subtitles";
            this.btnOpenSubtitles.UseVisualStyleBackColor = true;
            this.btnOpenSubtitles.Click += new System.EventHandler(this.btnOpenSubtitles_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 681);
            this.Controls.Add(this.pnMiddle);
            this.Controls.Add(this.pnDown);
            this.Margin = new System.Windows.Forms.Padding(1);
            this.Name = "frmMain";
            this.Text = "MV WinForms Dotnet Player";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.pnDown.ResumeLayout(false);
            this.pnDown.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbVolume)).EndInit();
            this.pnMiddle.ResumeLayout(false);
            this.pnSlider.ResumeLayout(false);
            this.pnSlider.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbPosition)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnOpenMedia;
        private System.Windows.Forms.Button btnPlay;
        private MV.DotNet.WinForms.MVPlayer.MVPlayer mvPlayer1;
        private System.Windows.Forms.Panel pnDown;
        private System.Windows.Forms.Panel pnMiddle;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Panel pnSlider;
        private System.Windows.Forms.TrackBar tbPosition;
        private System.Windows.Forms.Label lblVolume;
        private System.Windows.Forms.TrackBar tbVolume;
        private System.Windows.Forms.Label lblState;
        private System.Windows.Forms.Label lblPlayerState;
        private System.Windows.Forms.ComboBox cmbDecodingType;
        private System.Windows.Forms.Label lblDecodingType;
        private System.Windows.Forms.Label lblPositionData;
        private System.Windows.Forms.Label lblPosition;
        private System.Windows.Forms.ComboBox cmbSizeMode;
        private System.Windows.Forms.Label lblSizeMode;
        private System.Windows.Forms.ComboBox cmbRenderMode;
        private System.Windows.Forms.Label lblRenderMode;
        private System.Windows.Forms.Label lblMediaSizeData;
        private System.Windows.Forms.Label lblMediaSize;
        private System.Windows.Forms.Button btnOpenSubtitles;
    }
}

