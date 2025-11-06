using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Problem3
{
    public partial class Form1 : Form
    {

        int card_width = 71;
        int card_height = 96;
        Image img;
        int[] cards;
        int[] poker;
        float scale;
        //92 cards
        bool isChecking;
        int selCard = -1;
        bool[] removed;

        private static readonly string[] MapRows = new[]
        {
            "++++@@@+++++++++++++",
            "@@@+@@@++@@@@@++++++",
            "@@@+@@@++@@@@@++++++",
            "@@@+@@@@+@@@@@++++++",
            "++++@@@@+@@@@@@@@+++",
            "++++@@@@+@@@@@@@@+++",
            "++++@@@@++++++@@@+++",
            "++++++++++++++++++++"
        };

        private static int[,] CreateMap(string[] rows)
        {
            if (rows == null || rows.Length == 0)
                throw new ArgumentException("Rows cannot be null or empty.");

            int cols = rows[0].Length;
            var result = new int[rows.Length, cols];

            for (int r = 0; r < rows.Length; r++)
            {
                if (rows[r].Length != cols)
                    throw new ArgumentException("All rows must have the same length.");

                for (int c = 0; c < cols; c++)
                {
                    char ch = rows[r][c];
                    if (ch == '+')
                        result[r, c] = 1;
                    else if (ch == '@')
                        result[r, c] = 0;
                    else
                        throw new ArgumentException("Unsupported map character: '" + ch + "'.");
                }
            }

            return result;
        }

        private readonly int[,] _map = CreateMap(MapRows);

        public Form1()
        {
            img = Properties.Resources.poker;
            InitializeComponent();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();
            scale = Math.Min(this.ClientSize.Width / (float)(card_width * _map.GetLength(1)),
                             this.ClientSize.Height / (float)(card_height * _map.GetLength(0)));

            initCards(GetSlotsCount());
        }

        private int GetSlotsCount()
        {
            int count = 0;
            int rows = _map.GetLength(0);
            int cols = _map.GetLength(1);
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    if (_map[r, c] == 1)
                        count++;
                }
            }
            return count;
        }

        public void initCards(int total)
        {
            poker = shuffelCards();
            cards = pickcards(poker, total);
            removed = new bool[total];
            selCard = -1;
            isChecking = false;
            this.Invalidate();
        }

        public int[] shuffelCards()
        {
            Random rand = new Random();
            int[] poker = new int[52];
            for (int i = 0; i < 52; i++)
            {
                poker[i] = i;
            }
            for (int i = 0; i < 52; i++)
            {
                int j = rand.Next(52);
                int temp = poker[i];
                poker[i] = poker[j];
                poker[j] = temp;
            }
            return poker;
        }

        public int[] pickcards(int[] poker, int total)
        {
            int[] cards = new int[total];
            for (int i = 0; i < total / 2; i++)
            {
                cards[i] = poker[i];
                cards[i + total / 2] = poker[i];
            }
            Random rand = new Random();
            for (int i = 0; i < total; i++)
            {
                int j = rand.Next(total);
                int temp = cards[i];
                cards[i] = cards[j];
                cards[j] = temp;
            }
            return cards;
        }

        public void printMapWithCards()
        {
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (cards == null || img == null)
                return;

            int rows = _map.GetLength(0);
            int cols = _map.GetLength(1);

            int scaledW = (int)(card_width * scale);
            int scaledH = (int)(card_height * scale);

            int index = 0;
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    if (_map[r, c] == 1)
                    {
                        if (index >= cards.Length)
                            return;

                        int idx = index++;
                        if (removed != null && idx < removed.Length && removed[idx])
                            continue;

                        int card = cards[idx];
                        int sx = (card % 13) * card_width;
                        int sy = (card / 13) * card_height;

                        var dest = new Rectangle(
                            (int)(c * card_width * scale),
                            (int)(r * card_height * scale),
                            scaledW,
                            scaledH);

                        var src = new Rectangle(sx, sy, card_width, card_height);

                        e.Graphics.DrawImage(img, dest, src, GraphicsUnit.Pixel);

                        if (idx == selCard)
                        {
                            using (var pen = new Pen(Color.Gold, Math.Max(2, (int)(2 * scale))))
                            {
                                e.Graphics.DrawRectangle(pen, dest);
                            }
                        }
                    }
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (_map != null && this.ClientSize.Width > 0 && this.ClientSize.Height > 0)
            {
                scale = Math.Min(this.ClientSize.Width / (float)(card_width * _map.GetLength(1)),
                                 this.ClientSize.Height / (float)(card_height * _map.GetLength(0)));
                this.Invalidate();
            }
        }

        private int GetIndexFromCell(int row, int col)
        {
            if (row < 0 || col < 0 || row >= _map.GetLength(0) || col >= _map.GetLength(1))
                return -1;
            int index = 0;
            for (int r = 0; r < _map.GetLength(0); r++)
            {
                for (int c = 0; c < _map.GetLength(1); c++)
                {
                    if (_map[r, c] == 1)
                    {
                        if (r == row && c == col)
                            return index;
                        index++;
                    }
                }
            }
            return -1;
        }

        private bool AllRemoved()
        {
            if (removed == null || removed.Length == 0) return false;
            for (int i = 0; i < removed.Length; i++)
            {
                if (!removed[i]) return false;
            }
            return true;
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            if (cards == null || removed == null)
                return;

            if (isChecking) return; 

            int cols = _map.GetLength(1);
            int rows = _map.GetLength(0);

            int col = (int)(e.X / (card_width * scale));
            int row = (int)(e.Y / (card_height * scale));

            if (row < 0 || row >= rows || col < 0 || col >= cols)
                return;

            if (_map[row, col] != 1)
                return;

            int idx = GetIndexFromCell(row, col);
            if (idx < 0 || idx >= cards.Length)
                return;
            if (removed[idx])
                return; 

            if (selCard == -1)
            {
                selCard = idx;
                this.Invalidate();
                return;
            }

            if (idx == selCard)
            {
                selCard = -1;
                this.Invalidate();
                return;
            }

            if (cards[idx] == cards[selCard])
            {
                removed[idx] = true;
                removed[selCard] = true;
                selCard = -1;

                if (AllRemoved())
                {
                    this.Invalidate();
                    MessageBox.Show("恭喜通通移除，遊戲結束！", "完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    this.Invalidate();
                }
            }
            else
            {
                selCard = -1;
                this.Invalidate();
            }
        }
    }
}
