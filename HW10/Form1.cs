using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HW10.Properties;

namespace HW10
{
    public partial class Form1 : Form
    {
        // Board size (expanded for overflow test). Remove duplicate declarations.
        const int board_width = 100;
        const int board_height = 100;
        const int bomb_count = 10;

        int[,] board = new int[board_width, board_height];
        int[,] bombMap = new int[board_width, board_height];
        bool[,] revealed = new bool[board_width, board_height];

        bool legacyMode = true;
        bool boardInitialized = false;

        // Sprite sheet: expected order -> 0..8 (numbers), 9 (bomb), 10 (hidden)
        private Image sprite = Resources.gaming_SpriteSheet;
        const int SpriteCols = 4; // adjust if your sheet differs
        const int SpriteRows = 4;  // adjust if your sheet differs
        const int SpriteIndexBomb = 10;
        const int SpriteIndexHidden = 9;

        readonly Random _rand = new Random();

        public Form1()
        {
            InitializeComponent();
            this.panel2.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(this.panel2, true, null);

            panel2.Paint += panel2_Paint;
            panel2.MouseDown += panel2_MouseDown;
            panel2.Resize += (s, e) => panel2.Invalidate();
        }

        // Compute source rectangle in the sprite sheet for a given tile index
        Rectangle GetSpriteSourceRect(int index)
        {
            if (sprite == null || SpriteCols <= 0 || SpriteRows <= 0) return Rectangle.Empty;
            int cellW = sprite.Width / SpriteCols;
            int cellH = sprite.Height / SpriteRows;
            if (cellW <= 0 || cellH <= 0) return Rectangle.Empty;
            if (index < 0) index = 0;
            int col = index % SpriteCols;
            int row = index / SpriteCols;
            if (row >= SpriteRows) row = SpriteRows - 1;
            return new Rectangle(col * cellW, row * cellH, cellW, cellH);
        }

        void DrawTile(Graphics g, int index, RectangleF dest)
        {
            if (sprite == null)
            {
                // fallback: draw text if no sprite
                using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                using (Font font = new Font("Arial", Math.Max(8f, dest.Height * 0.45f), GraphicsUnit.Pixel))
                using (Brush b = new SolidBrush(Color.Black))
                {
                    string txt = index == SpriteIndexHidden ? "#" : (index == SpriteIndexBomb ? "*" : index.ToString());
                    g.DrawString(txt, font, b, dest, sf);
                }
                return;
            }
            Rectangle src = GetSpriteSourceRect(index);
            if (src.Width <= 0 || src.Height <= 0)
            {
                // invalid sheet settings; fallback
                using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                using (Font font = new Font("Arial", Math.Max(8f, dest.Height * 0.45f), GraphicsUnit.Pixel))
                using (Brush b = new SolidBrush(Color.Black))
                {
                    string txt = index == SpriteIndexHidden ? "#" : (index == SpriteIndexBomb ? "*" : index.ToString());
                    g.DrawString(txt, font, b, dest, sf);
                }
                return;
            }
            g.DrawImage(sprite, dest, src, GraphicsUnit.Pixel);
        }

        void initBoard()
        {
            legacyMode = true;
            boardInitialized = true;
            for (int y = 0; y < board_height; y++)
            {
                for (int x = 0; x < board_width; x++)
                {
                    board[x, y] = 0;
                    revealed[x, y] = false;
                    bombMap[x, y] = 0;
                }
            }
        }

        void initBoardWithBomb()
        {
            legacyMode = false;
            boardInitialized = true;
            for (int y = 0; y < board_height; y++)
            {
                for (int x = 0; x < board_width; x++)
                {
                    bombMap[x, y] = 0;
                    board[x, y] = 0;
                    revealed[x, y] = false;
                }
            }
            int bombsPlaced = 0;
            while (bombsPlaced < bomb_count)
            {
                int x = _rand.Next(board_width);
                int y = _rand.Next(board_height);
                if (bombMap[x, y] == 0)
                {
                    bombMap[x, y] = 1;
                    bombsPlaced++;
                }
            }
            for (int y = 0; y < board_height; y++)
            {
                for (int x = 0; x < board_width; x++)
                {
                    if (bombMap[x, y] == 1)
                    {
                        board[x, y] = 0;
                        continue;
                    }
                    int cnt = 0;
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        for (int dx = -1; dx <= 1; dx++)
                        {
                            if (dx == 0 && dy == 0) continue;
                            int nx = x + dx;
                            int ny = y + dy;
                            if (nx >= 0 && nx < board_width && ny >= 0 && ny < board_height && bombMap[nx, ny] == 1)
                            {
                                cnt++;
                            }
                        }
                    }
                    board[x, y] = cnt;
                }
            }
        }

        void drawBoard()
        {
            panel2.Invalidate();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Color.White);

            if (!boardInitialized)
            {
                return;
            }

            float scaleX = (float)panel2.Width / board_width;
            float scaleY = (float)panel2.Height / board_height;
            float scale = Math.Min(scaleX, scaleY);

            float boardPixelWidth = board_width * scale;
            float boardPixelHeight = board_height * scale;

            float offsetX = (panel2.Width - boardPixelWidth) * 0.5f;
            float offsetY = (panel2.Height - boardPixelHeight) * 0.5f;

            using (Pen pen = new Pen(Color.Red))
            {
                // Horizontal lines
                for (int i = 0; i <= board_height; i++)
                {
                    float y = offsetY + i * scale;
                    g.DrawLine(pen, offsetX, y, offsetX + boardPixelWidth, y);
                }

                // Vertical lines
                for (int j = 0; j <= board_width; j++)
                {
                    float x = offsetX + j * scale;
                    g.DrawLine(pen, x, offsetY, x, offsetY + boardPixelHeight);
                }
            }

            for (int i = 0; i < board_height; i++)
            {
                for (int j = 0; j < board_width; j++)
                {
                    RectangleF cellRect = new RectangleF(
                        offsetX + j * scale,
                        offsetY + i * scale,
                        scale,
                        scale
                    );

                    if (legacyMode)
                    {
                        int val = board[j, i];
                        if (val == 9)
                        {
                            DrawTile(g, SpriteIndexBomb, cellRect);
                        }
                        else if (val >= 0 && val <= 8)
                        {
                            DrawTile(g, val, cellRect);
                        }
                        else
                        {
                            DrawTile(g, 0, cellRect);
                        }
                    }
                    else
                    {
                        if (!revealed[j, i])
                        {
                            DrawTile(g, SpriteIndexHidden, cellRect);
                        }
                        else
                        {
                            if (bombMap[j, i] == 1)
                            {
                                DrawTile(g, SpriteIndexBomb, cellRect);
                            }
                            else
                            {
                                int val = board[j, i];
                                if (val >= 0 && val <= 8)
                                {
                                    DrawTile(g, val, cellRect);
                                }
                                else
                                {
                                    DrawTile(g, 0, cellRect);
                                }
                            }
                        }
                    }
                }
            }
        }

        void updateMine(int x, int y)
        {
            if (x < 0 || x >= board_width || y < 0 || y >= board_height)
                return;
            if (board[x, y] == 9)
            {
                return;
            }
            board[x, y] = 9;
            updateMine_Helper(x - 1, y - 1);
            updateMine_Helper(x - 1, y);
            updateMine_Helper(x - 1, y + 1);
            updateMine_Helper(x, y - 1);
            updateMine_Helper(x, y + 1);
            updateMine_Helper(x + 1, y - 1);
            updateMine_Helper(x + 1, y);
            updateMine_Helper(x + 1, y + 1);
        }

        void updateMine_Helper(int x, int y)
        {
            if (x < 0 || x >= board_width || y < 0 || y >= board_height)
            {
                return;
            }
            if (board[x, y] != 9)
            {
                board[x, y] += 1;
            }
        }

        void FloodReveal(int startX, int startY)
        {
            if (startX < 0 || startX >= board_width || startY < 0 || startY >= board_height)
                return;
            if (bombMap[startX, startY] == 1) return;

            var queue = new Queue<(int x, int y)>();
            queue.Enqueue((startX, startY));

            while (queue.Count > 0)
            {
                var (x, y) = queue.Dequeue();
                if (revealed[x, y]) continue;
                if (bombMap[x, y] == 1) continue;
                revealed[x, y] = true;

                if (board[x, y] == 0)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        for (int dx = -1; dx <= 1; dx++)
                        {
                            if (dx == 0 && dy == 0) continue;
                            int nx = x + dx;
                            int ny = y + dy;
                            if (nx >= 0 && nx < board_width && ny >= 0 && ny < board_height)
                            {
                                if (!revealed[nx, ny] && bombMap[nx, ny] == 0)
                                {
                                    queue.Enqueue((nx, ny));
                                }
                            }
                        }
                    }
                }
            }
        }


        // void Reveal(int x, int y)
        // {
        //     if (x < 0 || x >= board_width || y < 0 || y >= board_height)
        //         return;
        //     if (revealed[x, y])
        //         return;
        //     if (bombMap[x, y] == 1)
        //         return;
        //     revealed[x, y] = true;
        //     if (board[x, y] == 0)
        //     {
        //         for (int dy = -1; dy <= 1; dy++)
        //         {
        //             for (int dx = -1; dx <= 1; dx++)
        //             {
        //                 if (dx == 0 && dy == 0) continue;
        //                 Reveal(x + dx, y + dy);
        //             }
        //         }
        //     }
        // }

        private void button1_Click(object sender, EventArgs e)
        {
            initBoard();
            drawBoard();
        }

        private void panel2_MouseDown(object sender, MouseEventArgs e)
        {
            if (!boardInitialized) return;

            float scaleX = (float)panel2.Width / board_width;
            float scaleY = (float)panel2.Height / board_height;
            float scale = Math.Min(scaleX, scaleY);
            float boardPixelWidth = board_width * scale;
            float boardPixelHeight = board_height * scale;

            float offsetX = (panel2.Width - boardPixelWidth) * 0.5f;
            float offsetY = (panel2.Height - boardPixelHeight) * 0.5f;
            int j = (int)((e.X - offsetX) / scale);
            int i = (int)((e.Y - offsetY) / scale);

            if (i < 0 || i >= board_height || j < 0 || j >= board_width)
                return;

            if (legacyMode)
            {
                if (e.Button == MouseButtons.Right)
                {
                    updateMine(j, i);
                    drawBoard();
                }
            }
            else
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (bombMap[j, i] == 1)
                    {
                        for (int y = 0; y < board_height; y++)
                            for (int x = 0; x < board_width; x++)
                                if (bombMap[x, y] == 1) revealed[x, y] = true;
                    }
                    else
                    {
                        FloodReveal(j, i); // 使用迭代版本
                        // Reveal(j, i); // 舊遞迴版本 (保留註解)
                    }
                    drawBoard();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            initBoardWithBomb();
            drawBoard();
        }
    }
}
