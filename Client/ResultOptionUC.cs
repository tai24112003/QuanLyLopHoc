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
        private string Title { get; set; }
        private readonly  Func<Result, bool> SelectAnswer;
        public ResultOptionUC(Result result, Func<Result, bool> selectAnswer, string title)
        {
            InitializeComponent();

            SelectAnswer = selectAnswer;
            this.Result = result;
            Title = title;
            lbl_result.Text =this.Title+". "+ result.Content;
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
            this.lbl_result.BackColor = IsSelect ? Color.Blue: Color.LightGreen;
            this.lbl_result.ForeColor = IsSelect ? Color.White: Color.Black;
        }

        private void ResultOptionUC_EnabledChanged(object sender, EventArgs e)
        {
            if (this.Enabled)
            {
                this.lbl_result.BackColor = Color.LightGreen; // Màu nền khi được bật
            }
            else if(!IsSelect)
            {
                this.lbl_result.BackColor = Color.LightGray; // Màu nền khi bị vô hiệu hóa
            }
        }
    }
}
