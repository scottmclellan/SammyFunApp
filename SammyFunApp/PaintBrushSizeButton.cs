using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SammyFunApp
{
    public class PaintBrushSizeButton : ToolStripButton
    {
        public int PenSize { get; private set; }
        public PaintBrushSizeButton(EventHandler eventHandler, int penSize, int maxPenSize = 55)
         : base("", DrawBrushIcon(80, penSize, maxPenSize), eventHandler)
        {
            AutoSize = false;
            Width = 80;
            Height = 80;
            PenSize = penSize;
        }

        protected static Image DrawBrushIcon(int width, double penSize, double maxPenSize = 55)
        {
            double ratio = penSize / maxPenSize;

            int circleDiameter = (int)((double)width * ratio);
            int x, y;
            x = y = (width - circleDiameter) / 2;

            Bitmap img = new Bitmap(width, width);
            using (Graphics imgGraphics = Graphics.FromImage(img))
            {
                imgGraphics.FillEllipse(Brushes.Black, x, y, circleDiameter, circleDiameter);
            }

            return img;
        }

    }
}
