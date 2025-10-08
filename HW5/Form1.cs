using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HW5
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

        }

        private void button_Rec_Click(object sender, EventArgs e)
        {
            //gen_ten_rectangles();
            gen_ten_rectangle_without_collision_in_panel();
        }

        private void button_circle_Click(object sender, EventArgs e)
        {
            //gen_ten_circles();
            gen_ten_circle_all_in_Panel();
        }

        enum Color { Red, Green, Blue, Yellow, Orange, Purple, Black, White, Gray, Brown };

        private System.Drawing.Color GetSaturatedColor(Color color)
        {
            switch (color)
            {
                case Color.Red:
                    return System.Drawing.Color.FromArgb(255, 0, 0);
                case Color.Green:
                    return System.Drawing.Color.FromArgb(0, 255, 0);
                case Color.Blue:
                    return System.Drawing.Color.FromArgb(0, 0, 255);
                case Color.Yellow:
                    return System.Drawing.Color.FromArgb(255, 255, 0);
                case Color.Orange:
                    return System.Drawing.Color.FromArgb(255, 140, 0);
                case Color.Purple:
                    return System.Drawing.Color.FromArgb(128, 0, 128);
                case Color.Black:
                    return System.Drawing.Color.FromArgb(0, 0, 0);
                case Color.White:
                    return System.Drawing.Color.FromArgb(255, 255, 255);
                case Color.Gray:
                    return System.Drawing.Color.FromArgb(128, 128, 128);
                case Color.Brown:
                    return System.Drawing.Color.FromArgb(139, 69, 19);
                default:
                    return System.Drawing.Color.Black;
            }
        }

        private void gen_ten_circles()
        {

            Random rand = new Random();
            Graphics g = panel1.CreateGraphics();
            g.Clear(System.Drawing.Color.FromArgb(0xad, 0xd8, 0xe6));
            for (int i = 0; i < 10; i++)
            {
                int x = rand.Next(0, panel1.Width - 50);
                int y = rand.Next(0, panel1.Height - 50);
                int diameter = rand.Next(20, 100);
                Color color = (Color)rand.Next(Enum.GetNames(typeof(Color)).Length);
                //Brush brush = new SolidBrush(System.Drawing.Color.FromName(color.ToString()));
                // only stroke
                Pen pen = new Pen(GetSaturatedColor(color));
                g.DrawEllipse(pen, x, y, diameter, diameter);
                //g.FillEllipse(brush, x, y, diameter, diameter);
            }
        }

        private void gen_ten_rectangles()
        {
            Random rand = new Random();
            Graphics g = panel1.CreateGraphics();
            g.Clear(System.Drawing.Color.FromArgb(0xad, 0xd8, 0xe6));
            for (int i = 0; i < 10; i++)
            {
                int x = rand.Next(0, panel1.Width - 50);
                int y = rand.Next(0, panel1.Height - 50);
                int width = rand.Next(20, 100);
                int height = rand.Next(20, 100);
                Color color = (Color)rand.Next(Enum.GetNames(typeof(Color)).Length);
                //Brush brush = new SolidBrush(System.Drawing.Color.FromName(color.ToString()));
                // only stroke
                Pen pen = new Pen(GetSaturatedColor(color));
                g.DrawRectangle(pen, x, y, width, height);
                //g.FillRectangle(brush, x, y, width, height);
            }
        }

        private void gen_ten_circle_all_in_Panel()
        {
            Random rand = new Random();
            Graphics g = panel1.CreateGraphics();
            g.Clear(System.Drawing.Color.FromArgb(0xad, 0xd8, 0xe6));
            for (int i = 0; i < 10; i++)
            {
                int diameter = rand.Next(20, 100);
                int x = rand.Next(0, panel1.Width - diameter);
                int y = rand.Next(0, panel1.Height - diameter);
                Color color = (Color)rand.Next(Enum.GetNames(typeof(Color)).Length);
                //Brush brush = new SolidBrush(System.Drawing.Color.FromName(color.ToString()));
                // only stroke
                Pen pen = new Pen(GetSaturatedColor(color));
                g.DrawEllipse(pen, x, y, diameter, diameter);
                //g.FillEllipse(brush, x, y, diameter, diameter);
            }
        }

        private bool AABB(Rectangle a, Rectangle b)
        {
            return a.Left < b.Right &&
                   a.Right > b.Left &&
                   a.Top < b.Bottom &&
                   a.Bottom > b.Top;
        }

        private void gen_ten_rectangle_without_collision_in_panel()
        {
            Random rand = new Random();
            Graphics g = panel1.CreateGraphics();
            g.Clear(System.Drawing.Color.FromArgb(0xad, 0xd8, 0xe6));
            List<Rectangle> rectangles = new List<Rectangle>();
            for (int i = 0; i < 10; i++)
            {
                int width = rand.Next(20, 100);
                int height = rand.Next(20, 100);
                int x, y;
                Rectangle newRect;
                bool intersects;
                do
                {
                    x = rand.Next(0, panel1.Width - width);
                    y = rand.Next(0, panel1.Height - height);
                    newRect = new Rectangle(x, y, width, height);
                    intersects = rectangles.Any(r => AABB(r, newRect));
                } while (intersects);
                rectangles.Add(newRect);
                Color color = (Color)rand.Next(Enum.GetNames(typeof(Color)).Length);
                Pen pen = new Pen(GetSaturatedColor(color));
                g.DrawRectangle(pen, newRect);
            }
        }
    }
}
