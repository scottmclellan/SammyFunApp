using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SammyFunApp
{
    public class MainWindowViewModel
    {

        public List<ColourDatabase.Colour> Colours { get; private set; }
        public List<int> PenSizes { get; private set; }
        public List<string> PeekabooBuddies { get; private set; }

        private readonly string _colourPath;
        public MainWindowViewModel(string colourPath)
        {
            _colourPath = colourPath;
            Initialize();
        }

        private void Initialize()
        {
            //load colours
            Colours = ColourDatabase.Populate(_colourPath).Colours;

            //load pen sizes
            PenSizes = new List<int> { 2, 5, 8, 13, 21, 34, 55 };

            //load buddies
            PeekabooBuddies = new List<string> { "mcqueen", "mater", "doc", "mcmissile", "luigi", "sally" };
        }


        
    }
}
