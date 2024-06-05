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
            this.BtnExtraction = new System.Windows.Forms.Button();
            this.BtnDistortionRun = new System.Windows.Forms.Button();
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
            this.TextVectorMap = new System.Windows.Forms.TextBox();
            this.BtnVectorMap = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // BtnExtraction
            // 
            this.BtnExtraction.Location = new System.Drawing.Point(29, 325);
            this.BtnExtraction.Name = "BtnExtraction";
            this.BtnExtraction.Size = new System.Drawing.Size(192, 91);
            this.BtnExtraction.TabIndex = 0;
            this.BtnExtraction.Text = "왜곡보정추출";
            this.BtnExtraction.UseVisualStyleBackColor = true; 
            this.BtnExtraction.Click += new System.EventHandler(this.BtnExtraction_Click);
            // 
            // BtnDistortionRun
            // 
            this.BtnDistortionRun.Location = new System.Drawing.Point(432, 316);
            this.BtnDistortionRun.Name = "BtnDistortionRun";
            this.BtnDistortionRun.Size = new System.Drawing.Size(192, 108);
            this.BtnDistortionRun.TabIndex = 1;
            this.BtnDistortionRun.Text = "왜곡보정실행";
            this.BtnDistortionRun.UseVisualStyleBackColor = true;
            this.BtnDistortionRun.Click += new System.EventHandler(this.BtnDistortionRun_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(252, 325);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "추출 시간";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(655, 316);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "실행 시간";
            // 
            // extractTime
            // 
            this.extractTime.Location = new System.Drawing.Point(243, 375);
            this.extractTime.Name = "extractTime";
            this.extractTime.Size = new System.Drawing.Size(100, 21);
            this.extractTime.TabIndex = 4;
            // 
            // correctionTime
            // 
            this.correctionTime.Location = new System.Drawing.Point(646, 361);
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
            this.BtnLoadFolder.Text = "폴더 선택";
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
            this.ImageListBox.SelectedIndexChanged += new System.EventHandler(this.ImageListBox_Select);
            // 
            // TextTargetPath
            // 
            this.TextTargetPath.Location = new System.Drawing.Point(29, 269);
            this.TextTargetPath.Name = "TextTargetPath";
            this.TextTargetPath.Size = new System.Drawing.Size(192, 21);
            this.TextTargetPath.TabIndex = 10;
            // 
            // BtnTarget
            // 
            this.BtnTarget.Location = new System.Drawing.Point(243, 269);
            this.BtnTarget.Name = "BtnTarget";
            this.BtnTarget.Size = new System.Drawing.Size(100, 24);
            this.BtnTarget.TabIndex = 9;
            this.BtnTarget.Text = "도트 타겟 선택";
            this.BtnTarget.UseVisualStyleBackColor = true;
            this.BtnTarget.Click += new System.EventHandler(this.BtnTarget_Click);
            // 
            // ChkSingle
            // 
            this.ChkSingle.AutoSize = true;
            this.ChkSingle.Location = new System.Drawing.Point(432, 277);
            this.ChkSingle.Name = "ChkSingle";
            this.ChkSingle.Size = new System.Drawing.Size(76, 16);
            this.ChkSingle.TabIndex = 11;
            this.ChkSingle.Text = "단일 보정";
            this.ChkSingle.UseVisualStyleBackColor = true;
            this.ChkSingle.CheckedChanged += new System.EventHandler(this.ChkSingle_CheckedChanged);
            // 
            // ChkTotal
            // 
            this.ChkTotal.AutoSize = true;
            this.ChkTotal.Location = new System.Drawing.Point(548, 277);
            this.ChkTotal.Name = "ChkTotal";
            this.ChkTotal.Size = new System.Drawing.Size(76, 16);
            this.ChkTotal.TabIndex = 11;
            this.ChkTotal.Text = "전체 보정";
            this.ChkTotal.UseVisualStyleBackColor = true;
            this.ChkTotal.CheckedChanged += new System.EventHandler(this.ChkTotal_CheckedChanged);
            // 
            // TextVectorMap
            // 
            this.TextVectorMap.Location = new System.Drawing.Point(32, 215);
            this.TextVectorMap.Name = "TextVectorMap";
            this.TextVectorMap.Size = new System.Drawing.Size(572, 21);
            this.TextVectorMap.TabIndex = 12;
            // 
            // BtnVectorMap
            // 
            this.BtnVectorMap.Location = new System.Drawing.Point(620, 215);
            this.BtnVectorMap.Name = "BtnVectorMap";
            this.BtnVectorMap.Size = new System.Drawing.Size(84, 24);
            this.BtnVectorMap.TabIndex = 13;
            this.BtnVectorMap.Text = "벡터 맵 선택";
            this.BtnVectorMap.UseVisualStyleBackColor = true;
            this.BtnVectorMap.Click += new System.EventHandler(this.BtnVectorMap_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.BtnVectorMap);
            this.Controls.Add(this.TextVectorMap);
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
            this.Controls.Add(this.BtnDistortionRun);
            this.Controls.Add(this.BtnExtraction);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BtnExtraction;
        private System.Windows.Forms.Button BtnDistortionRun;
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
        private System.Windows.Forms.TextBox TextVectorMap;
        private System.Windows.Forms.Button BtnVectorMap;
    }
}

