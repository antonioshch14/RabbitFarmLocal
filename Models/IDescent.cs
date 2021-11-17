using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitFarmLocal.Models
{
    interface IDescent
    {
        public static string Breed { get; set; }       
        public string BreedString { get; set; }
        public Dictionary<int, int> BreedDict { get; set; }
       
        public void GetBreedDictionary();
        public void SetBreedString();
        public void SetBreedStringToDisplay();
        public void CreateBreedDictionary();
    }
}
