using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitFarmLocal.Models
{
    public class BreedsModel
    {
        public int Id { get; set; }
        [DisplayName("Порода")]
        public string Name { get; set; }
    }
    public class BreedMixModel
    {
        [DisplayName("Порода")]
        public Descent BreedOfRab { get; set; }
        [DisplayName("Число кролликов такой породы")]
        public int NumberRabbitsOfThisBreed { get; set; }
    }
  
}
