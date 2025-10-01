using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HW4
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        bool flag = false;
        int rotseed = 0;
        private void table9b9_btn_Click(object sender, EventArgs e)
        {
            if (!flag)
            {
                label1.Text = gen9b9();
                flag = true;
            }
            switch (rotseed)
            {
                case 0:
                    label1.ForeColor = Color.Red;
                    break;
                case 1:
                    label1.ForeColor = Color.Green;
                    break;
                case 2:
                    label1.ForeColor = Color.Blue;
                    break;
            }
            rotseed = (rotseed + 1) % 3;
        }

        private string gen9b9()
        {
            int n = 9;
            string result = "";
            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= n; j++)
                {
                    if (j == 1)
                        result += $"{i}x{j}={i * j}";
                    else
                        result += $" {i}x{j}={i * j,2}";
                }
                result += "\n";
            }
            return result;
        }
    }
}
