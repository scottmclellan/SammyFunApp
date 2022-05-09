using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SammyFunApp
{
    public static class FartHelper
    {
        private static int _fartIndex = 1;
        private static int _fartCount = GetFartCount();

        private static int GetFartCount()
        {
            var fartCountAppSetting = ConfigurationManager.AppSettings.Get("FartCount");

            if (fartCountAppSetting == null || !int.TryParse(fartCountAppSetting, out int fartCount))
                return 10;

            return fartCount;

        }

        public static void PlayRandom()
        {
            System.Diagnostics.Debug.WriteLine($"FartIndex: {_fartIndex}");
            SpeechHelper.Instance.Speak($"fart{_fartIndex}");

            if (_fartIndex == _fartCount)
                _fartIndex = 1;
            else
                _fartIndex++;
        }
    }
}
