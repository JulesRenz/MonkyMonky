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

        private string originalFileName;
        public FormNewFileDialog(string originalFileName)
        {
            InitializeComponent();
            this.originalFileName = originalFileName;
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

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            //https://msdn.microsoft.com/de-de/library/system.windows.forms.keypresseventargs.keychar(v=vs.110).aspx
            if(e.KeyChar == (char)Keys.Return)
            {
                button1.PerformClick();
            }
        }

        private void FormNewFileDialog_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            this.Show();
            this.WindowState = FormWindowState.Normal;

            textBox1.Text = originalFileName;
            textBox1.Focus();
            textBox1.SelectAll();
        }
    }
}
