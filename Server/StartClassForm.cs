using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Server
{
    public partial class StartClassForm : Form
    {
        private readonly UserBLL _userBLL;
        private readonly SubjectBLL _subjectBLL;

        public StartClassForm(UserBLL userBLL, SubjectBLL subjectBLL)
        {
            InitializeComponent();

            this.BackColor = Color.Purple;
            this.TransparencyKey = Color.Purple;
            _userBLL = userBLL;
            _subjectBLL = subjectBLL;

            this.Load += new EventHandler(MainForm_Load);

        }


        private async void MainForm_Load(object sender, EventArgs e)
        {
            await SetupUserAutoComplete();
            await SetupSubjectAutoComplete();
        }

        private async Task SetupUserAutoComplete()
        {
            try
            {
                string role = "GV"; // Role cần lấy
                List<User> users = await _userBLL.GetListUser(role);

                AutoCompleteStringCollection userCollection = new AutoCompleteStringCollection();
                foreach (var user in users)
                {
                    userCollection.Add(user.Name);
                }
                txtName.AutoCompleteCustomSource = userCollection;
                txtName.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                txtName.AutoCompleteSource = AutoCompleteSource.CustomSource;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private async Task SetupSubjectAutoComplete()
        {
            try
            {
                List<Subject> subjects = await _subjectBLL.GetAllSubjects();

                AutoCompleteStringCollection subjectCollection = new AutoCompleteStringCollection();
                foreach (var subject in subjects)
                {
                    subjectCollection.Add(subject.name);
                }
                txtSubject.AutoCompleteCustomSource = subjectCollection;
                txtSubject.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                txtSubject.AutoCompleteSource = AutoCompleteSource.CustomSource;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void cbbSession_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbbSession.SelectedIndex == 0) { 
                cbbStart.SelectedIndex = 0;
                cbbEnd.SelectedIndex = 0;
            }
            else if (cbbSession.SelectedIndex == 1) { cbbStart.SelectedIndex = 1; cbbEnd.SelectedIndex = 1; }
            else if (cbbSession.SelectedIndex == 2) { cbbStart.SelectedIndex = 2; cbbEnd.SelectedIndex = 2; }
            else if(cbbSession.SelectedIndex == 3) { cbbStart.SelectedIndex = 3; cbbEnd.SelectedIndex = 3; }
            else { cbbEnd.SelectedIndex = -1; cbbStart.SelectedIndex = -1; }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            this.Hide();

        }
    }
}
