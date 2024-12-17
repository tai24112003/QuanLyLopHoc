using DAL.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Encoder = System.Drawing.Imaging.Encoder;


namespace Server
{
    public partial class QuestionInfo : UserControl
    {   
        public Quest Quest { set; get; }

        private bool IsChangedImage { get; set; }
        public QuestionInfo(Quest quest)
        {
            InitializeComponent();
            Quest = quest;
            IsChangedImage = false;
            Init();
        }

        private void Init()
        {
            if (Quest.ImageData != null)
            {
                using (var ms = new MemoryStream(Quest.ImageData))
                {
                    // Chuyển byte array thành Image và hiển thị trong PictureBox
                    ptb_image.Image = Image.FromStream(ms);
                    ptb_image.SizeMode = PictureBoxSizeMode.Zoom;
                    lbl_imageSize.Text = (ms.Length / (1024.0 * 1024.0)).ToString("F2") + " MB";
                }
            }
            else
            {
                ptb_image.Image = Properties.Resources.img_default;
                ptb_image.SizeMode = PictureBoxSizeMode.Zoom;
                lbl_imageSize.Text = (0 / (1024.0 * 1024.0)).ToString("F2") + " MB";
            }

            txt_ques_typing.Text=Quest.Content;
            foreach (Result item in Quest.Results)
            {
                pnl_answers.Height = (Quest.Results.Count+1) / 2 * 96;
                pnl_answers.Controls.Add(new ResultInfo(item, CheckQuestType));
            }
        }

        public void GetNewQuestContent()
        {
            Quest.Content = txt_ques_typing.Text.Trim();
            if (IsChangedImage)
            {
                long q = 100L;
                if (ptb_image.Image != null)
                {
                    using (var ms = new MemoryStream())
                    {
                        SaveImageWithQuality(ptb_image.Image, ms, q);

                        // Kiểm tra dung lượng tệp, nếu tệp > 1 MB thì giảm chất lượng thêm
                        while (ms.Length > 1 * 1024 * 1024) // Nếu dung lượng tệp > 1 MB
                        {
                            q -= 10L;
                            ms.SetLength(0); // Đặt lại MemoryStream

                            SaveImageWithQuality(ptb_image.Image, ms, q);
                        }

                        // Chuyển dữ liệu trong MemoryStream thành byte array
                        Quest.ImageData = ms.ToArray();
                        lbl_imageSize.Text = (ms.Length / (1024.0 * 1024.0)).ToString("F2") + " MB";
                    }
                }
                IsChangedImage = false;
            }
            GetNewResultContent();
        }

        private void GetNewResultContent()
        {
            Quest.Results.Clear();

            foreach (ResultInfo item in pnl_answers.Controls) { 
                item.GetNewResultInfo();
                int newId = Quest.CreateIndexResultInQuest();
                item.Result.Id = newId;
                Quest.Results.Add(item.Result);
            }
        }
        private void SaveImageWithQuality(Image image, MemoryStream ms, long quality)
        {
            // Lấy thông tin codec của JPEG
            ImageCodecInfo jpgEncoder = GetEncoderInfo("image/jpeg");

            // Thiết lập tham số chất lượng (0-100, 100 là chất lượng cao nhất)
            EncoderParameter qualityParam = new EncoderParameter(Encoder.Quality, quality);
            EncoderParameters encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = qualityParam;

            // Lưu hình ảnh vào MemoryStream với chất lượng giảm
            image.Save(ms, jpgEncoder, encoderParams);
        }

        private ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            // Lấy thông tin codec cho định dạng MIME
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.MimeType == mimeType)
                {
                    return codec;
                }
            }
            return null;
        }
        private bool CheckQuestType()
        {
            if (Quest.Type == QuestType.SingleSeclect)
            {
                foreach (ResultInfo item in pnl_answers.Controls)
                {
                    if (item.GetState())
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private void btn_add_result_Click(object sender, EventArgs e)
        {
            int numResultContro = pnl_answers.Controls.Count;
            if (numResultContro == 6) { 
                return;
            }
            pnl_answers.Controls.Add(new ResultInfo(new Result(numResultContro), CheckQuestType));

            pnl_answers.Height = (numResultContro+2) / 2 * 96;

        }
        private void btn_del_result_Click(object sender, EventArgs e)
        {
            int numResultContro = pnl_answers.Controls.Count;
            if (numResultContro == 4)
            {
                return;
            }
            pnl_answers.Controls.RemoveAt(--numResultContro);

            pnl_answers.Height = (numResultContro+1) / 2 * 96;

        }
        private void txt_ques_typing_TextChanged(object sender, EventArgs e)
        {
            int fixedWidth = txt_ques_typing.Width;

            // Đo kích thước của văn bản với chiều rộng cố định và phông chữ hiện tại
            Size textSize = TextRenderer.MeasureText(txt_ques_typing.Text, txt_ques_typing.Font,
                                                      new Size(fixedWidth, int.MaxValue),
                                                      TextFormatFlags.WordBreak);

            // Đặt lại chiều cao của TextBox để vừa với nội dung
            txt_ques_typing.Height = Math.Min(textSize.Height + 20, 135);
        }

        private void ptb_image_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Chọn file hình ảnh",
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Kiểm tra nếu tệp hình ảnh hợp lệ
                if (!string.IsNullOrEmpty(openFileDialog.FileName))
                {
                    try
                    {
                        FileInfo fileInfo = new FileInfo(openFileDialog.FileName);
                        long fileSizeInBytes = fileInfo.Length;
                        lbl_imageSize.Text = (fileSizeInBytes / (1024.0 * 1024.0)).ToString("F2") + " MB";

                        // Mở hình ảnh và hiển thị trong PictureBox
                        ptb_image.Image = new Bitmap(openFileDialog.FileName);
                        ptb_image.SizeMode = PictureBoxSizeMode.Zoom;
                        IsChangedImage = true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi tải hình ảnh: " + ex.Message);
                    }
                }
            }
        }
    }
}
