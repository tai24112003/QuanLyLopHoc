namespace Server
{
    partial class svForm
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
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.tenmay = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Ocung = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.CPU = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ram = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.mssv = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.IPC = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chuot = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.banphim = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.manhinh = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lv_client = new System.Windows.Forms.ListView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripButton4 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSplitButton5 = new System.Windows.Forms.ToolStripSplitButton();
            this.sendWork = new System.Windows.Forms.ToolStripSplitButton();
            this.sendFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reciveFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnLock = new System.Windows.Forms.ToolStripButton();
            this.btnUnlock = new System.Windows.Forms.ToolStripButton();
            this.randomStudent = new System.Windows.Forms.ToolStripButton();
            this.quickLauch = new System.Windows.Forms.ToolStripSplitButton();
            this.switchMode = new System.Windows.Forms.ToolStripButton();
            this.SlideShow = new System.Windows.Forms.ToolStripButton();
            this.StopSlideShow = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.AutoSize = false;
            this.toolStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(80, 80);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSplitButton5,
            this.sendWork,
            this.btnLock,
            this.btnUnlock,
            this.quickLauch,
            this.switchMode,
            this.randomStudent,
            this.SlideShow,
            this.StopSlideShow});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1371, 98);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStrip2
            // 
            this.toolStrip2.AutoSize = false;
            this.toolStrip2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(7)))), ((int)(((byte)(71)))), ((int)(((byte)(161)))));
            this.toolStrip2.Dock = System.Windows.Forms.DockStyle.Left;
            this.toolStrip2.ImageScalingSize = new System.Drawing.Size(100, 100);
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton4,
            this.toolStripButton3,
            this.toolStripButton2,
            this.toolStripButton1});
            this.toolStrip2.Location = new System.Drawing.Point(0, 98);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(100, 652);
            this.toolStrip2.TabIndex = 1;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // tenmay
            // 
            this.tenmay.Text = "Tên Máy";
            this.tenmay.Width = 90;
            // 
            // Ocung
            // 
            this.Ocung.Text = "Ổ Cứng";
            this.Ocung.Width = 90;
            // 
            // CPU
            // 
            this.CPU.Text = "CPU";
            this.CPU.Width = 90;
            // 
            // ram
            // 
            this.ram.Text = "Ram";
            this.ram.Width = 90;
            // 
            // mssv
            // 
            this.mssv.Text = "MSSV";
            this.mssv.Width = 90;
            // 
            // IPC
            // 
            this.IPC.Text = "IPC";
            this.IPC.Width = 90;
            // 
            // chuot
            // 
            this.chuot.Text = "Chuột";
            this.chuot.Width = 90;
            // 
            // banphim
            // 
            this.banphim.Text = "Bàn Phím";
            this.banphim.Width = 90;
            // 
            // manhinh
            // 
            this.manhinh.Text = "Màn Hình";
            this.manhinh.Width = 90;
            // 
            // lv_client
            // 
            this.lv_client.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lv_client.CausesValidation = false;
            this.lv_client.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.tenmay,
            this.Ocung,
            this.CPU,
            this.ram,
            this.mssv,
            this.IPC,
            this.chuot,
            this.banphim,
            this.manhinh});
            this.lv_client.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lv_client.FullRowSelect = true;
            this.lv_client.HideSelection = false;
            this.lv_client.Location = new System.Drawing.Point(103, 98);
            this.lv_client.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.lv_client.Name = "lv_client";
            this.lv_client.Size = new System.Drawing.Size(1013, 650);
            this.lv_client.TabIndex = 2;
            this.lv_client.UseCompatibleStateImageBehavior = false;
            this.lv_client.View = System.Windows.Forms.View.Details;
            this.lv_client.SelectedIndexChanged += new System.EventHandler(this.lv_client_SelectedIndexChanged);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // toolStripButton4
            // 
            this.toolStripButton4.AutoSize = false;
            this.toolStripButton4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton4.Image = global::Server.Properties.Resources.home_button;
            this.toolStripButton4.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.toolStripButton4.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton4.Margin = new System.Windows.Forms.Padding(0);
            this.toolStripButton4.Name = "toolStripButton4";
            this.toolStripButton4.Size = new System.Drawing.Size(50, 50);
            this.toolStripButton4.Text = "Refresh";
            this.toolStripButton4.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.AutoSize = false;
            this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton3.Image = global::Server.Properties.Resources.keyboard__1_;
            this.toolStripButton3.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(50, 50);
            this.toolStripButton3.Text = "Refresh";
            this.toolStripButton3.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.AutoSize = false;
            this.toolStripButton2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(7)))), ((int)(((byte)(71)))), ((int)(((byte)(161)))));
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton2.Image = global::Server.Properties.Resources.web;
            this.toolStripButton2.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(50, 50);
            this.toolStripButton2.Text = "Refresh";
            this.toolStripButton2.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.AutoSize = false;
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = global::Server.Properties.Resources.monitor__2_;
            this.toolStripButton1.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(50, 50);
            this.toolStripButton1.Text = "Refresh";
            this.toolStripButton1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // toolStripSplitButton5
            // 
            this.toolStripSplitButton5.AutoSize = false;
            this.toolStripSplitButton5.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.toolStripSplitButton5.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripSplitButton5.Image = global::Server.Properties.Resources.management_service;
            this.toolStripSplitButton5.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.toolStripSplitButton5.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripSplitButton5.Margin = new System.Windows.Forms.Padding(70, 1, 10, 2);
            this.toolStripSplitButton5.Name = "toolStripSplitButton5";
            this.toolStripSplitButton5.Size = new System.Drawing.Size(60, 70);
            this.toolStripSplitButton5.Text = "QL lớp";
            this.toolStripSplitButton5.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // sendWork
            // 
            this.sendWork.AutoSize = false;
            this.sendWork.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sendFileToolStripMenuItem,
            this.reciveFileToolStripMenuItem});
            this.sendWork.Font = new System.Drawing.Font("Arial", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sendWork.Image = global::Server.Properties.Resources.briefcase__1_;
            this.sendWork.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.sendWork.Margin = new System.Windows.Forms.Padding(15, 1, 40, 2);
            this.sendWork.Name = "sendWork";
            this.sendWork.Size = new System.Drawing.Size(70, 70);
            this.sendWork.Text = "Gửi/Thu Tệp";
            this.sendWork.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.sendWork.ButtonClick += new System.EventHandler(this.sendWork_ButtonClick);
            // 
            // sendFileToolStripMenuItem
            // 
            this.sendFileToolStripMenuItem.Name = "sendFileToolStripMenuItem";
            this.sendFileToolStripMenuItem.Size = new System.Drawing.Size(148, 26);
            this.sendFileToolStripMenuItem.Text = "Send File";
            this.sendFileToolStripMenuItem.Click += new System.EventHandler(this.sendFileToolStripMenuItem_Click);
            // 
            // reciveFileToolStripMenuItem
            // 
            this.reciveFileToolStripMenuItem.Name = "reciveFileToolStripMenuItem";
            this.reciveFileToolStripMenuItem.Size = new System.Drawing.Size(148, 26);
            this.reciveFileToolStripMenuItem.Text = "Recive File";
            this.reciveFileToolStripMenuItem.Click += new System.EventHandler(this.reciveFileToolStripMenuItem_Click);
            // 
            // btnLock
            // 
            this.btnLock.AutoSize = false;
            this.btnLock.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLock.Image = global::Server.Properties.Resources._lock;
            this.btnLock.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnLock.Margin = new System.Windows.Forms.Padding(15, 1, 10, 2);
            this.btnLock.Name = "btnLock";
            this.btnLock.Size = new System.Drawing.Size(70, 70);
            this.btnLock.Text = "Khóa Máy";
            this.btnLock.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // btnUnlock
            // 
            this.btnUnlock.AutoSize = false;
            this.btnUnlock.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUnlock.Image = global::Server.Properties.Resources.unlock;
            this.btnUnlock.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnUnlock.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnUnlock.Margin = new System.Windows.Forms.Padding(15, 1, 40, 2);
            this.btnUnlock.Name = "btnUnlock";
            this.btnUnlock.Size = new System.Drawing.Size(70, 70);
            this.btnUnlock.Text = "Mở khóa";
            this.btnUnlock.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // randomStudent
            // 
            this.randomStudent.AutoSize = false;
            this.randomStudent.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.randomStudent.Image = global::Server.Properties.Resources.dices;
            this.randomStudent.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.randomStudent.Margin = new System.Windows.Forms.Padding(15, 1, 10, 2);
            this.randomStudent.Name = "randomStudent";
            this.randomStudent.Size = new System.Drawing.Size(60, 70);
            this.randomStudent.Text = "Ngẫu nhiên";
            this.randomStudent.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // quickLauch
            // 
            this.quickLauch.AutoSize = false;
            this.quickLauch.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.quickLauch.Image = global::Server.Properties.Resources.send;
            this.quickLauch.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.quickLauch.Margin = new System.Windows.Forms.Padding(15, 1, 10, 2);
            this.quickLauch.Name = "quickLauch";
            this.quickLauch.Size = new System.Drawing.Size(70, 70);
            this.quickLauch.Text = "Mở Nhanh";
            this.quickLauch.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // switchMode
            // 
            this.switchMode.AutoSize = false;
            this.switchMode.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.switchMode.Image = global::Server.Properties.Resources.loading_arrow;
            this.switchMode.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.switchMode.Margin = new System.Windows.Forms.Padding(15, 1, 40, 2);
            this.switchMode.Name = "switchMode";
            this.switchMode.Size = new System.Drawing.Size(50, 70);
            this.switchMode.Text = "Đổi";
            this.switchMode.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.switchMode.Click += new System.EventHandler(this.toolStripButton6_Click);
            // 
            // SlideShow
            // 
            this.SlideShow.AutoSize = false;
            this.SlideShow.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SlideShow.Image = global::Server.Properties.Resources.slide_show;
            this.SlideShow.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.SlideShow.Margin = new System.Windows.Forms.Padding(15, 1, 10, 2);
            this.SlideShow.Name = "SlideShow";
            this.SlideShow.Size = new System.Drawing.Size(60, 70);
            this.SlideShow.Text = "Trình Chiếu";
            this.SlideShow.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.SlideShow.Click += new System.EventHandler(this.SlideShowClick);
            // 
            // StopSlideShow
            // 
            this.StopSlideShow.AutoSize = false;
            this.StopSlideShow.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StopSlideShow.Image = global::Server.Properties.Resources.slide_show;
            this.StopSlideShow.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.StopSlideShow.Margin = new System.Windows.Forms.Padding(15, 1, 10, 2);
            this.StopSlideShow.Name = "StopSlideShow";
            this.StopSlideShow.Size = new System.Drawing.Size(60, 70);
            this.StopSlideShow.Text = "Dừng Chiếu";
            this.StopSlideShow.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.StopSlideShow.Click += new System.EventHandler(this.StopSlideShow_Click);
            // 
            // svForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ClientSize = new System.Drawing.Size(1371, 750);
            this.Controls.Add(this.lv_client);
            this.Controls.Add(this.toolStrip2);
            this.Controls.Add(this.toolStrip1);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "svForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Server Form";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.serverForm_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lv_client_MouseClick);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripSplitButton toolStripSplitButton5;
        private System.Windows.Forms.ToolStripSplitButton sendWork;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripButton toolStripButton4;
        private System.Windows.Forms.ToolStripButton toolStripButton3;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ColumnHeader tenmay;
        private System.Windows.Forms.ColumnHeader Ocung;
        private System.Windows.Forms.ColumnHeader CPU;
        private System.Windows.Forms.ColumnHeader ram;
        private System.Windows.Forms.ColumnHeader mssv;
        private System.Windows.Forms.ColumnHeader IPC;
        private System.Windows.Forms.ColumnHeader chuot;
        private System.Windows.Forms.ColumnHeader banphim;
        private System.Windows.Forms.ColumnHeader manhinh;
        private System.Windows.Forms.ListView lv_client;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripButton btnUnlock;
        private System.Windows.Forms.ToolStripSplitButton quickLauch;
        private System.Windows.Forms.ToolStripButton switchMode;
        private System.Windows.Forms.ToolStripButton SlideShow;
        private System.Windows.Forms.ToolStripButton StopSlideShow;
        private System.Windows.Forms.ToolStripMenuItem sendFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reciveFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton btnLock;
        private System.Windows.Forms.ToolStripButton randomStudent;
    }
}

