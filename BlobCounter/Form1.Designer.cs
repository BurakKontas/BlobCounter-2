namespace BlobCounter
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.Stage1_Button = new System.Windows.Forms.Button();
            this.Upload_Button = new System.Windows.Forms.Button();
            this.threshold_1 = new System.Windows.Forms.NumericUpDown();
            this.threshold_2 = new System.Windows.Forms.NumericUpDown();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.threshold_4 = new System.Windows.Forms.NumericUpDown();
            this.threshold_3 = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.threshold_1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.threshold_2)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.threshold_4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.threshold_3)).BeginInit();
            this.SuspendLayout();
            // 
            // Stage1_Button
            // 
            this.Stage1_Button.Location = new System.Drawing.Point(12, 13);
            this.Stage1_Button.Name = "Stage1_Button";
            this.Stage1_Button.Size = new System.Drawing.Size(75, 23);
            this.Stage1_Button.TabIndex = 1;
            this.Stage1_Button.Text = "Stage1";
            this.Stage1_Button.UseVisualStyleBackColor = true;
            this.Stage1_Button.Click += new System.EventHandler(this.Stage1_Button_Click);
            // 
            // Upload_Button
            // 
            this.Upload_Button.Location = new System.Drawing.Point(853, 9);
            this.Upload_Button.Name = "Upload_Button";
            this.Upload_Button.Size = new System.Drawing.Size(75, 23);
            this.Upload_Button.TabIndex = 2;
            this.Upload_Button.Text = "Upload";
            this.Upload_Button.UseVisualStyleBackColor = true;
            this.Upload_Button.Click += new System.EventHandler(this.Upload_Button_Click);
            // 
            // threshold_1
            // 
            this.threshold_1.Location = new System.Drawing.Point(265, 12);
            this.threshold_1.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.threshold_1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.threshold_1.Name = "threshold_1";
            this.threshold_1.Size = new System.Drawing.Size(120, 20);
            this.threshold_1.TabIndex = 3;
            this.threshold_1.Value = new decimal(new int[] {
            185,
            0,
            0,
            0});
            // 
            // threshold_2
            // 
            this.threshold_2.Location = new System.Drawing.Point(391, 12);
            this.threshold_2.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.threshold_2.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.threshold_2.Name = "threshold_2";
            this.threshold_2.Size = new System.Drawing.Size(120, 20);
            this.threshold_2.TabIndex = 4;
            this.threshold_2.Value = new decimal(new int[] {
            110,
            0,
            0,
            0});
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.pictureBox2);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Location = new System.Drawing.Point(13, 43);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(910, 206);
            this.panel1.TabIndex = 5;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox2.Location = new System.Drawing.Point(6474, 0);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(301, 271);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox2.TabIndex = 1;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(3, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1900, 128);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // threshold_4
            // 
            this.threshold_4.Location = new System.Drawing.Point(643, 12);
            this.threshold_4.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.threshold_4.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.threshold_4.Name = "threshold_4";
            this.threshold_4.Size = new System.Drawing.Size(120, 20);
            this.threshold_4.TabIndex = 6;
            this.threshold_4.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // threshold_3
            // 
            this.threshold_3.Location = new System.Drawing.Point(517, 12);
            this.threshold_3.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.threshold_3.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.threshold_3.Name = "threshold_3";
            this.threshold_3.Size = new System.Drawing.Size(120, 20);
            this.threshold_3.TabIndex = 7;
            this.threshold_3.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(935, 261);
            this.Controls.Add(this.threshold_3);
            this.Controls.Add(this.threshold_4);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.threshold_2);
            this.Controls.Add(this.threshold_1);
            this.Controls.Add(this.Upload_Button);
            this.Controls.Add(this.Stage1_Button);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.threshold_1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.threshold_2)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.threshold_4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.threshold_3)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button Stage1_Button;
        private System.Windows.Forms.Button Upload_Button;
        private System.Windows.Forms.NumericUpDown threshold_1;
        private System.Windows.Forms.NumericUpDown threshold_2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.NumericUpDown threshold_4;
        private System.Windows.Forms.NumericUpDown threshold_3;
    }
}

