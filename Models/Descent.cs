using RabbitFarmLocal.Start;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace RabbitFarmLocal.Models
{
    public class Descent
    {
       
        [DisplayName("Порода")]
        public string Breed { get; set; }
        [DisplayName("Порода")]
        public string BreedString { get; set; }

        public Dictionary<int, int> BreedDict { get; set; }
        
        public  Dictionary<int,int> GetBreedDictionary(string Br)
        {
            Dictionary<int, int> output = new Dictionary<int, int>();
            if (Br != null && Br!="")
            {
                output = Br.Split(';')
                  .Select(s => s.Split(','))
                  .ToDictionary(
                  p => Convert.ToInt32(p[0].Trim())
                  , p => Convert.ToInt32(p[1].Trim())
                  );
            }
            return output;
        }
        public  string SetBreedString(Dictionary<int,int> BrD)
        {
            string output = "";
            if (BrD != null)
            {
                output = string.Join("; ", BrD.Select(
                p => string.Format("{0}, {1}", p.Key, p.Value)
                ));
            }
            return output;
        }
        public  string SetBreedStringToDisplay(string Br)
        {
            Dictionary<int, int> BrD= GetBreedDictionary(Br);
            string output="";
            if (BrD != null)
            {

                List<BreedsModel> breeds = ConstantsSingelton.GetListOfBreeds();
                Dictionary<string, int> dicRabBreeds = new Dictionary<string, int>();
                foreach (KeyValuePair<int, int> kvpBD in BrD)
                {
                    dicRabBreeds.Add(breeds.Find(x => x.Id == kvpBD.Key).Name, kvpBD.Value);

                }
                output = string.Join(", ", dicRabBreeds.Select(
                   p => string.Format("{0}:{1}", p.Key, p.Value)
                   ));
            }
            return output;
        }
    }
}