using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitFarmLocal.Models.Chart
{
    public class WeightChart
     {
        public WeightChart (int entries)
        {
            data = new DataWeight[entries];
            for(int i = 0; i < entries; i++)
            {
                data[i] = new DataWeight();
            }
        }
        public DataWeight[] data { get; set; }
    }
    public class DataWeight 
    {
        public string y { get; set; }
        public float x { get; set; }

    }
}
