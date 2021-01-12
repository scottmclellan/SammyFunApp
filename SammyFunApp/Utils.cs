using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SammyFunApp.Utils
{
    public static class ResourceHelper
    {
        public static Stream GetResourceData(string resourceName)
        {
            var embeddedResource = Assembly.GetExecutingAssembly().GetManifestResourceNames().FirstOrDefault(s => string.Compare(s, resourceName, true) == 0);

            if (!string.IsNullOrWhiteSpace(embeddedResource))
            {
                return Assembly.GetExecutingAssembly().GetManifestResourceStream(embeddedResource);
            }

            return null;
        }

    }
}
