using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MemoryGame
{
    public partial class Form1 : Form
    {
        //load images
        int card_width = 71;
        int card_height = 96;
        int cards_in_row = 5;
        int cards_in_col = 4;
        int gap = 3;
        const int card_back_index = 55;
        Image img;
        int[] cards;
        int[] poker;
        int[] flippedCard;
        float scale;
        int prev_card_index;
        public Form1()
        {
            InitializeComponent();
            img = Properties.Resources.poker;
            this.panel_board.Paint += new PaintEventHandler(this.panel_board_Paint);
            typeof(Panel).InvokeMember("DoubleBuffered", System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic, null, panel_board, new object[] { true });
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
            // get from first total/2 card from poker and make pairs
            int[] cards = new int[total];
            for (int i = 0; i < total / 2; i++)
            {
                cards[i] = poker[i];
                cards[i + total / 2] = poker[i];
            }
            // shuffle cards
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
        
        public void drawCardsbyFlipState(Graphics g , float scale)
        {
            // Draw card front if flippedCard is 1, otherwise draw back
           
            for (int i = 0; i < cards_in_col; i++)
            {
                for (int j = 0; j < cards_in_row; j++)
                {
                    int index = i * cards_in_row + j;
                    int card_index;
                    if (flippedCard[index] == 1)
                    {
                        card_index = cards[index];
                    }
                    else
                    {
                        card_index = card_back_index;
                    }
                    int x = card_index / 13;
                    int y = card_index % 13;
                    RectangleF destRect = new RectangleF(
                        j * (card_width * scale + gap),
                        i * (card_height * scale + gap),
                        card_width * scale,
                        card_height * scale);
                    Rectangle srcRect = new Rectangle(y * card_width, x * card_height, card_width, card_height);
                    g.DrawImage(img, destRect, srcRect, GraphicsUnit.Pixel);
                }
            }

        }

        public void initFlippedCard(int total)
        {
            flippedCard = new int[total];
            for (int i = 0; i < total; i++)
            {
                flippedCard[i] = 0;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // calculate scale to fit cards in panel
            scale = Math.Min(
                (float)(panel_board.Width - (cards_in_row - 1) * gap) / (cards_in_row * card_width), 
                (float)(panel_board.Height - (cards_in_col - 1) * gap) / (cards_in_col * card_height));
            // draw cards
            int total_cards = cards_in_row * cards_in_col;
            initFlippedCard(total_cards);
            poker = shuffelCards();
            cards = pickcards(poker, total_cards);
            panel_board.Invalidate();

        }

        private void panel_board_MouseClick(object sender, MouseEventArgs e)
        {
            if (cards == null) return;
            //get clicked card index
            int col = (int)(e.X / (card_width * scale + gap));
            int row = (int)(e.Y / (card_height * scale + gap));
            // check if click out of bounds
            if (col >= cards_in_row || row >= cards_in_col)
            {
                return;
            }

            int index = row * cards_in_row + col;
            int total_cards = cards_in_row * cards_in_col;
            if (index >= 0 && index < total_cards)
            {
                flippedCard[index] = flippedCard[index] == 1 ? 0 : 1;
                panel_board.Invalidate();
            }
        }

        private void panel_board_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(Color.Gray);
            if (cards != null)
            {
                drawCardsbyFlipState(e.Graphics, scale);
            }
        }
    }
}
