namespace DownloadImageFromWeb
{
    partial class DownLoadImageFromWeb
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.btnDownload = new System.Windows.Forms.Button();
            this.txtWebUrl = new System.Windows.Forms.TextBox();
            this.richTextBox = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // btnDownload
            // 
            this.btnDownload.Location = new System.Drawing.Point(432, 19);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(75, 23);
            this.btnDownload.TabIndex = 0;
            this.btnDownload.Text = "开始下载";
            this.btnDownload.UseVisualStyleBackColor = true;
            this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
            // 
            // txtWebUrl
            // 
            this.txtWebUrl.Location = new System.Drawing.Point(12, 19);
            this.txtWebUrl.Name = "txtWebUrl";
            this.txtWebUrl.Size = new System.Drawing.Size(414, 21);
            this.txtWebUrl.TabIndex = 1;
            this.txtWebUrl.Text = "http://news.baidu.com";
            // 
            // richTextBox
            // 
            this.richTextBox.Location = new System.Drawing.Point(12, 63);
            this.richTextBox.Name = "richTextBox";
            this.richTextBox.Size = new System.Drawing.Size(495, 373);
            this.richTextBox.TabIndex = 2;
            this.richTextBox.Text = "";
            // 
            // DownLoadImageFromWeb
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(519, 448);
            this.Controls.Add(this.richTextBox);
            this.Controls.Add(this.txtWebUrl);
            this.Controls.Add(this.btnDownload);
            this.Name = "DownLoadImageFromWeb";
            this.Text = "DownLoadImageFromWeb";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnDownload;
        private System.Windows.Forms.TextBox txtWebUrl;
        private System.Windows.Forms.RichTextBox richTextBox;
    }
}

