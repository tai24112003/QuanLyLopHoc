namespace Server
{
    partial class ThumbnailTest
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.btn_des_ques = new System.Windows.Forms.Button();
            this.lbl_test_name = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.btn_des_ques);
            this.panel1.Controls.Add(this.lbl_test_name);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(2, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(120, 56);
            this.panel1.TabIndex = 1;
            this.panel1.Click += new System.EventHandler(this.panel1_Click);
            // 
            // btn_des_ques
            // 
            this.btn_des_ques.BackgroundImage = global::Server.Properties.Resources.close;
            this.btn_des_ques.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btn_des_ques.Location = new System.Drawing.Point(91, -1);
            this.btn_des_ques.Name = "btn_des_ques";
            this.btn_des_ques.Size = new System.Drawing.Size(28, 26);
            this.btn_des_ques.TabIndex = 9;
            this.btn_des_ques.UseVisualStyleBackColor = true;
            // 
            // lbl_test_name
            // 
            this.lbl_test_name.AutoSize = true;
            this.lbl_test_name.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_test_name.Location = new System.Drawing.Point(8, 24);
            this.lbl_test_name.Name = "lbl_test_name";
            this.lbl_test_name.Size = new System.Drawing.Size(44, 16);
            this.lbl_test_name.TabIndex = 0;
            this.lbl_test_name.Text = "label1";
            this.lbl_test_name.Click += new System.EventHandler(this.panel1_Click);
            // 
            // ThumbnailTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Controls.Add(this.panel1);
            this.Name = "ThumbnailTest";
            this.Padding = new System.Windows.Forms.Padding(2);
            this.Size = new System.Drawing.Size(124, 60);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lbl_test_name;
        private System.Windows.Forms.Button btn_des_ques;
    }
}
