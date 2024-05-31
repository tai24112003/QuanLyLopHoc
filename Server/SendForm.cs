using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server
{
    public partial class SendForm : Form
    {
        public event Action<List<string>, string> FilesSelected;

        public SendForm()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "d:\\";
                openFileDialog.Filter = "All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;
                openFileDialog.Multiselect = true; // Allow selecting multiple files

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string[] filePaths = openFileDialog.FileNames;
                    // Display selected file paths
                    txtFilePath.Text = string.Join(Environment.NewLine, filePaths);
                }
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string[] filePaths = txtFilePath.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            string toPath = txtToPath.Text;
            FilesSelected?.Invoke(filePaths.ToList(), toPath); // Trigger event with selected file paths
            this.Close();
        }

        private void btnCancle_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
