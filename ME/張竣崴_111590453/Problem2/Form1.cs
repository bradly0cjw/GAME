using Problem2.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Problem2
{
    public partial class Form1 : Form
    {
        private readonly List<SimpleSprite> _sprites = new List<SimpleSprite>();

        public Form1()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
             ControlStyles.UserPaint |
             ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();
            InitializeComponent();

            this.Paint += Form1_Paint;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            foreach (var sprite in _sprites.ToArray())
            {
                sprite.Draw(e.Graphics);
            }
        }

        private void Form1_Click(object sender, EventArgs e)
        {
            Point p = this.PointToClient(Cursor.Position);

            SimpleSprite sprite = new SimpleSprite(this, p.X, p.Y);
            sprite.setImage(Resources.explosion);
            sprite.Completed += (s, args) =>
            {
                var spr = (SimpleSprite)s;
                _sprites.Remove(spr);
                this.Invalidate(spr.Bounds);
                spr.Dispose();
            };
            _sprites.Add(sprite);
            sprite.start();
        }
    }

    class SimpleSprite : IDisposable
    {
        System.Windows.Forms.Timer uiTimer = null;
        Image image = Resources.explosion;
        int spriteCount = 0;
        int frameW = 64;

        Control target;
        int drawX;
        int drawY;

        public event EventHandler Completed;

        public SimpleSprite()
        {
            uiTimer = new System.Windows.Forms.Timer();
            uiTimer.Interval = 100;
            uiTimer.Tick += UiTimer_Tick;
        }

        public SimpleSprite(Control target, int x, int y) : this()
        {
            this.target = target;
            this.drawX = x;
            this.drawY = y;
        }
        public void setImage(Image img)
        {
            this.image = img;
        }
        public void start()
        {
            spriteCount = 0;
            uiTimer.Start();
        }

        public Rectangle Bounds => new Rectangle(drawX - frameW / 2, drawY - frameW / 2, frameW, frameW);

        int[] index2RC(int index, int col)
        {
            int[] rc = new int[2];
            rc[0] = index / col;
            rc[1] = index % col;
            return rc;
        }

        public void Draw(Graphics g)
        {
            if (spriteCount <= 0 || spriteCount > 25) return;
            int[] rc = index2RC(spriteCount - 1, 5);
            int sx = rc[1] * frameW;
            int sy = rc[0] * frameW;
            g.DrawImage(image, Bounds, new Rectangle(sx, sy, frameW, frameW), GraphicsUnit.Pixel);
        }

        private void UiTimer_Tick(object sender, EventArgs e)
        {
            spriteCount++;
            if (spriteCount <= 25)
            {
                target?.Invalidate(Bounds);
            }
            else
            {
                uiTimer.Stop();
                target?.Invalidate(Bounds);
                Completed?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Dispose()
        {
            uiTimer?.Dispose();
        }
    }
}
