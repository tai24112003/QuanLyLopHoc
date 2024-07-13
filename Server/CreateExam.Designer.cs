
namespace Server
{
    partial class CreateExam
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
            this.btnGenerat = new System.Windows.Forms.Button();
            this.lstExam = new System.Windows.Forms.ListView();
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.wbrReview = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // btnGenerat
            // 
            this.btnGenerat.BackColor = System.Drawing.Color.DodgerBlue;
            this.btnGenerat.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGenerat.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.btnGenerat.Location = new System.Drawing.Point(2, 489);
            this.btnGenerat.Margin = new System.Windows.Forms.Padding(4);
            this.btnGenerat.Name = "btnGenerat";
            this.btnGenerat.Size = new System.Drawing.Size(209, 52);
            this.btnGenerat.TabIndex = 36;
            this.btnGenerat.Text = "Lấy đề";
            this.btnGenerat.UseVisualStyleBackColor = false;
            this.btnGenerat.Click += new System.EventHandler(this.btnGenerat_Click);
            // 
            // lstExam
            // 
            this.lstExam.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6});
            this.lstExam.HideSelection = false;
            this.lstExam.Location = new System.Drawing.Point(2, 3);
            this.lstExam.Margin = new System.Windows.Forms.Padding(4);
            this.lstExam.Name = "lstExam";
            this.lstExam.Size = new System.Drawing.Size(454, 478);
            this.lstExam.TabIndex = 25;
            this.lstExam.UseCompatibleStateImageBehavior = false;
            this.lstExam.View = System.Windows.Forms.View.Details;
            this.lstExam.SelectedIndexChanged += new System.EventHandler(this.lstExam_SelectedIndexChanged);
            this.lstExam.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lstExam_MouseClick);
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "#";
            this.columnHeader4.Width = 35;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Tên";
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Số câu";
            // 
            // wbrReview
            // 
            this.wbrReview.Location = new System.Drawing.Point(463, 3);
            this.wbrReview.MinimumSize = new System.Drawing.Size(20, 20);
            this.wbrReview.Name = "wbrReview";
            this.wbrReview.Size = new System.Drawing.Size(566, 250);
            this.wbrReview.TabIndex = 37;
            // 
            // CreateExam
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1041, 554);
            this.Controls.Add(this.wbrReview);
            this.Controls.Add(this.lstExam);
            this.Controls.Add(this.btnGenerat);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "CreateExam";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CreateExam";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.CreateExam_Load);
            this.Resize += new System.EventHandler(this.CreateExam_Resize);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnGenerat;
        private System.Windows.Forms.ListView lstExam;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.WebBrowser wbrReview;
    }
}