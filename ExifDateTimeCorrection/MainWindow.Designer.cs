namespace ExifDateTimeCorrection
{
    partial class MainWindow
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            sourceFolderTB = new TextBox();
            label1 = new Label();
            sourceFolderBtn = new Button();
            destFolderBtn = new Button();
            label2 = new Label();
            destFolderTB = new TextBox();
            processBtn = new Button();
            backgroundProcess = new System.ComponentModel.BackgroundWorker();
            processBar = new ProgressBar();
            labelFileName = new Label();
            cancelBtn = new Button();
            SuspendLayout();
            // 
            // sourceFolderTB
            // 
            sourceFolderTB.Location = new Point(110, 6);
            sourceFolderTB.Name = "sourceFolderTB";
            sourceFolderTB.Size = new Size(252, 27);
            sourceFolderTB.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(61, 20);
            label1.TabIndex = 1;
            label1.Text = "Source :";
            // 
            // sourceFolderBtn
            // 
            sourceFolderBtn.Location = new Point(368, 5);
            sourceFolderBtn.Name = "sourceFolderBtn";
            sourceFolderBtn.Size = new Size(46, 29);
            sourceFolderBtn.TabIndex = 2;
            sourceFolderBtn.Text = "...";
            sourceFolderBtn.UseVisualStyleBackColor = true;
            sourceFolderBtn.Click += btnFolder_Click;
            // 
            // destFolderBtn
            // 
            destFolderBtn.Location = new Point(368, 59);
            destFolderBtn.Name = "destFolderBtn";
            destFolderBtn.Size = new Size(46, 29);
            destFolderBtn.TabIndex = 5;
            destFolderBtn.Text = "...";
            destFolderBtn.UseVisualStyleBackColor = true;
            destFolderBtn.Click += destFolderBtn_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 63);
            label2.Name = "label2";
            label2.Size = new Size(92, 20);
            label2.TabIndex = 4;
            label2.Text = "Destination :";
            // 
            // destFolderTB
            // 
            destFolderTB.Location = new Point(110, 60);
            destFolderTB.Name = "destFolderTB";
            destFolderTB.Size = new Size(252, 27);
            destFolderTB.TabIndex = 3;
            // 
            // processBtn
            // 
            processBtn.Location = new Point(320, 108);
            processBtn.Name = "processBtn";
            processBtn.Size = new Size(94, 29);
            processBtn.TabIndex = 6;
            processBtn.Text = "Process";
            processBtn.UseVisualStyleBackColor = true;
            processBtn.Click += processBtn_Click;
            // 
            // backgroundProcess
            // 
            backgroundProcess.WorkerReportsProgress = true;
            backgroundProcess.WorkerSupportsCancellation = true;
            backgroundProcess.DoWork += backgroundSearch_DoWork;
            backgroundProcess.ProgressChanged += backgroundSearch_ProgressChanged;
            backgroundProcess.RunWorkerCompleted += backgroundSearch_RunWorkerCompleted;
            // 
            // processBar
            // 
            processBar.Location = new Point(12, 231);
            processBar.Name = "processBar";
            processBar.Size = new Size(402, 29);
            processBar.TabIndex = 7;
            // 
            // labelFileName
            // 
            labelFileName.AutoSize = true;
            labelFileName.Location = new Point(12, 208);
            labelFileName.Name = "labelFileName";
            labelFileName.Size = new Size(0, 20);
            labelFileName.TabIndex = 8;
            // 
            // cancelBtn
            // 
            cancelBtn.Location = new Point(320, 143);
            cancelBtn.Name = "cancelBtn";
            cancelBtn.Size = new Size(94, 29);
            cancelBtn.TabIndex = 9;
            cancelBtn.Text = "Cancel";
            cancelBtn.UseVisualStyleBackColor = true;
            cancelBtn.Click += cancelBtn_Click;
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(435, 272);
            Controls.Add(cancelBtn);
            Controls.Add(labelFileName);
            Controls.Add(processBar);
            Controls.Add(processBtn);
            Controls.Add(destFolderBtn);
            Controls.Add(label2);
            Controls.Add(destFolderTB);
            Controls.Add(sourceFolderBtn);
            Controls.Add(label1);
            Controls.Add(sourceFolderTB);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "MainWindow";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Exif DateTime Correction";
            Load += MainWindow_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox sourceFolderTB;
        private Label label1;
        private Button sourceFolderBtn;
        private Button destFolderBtn;
        private Label label2;
        private TextBox destFolderTB;
        private Button processBtn;
        private System.ComponentModel.BackgroundWorker backgroundProcess;
        private ProgressBar processBar;
        private Label labelFileName;
        private Button cancelBtn;
    }
}
