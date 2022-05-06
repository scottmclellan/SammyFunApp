using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        public static string CreateFileName(this string word)
        {
            string fileName = Regex.Replace(word.ToLower(), "[^a-zA-Z0-9 -]", "");
            return $"{CreateMD5(fileName)}.wav";
        }

        public static string CreateFileNameMp3(this string word)
        {
            string fileName = Regex.Replace(word.ToLower(), "[^a-zA-Z0-9 -]", "");
            return $"{CreateMD5(fileName)}.wav";
        }

        public static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                //// Convert the byte array to hexadecimal string prior to .NET 5
                StringBuilder sb = new System.Text.StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        public static byte[] ToArray(this Stream stream)
        {
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}
