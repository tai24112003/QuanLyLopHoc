using DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace testUdpTcp
{
    public partial class ResultOptionUC : UserControl
    {
        private Result Result;
        private readonly Action<Result> SelectAnswer;
        public ResultOptionUC(Result result, Action<Result> selectAnswer)
        {
            InitializeComponent();

            SelectAnswer = selectAnswer;
            this.Result = result;
            lbl_result.Text = result.Content;
            ConfigureLabel();
            ToolTip toolTip = new ToolTip();
            toolTip.SetToolTip(lbl_result, Result.Content);
        }

        public void ChangeSize( int pW, int pH)
        {
            this.Width = pW / 2 - 6;
        }
        private void ConfigureLabel()
        {
            // Giả sử lblContent là Label bên trong UserControl
            lbl_result.AutoSize = false;
            lbl_result.Dock = DockStyle.Fill; // Để lấp đầy UserControl
            lbl_result.TextAlign = ContentAlignment.MiddleCenter; // Căn giữa nội dung
            lbl_result.AutoEllipsis = true; // Hiển thị dấu "..." khi văn bản quá dài
            lbl_result.BackColor = Color.AntiqueWhite;  
        }

        private void lbl_result_Click(object sender, EventArgs e)
        {   
            this.BorderStyle = BorderStyle.Fixed3D;
            this.lbl_result.BackColor = Color.LightGray;
            SelectAnswer?.Invoke(Result);
        }

    }
}
