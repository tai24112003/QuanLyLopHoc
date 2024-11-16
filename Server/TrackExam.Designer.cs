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
            this.lbl_testTitle = new System.Windows.Forms.Label();
            this.lbl_state = new System.Windows.Forms.Label();
            this.lbl_numQuest = new System.Windows.Forms.Label();
            this.lbl_currentQuest = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pnl_chart = new System.Windows.Forms.Panel();
            this.pnl_slideQuests = new System.Windows.Forms.FlowLayoutPanel();
            this.btn_switchView = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.lbl_titleChart = new System.Windows.Forms.Label();
            this.pnl_chart.SuspendLayout();
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
            this.panel1.Size = new System.Drawing.Size(1256, 2);
            this.panel1.TabIndex = 4;
            // 
            // pnl_chart
            // 
            this.pnl_chart.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnl_chart.Controls.Add(this.btn_switchView);
            this.pnl_chart.Location = new System.Drawing.Point(222, 90);
            this.pnl_chart.Name = "pnl_chart";
            this.pnl_chart.Size = new System.Drawing.Size(1038, 656);
            this.pnl_chart.TabIndex = 5;
            // 
            // pnl_slideQuests
            // 
            this.pnl_slideQuests.AutoScroll = true;
            this.pnl_slideQuests.Location = new System.Drawing.Point(16, 90);
            this.pnl_slideQuests.Name = "pnl_slideQuests";
            this.pnl_slideQuests.Size = new System.Drawing.Size(200, 656);
            this.pnl_slideQuests.TabIndex = 6;
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
            // lbl_titleChart
            // 
            this.lbl_titleChart.AutoSize = true;
            this.lbl_titleChart.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_titleChart.Location = new System.Drawing.Point(616, 734);
            this.lbl_titleChart.Name = "lbl_titleChart";
            this.lbl_titleChart.Size = new System.Drawing.Size(268, 25);
            this.lbl_titleChart.TabIndex = 1;
            this.lbl_titleChart.Text = "Biểu đồ lựa chọn đáp án";
            // 
            // TrackExam
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1284, 771);
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
            this.pnl_chart.ResumeLayout(false);
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
    }
}