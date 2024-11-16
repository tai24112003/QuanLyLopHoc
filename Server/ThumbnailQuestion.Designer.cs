namespace Server
{
    partial class ThumbnailQuestion
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
            this.components = new System.ComponentModel.Container();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.lbl_ques_time = new System.Windows.Forms.Label();
            this.lbl_num_answers = new System.Windows.Forms.Label();
            this.btn_del_ques = new System.Windows.Forms.Button();
            this.txt_title_question = new System.Windows.Forms.TextBox();
            this.btn_dou_ques = new System.Windows.Forms.Button();
            this.lbl_question = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.lbl_ques_time);
            this.panel1.Controls.Add(this.lbl_num_answers);
            this.panel1.Controls.Add(this.btn_del_ques);
            this.panel1.Controls.Add(this.txt_title_question);
            this.panel1.Controls.Add(this.btn_dou_ques);
            this.panel1.Controls.Add(this.lbl_question);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(1, 1);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(166, 96);
            this.panel1.TabIndex = 1;
            this.panel1.Click += new System.EventHandler(this.ThumbnailQuestion_Click);
            // 
            // lbl_ques_time
            // 
            this.lbl_ques_time.AutoSize = true;
            this.lbl_ques_time.Location = new System.Drawing.Point(46, 61);
            this.lbl_ques_time.Name = "lbl_ques_time";
            this.lbl_ques_time.Size = new System.Drawing.Size(19, 13);
            this.lbl_ques_time.TabIndex = 6;
            this.lbl_ques_time.Text = "50";
            this.lbl_ques_time.Click += new System.EventHandler(this.ThumbnailQuestion_Click);
            // 
            // lbl_num_answers
            // 
            this.lbl_num_answers.AutoSize = true;
            this.lbl_num_answers.Location = new System.Drawing.Point(101, 61);
            this.lbl_num_answers.Name = "lbl_num_answers";
            this.lbl_num_answers.Size = new System.Drawing.Size(55, 13);
            this.lbl_num_answers.TabIndex = 7;
            this.lbl_num_answers.Text = "Đáp Án: 4";
            this.lbl_num_answers.Click += new System.EventHandler(this.ThumbnailQuestion_Click);
            // 
            // btn_del_ques
            // 
            this.btn_del_ques.BackgroundImage = global::Server.Properties.Resources.close;
            this.btn_del_ques.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btn_del_ques.Location = new System.Drawing.Point(7, 61);
            this.btn_del_ques.Name = "btn_del_ques";
            this.btn_del_ques.Size = new System.Drawing.Size(28, 26);
            this.btn_del_ques.TabIndex = 8;
            this.btn_del_ques.UseVisualStyleBackColor = true;
            // 
            // txt_title_question
            // 
            this.txt_title_question.BackColor = System.Drawing.Color.White;
            this.txt_title_question.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txt_title_question.Cursor = System.Windows.Forms.Cursors.Default;
            this.txt_title_question.Enabled = false;
            this.txt_title_question.Location = new System.Drawing.Point(49, 32);
            this.txt_title_question.Name = "txt_title_question";
            this.txt_title_question.ReadOnly = true;
            this.txt_title_question.Size = new System.Drawing.Size(111, 13);
            this.txt_title_question.TabIndex = 4;
            this.txt_title_question.Click += new System.EventHandler(this.ThumbnailQuestion_Click);
            // 
            // btn_dou_ques
            // 
            this.btn_dou_ques.BackgroundImage = global::Server.Properties.Resources.duplicate_icon;
            this.btn_dou_ques.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btn_dou_ques.Location = new System.Drawing.Point(7, 32);
            this.btn_dou_ques.Name = "btn_dou_ques";
            this.btn_dou_ques.Size = new System.Drawing.Size(28, 23);
            this.btn_dou_ques.TabIndex = 9;
            this.btn_dou_ques.UseVisualStyleBackColor = true;
            // 
            // lbl_question
            // 
            this.lbl_question.AutoSize = true;
            this.lbl_question.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_question.Location = new System.Drawing.Point(10, 9);
            this.lbl_question.Name = "lbl_question";
            this.lbl_question.Size = new System.Drawing.Size(89, 13);
            this.lbl_question.TabIndex = 5;
            this.lbl_question.Text = "question_label";
            this.lbl_question.Click += new System.EventHandler(this.ThumbnailQuestion_Click);
            // 
            // ThumbnailQuestion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.panel1);
            this.Name = "ThumbnailQuestion";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.Size = new System.Drawing.Size(168, 98);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lbl_ques_time;
        private System.Windows.Forms.Label lbl_num_answers;
        private System.Windows.Forms.Button btn_del_ques;
        private System.Windows.Forms.TextBox txt_title_question;
        private System.Windows.Forms.Button btn_dou_ques;
        private System.Windows.Forms.Label lbl_question;
    }
}
