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
    public partial class Rotate : Form
    {
        public int Angle { get; private set; }
        public string Direction { get; private set; }
        public Rotate()
        {
            InitializeComponent();
            directionComboBox.SelectedIndex = 0;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Angle = (int)numericUpDown.Value;
            Direction = directionComboBox.SelectedItem.ToString();
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
