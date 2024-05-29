namespace WindowsFormsApp1
{
    partial class Form1
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.extractTime = new System.Windows.Forms.TextBox();
            this.correctionTime = new System.Windows.Forms.TextBox();
            this.BtnLoadFolder = new System.Windows.Forms.Button();
            this.FilePathText = new System.Windows.Forms.TextBox();
            this.ImageListBox = new System.Windows.Forms.ListBox();
            this.TextTargetPath = new System.Windows.Forms.TextBox();
            this.BtnTarget = new System.Windows.Forms.Button();
            this.ChkSingle = new System.Windows.Forms.CheckBox();
            this.ChkTotal = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(32, 297);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(192, 91);
            this.button1.TabIndex = 0;
            this.button1.Text = "왜곡보정추출";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(435, 288);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(192, 108);
            this.button2.TabIndex = 1;
            this.button2.Text = "왜곡보정실행";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(255, 297);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "추출 시간";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(658, 288);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "실행 시간";
            // 
            // extractTime
            // 
            this.extractTime.Location = new System.Drawing.Point(246, 347);
            this.extractTime.Name = "extractTime";
            this.extractTime.Size = new System.Drawing.Size(100, 21);
            this.extractTime.TabIndex = 4;
            // 
            // correctionTime
            // 
            this.correctionTime.Location = new System.Drawing.Point(649, 333);
            this.correctionTime.Name = "correctionTime";
            this.correctionTime.Size = new System.Drawing.Size(100, 21);
            this.correctionTime.TabIndex = 5;
            // 
            // BtnLoadFolder
            // 
            this.BtnLoadFolder.Location = new System.Drawing.Point(620, 27);
            this.BtnLoadFolder.Name = "BtnLoadFolder";
            this.BtnLoadFolder.Size = new System.Drawing.Size(84, 24);
            this.BtnLoadFolder.TabIndex = 6;
            this.BtnLoadFolder.Text = "폴더";
            this.BtnLoadFolder.UseVisualStyleBackColor = true;
            this.BtnLoadFolder.Click += new System.EventHandler(this.LoadFileBtn_Click);
            // 
            // FilePathText
            // 
            this.FilePathText.Location = new System.Drawing.Point(32, 30);
            this.FilePathText.Name = "FilePathText";
            this.FilePathText.Size = new System.Drawing.Size(572, 21);
            this.FilePathText.TabIndex = 7;
            // 
            // ImageListBox
            // 
            this.ImageListBox.FormattingEnabled = true;
            this.ImageListBox.ItemHeight = 12;
            this.ImageListBox.Location = new System.Drawing.Point(32, 68);
            this.ImageListBox.Name = "ImageListBox";
            this.ImageListBox.Size = new System.Drawing.Size(572, 112);
            this.ImageListBox.TabIndex = 8;
            // 
            // TextTargetPath
            // 
            this.TextTargetPath.Location = new System.Drawing.Point(32, 241);
            this.TextTargetPath.Name = "TextTargetPath";
            this.TextTargetPath.Size = new System.Drawing.Size(192, 21);
            this.TextTargetPath.TabIndex = 10;
            // 
            // BtnTarget
            // 
            this.BtnTarget.Location = new System.Drawing.Point(246, 241);
            this.BtnTarget.Name = "BtnTarget";
            this.BtnTarget.Size = new System.Drawing.Size(84, 24);
            this.BtnTarget.TabIndex = 9;
            this.BtnTarget.Text = "타겟 선택";
            this.BtnTarget.UseVisualStyleBackColor = true;
            this.BtnTarget.Click += new System.EventHandler(this.BtnTarget_Click);
            // 
            // ChkSingle
            // 
            this.ChkSingle.AutoSize = true;
            this.ChkSingle.Location = new System.Drawing.Point(435, 249);
            this.ChkSingle.Name = "ChkSingle";
            this.ChkSingle.Size = new System.Drawing.Size(76, 16);
            this.ChkSingle.TabIndex = 11;
            this.ChkSingle.Text = "단일 검사";
            this.ChkSingle.UseVisualStyleBackColor = true;
            this.ChkSingle.CheckedChanged += new System.EventHandler(this.ChkSingle_CheckedChanged);
            // 
            // ChkTotal
            // 
            this.ChkTotal.AutoSize = true;
            this.ChkTotal.Location = new System.Drawing.Point(551, 249);
            this.ChkTotal.Name = "ChkTotal";
            this.ChkTotal.Size = new System.Drawing.Size(76, 16);
            this.ChkTotal.TabIndex = 11;
            this.ChkTotal.Text = "전체 검사";
            this.ChkTotal.UseVisualStyleBackColor = true;
            this.ChkTotal.CheckedChanged += new System.EventHandler(this.ChkTotal_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.ChkTotal);
            this.Controls.Add(this.ChkSingle);
            this.Controls.Add(this.TextTargetPath);
            this.Controls.Add(this.BtnTarget);
            this.Controls.Add(this.ImageListBox);
            this.Controls.Add(this.FilePathText);
            this.Controls.Add(this.BtnLoadFolder);
            this.Controls.Add(this.correctionTime);
            this.Controls.Add(this.extractTime);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox extractTime;
        private System.Windows.Forms.TextBox correctionTime;
        private System.Windows.Forms.Button BtnLoadFolder;
        private System.Windows.Forms.TextBox FilePathText;
        private System.Windows.Forms.ListBox ImageListBox;
        private System.Windows.Forms.TextBox TextTargetPath;
        private System.Windows.Forms.Button BtnTarget;
        private System.Windows.Forms.CheckBox ChkSingle;
        private System.Windows.Forms.CheckBox ChkTotal;
    }
}

