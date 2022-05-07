using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SammyFunApp
{
    public static class FartHelper
    {
        private static Random random = new Random();    

        public static void PlayRandom()
        {
           var fartIndex= random.Next(10);

            SpeechHelper.Instance.Speak($"fart{fartIndex}");
        }
    }
}
