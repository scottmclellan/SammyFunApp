using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
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


        public static string CreateFileName(this string word)
        {
            string fileName = Regex.Replace(word, "[^a-zA-Z0-9 -]", "");
            fileName = fileName.Replace(" ", "_");
            return fileName.Substring(0, Math.Min(fileName.Length, 50));
        }


    }
}
