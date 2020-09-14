namespace MV.DotNet.WinForms.MVPlayer
{
    partial class MVPlayer
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.eventsDispatcher = new System.Windows.Forms.Timer(this.components);
            this.pnD3D = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // eventsDispatcher
            // 
            this.eventsDispatcher.Enabled = true;
            this.eventsDispatcher.Interval = 10;
            this.eventsDispatcher.Tick += new System.EventHandler(this.eventsDispatcher_Tick);
            // 
            // pnD3D
            // 
            this.pnD3D.BackColor = System.Drawing.Color.Black;
            this.pnD3D.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnD3D.Location = new System.Drawing.Point(0, 0);
            this.pnD3D.Name = "pnD3D";
            this.pnD3D.Size = new System.Drawing.Size(563, 369);
            this.pnD3D.TabIndex = 0;
            // 
            // MVPlayer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.pnD3D);
            this.Name = "MVPlayer";
            this.Size = new System.Drawing.Size(563, 369);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer eventsDispatcher;
        private System.Windows.Forms.Panel pnD3D;
    }
}
