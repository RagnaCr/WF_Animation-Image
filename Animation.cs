using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SP_coursework
{
    public partial class Animation : Form
    {
        public List<string> ImagePaths { get; private set; }
        public int Interval { get; private set; }
        public Animation()
        {
            InitializeComponent();
        }
        private void addButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Multiselect = true;
                openFileDialog.Filter = "Image Files|*.bmp;*.jpg;*.png;*.gif;*.tiff";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    foreach (var file in openFileDialog.FileNames)
                    {
                        imagePathsListBox.Items.Add(file);
                    }
                }
            }
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            var selectedItems = imagePathsListBox.SelectedItems.Cast<string>().ToList();
            foreach (var item in selectedItems)
            {
                imagePathsListBox.Items.Remove(item);
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            ImagePaths = imagePathsListBox.Items.Cast<string>().ToList();
            Interval = (int)intervalNumericUpDown.Value;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
