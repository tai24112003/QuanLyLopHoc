namespace Server
{
    partial class SvExamForm
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
            this.components = new System.ComponentModel.Container();
            this.pnl_header = new System.Windows.Forms.Panel();
            this.btn_send_test = new System.Windows.Forms.Button();
            this.btn_add_test = new System.Windows.Forms.Button();
            this.btn_doExam = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txt_max_point = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txt_test_title = new System.Windows.Forms.TextBox();
            this.pnl_test_list = new System.Windows.Forms.FlowLayoutPanel();
            this.btn_import = new System.Windows.Forms.Button();
            this.btn_export = new System.Windows.Forms.Button();
            this.lbl_num_of_quest = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_add_question = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pnl_slide_question = new System.Windows.Forms.FlowLayoutPanel();
            this.pnl_body = new System.Windows.Forms.Panel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btn_del_ques = new System.Windows.Forms.Button();
            this.btn_save = new System.Windows.Forms.Button();
            this.pnl_setting_ques = new System.Windows.Forms.Panel();
            this.lbl_quest_time = new System.Windows.Forms.Label();
            this.cbb_quest_time = new System.Windows.Forms.ComboBox();
            this.lbl_quest_type = new System.Windows.Forms.Label();
            this.cbb_quest_type = new System.Windows.Forms.ComboBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.pnl_header.SuspendLayout();
            this.pnl_setting_ques.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnl_header
            // 
            this.pnl_header.Controls.Add(this.btn_send_test);
            this.pnl_header.Controls.Add(this.btn_add_test);
            this.pnl_header.Controls.Add(this.btn_doExam);
            this.pnl_header.Controls.Add(this.label3);
            this.pnl_header.Controls.Add(this.txt_max_point);
            this.pnl_header.Controls.Add(this.label2);
            this.pnl_header.Controls.Add(this.txt_test_title);
            this.pnl_header.Controls.Add(this.pnl_test_list);
            this.pnl_header.Controls.Add(this.btn_import);
            this.pnl_header.Controls.Add(this.btn_export);
            this.pnl_header.Location = new System.Drawing.Point(12, 12);
            this.pnl_header.Name = "pnl_header";
            this.pnl_header.Size = new System.Drawing.Size(1209, 87);
            this.pnl_header.TabIndex = 1;
            // 
            // btn_send_test
            // 
            this.btn_send_test.BackgroundImage = global::Server.Properties.Resources.send;
            this.btn_send_test.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btn_send_test.Location = new System.Drawing.Point(926, 4);
            this.btn_send_test.Name = "btn_send_test";
            this.btn_send_test.Size = new System.Drawing.Size(65, 79);
            this.btn_send_test.TabIndex = 7;
            this.btn_send_test.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.toolTip1.SetToolTip(this.btn_send_test, "Phát đề");
            this.btn_send_test.UseVisualStyleBackColor = true;
            this.btn_send_test.Click += new System.EventHandler(this.btn_start_test_Click);
            // 
            // btn_add_test
            // 
            this.btn_add_test.BackgroundImage = global::Server.Properties.Resources.add__1_;
            this.btn_add_test.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btn_add_test.Location = new System.Drawing.Point(855, 4);
            this.btn_add_test.Name = "btn_add_test";
            this.btn_add_test.Size = new System.Drawing.Size(65, 79);
            this.btn_add_test.TabIndex = 4;
            this.btn_add_test.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.toolTip1.SetToolTip(this.btn_add_test, "Đề kiểm tra mới");
            this.btn_add_test.UseVisualStyleBackColor = true;
            this.btn_add_test.Click += new System.EventHandler(this.btn_add_test_Click);
            // 
            // btn_doExam
            // 
            this.btn_doExam.BackgroundImage = global::Server.Properties.Resources.start;
            this.btn_doExam.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btn_doExam.Location = new System.Drawing.Point(997, 4);
            this.btn_doExam.Name = "btn_doExam";
            this.btn_doExam.Size = new System.Drawing.Size(70, 79);
            this.btn_doExam.TabIndex = 8;
            this.btn_doExam.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.toolTip1.SetToolTip(this.btn_doExam, "Phát đề");
            this.btn_doExam.UseVisualStyleBackColor = true;
            this.btn_doExam.Click += new System.EventHandler(this.btn_doExam_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 46);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(61, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Điểm tối đa";
            // 
            // txt_max_point
            // 
            this.txt_max_point.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_max_point.Location = new System.Drawing.Point(8, 63);
            this.txt_max_point.Name = "txt_max_point";
            this.txt_max_point.Size = new System.Drawing.Size(58, 24);
            this.txt_max_point.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Tên bài kiểm tra";
            // 
            // txt_test_title
            // 
            this.txt_test_title.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_test_title.Location = new System.Drawing.Point(7, 19);
            this.txt_test_title.Name = "txt_test_title";
            this.txt_test_title.Size = new System.Drawing.Size(261, 24);
            this.txt_test_title.TabIndex = 2;
            // 
            // pnl_test_list
            // 
            this.pnl_test_list.Location = new System.Drawing.Point(285, 3);
            this.pnl_test_list.Name = "pnl_test_list";
            this.pnl_test_list.Size = new System.Drawing.Size(564, 84);
            this.pnl_test_list.TabIndex = 1;
            // 
            // btn_import
            // 
            this.btn_import.BackgroundImage = global::Server.Properties.Resources.import1;
            this.btn_import.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btn_import.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btn_import.Location = new System.Drawing.Point(1071, 4);
            this.btn_import.Name = "btn_import";
            this.btn_import.Size = new System.Drawing.Size(65, 79);
            this.btn_import.TabIndex = 0;
            this.btn_import.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.toolTip1.SetToolTip(this.btn_import, "Nhập file excel");
            this.btn_import.UseVisualStyleBackColor = true;
            this.btn_import.Click += new System.EventHandler(this.btn_import_Click);
            // 
            // btn_export
            // 
            this.btn_export.BackgroundImage = global::Server.Properties.Resources.export_gray;
            this.btn_export.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btn_export.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btn_export.Location = new System.Drawing.Point(1142, 4);
            this.btn_export.Name = "btn_export";
            this.btn_export.Size = new System.Drawing.Size(65, 79);
            this.btn_export.TabIndex = 0;
            this.btn_export.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.toolTip1.SetToolTip(this.btn_export, "Lưu/Xuất file excel");
            this.btn_export.UseVisualStyleBackColor = true;
            // 
            // lbl_num_of_quest
            // 
            this.lbl_num_of_quest.AutoSize = true;
            this.lbl_num_of_quest.Location = new System.Drawing.Point(181, 102);
            this.lbl_num_of_quest.Name = "lbl_num_of_quest";
            this.lbl_num_of_quest.Size = new System.Drawing.Size(35, 13);
            this.lbl_num_of_quest.TabIndex = 1;
            this.lbl_num_of_quest.Text = "label1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(123, 103);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Câu Hỏi";
            // 
            // btn_add_question
            // 
            this.btn_add_question.Location = new System.Drawing.Point(33, 705);
            this.btn_add_question.Name = "btn_add_question";
            this.btn_add_question.Size = new System.Drawing.Size(142, 56);
            this.btn_add_question.TabIndex = 4;
            this.btn_add_question.Text = "Thêm Câu Hỏi";
            this.btn_add_question.UseVisualStyleBackColor = true;
            this.btn_add_question.Click += new System.EventHandler(this.add_question_btn_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Desktop;
            this.panel1.Location = new System.Drawing.Point(225, 105);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(2, 662);
            this.panel1.TabIndex = 5;
            // 
            // pnl_slide_question
            // 
            this.pnl_slide_question.Location = new System.Drawing.Point(19, 118);
            this.pnl_slide_question.Name = "pnl_slide_question";
            this.pnl_slide_question.Size = new System.Drawing.Size(200, 562);
            this.pnl_slide_question.TabIndex = 6;
            // 
            // pnl_body
            // 
            this.pnl_body.Location = new System.Drawing.Point(234, 111);
            this.pnl_body.Name = "pnl_body";
            this.pnl_body.Size = new System.Drawing.Size(760, 650);
            this.pnl_body.TabIndex = 2;
            // 
            // btn_del_ques
            // 
            this.btn_del_ques.BackgroundImage = global::Server.Properties.Resources.close;
            this.btn_del_ques.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btn_del_ques.Location = new System.Drawing.Point(146, 578);
            this.btn_del_ques.Name = "btn_del_ques";
            this.btn_del_ques.Size = new System.Drawing.Size(50, 50);
            this.btn_del_ques.TabIndex = 6;
            this.btn_del_ques.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.toolTip1.SetToolTip(this.btn_del_ques, "Xóa câu hỏi");
            this.btn_del_ques.UseVisualStyleBackColor = true;
            this.btn_del_ques.Click += new System.EventHandler(this.btn_del_ques_Click);
            // 
            // btn_save
            // 
            this.btn_save.BackgroundImage = global::Server.Properties.Resources.save;
            this.btn_save.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btn_save.Location = new System.Drawing.Point(30, 578);
            this.btn_save.Name = "btn_save";
            this.btn_save.Size = new System.Drawing.Size(50, 50);
            this.btn_save.TabIndex = 5;
            this.btn_save.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.toolTip1.SetToolTip(this.btn_save, "Lưu thay đổi");
            this.btn_save.UseVisualStyleBackColor = true;
            this.btn_save.Click += new System.EventHandler(this.btn_save_Click);
            // 
            // pnl_setting_ques
            // 
            this.pnl_setting_ques.Controls.Add(this.btn_del_ques);
            this.pnl_setting_ques.Controls.Add(this.btn_save);
            this.pnl_setting_ques.Controls.Add(this.lbl_quest_time);
            this.pnl_setting_ques.Controls.Add(this.cbb_quest_time);
            this.pnl_setting_ques.Controls.Add(this.lbl_quest_type);
            this.pnl_setting_ques.Controls.Add(this.cbb_quest_type);
            this.pnl_setting_ques.Location = new System.Drawing.Point(1008, 111);
            this.pnl_setting_ques.Name = "pnl_setting_ques";
            this.pnl_setting_ques.Size = new System.Drawing.Size(213, 650);
            this.pnl_setting_ques.TabIndex = 7;
            // 
            // lbl_quest_time
            // 
            this.lbl_quest_time.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lbl_quest_time.AutoSize = true;
            this.lbl_quest_time.Location = new System.Drawing.Point(3, 63);
            this.lbl_quest_time.Name = "lbl_quest_time";
            this.lbl_quest_time.Size = new System.Drawing.Size(79, 13);
            this.lbl_quest_time.TabIndex = 3;
            this.lbl_quest_time.Text = "Thời gian trả lời";
            // 
            // cbb_quest_time
            // 
            this.cbb_quest_time.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cbb_quest_time.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbb_quest_time.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbb_quest_time.FormattingEnabled = true;
            this.cbb_quest_time.Location = new System.Drawing.Point(3, 79);
            this.cbb_quest_time.Name = "cbb_quest_time";
            this.cbb_quest_time.Size = new System.Drawing.Size(207, 24);
            this.cbb_quest_time.TabIndex = 2;
            // 
            // lbl_quest_type
            // 
            this.lbl_quest_type.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lbl_quest_type.AutoSize = true;
            this.lbl_quest_type.Location = new System.Drawing.Point(3, 14);
            this.lbl_quest_type.Name = "lbl_quest_type";
            this.lbl_quest_type.Size = new System.Drawing.Size(65, 13);
            this.lbl_quest_type.TabIndex = 1;
            this.lbl_quest_type.Text = "Loại câu hỏi";
            // 
            // cbb_quest_type
            // 
            this.cbb_quest_type.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cbb_quest_type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbb_quest_type.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbb_quest_type.FormattingEnabled = true;
            this.cbb_quest_type.Location = new System.Drawing.Point(3, 30);
            this.cbb_quest_type.Name = "cbb_quest_type";
            this.cbb_quest_type.Size = new System.Drawing.Size(207, 24);
            this.cbb_quest_type.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.Desktop;
            this.panel2.Location = new System.Drawing.Point(225, 103);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(995, 2);
            this.panel2.TabIndex = 6;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.SystemColors.Desktop;
            this.panel3.Location = new System.Drawing.Point(1000, 105);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(2, 662);
            this.panel3.TabIndex = 6;
            // 
            // SvExamForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1233, 779);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.pnl_setting_ques);
            this.Controls.Add(this.lbl_num_of_quest);
            this.Controls.Add(this.pnl_slide_question);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btn_add_question);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pnl_body);
            this.Controls.Add(this.pnl_header);
            this.Name = "SvExamForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ExamForm";
            this.pnl_header.ResumeLayout(false);
            this.pnl_header.PerformLayout();
            this.pnl_setting_ques.ResumeLayout(false);
            this.pnl_setting_ques.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel pnl_header;
        private System.Windows.Forms.Button btn_export;
        private System.Windows.Forms.Button btn_import;
        private System.Windows.Forms.Label lbl_num_of_quest;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_add_question;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.FlowLayoutPanel pnl_slide_question;
        private System.Windows.Forms.Panel pnl_body;
        private System.Windows.Forms.FlowLayoutPanel pnl_test_list;
        private System.Windows.Forms.TextBox txt_test_title;
        private System.Windows.Forms.Button btn_add_test;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Panel pnl_setting_ques;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lbl_quest_time;
        private System.Windows.Forms.ComboBox cbb_quest_time;
        private System.Windows.Forms.Label lbl_quest_type;
        private System.Windows.Forms.ComboBox cbb_quest_type;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txt_max_point;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button btn_del_ques;
        private System.Windows.Forms.Button btn_save;
        private System.Windows.Forms.Button btn_send_test;
        private System.Windows.Forms.Button btn_doExam;
    }
}