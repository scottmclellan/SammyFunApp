using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;

namespace SammyFunApp
{
    public static class SpeechHelper
    {
        private static string voiceName = "Microsoft Zira Desktop";
        public static void SpeakAsync(string text, Action<Task> onComplete = null)
        {
            var task = Task.Run(() =>
            {
                using (var speaker = new SpeechSynthesizer())
                {
                    speaker.SelectVoice(voiceName);
                    speaker.Speak(text);
                }
            });

            if(onComplete != null)
                task.ContinueWith(onComplete, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public static void Speak(string text)
        {
            using (var speaker = new SpeechSynthesizer())
            {
                speaker.SelectVoice(voiceName);
                speaker.Speak(text);
            }
        }
    }
}
