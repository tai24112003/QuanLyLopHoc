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
            this.pnl_test_list = new System.Windows.Forms.FlowLayoutPanel();
            this.btn_import = new System.Windows.Forms.Button();
            this.btn_export = new System.Windows.Forms.Button();
            this.lbl_num_of_quest = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_add_question = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pnl_slide_question = new System.Windows.Forms.FlowLayoutPanel();
            this.pnl_body = new System.Windows.Forms.Panel();
            this.btn_save = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btn_refesh_filter = new System.Windows.Forms.Button();
            this.btn_confirmSetExam = new System.Windows.Forms.Button();
            this.btn_stop_exam = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.txt_maxPoint = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txt_testTitle = new System.Windows.Forms.TextBox();
            this.pnl_exam_info = new System.Windows.Forms.Panel();
            this.lbl_time_remaining = new System.Windows.Forms.Label();
            this.cbb_questTypeFilter = new System.Windows.Forms.ComboBox();
            this.lbl_state_exam = new System.Windows.Forms.Label();
            this.grb_setting_quest = new System.Windows.Forms.GroupBox();
            this.lbl_quest_time = new System.Windows.Forms.Label();
            this.cbb_doQuestTime = new System.Windows.Forms.ComboBox();
            this.lbl_quest_type = new System.Windows.Forms.Label();
            this.cbb_questType = new System.Windows.Forms.ComboBox();
            this.grb_setting_exam = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cbb_restEachTime_exam = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cbb_doQuestTime_exam = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cbb_questType_exam = new System.Windows.Forms.ComboBox();
            this.btn_trackTheExam = new System.Windows.Forms.Button();
            this.pnl_header.SuspendLayout();
            this.pnl_exam_info.SuspendLayout();
            this.grb_setting_quest.SuspendLayout();
            this.grb_setting_exam.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnl_header
            // 
            this.pnl_header.Controls.Add(this.btn_send_test);
            this.pnl_header.Controls.Add(this.btn_add_test);
            this.pnl_header.Controls.Add(this.btn_doExam);
            this.pnl_header.Controls.Add(this.pnl_test_list);
            this.pnl_header.Controls.Add(this.btn_import);
            this.pnl_header.Controls.Add(this.btn_export);
            this.pnl_header.Location = new System.Drawing.Point(283, 12);
            this.pnl_header.Name = "pnl_header";
            this.pnl_header.Size = new System.Drawing.Size(937, 91);
            this.pnl_header.TabIndex = 1;
            // 
            // btn_send_test
            // 
            this.btn_send_test.BackgroundImage = global::Server.Properties.Resources.send;
            this.btn_send_test.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btn_send_test.Location = new System.Drawing.Point(649, 4);
            this.btn_send_test.Name = "btn_send_test";
            this.btn_send_test.Size = new System.Drawing.Size(65, 79);
            this.btn_send_test.TabIndex = 7;
            this.btn_send_test.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.toolTip1.SetToolTip(this.btn_send_test, "Phát đề");
            this.btn_send_test.UseVisualStyleBackColor = true;
            this.btn_send_test.Click += new System.EventHandler(this.btn_sendExam_Click);
            // 
            // btn_add_test
            // 
            this.btn_add_test.BackgroundImage = global::Server.Properties.Resources.add__1_;
            this.btn_add_test.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btn_add_test.Location = new System.Drawing.Point(578, 4);
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
            this.btn_doExam.Location = new System.Drawing.Point(720, 4);
            this.btn_doExam.Name = "btn_doExam";
            this.btn_doExam.Size = new System.Drawing.Size(70, 79);
            this.btn_doExam.TabIndex = 8;
            this.btn_doExam.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.toolTip1.SetToolTip(this.btn_doExam, "Bắt đầu thi");
            this.btn_doExam.UseVisualStyleBackColor = true;
            this.btn_doExam.Click += new System.EventHandler(this.btn_doExam_Click);
            // 
            // pnl_test_list
            // 
            this.pnl_test_list.Location = new System.Drawing.Point(3, 3);
            this.pnl_test_list.Name = "pnl_test_list";
            this.pnl_test_list.Size = new System.Drawing.Size(564, 84);
            this.pnl_test_list.TabIndex = 1;
            // 
            // btn_import
            // 
            this.btn_import.BackgroundImage = global::Server.Properties.Resources.import1;
            this.btn_import.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btn_import.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btn_import.Location = new System.Drawing.Point(794, 4);
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
            this.btn_export.Location = new System.Drawing.Point(865, 4);
            this.btn_export.Name = "btn_export";
            this.btn_export.Size = new System.Drawing.Size(65, 79);
            this.btn_export.TabIndex = 0;
            this.btn_export.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.toolTip1.SetToolTip(this.btn_export, "Lưu/Xuất file excel");
            this.btn_export.UseVisualStyleBackColor = true;
            this.btn_export.Click += new System.EventHandler(this.btn_export_Click);
            // 
            // lbl_num_of_quest
            // 
            this.lbl_num_of_quest.AutoSize = true;
            this.lbl_num_of_quest.Location = new System.Drawing.Point(173, 165);
            this.lbl_num_of_quest.Name = "lbl_num_of_quest";
            this.lbl_num_of_quest.Size = new System.Drawing.Size(35, 13);
            this.lbl_num_of_quest.TabIndex = 1;
            this.lbl_num_of_quest.Text = "label1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(116, 165);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Câu Hỏi";
            // 
            // btn_add_question
            // 
            this.btn_add_question.Location = new System.Drawing.Point(44, 696);
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
            this.panel1.Location = new System.Drawing.Point(225, 155);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(2, 600);
            this.panel1.TabIndex = 5;
            // 
            // pnl_slide_question
            // 
            this.pnl_slide_question.Location = new System.Drawing.Point(19, 182);
            this.pnl_slide_question.Name = "pnl_slide_question";
            this.pnl_slide_question.Size = new System.Drawing.Size(200, 490);
            this.pnl_slide_question.TabIndex = 6;
            // 
            // pnl_body
            // 
            this.pnl_body.Location = new System.Drawing.Point(234, 174);
            this.pnl_body.Name = "pnl_body";
            this.pnl_body.Size = new System.Drawing.Size(760, 580);
            this.pnl_body.TabIndex = 2;
            // 
            // btn_save
            // 
            this.btn_save.BackgroundImage = global::Server.Properties.Resources.save;
            this.btn_save.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btn_save.Location = new System.Drawing.Point(1100, 692);
            this.btn_save.Name = "btn_save";
            this.btn_save.Size = new System.Drawing.Size(50, 50);
            this.btn_save.TabIndex = 5;
            this.btn_save.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.toolTip1.SetToolTip(this.btn_save, "Lưu thay đổi");
            this.btn_save.UseVisualStyleBackColor = true;
            this.btn_save.Click += new System.EventHandler(this.btn_save_Click);
            // 
            // btn_refesh_filter
            // 
            this.btn_refesh_filter.BackgroundImage = global::Server.Properties.Resources.loading_arrow;
            this.btn_refesh_filter.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btn_refesh_filter.Location = new System.Drawing.Point(197, 119);
            this.btn_refesh_filter.Name = "btn_refesh_filter";
            this.btn_refesh_filter.Size = new System.Drawing.Size(30, 30);
            this.btn_refesh_filter.TabIndex = 13;
            this.toolTip1.SetToolTip(this.btn_refesh_filter, "Làm mới danh sách câu hỏi");
            this.btn_refesh_filter.UseVisualStyleBackColor = true;
            this.btn_refesh_filter.Click += new System.EventHandler(this.btn_refesh_filter_Click);
            // 
            // btn_confirmSetExam
            // 
            this.btn_confirmSetExam.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_confirmSetExam.Location = new System.Drawing.Point(132, 192);
            this.btn_confirmSetExam.Name = "btn_confirmSetExam";
            this.btn_confirmSetExam.Size = new System.Drawing.Size(75, 23);
            this.btn_confirmSetExam.TabIndex = 14;
            this.btn_confirmSetExam.Text = "Xác nhận";
            this.toolTip1.SetToolTip(this.btn_confirmSetExam, "Xác nhận cài đặt bài kiểm tra");
            this.btn_confirmSetExam.UseVisualStyleBackColor = true;
            this.btn_confirmSetExam.Click += new System.EventHandler(this.btn_confirmSetExam_Click);
            // 
            // btn_stop_exam
            // 
            this.btn_stop_exam.BackgroundImage = global::Server.Properties.Resources.close;
            this.btn_stop_exam.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btn_stop_exam.Location = new System.Drawing.Point(1100, 585);
            this.btn_stop_exam.Name = "btn_stop_exam";
            this.btn_stop_exam.Size = new System.Drawing.Size(50, 50);
            this.btn_stop_exam.TabIndex = 17;
            this.btn_stop_exam.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.toolTip1.SetToolTip(this.btn_stop_exam, "Hủy thi");
            this.btn_stop_exam.UseVisualStyleBackColor = true;
            this.btn_stop_exam.Click += new System.EventHandler(this.btn_stop_exam_Click);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.Desktop;
            this.panel2.Location = new System.Drawing.Point(20, 154);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1201, 2);
            this.panel2.TabIndex = 6;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.SystemColors.Desktop;
            this.panel3.Location = new System.Drawing.Point(1000, 155);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(2, 600);
            this.panel3.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 49);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(61, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Điểm tối đa";
            // 
            // txt_maxPoint
            // 
            this.txt_maxPoint.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_maxPoint.Location = new System.Drawing.Point(2, 65);
            this.txt_maxPoint.Name = "txt_maxPoint";
            this.txt_maxPoint.Size = new System.Drawing.Size(58, 24);
            this.txt_maxPoint.TabIndex = 9;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Tên bài kiểm tra";
            // 
            // txt_testTitle
            // 
            this.txt_testTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_testTitle.Location = new System.Drawing.Point(2, 21);
            this.txt_testTitle.Name = "txt_testTitle";
            this.txt_testTitle.Size = new System.Drawing.Size(261, 24);
            this.txt_testTitle.TabIndex = 7;
            // 
            // pnl_exam_info
            // 
            this.pnl_exam_info.Controls.Add(this.lbl_time_remaining);
            this.pnl_exam_info.Controls.Add(this.label3);
            this.pnl_exam_info.Controls.Add(this.label2);
            this.pnl_exam_info.Controls.Add(this.txt_maxPoint);
            this.pnl_exam_info.Controls.Add(this.txt_testTitle);
            this.pnl_exam_info.Location = new System.Drawing.Point(12, 12);
            this.pnl_exam_info.Name = "pnl_exam_info";
            this.pnl_exam_info.Size = new System.Drawing.Size(265, 91);
            this.pnl_exam_info.TabIndex = 8;
            // 
            // lbl_time_remaining
            // 
            this.lbl_time_remaining.AutoSize = true;
            this.lbl_time_remaining.Location = new System.Drawing.Point(92, 72);
            this.lbl_time_remaining.Name = "lbl_time_remaining";
            this.lbl_time_remaining.Size = new System.Drawing.Size(35, 13);
            this.lbl_time_remaining.TabIndex = 11;
            this.lbl_time_remaining.Text = "label4";
            // 
            // cbb_questTypeFilter
            // 
            this.cbb_questTypeFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbb_questTypeFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbb_questTypeFilter.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbb_questTypeFilter.FormattingEnabled = true;
            this.cbb_questTypeFilter.Location = new System.Drawing.Point(20, 124);
            this.cbb_questTypeFilter.Name = "cbb_questTypeFilter";
            this.cbb_questTypeFilter.Size = new System.Drawing.Size(166, 24);
            this.cbb_questTypeFilter.TabIndex = 1;
            this.cbb_questTypeFilter.SelectedIndexChanged += new System.EventHandler(this.cbb_quest_type_filter_SelectedIndexChanged);
            // 
            // lbl_state_exam
            // 
            this.lbl_state_exam.AutoSize = true;
            this.lbl_state_exam.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_state_exam.Location = new System.Drawing.Point(263, 132);
            this.lbl_state_exam.Name = "lbl_state_exam";
            this.lbl_state_exam.Size = new System.Drawing.Size(50, 16);
            this.lbl_state_exam.TabIndex = 12;
            this.lbl_state_exam.Text = "label4";
            // 
            // grb_setting_quest
            // 
            this.grb_setting_quest.Controls.Add(this.lbl_quest_time);
            this.grb_setting_quest.Controls.Add(this.cbb_doQuestTime);
            this.grb_setting_quest.Controls.Add(this.lbl_quest_type);
            this.grb_setting_quest.Controls.Add(this.cbb_questType);
            this.grb_setting_quest.Location = new System.Drawing.Point(1008, 162);
            this.grb_setting_quest.Name = "grb_setting_quest";
            this.grb_setting_quest.Size = new System.Drawing.Size(213, 120);
            this.grb_setting_quest.TabIndex = 14;
            this.grb_setting_quest.TabStop = false;
            this.grb_setting_quest.Text = "Cài đặt câu hỏi hiện tại";
            // 
            // lbl_quest_time
            // 
            this.lbl_quest_time.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbl_quest_time.AutoSize = true;
            this.lbl_quest_time.Location = new System.Drawing.Point(3, 71);
            this.lbl_quest_time.Name = "lbl_quest_time";
            this.lbl_quest_time.Size = new System.Drawing.Size(79, 13);
            this.lbl_quest_time.TabIndex = 7;
            this.lbl_quest_time.Text = "Thời gian trả lời";
            // 
            // cbb_doQuestTime
            // 
            this.cbb_doQuestTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbb_doQuestTime.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbb_doQuestTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbb_doQuestTime.FormattingEnabled = true;
            this.cbb_doQuestTime.Location = new System.Drawing.Point(3, 86);
            this.cbb_doQuestTime.Name = "cbb_doQuestTime";
            this.cbb_doQuestTime.Size = new System.Drawing.Size(207, 24);
            this.cbb_doQuestTime.TabIndex = 6;
            this.cbb_doQuestTime.DropDown += new System.EventHandler(this.cbb_quest_type_DropDown);
            this.cbb_doQuestTime.SelectedIndexChanged += new System.EventHandler(this.cbb_quest_time_SelectedIndexChanged);
            // 
            // lbl_quest_type
            // 
            this.lbl_quest_type.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbl_quest_type.AutoSize = true;
            this.lbl_quest_type.Location = new System.Drawing.Point(3, 16);
            this.lbl_quest_type.Name = "lbl_quest_type";
            this.lbl_quest_type.Size = new System.Drawing.Size(65, 13);
            this.lbl_quest_type.TabIndex = 5;
            this.lbl_quest_type.Text = "Loại câu hỏi";
            // 
            // cbb_questType
            // 
            this.cbb_questType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbb_questType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbb_questType.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbb_questType.FormattingEnabled = true;
            this.cbb_questType.Location = new System.Drawing.Point(3, 31);
            this.cbb_questType.Name = "cbb_questType";
            this.cbb_questType.Size = new System.Drawing.Size(207, 24);
            this.cbb_questType.TabIndex = 4;
            this.cbb_questType.DropDown += new System.EventHandler(this.cbb_quest_type_DropDown);
            this.cbb_questType.SelectedIndexChanged += new System.EventHandler(this.cbb_quest_type_SelectedIndexChanged);
            // 
            // grb_setting_exam
            // 
            this.grb_setting_exam.Controls.Add(this.btn_confirmSetExam);
            this.grb_setting_exam.Controls.Add(this.label6);
            this.grb_setting_exam.Controls.Add(this.cbb_restEachTime_exam);
            this.grb_setting_exam.Controls.Add(this.label4);
            this.grb_setting_exam.Controls.Add(this.cbb_doQuestTime_exam);
            this.grb_setting_exam.Controls.Add(this.label5);
            this.grb_setting_exam.Controls.Add(this.cbb_questType_exam);
            this.grb_setting_exam.Location = new System.Drawing.Point(1008, 288);
            this.grb_setting_exam.Name = "grb_setting_exam";
            this.grb_setting_exam.Size = new System.Drawing.Size(213, 221);
            this.grb_setting_exam.TabIndex = 15;
            this.grb_setting_exam.TabStop = false;
            this.grb_setting_exam.Text = "Điều chỉnh bài kiểm tra";
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 103);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(139, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Thời gian nghỉ giữa các câu";
            // 
            // cbb_restEachTime_exam
            // 
            this.cbb_restEachTime_exam.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbb_restEachTime_exam.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbb_restEachTime_exam.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbb_restEachTime_exam.FormattingEnabled = true;
            this.cbb_restEachTime_exam.Location = new System.Drawing.Point(3, 118);
            this.cbb_restEachTime_exam.Name = "cbb_restEachTime_exam";
            this.cbb_restEachTime_exam.Size = new System.Drawing.Size(207, 24);
            this.cbb_restEachTime_exam.TabIndex = 12;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 61);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(79, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Thời gian trả lời";
            // 
            // cbb_doQuestTime_exam
            // 
            this.cbb_doQuestTime_exam.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbb_doQuestTime_exam.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbb_doQuestTime_exam.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbb_doQuestTime_exam.FormattingEnabled = true;
            this.cbb_doQuestTime_exam.Location = new System.Drawing.Point(3, 76);
            this.cbb_doQuestTime_exam.Name = "cbb_doQuestTime_exam";
            this.cbb_doQuestTime_exam.Size = new System.Drawing.Size(207, 24);
            this.cbb_doQuestTime_exam.TabIndex = 10;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(4, 20);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Loại câu hỏi";
            // 
            // cbb_questType_exam
            // 
            this.cbb_questType_exam.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbb_questType_exam.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbb_questType_exam.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbb_questType_exam.FormattingEnabled = true;
            this.cbb_questType_exam.Location = new System.Drawing.Point(4, 35);
            this.cbb_questType_exam.Name = "cbb_questType_exam";
            this.cbb_questType_exam.Size = new System.Drawing.Size(207, 24);
            this.cbb_questType_exam.TabIndex = 8;
            // 
            // btn_trackTheExam
            // 
            this.btn_trackTheExam.Location = new System.Drawing.Point(1057, 539);
            this.btn_trackTheExam.Name = "btn_trackTheExam";
            this.btn_trackTheExam.Size = new System.Drawing.Size(132, 40);
            this.btn_trackTheExam.TabIndex = 16;
            this.btn_trackTheExam.Text = "Theo dõi quá trình thi";
            this.btn_trackTheExam.UseVisualStyleBackColor = true;
            this.btn_trackTheExam.Click += new System.EventHandler(this.btn_trackTheExam_Click);
            // 
            // SvExamForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1231, 761);
            this.Controls.Add(this.btn_stop_exam);
            this.Controls.Add(this.btn_trackTheExam);
            this.Controls.Add(this.grb_setting_exam);
            this.Controls.Add(this.grb_setting_quest);
            this.Controls.Add(this.btn_refesh_filter);
            this.Controls.Add(this.lbl_state_exam);
            this.Controls.Add(this.cbb_questTypeFilter);
            this.Controls.Add(this.pnl_exam_info);
            this.Controls.Add(this.btn_save);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.lbl_num_of_quest);
            this.Controls.Add(this.pnl_slide_question);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btn_add_question);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pnl_body);
            this.Controls.Add(this.pnl_header);
            this.KeyPreview = true;
            this.Name = "SvExamForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ExamForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SvExamForm_FormClosing);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.SvExamForm_KeyPress);
            this.pnl_header.ResumeLayout(false);
            this.pnl_exam_info.ResumeLayout(false);
            this.pnl_exam_info.PerformLayout();
            this.grb_setting_quest.ResumeLayout(false);
            this.grb_setting_quest.PerformLayout();
            this.grb_setting_exam.ResumeLayout(false);
            this.grb_setting_exam.PerformLayout();
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
        private System.Windows.Forms.Button btn_add_test;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button btn_save;
        private System.Windows.Forms.Button btn_send_test;
        private System.Windows.Forms.Button btn_doExam;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txt_maxPoint;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txt_testTitle;
        private System.Windows.Forms.Panel pnl_exam_info;
        private System.Windows.Forms.Label lbl_time_remaining;
        private System.Windows.Forms.ComboBox cbb_questTypeFilter;
        private System.Windows.Forms.Label lbl_state_exam;
        private System.Windows.Forms.Button btn_refesh_filter;
        private System.Windows.Forms.GroupBox grb_setting_quest;
        private System.Windows.Forms.GroupBox grb_setting_exam;
        private System.Windows.Forms.Label lbl_quest_time;
        private System.Windows.Forms.ComboBox cbb_doQuestTime;
        private System.Windows.Forms.Label lbl_quest_type;
        private System.Windows.Forms.ComboBox cbb_questType;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cbb_restEachTime_exam;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cbb_doQuestTime_exam;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cbb_questType_exam;
        private System.Windows.Forms.Button btn_confirmSetExam;
        private System.Windows.Forms.Button btn_trackTheExam;
        private System.Windows.Forms.Button btn_stop_exam;
    }
}