using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace VideoResizerCutter
{
    public partial class CustomRes : Form
    {
        Form1 form1;
        public CustomRes(Form1 form1)
        {
            InitializeComponent();
            this.form1 = form1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!Regex.IsMatch(textBox1.Text, "^[0-9]{0,999}$"))
            {
                MessageBox.Show("Invalid number.");
                textBox1.Text = "";
                textBox1.BackColor = Color.LightCoral;
                return;
            }

            if (!Regex.IsMatch(textBox2.Text, "^[0-9]{0,999}$"))
            {
                MessageBox.Show("Invalid number.");
                textBox2.Text = "";
                textBox2.BackColor = Color.LightCoral;
                return;
            }

            string width = textBox1.Text;
            string height = textBox2.Text;
            form1.setResolution(width, height);
            this.Close();
        }
    }
}
