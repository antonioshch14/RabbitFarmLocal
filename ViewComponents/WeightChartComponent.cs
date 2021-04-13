using RabbitFarmLocal.Models;
using RabbitFarmLocal.Models.Chart;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using static RabbitFarmLocal.BusinessLogic.RabbitProcessor;
using System;

namespace RabbitFarmLocal.ViewComponents
{
    [ViewComponent(Name = "weightjs")]
    public class WeightChartComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string name, string name2) //string rabId, string partId
        {
            int rabId = Convert.ToInt32(name);
            int partId = Convert.ToInt32(name2);
            List<FattWeightModel> wgt = FattWeight.Load(rabId, partId);
            ChartJs chart = new ChartJs ();
            //{
            //    type = "bar",
            //    responsive = true
            //};
            chart.type = "line";
            chart.data.datasets.Add(new Dataset());
            chart.responsive = true;
            chart.options.scales.yAxes.Add(new yAxes());
            chart.options.scales.yAxes[0].ticks.beginAtZero = true;
            chart.data.datasets[0].borderColor.Add("rgba(255, 0, 0, 1)");
            chart.data.datasets[0].backgroundColor.Add("rgba(0, 0, 0, 0)");
            int ii = 0;
            for (int i = wgt.Count-1; i >= 0; i--)
            {                               
                chart.data.datasets[0].data.Add(new string(""));
                chart.data.datasets[0].data[ii] = wgt[i].Weight.ToString();
                chart.data.labels.Add(new string(""));
                chart.data.labels[ii] = wgt[i].DateString;
                ii++;
            }
           

            var chartModel = new WeightChartViewModel()
            {
                Chart = chart,
                ChartJson = JsonConvert.SerializeObject(chart, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                })
            };

            return View(chartModel);
        }
    }
}
