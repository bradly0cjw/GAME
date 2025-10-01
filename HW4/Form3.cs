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
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Refresh();

            Graphics g = this.CreateGraphics();
            g.Clear(Color.Black);
            Task.Run(() => DrawAxis());
            Task.Run(() => DrawSineWave());
            Task.Run(() => DrawCosineWave());


        }

        private void DrawAxis()
        {
                       // create X axis
            Pen p = new Pen(Color.Cyan);
            p.Width = 3;
            Graphics g = this.CreateGraphics();
            g.DrawLine(p, 50, 300, 750, 300);
        }

        private void DrawSineWave()
        {
            // create Sine wave
            Pen p2 = new Pen(Color.Red);
            p2.Width = 3;
            Graphics g = this.CreateGraphics();
            for (int x = 0; x < 700; x++)
            {
                int y = (int)(100 * Math.Sin((x * 2 * Math.PI) / 180));
                g.DrawLine(p2, 50 + x, 300 - y, 51 + x, 300 - (int)(100 * Math.Sin(((x + 1) * 2 * Math.PI) / 180)));
            }
        }

        private void DrawCosineWave()
        {
            // create Cosine wave
            Pen p3 = new Pen(Color.Yellow);
            p3.Width = 3;
            Graphics g = this.CreateGraphics();
            for (int x = 0; x < 700; x++)
            {
                int y = (int)(100 * Math.Cos((x * 2 * Math.PI) / 180));
                g.DrawLine(p3, 50 + x, 300 - y, 51 + x, 300 - (int)(100 * Math.Cos(((x + 1) * 2 * Math.PI) / 180)));
            }
        }
    }
}
