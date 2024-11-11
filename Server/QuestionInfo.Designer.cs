namespace Server
{
    partial class QuestionInfo
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
            this.txt_ques_typing = new System.Windows.Forms.TextBox();
            this.btn_add_result = new System.Windows.Forms.Button();
            this.pnl_answers = new System.Windows.Forms.FlowLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btn_del_result = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txt_ques_typing
            // 
            this.txt_ques_typing.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_ques_typing.Location = new System.Drawing.Point(76, 10);
            this.txt_ques_typing.Multiline = true;
            this.txt_ques_typing.Name = "txt_ques_typing";
            this.txt_ques_typing.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txt_ques_typing.Size = new System.Drawing.Size(664, 54);
            this.txt_ques_typing.TabIndex = 2;
            this.txt_ques_typing.TextChanged += new System.EventHandler(this.txt_ques_typing_TextChanged);
            // 
            // btn_add_result
            // 
            this.btn_add_result.Location = new System.Drawing.Point(220, 487);
            this.btn_add_result.Name = "btn_add_result";
            this.btn_add_result.Size = new System.Drawing.Size(130, 40);
            this.btn_add_result.TabIndex = 4;
            this.btn_add_result.Text = "Thêm đáp án";
            this.btn_add_result.UseVisualStyleBackColor = true;
            this.btn_add_result.Click += new System.EventHandler(this.btn_add_result_Click);
            // 
            // pnl_answers
            // 
            this.pnl_answers.Location = new System.Drawing.Point(20, 175);
            this.pnl_answers.Name = "pnl_answers";
            this.pnl_answers.Size = new System.Drawing.Size(720, 189);
            this.pnl_answers.TabIndex = 5;
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::Server.Properties.Resources.register;
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel1.Location = new System.Drawing.Point(20, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(50, 50);
            this.panel1.TabIndex = 6;
            // 
            // btn_del_result
            // 
            this.btn_del_result.Location = new System.Drawing.Point(410, 487);
            this.btn_del_result.Name = "btn_del_result";
            this.btn_del_result.Size = new System.Drawing.Size(130, 40);
            this.btn_del_result.TabIndex = 7;
            this.btn_del_result.Text = "Bớt đáp án";
            this.btn_del_result.UseVisualStyleBackColor = true;
            this.btn_del_result.Click += new System.EventHandler(this.btn_del_result_Click);
            // 
            // QuestionInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btn_del_result);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pnl_answers);
            this.Controls.Add(this.btn_add_result);
            this.Controls.Add(this.txt_ques_typing);
            this.Name = "QuestionInfo";
            this.Size = new System.Drawing.Size(760, 650);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox txt_ques_typing;
        private System.Windows.Forms.Button btn_add_result;
        private System.Windows.Forms.FlowLayoutPanel pnl_answers;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btn_del_result;
    }
}
