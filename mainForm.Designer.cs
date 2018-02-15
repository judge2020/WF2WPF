namespace Convert_to_WPF
{
    partial class mainForm
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
            this.bSourceProject = new System.Windows.Forms.Button();
            this.tbSource = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbDest = new System.Windows.Forms.TextBox();
            this.rtbOutput = new System.Windows.Forms.RichTextBox();
            this.bConvert = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // bSourceProject
            // 
            this.bSourceProject.Location = new System.Drawing.Point(759, 29);
            this.bSourceProject.Name = "bSourceProject";
            this.bSourceProject.Size = new System.Drawing.Size(75, 23);
            this.bSourceProject.TabIndex = 0;
            this.bSourceProject.Text = "Browse...";
            this.bSourceProject.UseVisualStyleBackColor = true;
            this.bSourceProject.Click += new System.EventHandler(this.bSourceProject_Click);
            // 
            // tbSource
            // 
            this.tbSource.Location = new System.Drawing.Point(12, 29);
            this.tbSource.Name = "tbSource";
            this.tbSource.Size = new System.Drawing.Size(741, 22);
            this.tbSource.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "Source Project";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(127, 17);
            this.label2.TabIndex = 5;
            this.label2.Text = "Destination Project";
            // 
            // tbDest
            // 
            this.tbDest.Location = new System.Drawing.Point(12, 74);
            this.tbDest.Name = "tbDest";
            this.tbDest.Size = new System.Drawing.Size(741, 22);
            this.tbDest.TabIndex = 4;
            // 
            // rtbOutput
            // 
            this.rtbOutput.Location = new System.Drawing.Point(12, 102);
            this.rtbOutput.Name = "rtbOutput";
            this.rtbOutput.ReadOnly = true;
            this.rtbOutput.Size = new System.Drawing.Size(741, 359);
            this.rtbOutput.TabIndex = 6;
            this.rtbOutput.Text = "";
            // 
            // bConvert
            // 
            this.bConvert.Location = new System.Drawing.Point(759, 74);
            this.bConvert.Name = "bConvert";
            this.bConvert.Size = new System.Drawing.Size(75, 23);
            this.bConvert.TabIndex = 7;
            this.bConvert.Text = "Convert";
            this.bConvert.UseVisualStyleBackColor = true;
            this.bConvert.Click += new System.EventHandler(this.bConvert_Click);
            // 
            // mainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(846, 473);
            this.Controls.Add(this.bConvert);
            this.Controls.Add(this.rtbOutput);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbDest);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbSource);
            this.Controls.Add(this.bSourceProject);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Name = "mainForm";
            this.Text = "Convert Windows Form to WPF";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bSourceProject;
        private System.Windows.Forms.TextBox tbSource;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbDest;
        private System.Windows.Forms.RichTextBox rtbOutput;
        private System.Windows.Forms.Button bConvert;
    }
}

