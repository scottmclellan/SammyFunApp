
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SammyFunApp
{
    public static class GoogleHelper
    {
    

        private static string GetUrl( string query)
        {
            //replace spaces with + symbol
            query = query.Replace(" ", "+");

         
            return string.Format("https://kgsearch.googleapis.com/v1/entities:search?query={0}&key={1}&limit=10", query, "AIzaSyBLgAyczIPcqgle2-3Kxxzml1Lx2kxFA6U");
        }

        public static string Search(string query)
        {
            string url = GetUrl( query);

            WebRequest wr = WebRequest.Create(url);
            wr.Timeout = 60000;

            try
            {
                HttpWebResponse response = (HttpWebResponse)wr.GetResponse();
                string responseText = "";

                using (var reader = new System.IO.StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    responseText = reader.ReadToEnd();
                    //do somthing with responseText 
                }

                return responseText;
            }
            catch (Exception)
            {
                return "";
            }
        }


    }

}
