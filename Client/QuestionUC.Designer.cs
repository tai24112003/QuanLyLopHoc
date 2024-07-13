namespace testUdpTcp
{
    partial class QuestionUC
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
            this.pnContain = new System.Windows.Forms.Panel();
            this.pnOption = new System.Windows.Forms.Panel();
            this.lblQuestion = new System.Windows.Forms.Label();
            this.wbQuestion = new System.Windows.Forms.WebBrowser();
            this.pnContain.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnContain
            // 
            this.pnContain.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnContain.Controls.Add(this.wbQuestion);
            this.pnContain.Controls.Add(this.pnOption);
            this.pnContain.Controls.Add(this.lblQuestion);
            this.pnContain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnContain.Location = new System.Drawing.Point(0, 0);
            this.pnContain.Name = "pnContain";
            this.pnContain.Size = new System.Drawing.Size(959, 233);
            this.pnContain.TabIndex = 0;
            // 
            // pnOption
            // 
            this.pnOption.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnOption.Location = new System.Drawing.Point(0, 91);
            this.pnOption.Name = "pnOption";
            this.pnOption.Size = new System.Drawing.Size(957, 140);
            this.pnOption.TabIndex = 1;
            // 
            // lblQuestion
            // 
            this.lblQuestion.AutoSize = true;
            this.lblQuestion.Font = new System.Drawing.Font("Tahoma", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblQuestion.Location = new System.Drawing.Point(50, 15);
            this.lblQuestion.Name = "lblQuestion";
            this.lblQuestion.Size = new System.Drawing.Size(72, 28);
            this.lblQuestion.TabIndex = 0;
            this.lblQuestion.Text = "label1";
            // 
            // wbQuestion
            // 
            this.wbQuestion.Location = new System.Drawing.Point(3, 2);
            this.wbQuestion.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.wbQuestion.MinimumSize = new System.Drawing.Size(20, 20);
            this.wbQuestion.Name = "wbQuestion";
            this.wbQuestion.Size = new System.Drawing.Size(955, 92);
            this.wbQuestion.TabIndex = 2;
            // 
            // QuestionUC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnContain);
            this.Margin = new System.Windows.Forms.Padding(0, 0, 0, 30);
            this.Name = "QuestionUC";
            this.Size = new System.Drawing.Size(959, 233);
            this.pnContain.ResumeLayout(false);
            this.pnContain.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnContain;
        private System.Windows.Forms.Panel pnOption;
        private System.Windows.Forms.Label lblQuestion;
        private System.Windows.Forms.WebBrowser wbQuestion;
    }
}
