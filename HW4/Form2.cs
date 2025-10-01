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
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //clear previous drawings
            this.Refresh();

            int[] bingoNumbers = genBingo();
            Pen p = new Pen(Color.Red);
            Graphics g = this.CreateGraphics();
            int init = 100;
            int len = 180;
            int cells = 5;
            int cellSize = len / cells;
            // vertical lines
            for (int i = 0; i <= cells; i++)
                g.DrawLine(p, init + i * cellSize, init, init + i * cellSize, init + len);
            // horizontal lines
            for (int i = 0; i <= cells; i++)
                g.DrawLine(p, init, init + i * cellSize, init + len, init + i * cellSize);
            // draw numbers
            int offsetX = 3;
            int offsetY = 5;
            int singledDigitOffsetX = 8;
            Brush brushColor = Brushes.Blue;
            Font f = new Font("Microsoft JhengHei", 16);
            for (int i = 0; i < cells; i++)
                for (int j = 0; j < cells; j++)
                {
                    string numStr = bingoNumbers[i * cells + j].ToString();
                    SizeF strSize = g.MeasureString(numStr, f);
                    float x = init + j * cellSize + (cellSize - strSize.Width) / 2;
                    float y = init + i * cellSize + (cellSize - strSize.Height) / 2;
                    g.DrawString(numStr, f, brushColor, x, y);
                }
        }

        private int[] genBingo()
        {
            // Fisher-Yates Shuffle algorithm
            Random rand = new Random();
            int size = 25;
            int[] arr = new int[size];
            
            for (int i = 0; i < size; i++)
                arr[i] = i + 1;

            // shuffle 3 times
            for (int k = 0; k < 3; k++)
            {
                for (int i = size - 1; i > 0; i--)
                {
                    int j = rand.Next(0, i + 1);
                    // Swap arr[i] and arr[j]
                    int temp = arr[i];
                    arr[i] = arr[j];
                    arr[j] = temp;
                }
                
            }

            return arr;
        }


    }
}
