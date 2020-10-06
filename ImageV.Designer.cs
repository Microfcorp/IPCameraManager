namespace IPCamera
{
    partial class ImageV
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImageV));
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.button1 = new System.Windows.Forms.Button();
            this.vievImage1 = new IPCamera.VievImage();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.vievImage1)).BeginInit();
            this.SuspendLayout();
            // 
            // trackBar1
            // 
            this.trackBar1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBar1.Location = new System.Drawing.Point(4, 406);
            this.trackBar1.Maximum = 60;
            this.trackBar1.Minimum = 1;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(794, 45);
            this.trackBar1.TabIndex = 1;
            this.trackBar1.Value = 30;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(12, 370);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(780, 30);
            this.button1.TabIndex = 2;
            this.button1.Text = "Сделать снимок";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // vievImage1
            // 
            this.vievImage1.FPS = 66F;
            this.vievImage1.IsAuthorized = false;
            this.vievImage1.Location = new System.Drawing.Point(12, 12);
            this.vievImage1.Name = "vievImage1";
            this.vievImage1.Running = false;
            this.vievImage1.Size = new System.Drawing.Size(780, 352);
            this.vievImage1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.vievImage1.TabIndex = 3;
            this.vievImage1.TabStop = false;
            this.vievImage1.URL = null;
            // 
            // ImageV
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(804, 456);
            this.Controls.Add(this.vievImage1);
            this.Controls.Add(this.trackBar1);
            this.Controls.Add(this.button1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ImageV";
            this.Text = "ImageV";
            this.Load += new System.EventHandler(this.ImageV_Load);
            this.ResizeEnd += new System.EventHandler(this.ImageV_ResizeEnd);
            this.Resize += new System.EventHandler(this.ImageV_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.vievImage1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.Button button1;
        private VievImage vievImage1;
    }
}