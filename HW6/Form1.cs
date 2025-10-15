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
    public partial class Form1 : Form
    {
        int size = 12;
        int minecount = 15;
        int[,] minefield = new int[12, 12];
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            genmine();
            minefield_chk_neighbors();
            printfield();
            drawPanel();
        }

        private void initMine()
        {
            // Initialize the minefield with 0s
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    minefield[i, j] = 0;
                }
            }
        }
        private void genmine()
        {
            initMine();
            Random rand = new Random();
            for (int i = 0; i < minecount; i++)
            {
                int x = rand.Next(0, size - 1);
                int y = rand.Next(0, size - 1);
                if (minefield[x, y] == 9)
                {
                    i--;
                    continue;
                }
                minefield[x, y] = 9;
            }

        }
        private void minefield_chk_neighbors()
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (minefield[i, j] == 9) continue;
                    int count = 0;
                    for (int x = -1; x <= 1; x++)
                    {
                        for (int y = -1; y <= 1; y++)
                        {
                            if (i + x < 0 || i + x >= size || j + y < 0 || j + y >= size) continue;
                            if (minefield[i + x, j + y] == 9) count++;
                        }
                    }
                    minefield[i, j] = count;
                }
            }

        }

        private void printfield()
        {
            string output = "";
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    output += minefield[i, j] + " ";
                }
                output += "\r\n";
            }
            label1.Text = output;
        }

        private void drawPanel()
        {
            Graphics g = panel1.CreateGraphics();
            g.Clear(Color.White);
            Font f = new Font("Microsoft JhengHei", 16);
            int cellSize = Math.Min(panel1.Width / size, panel1.Height / size);
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    Rectangle cellRect = new Rectangle(j * cellSize, i * cellSize, cellSize, cellSize);
                    g.DrawRectangle(Pens.Red, cellRect);

                    string text = minefield[i, j].ToString();
                    SizeF textSize = g.MeasureString(text, f);
                    PointF textPosition = new PointF(
                        cellRect.X + (cellSize - textSize.Width) / 2,
                        cellRect.Y + (cellSize - textSize.Height) / 2);
                    g.DrawString(text, f, Brushes.Blue, textPosition);

                }
            }
        }
    }
}
