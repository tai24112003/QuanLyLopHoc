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
        public Result Result { get; set; }
        public bool IsSelect {  get; set; }
        private string Title { get; set; }
        private bool IsShowResult {  get; set; }
        private readonly  Func<Result, bool> SelectAnswer;
        public ResultOptionUC(Result result, Func<Result, bool> selectAnswer, string title, bool state)
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
            IsShowResult = state;
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

        public void UpdateState()
        {
            this.BorderStyle =IsSelect?BorderStyle.Fixed3D:BorderStyle.FixedSingle;
            this.lbl_result.BackColor = IsSelect ? Color.Blue: Color.FromArgb(255,128,128,128);
            this.lbl_result.ForeColor = IsSelect ? Color.White : Color.White;
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
            else if(IsShowResult)
            {
               this.lbl_result.BackColor=Result.IsCorrect?Color.LightGreen : Color.Red;
            }
        }
    }
}
