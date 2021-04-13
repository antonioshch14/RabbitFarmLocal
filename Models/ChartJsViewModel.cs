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
}
