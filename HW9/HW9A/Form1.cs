using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HW9A.Properties;

namespace HW9A
{
    public partial class Form1 : Form
    {
        private const int Rows = 16;
        private const int Cols = 20;
        private const int Bombs = 20;

        private readonly Random _rng = new Random();
        private readonly int[,] _neighborCounts = new int[Rows, Cols];
        private readonly bool[,] _isBomb = new bool[Rows, Cols];
        private bool _hasMap;

        public Form1()
        {
            InitializeComponent();
        }

        private void GenerateMap()
        {
            // Clear previous state
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    _isBomb[r, c] = false;
                    _neighborCounts[r, c] = 0;
                }
            }

            int placed = 0;
            while (placed < Bombs)
            {
                int r = _rng.Next(Rows);
                int c = _rng.Next(Cols);
                if (_isBomb[r, c]) continue;

                _isBomb[r, c] = true;
                placed++;

                // Increment 8 neighbors' counts
                for (int dr = -1; dr <= 1; dr++)
                {
                    for (int dc = -1; dc <= 1; dc++)
                    {
                        if (dr == 0 && dc == 0) continue; // skip itself
                        int nr = r + dr;
                        int nc = c + dc;
                        if (nr < 0 || nr >= Rows || nc < 0 || nc >= Cols) continue;
                        if (_isBomb[nr, nc]) continue; // keep bombs without counts
                        _neighborCounts[nr, nc]++;
                    }
                }
            }

            _hasMap = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GenerateMap();
            panel2.Invalidate();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            if (!_hasMap) return;

            Graphics g = e.Graphics;
            g.Clear(panel2.BackColor);

            int w = panel2.ClientSize.Width;
            int h = panel2.ClientSize.Height;
            if (w <= 0 || h <= 0) return;

            int cellW = Math.Max(1, w / Cols);
            int cellH = Math.Max(1, h / Rows);
            int size = Math.Min(cellW, cellH);

            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    int x = c * size;
                    int y = r * size;

                    Image img = GetCellImage(r, c);
                    if (img != null)
                    {
                        g.DrawImage(img, new Rectangle(x, y, size, size));
                    }
                    else
                    {
                        // fallback: draw empty
                        using (var brush = new SolidBrush(Color.LightGray))
                        {
                            g.FillRectangle(brush, x, y, size, size);
                        }
                    }

                    // optional grid lines
                    using (var pen = new Pen(Color.DarkGray))
                    {
                        g.DrawRectangle(pen, x, y, size, size);
                    }
                }
            }
        }

        private static Image GetNumberImage(int count)
        {
            switch (count)
            {
                case 1: return Resources.open1;
                case 2: return Resources.open2;
                case 3: return Resources.open3;
                case 4: return Resources.open4;
                case 5: return Resources.open5;
                case 6: return Resources.open6;
                case 7: return Resources.open7;
                case 8: return Resources.open8;
                default: return null;
            }
        }

        private Image GetCellImage(int r, int c)
        {
            if (_isBomb[r, c])
            {
                return Resources.mine_ceil; // bomb
            }

            int n = _neighborCounts[r, c];
            if (n == 0)
            {
                return Resources.question; // use question for zero
            }
            return GetNumberImage(n);
        }
    }
}
