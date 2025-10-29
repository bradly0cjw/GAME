using PuzzleGame.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PuzzleGame
{

    public partial class Form1 : Form
    {
        int row = 3;
        int col = 3;
        int gap = 3;
        Image image;
        PuzzlePiece[] pp;
        public Form1()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
             ControlStyles.UserPaint |
             ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();
            image = Resources.sunset_with_snoopy;
            pictureBox1.Image = image;
            pp = createPuzzles(image, row, col);

            panel2.Paint += Panel2_Paint;

            printPuzzles();
            this.WindowState = FormWindowState.Maximized;

        }

        public void printPuzzles()
        {

            for (int i = 0; i < pp.Length; ++i)
            {
                panel2.Controls.Add(pp[i]);
                pp[i].Left = 800 + (i % col) * (pieceW + gap);
                pp[i].Top = 50 + (i / col) * (pieceH + gap);
                //pp[i].Width = pieceW * scale;
                //pp[i].Height = pieceH * scale;
                pp[i].SizeMode = PictureBoxSizeMode.AutoSize;
            }
        }
        private void Panel2_Paint(object sender, PaintEventArgs e)
        {
            //refersh panel2

            drawGrid(e.Graphics, 50, 50, row, col, pieceW, pieceH);
        }

        private void destroyPuzzles()
        {
            for (int i = 0; i < pp.Length; ++i)
            {
                panel2.Controls.Remove(pp[i]);
                pp[i].Dispose();
            }
        }

        private void button3b3_Click(object sender, EventArgs e)
        {
            destroyPuzzles();
            col = 3;
            row = 3;
            pp = createPuzzles(image, 3, 3);
            printPuzzles();
            panel2.Paint += Panel2_Paint;
            panel2.Invalidate();

        }


        private void button4b5_Click(object sender, EventArgs e)
        {
            destroyPuzzles();
            col = 5;
            row = 4;
            pp = createPuzzles(image, 4, 5);
            printPuzzles();
            panel2.Paint += Panel2_Paint;
            panel2.Invalidate();
        }

        int pieceW;
        int pieceH;

        public PuzzlePiece[] createPuzzles(Image image, int row, int col)
        {
            pieceW = (int)(image.Width / col);
            pieceH = (int)(image.Height / row);

            PuzzlePiece[] puzzles = new PuzzlePiece[row * col];
            Bitmap piece;

            int index = 0;
            for (int r = 0; r < row; r++)
            {
                for (int c = 0; c < col; ++c)
                {
                    puzzles[index] = new PuzzlePiece();
                    piece = new Bitmap(pieceW, pieceH);
                    Graphics g = Graphics.FromImage(piece);
                    g.DrawImage(image, new Rectangle(0, 0, pieceW, pieceH),
                                       new Rectangle(c * pieceW, r * pieceH, pieceW, pieceH),
                                       GraphicsUnit.Pixel);
                    puzzles[index].Image = piece;
                    puzzles[index].Width = pieceW;
                    puzzles[index].Height = pieceH;
                    puzzles[index].tag = index;
                    puzzles[index].CorrectX = 50 + c * pieceW;
                    puzzles[index].CorrectY = 50 + r * pieceH;

                    index++;
                }
            }
            return puzzles;
        }


        public void drawGrid(Graphics g, int sx, int sy, int row, int col, int pieceW, int pieceH)
        {
            Pen pen = new Pen(Color.Red);
            for (int r = 0; r <= row; ++r)
            {
                g.DrawLine(pen, sx, sy + r * pieceH, sx + col * pieceW, sy + r * pieceH);
            }
            for (int c = 0; c <= col; ++c)
            {
                g.DrawLine(pen, sx + c * pieceW, sy, sx + c * pieceW, sy + row * pieceH);
            }
            //label1.Text = "" + PuzzlePiece.released;
        }

        public void CheckCompletion()
        {
            foreach (PuzzlePiece piece in pp)
            {
                if (!piece.IsLocked)
                {
                    return;
                }
            }
            MessageBox.Show("Congratulations! You have completed the puzzle!", "Puzzle Solved");
        }


    }
    public class PuzzlePiece : PictureBox
    {
        public static int released = -1;  // 記錄當前的被釋放的拼圖區塊(piece)。

        public bool dragging = false;  // 是否點按了該拼圖(piece)。
        int prevX, prevY;
        public int tag = -1;  // 記錄拼圖(piece)的編號。
        public int CorrectX { get; set; }
        public int CorrectY { get; set; }
        public bool IsLocked { get; private set; } = false;
        private const int SnapDistance = 20;

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (IsLocked) return;

            if (tag >= 0)
            {
                dragging = true;
                prevX = e.X;
                prevY = e.Y;

                // child control 會依序加入，先加的數值小，會被畫在上面！
                // 將點選到的piece提升到最上面:
                this.Parent.Controls.SetChildIndex(this, 0);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (dragging)
            {
                int dx = e.X - prevX;
                int dy = e.Y - prevY;
                Left += dx;
                Top += dy;
            }
            //this.Parent.Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            dragging = false;  // 只在此設定會造成靠近但尚未釋放滑鼠時，出現來回晃動的現象！

            released = tag;

            if (!IsLocked && Math.Abs(this.Left - CorrectX) < SnapDistance &&
                Math.Abs(this.Top - CorrectY) < SnapDistance)
            {
                this.Location = new Point(CorrectX, CorrectY);
                this.IsLocked = true;
                (this.Parent.Parent as Form1)?.CheckCompletion();
            }

            this.Parent.Invalidate();  // 讓 parent 啟動重繪事件。
        }



    }
}
