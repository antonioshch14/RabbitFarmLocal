using System;

using RabbitFarmLocal.Models.Chart;
using RabbitFarmLocal.ViewComponents;
using Newtonsoft.Json;

namespace RabbitFarmLocal.Models
{
    public class ChartJsViewModel
    {
        public ChartJs Chart { get; set; }
        public string ChartJson { get; set; }
    }
    public class WeightChartViewModel
    {
        public ChartJs Chart { get; set; }
        public string ChartJson { get; set; }
    }
    public class WeightChartCurveViewModel
    {
        
        public string ChartJsonW { get; set; }
       
        public string ChartJsonF { get; set; }
        public string ChartJsonWWOG { get; set; }
        public string ChartJsonWST { get; set; }
        public string ChartJsonAllLines { get; set; }

    }


}
