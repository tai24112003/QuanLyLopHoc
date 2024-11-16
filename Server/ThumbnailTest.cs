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

namespace Server
{
    public partial class ThumbnailTest : UserControl
    {
        public Test Test { get; set; }

        private readonly Action<ThumbnailTest> SelectEventNoti;
        private readonly Action<ThumbnailTest> DeleteTest;
        private bool IsSelect=false;
        public ThumbnailTest(Test test, bool isSelect, Action<ThumbnailTest> selectEventNoti, Action<ThumbnailTest> deleteTest)
        {
            InitializeComponent();
            Test = test;
            IsSelect = isSelect;
            SelectEventNoti = selectEventNoti;
            DeleteTest = deleteTest;

            ToolTip toolTip = new ToolTip();
            toolTip.SetToolTip(btn_des_ques, "Xóa đề");

            btn_des_ques.Click += (sender, e) => DeleteTest?.Invoke(this);

            UpdateUI();
            UpdateState();
        }

        private void panel1_Click(object sender, EventArgs e)
        {
            IsSelect = true;
            UpdateState();
            this.SelectEventNoti?.Invoke(this);
        }

        private void UpdateState()
        {
            this.BorderStyle =IsSelect?BorderStyle.Fixed3D: BorderStyle.FixedSingle;
            this.BackColor = IsSelect ? Color.Gray : Color.White;
        }

        public void ChangeSelectState()
        {
            IsSelect=!IsSelect;
            UpdateState();
        }

        public void UpdateUI()
        {
            lbl_test_name.Text = Test.Title;
        }
      
    }
}
