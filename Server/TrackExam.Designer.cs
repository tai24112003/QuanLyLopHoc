namespace Server
{
    partial class TrackExam
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.lbl_testTitle = new System.Windows.Forms.Label();
            this.lbl_state = new System.Windows.Forms.Label();
            this.lbl_numQuest = new System.Windows.Forms.Label();
            this.lbl_currentQuest = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pnl_chart = new System.Windows.Forms.Panel();
            this.lbl_studentDo = new System.Windows.Forms.Label();
            this.btn_switchView = new System.Windows.Forms.Button();
            this.pnl_slideQuests = new System.Windows.Forms.FlowLayoutPanel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.lbl_titleChart = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lbl_numStudent = new System.Windows.Forms.Label();
            this.dgv_ranking = new System.Windows.Forms.DataGridView();
            this.lbl_fastest = new System.Windows.Forms.Label();
            this.pnl_chart.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_ranking)).BeginInit();
            this.SuspendLayout();
            // 
            // lbl_testTitle
            // 
            this.lbl_testTitle.AutoSize = true;
            this.lbl_testTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_testTitle.Location = new System.Drawing.Point(12, 9);
            this.lbl_testTitle.Name = "lbl_testTitle";
            this.lbl_testTitle.Size = new System.Drawing.Size(66, 24);
            this.lbl_testTitle.TabIndex = 0;
            this.lbl_testTitle.Text = "label1";
            // 
            // lbl_state
            // 
            this.lbl_state.AutoSize = true;
            this.lbl_state.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_state.Location = new System.Drawing.Point(13, 35);
            this.lbl_state.Name = "lbl_state";
            this.lbl_state.Size = new System.Drawing.Size(44, 16);
            this.lbl_state.TabIndex = 1;
            this.lbl_state.Text = "label1";
            // 
            // lbl_numQuest
            // 
            this.lbl_numQuest.AutoSize = true;
            this.lbl_numQuest.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_numQuest.Location = new System.Drawing.Point(172, 35);
            this.lbl_numQuest.Name = "lbl_numQuest";
            this.lbl_numQuest.Size = new System.Drawing.Size(44, 16);
            this.lbl_numQuest.TabIndex = 2;
            this.lbl_numQuest.Text = "label1";
            // 
            // lbl_currentQuest
            // 
            this.lbl_currentQuest.AutoSize = true;
            this.lbl_currentQuest.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_currentQuest.Location = new System.Drawing.Point(13, 56);
            this.lbl_currentQuest.Name = "lbl_currentQuest";
            this.lbl_currentQuest.Size = new System.Drawing.Size(44, 16);
            this.lbl_currentQuest.TabIndex = 3;
            this.lbl_currentQuest.Text = "label1";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Black;
            this.panel1.Location = new System.Drawing.Point(16, 82);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1456, 2);
            this.panel1.TabIndex = 4;
            // 
            // pnl_chart
            // 
            this.pnl_chart.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnl_chart.Controls.Add(this.lbl_fastest);
            this.pnl_chart.Controls.Add(this.lbl_studentDo);
            this.pnl_chart.Controls.Add(this.btn_switchView);
            this.pnl_chart.Location = new System.Drawing.Point(222, 90);
            this.pnl_chart.Name = "pnl_chart";
            this.pnl_chart.Size = new System.Drawing.Size(866, 656);
            this.pnl_chart.TabIndex = 5;
            // 
            // lbl_studentDo
            // 
            this.lbl_studentDo.AutoSize = true;
            this.lbl_studentDo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_studentDo.Location = new System.Drawing.Point(157, 6);
            this.lbl_studentDo.Name = "lbl_studentDo";
            this.lbl_studentDo.Size = new System.Drawing.Size(44, 16);
            this.lbl_studentDo.TabIndex = 9;
            this.lbl_studentDo.Text = "label1";
            // 
            // btn_switchView
            // 
            this.btn_switchView.Location = new System.Drawing.Point(3, 3);
            this.btn_switchView.Name = "btn_switchView";
            this.btn_switchView.Size = new System.Drawing.Size(90, 23);
            this.btn_switchView.TabIndex = 0;
            this.btn_switchView.Text = "Phần trăm ";
            this.toolTip1.SetToolTip(this.btn_switchView, "Chuyển đổi đơn vị hiển thị");
            this.btn_switchView.UseVisualStyleBackColor = true;
            this.btn_switchView.Click += new System.EventHandler(this.btn_switchView_Click);
            // 
            // pnl_slideQuests
            // 
            this.pnl_slideQuests.AutoScroll = true;
            this.pnl_slideQuests.Location = new System.Drawing.Point(16, 90);
            this.pnl_slideQuests.Name = "pnl_slideQuests";
            this.pnl_slideQuests.Size = new System.Drawing.Size(200, 656);
            this.pnl_slideQuests.TabIndex = 6;
            // 
            // lbl_titleChart
            // 
            this.lbl_titleChart.AutoSize = true;
            this.lbl_titleChart.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_titleChart.Location = new System.Drawing.Point(519, 734);
            this.lbl_titleChart.Name = "lbl_titleChart";
            this.lbl_titleChart.Size = new System.Drawing.Size(268, 25);
            this.lbl_titleChart.TabIndex = 1;
            this.lbl_titleChart.Text = "Biểu đồ lựa chọn đáp án";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(1184, 734);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(214, 25);
            this.label1.TabIndex = 2;
            this.label1.Text = "Xếp hạng sinh viên";
            // 
            // lbl_numStudent
            // 
            this.lbl_numStudent.AutoSize = true;
            this.lbl_numStudent.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_numStudent.Location = new System.Drawing.Point(358, 35);
            this.lbl_numStudent.Name = "lbl_numStudent";
            this.lbl_numStudent.Size = new System.Drawing.Size(44, 16);
            this.lbl_numStudent.TabIndex = 8;
            this.lbl_numStudent.Text = "label1";
            // 
            // dgv_ranking
            // 
            this.dgv_ranking.AllowUserToAddRows = false;
            this.dgv_ranking.BackgroundColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgv_ranking.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgv_ranking.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_ranking.Location = new System.Drawing.Point(1094, 90);
            this.dgv_ranking.Name = "dgv_ranking";
            this.dgv_ranking.Size = new System.Drawing.Size(376, 641);
            this.dgv_ranking.TabIndex = 1;
            // 
            // lbl_fastest
            // 
            this.lbl_fastest.AutoSize = true;
            this.lbl_fastest.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_fastest.Location = new System.Drawing.Point(409, 6);
            this.lbl_fastest.Name = "lbl_fastest";
            this.lbl_fastest.Size = new System.Drawing.Size(44, 16);
            this.lbl_fastest.TabIndex = 10;
            this.lbl_fastest.Text = "label1";
            // 
            // TrackExam
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1484, 771);
            this.Controls.Add(this.dgv_ranking);
            this.Controls.Add(this.lbl_numStudent);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbl_titleChart);
            this.Controls.Add(this.pnl_slideQuests);
            this.Controls.Add(this.pnl_chart);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lbl_currentQuest);
            this.Controls.Add(this.lbl_numQuest);
            this.Controls.Add(this.lbl_state);
            this.Controls.Add(this.lbl_testTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "TrackExam";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ExamProgress";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TrackExam_FormClosing);
            this.pnl_chart.ResumeLayout(false);
            this.pnl_chart.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_ranking)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbl_testTitle;
        private System.Windows.Forms.Label lbl_state;
        private System.Windows.Forms.Label lbl_numQuest;
        private System.Windows.Forms.Label lbl_currentQuest;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel pnl_chart;
        private System.Windows.Forms.FlowLayoutPanel pnl_slideQuests;
        private System.Windows.Forms.Button btn_switchView;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label lbl_titleChart;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbl_numStudent;
        private System.Windows.Forms.Label lbl_studentDo;
        private System.Windows.Forms.DataGridView dgv_ranking;
        private System.Windows.Forms.Label lbl_fastest;
    }
}