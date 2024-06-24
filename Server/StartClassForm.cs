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
                List<User> users = await _userBLL.GetUsersListByRole(role);

                AutoCompleteStringCollection userCollection = new AutoCompleteStringCollection();
                foreach (var user in users)
                {
                    userCollection.Add(user.Name);
                }

                cbName.AutoCompleteCustomSource = userCollection;
                cbName.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                cbName.AutoCompleteSource = AutoCompleteSource.CustomSource;
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

                cbSubject.AutoCompleteCustomSource = subjectCollection;
                cbSubject.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                cbSubject.AutoCompleteSource = AutoCompleteSource.CustomSource;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
