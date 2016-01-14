using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;

namespace MonkyMonky
{
    public partial class FormNewFileDialog : Form
    {
        public string newFileName { get; set; }
        public FormNewFileDialog(string originalFileName)
        {
            InitializeComponent();
            textBox1.Text = originalFileName;
            textBox1.Focus();
            textBox1.SelectAll();
        }

        public void Form2_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(textBox1.Text != String.Empty)
            {
                this.newFileName = textBox1.Text;
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
        }
    }
}
