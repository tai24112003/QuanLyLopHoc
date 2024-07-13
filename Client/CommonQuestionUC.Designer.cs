namespace testUdpTcp
{
    partial class CommonQuestionUC
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.flpnContain = new System.Windows.Forms.FlowLayoutPanel();
            this.wbQuestion = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // flpnContain
            // 
            this.flpnContain.Location = new System.Drawing.Point(0, 131);
            this.flpnContain.Margin = new System.Windows.Forms.Padding(0);
            this.flpnContain.Name = "flpnContain";
            this.flpnContain.Size = new System.Drawing.Size(957, 100);
            this.flpnContain.TabIndex = 1;
            this.flpnContain.ControlAdded += new System.Windows.Forms.ControlEventHandler(this.flpnContain_ControlAdded);
            this.flpnContain.Layout += new System.Windows.Forms.LayoutEventHandler(this.flpnContain_Layout);
            // 
            // wbQuestion
            // 
            this.wbQuestion.Location = new System.Drawing.Point(3, 3);
            this.wbQuestion.MinimumSize = new System.Drawing.Size(20, 20);
            this.wbQuestion.Name = "wbQuestion";
            this.wbQuestion.Size = new System.Drawing.Size(951, 127);
            this.wbQuestion.TabIndex = 2;
            // 
            // CommonQuestionUC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.wbQuestion);
            this.Controls.Add(this.flpnContain);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "CommonQuestionUC";
            this.Size = new System.Drawing.Size(957, 233);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.FlowLayoutPanel flpnContain;
        private System.Windows.Forms.WebBrowser wbQuestion;
    }
}
