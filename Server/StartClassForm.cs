// /Forms/StartClassForm.cs
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server
{
    public partial class StartClassForm : Form
    {
        private readonly UserBLL _userBLL;
        private readonly SubjectBLL _subjectBLL;
        private readonly ClassSessionController _classSessionController;

        public StartClassForm(UserBLL userBLL, SubjectBLL subjectBLL, ClassSessionController classSessionController)
        {
            InitializeComponent();

            this.BackColor = Color.Purple;
            this.TransparencyKey = Color.Purple;
            _userBLL = userBLL;
            _subjectBLL = subjectBLL;
            _classSessionController = classSessionController;

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
                string role = "GV";
                List<User> users = await _userBLL.GetListUser(role);

                AutoCompleteStringCollection userCollection = new AutoCompleteStringCollection();
                foreach (var user in users)
                {
                    userCollection.Add(user.name);
                }

                cbbName.DataSource = users;
                cbbName.AutoCompleteCustomSource = userCollection;
                cbbName.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                cbbName.AutoCompleteSource = AutoCompleteSource.CustomSource;
                cbbName.DisplayMember = "name";
                cbbName.ValueMember = "user_id";
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
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

                cbbSubject.DataSource = subjects;
                cbbSubject.AutoCompleteCustomSource = subjectCollection;
                cbbSubject.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                cbbSubject.AutoCompleteSource = AutoCompleteSource.CustomSource;
                cbbSubject.DisplayMember = "name";
                cbbSubject.ValueMember = "id";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void cbbSession_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbbSession.SelectedIndex == 0)
            {
                cbbStart.SelectedIndex = 0;
                cbbEnd.SelectedIndex = 0;
            }
            else if (cbbSession.SelectedIndex == 1)
            {
                cbbStart.SelectedIndex = 1;
                cbbEnd.SelectedIndex = 1;
            }
            else if (cbbSession.SelectedIndex == 2)
            {
                cbbStart.SelectedIndex = 2;
                cbbEnd.SelectedIndex = 2;
            }
            else if (cbbSession.SelectedIndex == 3)
            {
                cbbStart.SelectedIndex = 3;
                cbbEnd.SelectedIndex = 3;
            }
            else
            {
                cbbEnd.SelectedIndex = -1;
                cbbStart.SelectedIndex = -1;
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void btnSubmit_Click(object sender, EventArgs e)
        {
            this.Hide();

            string className = txtClass.Text + " - " + cbbSubject.SelectedItem.ToString();
            int userID = int.Parse(cbbName.SelectedValue.ToString());
            string roomID = "F71";
            try
            {
                ClassSession classSession = new ClassSession
                {
                    ClassName = className,
                    Session = cbbSession.SelectedIndex + 1,
                    StartTime = cbbStart.SelectedItem.ToString(),
                    EndTime = cbbEnd.SelectedItem.ToString(),
                    user_id = userID,
                    RoomID = roomID
                };

                await _classSessionController.StartNewClassSession(classSession);

                var roomBLL = ServiceLocator.ServiceProvider.GetRequiredService<RoomBLL>();

                svForm svForms = new svForm(userID, roomID, roomBLL);
                svForms.Show();

                Console.WriteLine("Class session started successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to start class session: " + ex.Message);
            }
        }

        private void cbbName_SelectedIndexChanged(object sender, EventArgs e)
        {
            MessageBox.Show(cbbName.SelectedValue + " - " + cbbName.SelectedText);
        }

        private void cbbSubject_SelectedIndexChanged(object sender, EventArgs e)
        {
            MessageBox.Show(cbbSubject.SelectedValue + " - " + cbbSubject.SelectedText);

        }
    }
}
