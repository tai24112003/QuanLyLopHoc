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
        private Result Result { get; set; }
        private bool IsSelect {  get; set; }
        private readonly  Func<Result, bool> SelectAnswer;
        public ResultOptionUC(Result result, Func<Result, bool> selectAnswer)
        {
            InitializeComponent();

            SelectAnswer = selectAnswer;
            this.Result = result;
            lbl_result.Text = result.Content;
            IsSelect=false;

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

            UpdateState();
        }

        private void lbl_result_Click(object sender, EventArgs e)
        {
            if (!SelectAnswer(Result)) return;

            IsSelect =!IsSelect;
            UpdateState();
        
        
        }

        private void UpdateState()
        {
            this.BorderStyle =IsSelect?BorderStyle.Fixed3D:BorderStyle.FixedSingle;
            this.lbl_result.BackColor = IsSelect ? Color.LightGray: Color.AntiqueWhite;
        }
    }
}
