using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SammyFunApp
{
    public class PaintColourButton : ToolStripButton
    {

        public PaintColourButton(string text, System.Drawing.Image image, EventHandler eventHandler, Color backGroundColor) 
            : base(text, image, eventHandler)
        {
            AutoSize = false;
            Width = 80;
            Height = 80;
            BackColor = backGroundColor;
            ForeColor = PerceivedBrightness(BackColor) > 130 ? Color.Black : Color.White;
        }
        private int PerceivedBrightness(Color c)
        {
            return (int)Math.Sqrt(
            c.R * c.R * .241 +
            c.G * c.G * .691 +
            c.B * c.B * .068);
        }

    }
}
