namespace Wnacg閱讀器
{
    partial class ShowBigPicture
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.txt_page = new System.Windows.Forms.TextBox();
            this.btn_nextView = new System.Windows.Forms.Button();
            this.btn_preView = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(565, 653);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // txt_page
            // 
            this.txt_page.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.txt_page.Location = new System.Drawing.Point(232, 662);
            this.txt_page.Name = "txt_page";
            this.txt_page.ReadOnly = true;
            this.txt_page.Size = new System.Drawing.Size(100, 22);
            this.txt_page.TabIndex = 1;
            this.txt_page.Text = "0/0";
            this.txt_page.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btn_nextView
            // 
            this.btn_nextView.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btn_nextView.Location = new System.Drawing.Point(363, 662);
            this.btn_nextView.Name = "btn_nextView";
            this.btn_nextView.Size = new System.Drawing.Size(75, 23);
            this.btn_nextView.TabIndex = 2;
            this.btn_nextView.Text = "下一頁";
            this.btn_nextView.UseVisualStyleBackColor = true;
            this.btn_nextView.Click += new System.EventHandler(this.btn_Click);
            // 
            // btn_preView
            // 
            this.btn_preView.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btn_preView.Location = new System.Drawing.Point(126, 662);
            this.btn_preView.Name = "btn_preView";
            this.btn_preView.Size = new System.Drawing.Size(75, 23);
            this.btn_preView.TabIndex = 4;
            this.btn_preView.Text = "上一頁";
            this.btn_preView.UseVisualStyleBackColor = true;
            this.btn_preView.Click += new System.EventHandler(this.btn_Click);
            // 
            // ShowBigPicture
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(565, 694);
            this.Controls.Add(this.btn_preView);
            this.Controls.Add(this.btn_nextView);
            this.Controls.Add(this.txt_page);
            this.Controls.Add(this.pictureBox1);
            this.Name = "ShowBigPicture";
            this.ShowIcon = false;
            this.Text = "載入中...";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TextBox txt_page;
        private System.Windows.Forms.Button btn_nextView;
        private System.Windows.Forms.Button btn_preView;
    }
}