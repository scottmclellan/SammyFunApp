using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SammyFunApp
{
    public class TrashBinButton : ToolStripButton
    {

        public TrashBinButton( EventHandler eventHandler) 
            : base("", null, eventHandler)
        {
            AutoSize = false;
            Width = 80;
            Height = 80;
        }
    }
}
