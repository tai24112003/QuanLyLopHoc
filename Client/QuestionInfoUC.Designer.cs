namespace testUdpTcp
{
    partial class QuestionInfoUC
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
            this.lbl_question = new System.Windows.Forms.Label();
            this.pnl_answers = new System.Windows.Forms.FlowLayoutPanel();
            this.pnl_quest = new System.Windows.Forms.Panel();
            this.lbl_countdown = new System.Windows.Forms.Label();
            this.pnl_quest.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbl_question
            // 
            this.lbl_question.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbl_question.AutoSize = true;
            this.lbl_question.BackColor = System.Drawing.Color.White;
            this.lbl_question.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_question.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lbl_question.Location = new System.Drawing.Point(3, 13);
            this.lbl_question.Name = "lbl_question";
            this.lbl_question.Size = new System.Drawing.Size(32, 24);
            this.lbl_question.TabIndex = 0;
            this.lbl_question.Text = "lbl";
            // 
            // pnl_answers
            // 
            this.pnl_answers.Location = new System.Drawing.Point(83, 251);
            this.pnl_answers.Name = "pnl_answers";
            this.pnl_answers.Size = new System.Drawing.Size(720, 283);
            this.pnl_answers.TabIndex = 1;
            // 
            // pnl_quest
            // 
            this.pnl_quest.Controls.Add(this.lbl_question);
            this.pnl_quest.Location = new System.Drawing.Point(83, 38);
            this.pnl_quest.Name = "pnl_quest";
            this.pnl_quest.Size = new System.Drawing.Size(720, 168);
            this.pnl_quest.TabIndex = 2;
            // 
            // lbl_countdown
            // 
            this.lbl_countdown.AutoSize = true;
            this.lbl_countdown.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_countdown.Location = new System.Drawing.Point(87, 17);
            this.lbl_countdown.Name = "lbl_countdown";
            this.lbl_countdown.Size = new System.Drawing.Size(66, 24);
            this.lbl_countdown.TabIndex = 3;
            this.lbl_countdown.Text = "label1";
            // 
            // QuestionInfoUC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Controls.Add(this.lbl_countdown);
            this.Controls.Add(this.pnl_quest);
            this.Controls.Add(this.pnl_answers);
            this.Name = "QuestionInfoUC";
            this.Size = new System.Drawing.Size(909, 612);
            this.VisibleChanged += new System.EventHandler(this.QuestionInfoUC_VisibleChanged);
            this.pnl_quest.ResumeLayout(false);
            this.pnl_quest.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbl_question;
        private System.Windows.Forms.FlowLayoutPanel pnl_answers;
        private System.Windows.Forms.Panel pnl_quest;
        private System.Windows.Forms.Label lbl_countdown;
    }
}
