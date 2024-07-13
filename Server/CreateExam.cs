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
    public partial class CreateExam : Form
    {
        private ContextMenuStrip contextMenuStrip;
        private ExamBLL examBLL;
        private string placeholderSearch = "Tìm kiếm";
        private int user_id ;
        private readonly IServiceProvider _serviceProvider;
        private Exam selectExam;

        public CreateExam()
        {
            InitializeComponent();
            InitializeContextMenu();
            lstExam.Width = (int)(this.Size.Width * 0.25);
        }

        public void Initialize(int user_id, ExamBLL examBLL)
        {
            this.user_id = user_id;
            this.examBLL = examBLL;
            InitializeContextMenu();

            // Thực hiện các logic khởi tạo khác nếu cần thiết
        }

        private void InitializeContextMenu()
        {
            contextMenuStrip = new ContextMenuStrip();

            ToolStripMenuItem viewDetailsItem = new ToolStripMenuItem("Sửa");
            //viewDetailsItem.Click += ViewDetailsItem_Click;
            contextMenuStrip.Items.Add(viewDetailsItem);

            ToolStripMenuItem viewListItem = new ToolStripMenuItem("Thêm câu hỏi mới");
            //viewListItem.Click += ViewListItem_Click;
            contextMenuStrip.Items.Add(viewListItem);

        }

        private void lstExam_MouseClick(object sender, MouseEventArgs e)
        {
            
        }


        private async void CreateExam_Load(object sender, EventArgs e)
        {
           var examData = await this.examBLL.GetListExam();
            foreach (Exam item in examData)
            {
                ListViewItem listViewItem = new ListViewItem(item.code);
                listViewItem.SubItems.Add(item.name);
                listViewItem.SubItems.Add(item.questionCount.ToString());
                listViewItem.SubItems.Add(item.id.ToString());
                lstExam.Items.Add(listViewItem);
            }
        }

        private void btnGenerat_Click(object sender, EventArgs e)
        {
            string folderName = @"D:\exam.json";
            Console.WriteLine(folderName);
            ExamBLL.WriteToJsonFile(selectExam, folderName);
            MessageBox.Show("Lưu đề thành công");
            this.Close();
        }

        private async void lstExam_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstExam.SelectedItems.Count > 0)
            {
                int selectedIndex = lstExam.SelectedIndices[0];

                string codeExam = lstExam.Items[selectedIndex].SubItems[0].Text; 
                string nameExam = lstExam.Items[selectedIndex].SubItems[1].Text; 
                string idE = lstExam.Items[selectedIndex].SubItems[3].Text;
                var questions = await this.examBLL.GetExam(int.Parse(idE));
                selectExam = questions;
                string contentHtml = "";
                foreach (var question in questions.questions)
                {
                    contentHtml += "<div class='item'>" + question.question.Replace("\\\"", "'") + "</div>";
                }
                string htmlTemplate = $@"
                <html>
                    <head>
                        <title></title>
                        <style>
                            body {{
                                font-family: Arial, sans-serif;
                                margin: 0;
                                padding: 0;
                                box-sizing: border-box;
                                display: flex;
                                justify-content: center;
                                align-items: center;
                                height: 100vh;
                            }}
                            .fullwidth-div {{
                                width: 100%;
                                text-align: center;
                                padding: 20px; /* Khoảng cách bên trong tùy chọn */
                                box-sizing: border-box;
                               
                            }}
                            .content-container {{
                                display: inline-block;
                                text-align: left; /* Căn trái nội dung bên trong nếu cần */
                            }}
                            .item{{
                                 margin-bottom: 40px;
                                 padding: 20px;
                                 background-color: #f0f0f0; /* Màu nền tùy chọn */
                             }}
                        </style>
                    </head>
                    <body>
                        <div class='fullwidth-div'>
                            <div class='content-container'>
                                {contentHtml}
                            </div>
                        </div>
                    </body>
                </html>";
                Console.Write(htmlTemplate);
                wbrReview.DocumentText = htmlTemplate;
            }
        }

        private void CreateExam_Resize(object sender, EventArgs e)
        {
            lstExam.Width = (int)(this.Size.Width * 0.25);
            lstExam.Height = (int)(this.Size.Height - btnGenerat.Height - 50);
            btnGenerat.Location = new Point(0, lstExam.Bottom);
            wbrReview.Width = (int)(this.Size.Width * 0.75)-10;
            wbrReview.Height = lstExam.Height;
            wbrReview.Location =new Point(lstExam.Right,0);
        }
    }
}
