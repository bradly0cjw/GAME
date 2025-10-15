using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HW6
{
    public partial class Form2 : Form
    {
        private Timer animationTimer = new Timer();
        Random rand = new Random();

        public struct velocity
        {
            public int x;
            public int y;
        }
        class Shape
        {
            public int x;
            public int y;
            public velocity ve = new velocity();
            public Color c;
        }

        class Circle : Shape
        {
            public int r;
            public Circle(int x, int y, int r, Color c)
            {
                this.x = x;
                this.y = y;
                this.r = r;
                this.c = c;
            }
        }
        class Rectangle : Shape
        {
            public int w;
            public int h;
            public Rectangle(int x, int y, int w, int h, Color c)
            {
                this.x = x;
                this.y = y;
                this.w = w;
                this.h = h;
                this.c = c;
            }
        }

        List<Shape> shapes = new List<Shape>();

        public Form2()
        {
            InitializeComponent();
            animationTimer.Interval = 30;
            animationTimer.Tick += AnimationTimer_Tick;
            doubleBufferedPanel1.Paint += panel1_Paint;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //for (int i = 0; i < 10; i++)
            //{
            //    Gensquare();
            //}
            //for (int i = 0; i < 6; i++)
            //{
            //    GenCirce();
            //}
            //Task.Run(() =>
            //{
            //    while (true)
            //    {
            //        bouncePhysics();
            //        panel1_Paint();
            //        System.Threading.Thread.Sleep(100);
            //    }
            //});

            if (animationTimer.Enabled)
            {
                animationTimer.Stop();
                shapes.Clear();
            }

            for (int i = 0; i < 6; i++)
            {
                Gensquare();
            }
            for (int i = 0; i < 6; i++)
            {
                GenCirce();
            }
            animationTimer.Start();
        }

        private void Gensquare()
        {

            int x = rand.Next(0, doubleBufferedPanel1.Width - 50);
            int y = rand.Next(0, doubleBufferedPanel1.Height - 50);
            int w = rand.Next(20, doubleBufferedPanel1.Width / 5);
            int h = rand.Next(20, doubleBufferedPanel1.Height / 5);
            Color c = Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256));
            Rectangle rect = new Rectangle(x, y, w, h, c);
            adjustInvalidShape(rect);
            rect.ve.x = rand.Next(-5, 6);
            rect.ve.y = rand.Next(-5, 6);
            shapes.Add(rect);

        }
        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            bouncePhysics();
            doubleBufferedPanel1.Invalidate();
        }

        private void GenCirce()
        {
            int x = rand.Next(20, doubleBufferedPanel1.Width - 20);
            int y = rand.Next(20, doubleBufferedPanel1.Height - 20);
            int r = rand.Next(10, Math.Min(doubleBufferedPanel1.Width, doubleBufferedPanel1.Height) / 5);
            Color c = Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256));
            Circle circle = new Circle(x, y, r, c);
            adjustInvalidShape(circle);
            circle.ve.x = rand.Next(-5, 6);
            circle.ve.y = rand.Next(-5, 6);
            shapes.Add(circle);
        }

        private void adjustInvalidShape(Shape s)
        {
            if (s == null)
                return;
            if (s is Circle circle)
            {
                if (circle.x - circle.r < 0)
                    circle.x = circle.r;
                if (circle.x + circle.r > doubleBufferedPanel1.Width)
                    circle.x = doubleBufferedPanel1.Width - circle.r;
                if (circle.y - circle.r < 0)
                    circle.y = circle.r;
                if (circle.y + circle.r > doubleBufferedPanel1.Height)
                    circle.y = doubleBufferedPanel1.Height - circle.r;
            }
            else if (s is Rectangle rect)
            {
                if (rect.x < 0)
                    rect.x = 0;
                if (rect.x + rect.w > doubleBufferedPanel1.Width)
                    rect.x = doubleBufferedPanel1.Width - rect.w;
                if (rect.y < 0)
                    rect.y = 0;
                if (rect.y + rect.h > doubleBufferedPanel1.Height)
                    rect.y = doubleBufferedPanel1.Height - rect.h;
            }
        }

        private void bouncePhysics()
        {
            foreach (var shape in shapes)
            {
                shape.x += shape.ve.x;
                shape.y += shape.ve.y;
                if (shape is Circle)
                {
                    Circle circle = (Circle)shape;
                    if (circle.x - circle.r < 0 || circle.x + circle.r > doubleBufferedPanel1.Width)
                    {
                        circle.ve.x = -circle.ve.x;
                    }
                    if (circle.y - circle.r < 0 || circle.y + circle.r > doubleBufferedPanel1.Height)
                    {
                        circle.ve.y = -circle.ve.y;
                    }
                }
                else if (shape is Rectangle)
                {
                    Rectangle rect = (Rectangle)shape;
                    if (rect.x < 0 || rect.x + rect.w > doubleBufferedPanel1.Width)
                    {
                        rect.ve.x = -rect.ve.x;
                    }
                    if (rect.y < 0 || rect.y + rect.h > doubleBufferedPanel1.Height)
                    {
                        rect.ve.y = -rect.ve.y;
                    }
                }
            }
        }
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.Clear(Color.White);
            foreach (var shape in shapes)
            {
                using (Pen pen = new Pen(shape.c))
                {
                    if (shape is Circle circle)
                    {
                        g.DrawEllipse(pen, circle.x - circle.r, circle.y - circle.r, circle.r * 2, circle.r * 2);
                    }
                    else if (shape is Rectangle rect)
                    {
                        g.DrawRectangle(pen, rect.x, rect.y, rect.w, rect.h);
                    }
                }
            }
        }
        private void panel1_Paint_Bitmap()
        {
            Bitmap bitmap = new Bitmap(doubleBufferedPanel1.Width, doubleBufferedPanel1.Height);
            using (Graphics bufferGraphics = Graphics.FromImage(bitmap))
            {
                bufferGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                bufferGraphics.Clear(Color.White);
                foreach (var shape in shapes)
                {
                    using (Pen pen = new Pen(shape.c))
                    {
                        if (shape is Circle circle)
                        {
                            bufferGraphics.DrawEllipse(pen, circle.x - circle.r, circle.y - circle.r, circle.r * 2, circle.r * 2);
                        }
                        else if (shape is Rectangle rect)
                        {
                            bufferGraphics.DrawRectangle(pen, rect.x, rect.y, rect.w, rect.h);
                        }
                    }
                }
            }

            using (Graphics panelGraphics = doubleBufferedPanel1.CreateGraphics())
            {
                panelGraphics.DrawImage(bitmap, 0, 0);
            }
            bitmap.Dispose();
        }
    }
}


