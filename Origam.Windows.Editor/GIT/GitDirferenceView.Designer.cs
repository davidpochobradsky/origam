﻿namespace Origam.Windows.Editor.GIT
{
    partial class GitDirferenceView
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
            this.singleColumnDiff1 = new Origam.Windows.Editor.GIT.SingleColumnDiff();
            this.TitleLabel = new System.Windows.Forms.Label();
            this.newFileLabel = new System.Windows.Forms.Label();
            this.oldFileLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // singleColumnDiff1
            // 
            this.singleColumnDiff1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.singleColumnDiff1.Location = new System.Drawing.Point(1, 76);
            this.singleColumnDiff1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.singleColumnDiff1.Name = "singleColumnDiff1";
            this.singleColumnDiff1.Size = new System.Drawing.Size(639, 442);
            this.singleColumnDiff1.TabIndex = 0;
            // 
            // TitleLabel
            // 
            this.TitleLabel.AutoSize = true;
            this.TitleLabel.Location = new System.Drawing.Point(12, 9);
            this.TitleLabel.Name = "TitleLabel";
            this.TitleLabel.Size = new System.Drawing.Size(104, 17);
            this.TitleLabel.TabIndex = 4;
            this.TitleLabel.Text = "Showing diff of:";
            // 
            // newFileLabel
            // 
            this.newFileLabel.AutoSize = true;
            this.newFileLabel.Location = new System.Drawing.Point(39, 55);
            this.newFileLabel.Name = "newFileLabel";
            this.newFileLabel.Size = new System.Drawing.Size(46, 17);
            this.newFileLabel.TabIndex = 6;
            this.newFileLabel.Text = "label3";
            // 
            // oldFileLabel
            // 
            this.oldFileLabel.AutoSize = true;
            this.oldFileLabel.Location = new System.Drawing.Point(39, 34);
            this.oldFileLabel.Name = "oldFileLabel";
            this.oldFileLabel.Size = new System.Drawing.Size(46, 17);
            this.oldFileLabel.TabIndex = 5;
            this.oldFileLabel.Text = "label2";
            // 
            // GitDirferenceView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(642, 519);
            this.Controls.Add(this.TitleLabel);
            this.Controls.Add(this.newFileLabel);
            this.Controls.Add(this.oldFileLabel);
            this.Controls.Add(this.singleColumnDiff1);
            this.Name = "GitDirferenceView";
            this.Text = "GitDirferenceView";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SingleColumnDiff singleColumnDiff1;
        private System.Windows.Forms.Label TitleLabel;
        private System.Windows.Forms.Label newFileLabel;
        private System.Windows.Forms.Label oldFileLabel;
    }
}