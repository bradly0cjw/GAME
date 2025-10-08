using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HW5
{
    public partial class Form2 : Form
    {
        int card_width = 71;
        int card_height = 96;
        int offset = 5;
        Image img;
        public Form2()
        {
            InitializeComponent();
            //img = Image.FromFile(@"..\..\poker.png");
            img = Properties.Resources.poker;
        }

        private void button_playcard_Click(object sender, EventArgs e)
        {
            //load image from file
            int[] poker = poker_shuffle();
            //get 5 random cards from poker
            int[] candidate = new int[5];
            Array.Copy(poker, candidate, 5);
            Graphics g = panel2.CreateGraphics();
            g.Clear(System.Drawing.Color.FromArgb(0xad, 0xd8, 0xe6));

            int panelWidth = panel2.ClientSize.Width;
            int panelHeight = panel2.ClientSize.Height;

            // Calculate scaling factors
            float scaleX = (float)panelWidth / (card_width * 2 + card_width + offset * 2);
            float scaleY = (float)panelHeight / (card_height * 2 + card_height + offset * 2);
            float scale = Math.Min(scaleX, scaleY);

            // Calculate scaled card size and offset
            int scaled_card_width = (int)(card_width * scale);
            int scaled_card_height = (int)(card_height * scale);
            int scaled_offset = (int)(offset * scale);

            // Center the pattern in the panel
            int patternWidth = scaled_card_width * 3 + scaled_offset * 2;
            int patternHeight = scaled_card_height * 3 + scaled_offset * 2;
            int startX = (panelWidth - patternWidth) / 2;
            int startY = (panelHeight - patternHeight) / 2;

            int[][] dest_loc = new int[5][];
            dest_loc[0] = new int[] { startX + scaled_offset, startY + scaled_offset };
            dest_loc[1] = new int[] { startX + scaled_offset + scaled_card_width * 2, startY + scaled_offset };
            dest_loc[2] = new int[] { startX + scaled_offset + scaled_card_width, startY + scaled_offset + scaled_card_height };
            dest_loc[3] = new int[] { startX + scaled_offset, startY + scaled_offset + scaled_card_height * 2 };
            dest_loc[4] = new int[] { startX + scaled_offset + scaled_card_width * 2, startY + scaled_offset + scaled_card_height * 2 };


            for (int i = 0; i < 5; i++)
            {
                int card_index = candidate[i];
                int x = dest_loc[i][0];
                int y = dest_loc[i][1];
                Rectangle srcRect = new Rectangle((card_index % 13) * card_width, (card_index / 13) * card_height, card_width, card_height);
                Rectangle destRect = new Rectangle(x, y, scaled_card_width, scaled_card_height);
                g.DrawImage(img, destRect, srcRect, GraphicsUnit.Pixel);
            }

        }

        private int[] poker_shuffle()
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
    }
}
