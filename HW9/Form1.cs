using HW9.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace HW9
{
    public partial class Form1 : Form
    {
        private readonly List<PokerCard> _cards = new List<PokerCard>();
        private readonly Timer _timer = new Timer();
        private readonly Random _rand = new Random();
        private Bitmap _backBuffer; // manual back buffer to eliminate flicker
        private PokerCard _pendingMatchCard; // for pair match logic
        public bool showBack=false;
        public Form1()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
            this.DoubleBuffered = true;

            // enable double-buffering on panel2 via reflection to reduce flicker
            typeof(Panel).InvokeMember("DoubleBuffered", BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic, null, panel2, new object[] { true });

            panel2.Paint += panel2_Paint;
            panel2.Resize += panel2_Resize;
            panel2.MouseDown += panel2_MouseDown;
            this.FormClosed += (s, e) => { _timer.Stop(); _backBuffer?.Dispose(); };

            _timer.Interval = 16; // ~60 FPS
            _timer.Tick += Timer_Tick;
        }

        private void EnsureBackBuffer()
        {
            int w = panel2.ClientSize.Width;
            int h = panel2.ClientSize.Height;
            if (w <= 0 || h <= 0) return;
            if (_backBuffer == null || _backBuffer.Width != w || _backBuffer.Height != h)
            {
                _backBuffer?.Dispose();
                _backBuffer = new Bitmap(w, h);
            }
        }

        private void panel2_Resize(object sender, EventArgs e)
        {
            EnsureBackBuffer();
        }

        // keep class for potential future use
        class DoubleBufferedPanel : Panel
        {
            public DoubleBufferedPanel()
            {
                this.DoubleBuffered = true;
                this.ResizeRedraw = true;
                this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
                this.UpdateStyles();
            }
        }

        class PokerCard
        {
            public int tag;
            public bool enabled = true;
            public int x, y; // position
            public int[] vec = new int[2]; // velocity
            private readonly Bitmap back;
            private readonly Bitmap front;
            private bool isFront = true;

            public int Width => front.Width;
            public int Height => front.Height;

            public PokerCard(int index, int px = 0, int py = 0)
            {
                tag = index;
                x = px;
                y = py;
                front = getPoker(index);
                back = getPoker(52); // back image assumed at index 52
                enabled = true;
            }
            int[] index2RC(int index, int col)
            {
                int[] rc = new int[2];
                if (index < 0) index = 0;
                if (index <= 51)
                {
                    rc[0] = index / col;
                    rc[1] = index % col;
                }
                else
                {
                    rc[0] = 4; // row of back image
                    rc[1] = 0;
                }
                return rc;
            }
            public Bitmap getPoker(int index)
            {
                int pokerW = 71;
                int pokerH = 96;
                Image poker = Resources.poker;
                Bitmap bmp = new Bitmap(pokerW, pokerH);
                int[] rc = index2RC(index, 13);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.DrawImage(poker,
                        new Rectangle(0, 0, pokerW, pokerH),
                        new Rectangle(rc[1] * pokerW, rc[0] * pokerH, pokerW, pokerH),
                        GraphicsUnit.Point);
                }
                return bmp;
            }
            public void setPos(int nx, int ny) { x = nx; y = ny; }
            public void setVec(int vx, int vy) { vec[0] = vx; vec[1] = vy; }
            public void update() { x += vec[0]; y += vec[1]; }
            public void collisionCheck(int boundW, int boundH)
            {
                bool collided = false;
                if (x < 0)
                {
                    x = 0; vec[0] = -vec[0]; collided = true;
                }
                else if (x + Width > boundW)
                {
                    x = Math.Max(0, boundW - Width); vec[0] = -vec[0]; collided = true;
                }
                if (y < 0)
                {
                    y = 0; vec[1] = -vec[1]; collided = true;
                }
                else if (y + Height > boundH)
                {
                    y = Math.Max(0, boundH - Height); vec[1] = -vec[1]; collided = true;
                }
                if (collided) isFront = !isFront;
            }
            public void draw(Graphics g,bool showBack)
            {
                if (!enabled) return;
                g.DrawImage((isFront | !showBack) ? front : back, x, y);
            }
            public bool HitTest(int px, int py) => enabled && px >= x && px < x + Width && py >= y && py < y + Height;
            public int Suit => tag / 13; // 0..3
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (_cards.Count == 0) return;
            int w = panel2.ClientSize.Width;
            int h = panel2.ClientSize.Height;
            foreach (var c in _cards)
            {
                c.update();
                c.collisionCheck(w, h);
            }
            panel2.Invalidate();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            EnsureBackBuffer();
            if (_backBuffer == null) return;
            using (Graphics g = Graphics.FromImage(_backBuffer))
            {
                g.Clear(panel2.BackColor);
                foreach (var c in _cards)
                    c.draw(g,showBack);
            }
            e.Graphics.DrawImageUnscaled(_backBuffer, 0, 0);
        }

        private void InitCardsAndStart()
        {
            _pendingMatchCard = null;
            _cards.Clear();
            EnsureBackBuffer();
            int w = Math.Max(1, panel2.ClientSize.Width);
            int h = Math.Max(1, panel2.ClientSize.Height);
            for (int i = 0; i < 52; i++)
            {
                var card = new PokerCard(i);
                int maxX = Math.Max(1, w - card.Width);
                int maxY = Math.Max(1, h - card.Height);
                card.setPos(_rand.Next(0, maxX), _rand.Next(0, maxY));
                int vx = 0, vy = 0;
                while (vx == 0) vx = _rand.Next(-5, 6);
                while (vy == 0) vy = _rand.Next(-5, 6);
                card.setVec(vx, vy);
                _cards.Add(card);
            }
            _timer.Start();
            panel2.Invalidate();
        }

        private void InitPairCardsAndStart()
        {
            _pendingMatchCard = null;
            _cards.Clear();
            EnsureBackBuffer();
            int w = Math.Max(1, panel2.ClientSize.Width);
            int h = Math.Max(1, panel2.ClientSize.Height);
            // choose 26 distinct indices
            var chosen = new HashSet<int>();
            while (chosen.Count < 26)
            {
                chosen.Add(_rand.Next(0, 52));
            }
            var list = new List<int>(chosen);
            // duplicate each (total 52)
            list.AddRange(list);
            // shuffle
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = _rand.Next(i + 1); int tmp = list[i]; list[i] = list[j]; list[j] = tmp;
            }
            foreach (var index in list)
            {
                var card = new PokerCard(index);
                int maxX = Math.Max(1, w - card.Width);
                int maxY = Math.Max(1, h - card.Height);
                card.setPos(_rand.Next(0, maxX), _rand.Next(0, maxY));
                int vx = 0, vy = 0; while (vx == 0) vx = _rand.Next(-4, 5); while (vy == 0) vy = _rand.Next(-4, 5);
                card.setVec(vx, vy); _cards.Add(card);
            }
            //_timer.Start();
            panel2.Invalidate();
        }

        private void panel2_MouseDown(object sender, MouseEventArgs e)
        {
            if (_cards.Count == 0) return;
            // find topmost hit card (iterate reverse)
            PokerCard hit = null;
            for (int i = _cards.Count - 1; i >= 0; i--)
            {
                if (_cards[i].HitTest(e.X, e.Y)) { hit = _cards[i]; break; }
            }
            if (hit == null) return;
            // bring to top by swapping with current top element (z-order) without changing position
            var top = _cards[_cards.Count - 1];
            if (!object.ReferenceEquals(hit, top))
            {
                int idx = _cards.IndexOf(hit);
                _cards[idx] = top;
                _cards[_cards.Count - 1] = hit;
            }
            // matching logic
            if (_pendingMatchCard == null)
            {
                _pendingMatchCard = hit;
            }
            else if (!object.ReferenceEquals(_pendingMatchCard, hit))
            {

                if (_pendingMatchCard.tag == hit.tag)
                {
                    // remove both
                    _pendingMatchCard.enabled = false;
                    hit.enabled = false;
                    _cards.Remove(_pendingMatchCard);
                    _cards.Remove(hit);
                }
                _pendingMatchCard = null;
            }
            panel2.Invalidate();
        }

        private void button1_Click(object sender, EventArgs e) => InitCardsAndStart();
        private void button2_Click(object sender, EventArgs e) => InitPairCardsAndStart();

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            showBack = checkBox1.Checked;
        }
    }
}
