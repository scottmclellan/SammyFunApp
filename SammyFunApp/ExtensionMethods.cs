using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SammyFunApp
{
    public static class ExtensionMethods
    {
        public static void Stop(this List<PeekabooBuddy> buddies)
        {
            foreach (var buddy in buddies)
                buddy.Stop();
        }

        public static void Start(this List<PeekabooBuddy> buddies)
        {
            foreach (var buddy in buddies)
                buddy.Start();
        }
    }
}
