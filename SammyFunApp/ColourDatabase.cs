using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SammyFunApp
{
    public class ColourDatabase
    {
        public List<Colour> Colours { get; set; }
        public ColourDatabase()
        {
           
        }

        public ColourObject GetRandomObject(string colourName)
        {
            var colour = Colours.Where(x => x.ColourName.ToUpper() == colourName.ToUpper()).FirstOrDefault();

            if( colour == null) return null;
            if (colour.AvailableObjects.Count == 0) return null;

            var obj = colour.AvailableObjects[new Random().Next(colour.AvailableObjects.Count)];
            obj.LastRecalled = DateTime.Now;
            return obj;
        }
        public static ColourDatabase Populate(string jsonPath)
        {
            string jsonContents = File.ReadAllText(jsonPath);
            var colourDB = JsonConvert.DeserializeObject<ColourDatabase>(jsonContents);
            return colourDB;
        }

        public static string Save(ColourDatabase db)
        {
            return JsonConvert.SerializeObject(db);
        }

        public class Colour
        {
            public string ColourName { get; set; }
            public List<ColourObject> Objects { get; set; }

            public List<ColourObject> AvailableObjects => Objects.Where(x => x.LastRecalled == null || DateTime.Now.Subtract(x.LastRecalled.Value).Minutes > 10).ToList();
        }

        public class ColourObject
        {
            public string Name { get; set; }
            public string Description { get; set; }

            public DateTime? LastRecalled { get; set; }
        }
       
    }
}
