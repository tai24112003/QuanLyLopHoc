namespace Server
{
    partial class ResultInfo
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
            this.lbl_answer_id = new System.Windows.Forms.Label();
            this.pnl_is_correct = new System.Windows.Forms.Panel();
            this.txt_answer_content = new System.Windows.Forms.TextBox();
            this.pnl_is_correct.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbl_answer_id
            // 
            this.lbl_answer_id.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.lbl_answer_id.AutoSize = true;
            this.lbl_answer_id.Font = new System.Drawing.Font("Microsoft Sans Serif", 18.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_answer_id.ForeColor = System.Drawing.SystemColors.Window;
            this.lbl_answer_id.Location = new System.Drawing.Point(15, 30);
            this.lbl_answer_id.Name = "lbl_answer_id";
            this.lbl_answer_id.Size = new System.Drawing.Size(31, 29);
            this.lbl_answer_id.TabIndex = 0;
            this.lbl_answer_id.Text = "A";
            this.lbl_answer_id.Click += new System.EventHandler(this.setCorrect_Click);
            // 
            // pnl_is_correct
            // 
            this.pnl_is_correct.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.pnl_is_correct.BackColor = System.Drawing.Color.MediumSeaGreen;
            this.pnl_is_correct.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnl_is_correct.Controls.Add(this.lbl_answer_id);
            this.pnl_is_correct.Location = new System.Drawing.Point(-1, 0);
            this.pnl_is_correct.Name = "pnl_is_correct";
            this.pnl_is_correct.Size = new System.Drawing.Size(60, 90);
            this.pnl_is_correct.TabIndex = 1;
            this.pnl_is_correct.Click += new System.EventHandler(this.setCorrect_Click);
            // 
            // txt_answer_content
            // 
            this.txt_answer_content.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txt_answer_content.Location = new System.Drawing.Point(64, 2);
            this.txt_answer_content.Multiline = true;
            this.txt_answer_content.Name = "txt_answer_content";
            this.txt_answer_content.Size = new System.Drawing.Size(281, 86);
            this.txt_answer_content.TabIndex = 2;
            // 
            // ResultInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.txt_answer_content);
            this.Controls.Add(this.pnl_is_correct);
            this.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.Name = "ResultInfo";
            this.Size = new System.Drawing.Size(348, 90);
            this.pnl_is_correct.ResumeLayout(false);
            this.pnl_is_correct.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbl_answer_id;
        private System.Windows.Forms.Panel pnl_is_correct;
        private System.Windows.Forms.TextBox txt_answer_content;
    }
}
