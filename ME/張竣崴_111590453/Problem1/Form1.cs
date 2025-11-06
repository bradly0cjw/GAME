using Problem1.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Problem1
{
    public partial class From1 : Form
    {
        public From1()
        {
            InitializeComponent();
        }
        private Bitmap getNameInText(String name)
        {
            int width = 720;
            int height = 560;
            Bitmap bmp = new Bitmap(width, height);

            using (Bitmap textBmp = new Bitmap(width, height))
            {
                using (Graphics g = Graphics.FromImage(textBmp))
                {
                    g.Clear(Color.White);
                    using (Font font = new Font("標楷體", 200))
                    using (Brush brush = new SolidBrush(Color.Black))
                    {
                        g.DrawString(name, font, brush, 0, 0);
                    }
                }

                using (Graphics gOut = Graphics.FromImage(bmp))
                {
                    gOut.Clear(Color.White);

                    int step = 10;      
                    int dotSize = 20;   
                    int threshold = 220; 

                    Image img = Resources.png_1089; 

                    for (int y = 0; y < height; y += step)
                    {
                        for (int x = 0; x < width; x += step)
                        {
                            Color c = textBmp.GetPixel(x, y);
                            if (c.A > 0 && (c.R < threshold || c.G < threshold || c.B < threshold))
                            {
                                var dest = new Rectangle(x - dotSize / 2, y - dotSize / 2, dotSize, dotSize);
                                gOut.DrawImage(img, dest);
                            }
                        }
                    }
                }
            }

            CreateGraphics().DrawImage(bmp, 100, 100);
            return bmp;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            getNameInText("竣崴");
        }
    }
}
