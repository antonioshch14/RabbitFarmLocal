using RabbitFarmLocal.Models;
using RabbitFarmLocal.Models.Chart;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using static RabbitFarmLocal.BusinessLogic.RabbitProcessor;
using System;
using System.Linq;

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
            ChartJs chart = new ChartJs(wgt.Count, true);
            chart.type = "line";
            chart.data.datasets.Add(new Dataset());
            chart.data.datasets[0].data = new List<CharData>();
            chart.data.datasets[0].spanGaps = false;
            chart.data.datasets[0].fill = false;
            chart.options.responsive = true;
            chart.options.scales.yAxes.Add(new yAxes());
            chart.options.scales.xAxes.Add(new xAxes());
            chart.options.scales.yAxes[0].ticks.beginAtZero = true;
            chart.options.scales.yAxes[0].display = true;
            chart.options.scales.yAxes[0].scaleLabel = new ScaleLabel()
            {
                display = true
             };
            chart.options.scales.xAxes[0].type = "linear";
            chart.options.scales.xAxes[0].display = true;
            chart.options.scales.xAxes[0].ticks.beginAtZero = false;
            chart.options.scales.xAxes[0].ticks.autoSkip = true;
            chart.options.scales.xAxes[0].ticks.maxTicksLimit = 10;
            //chart.options.scales.grace = "50%";
            //chart.options.scales.type = "linear";
            //chart.options.scales.y = new ScaleXY()
            //{
            //    min = wgt.Max(x => x.Weight),
            //    max = wgt.Min(x => x.Weight)

            //};
            chart.data.datasets[0].borderColor.Add("rgba(255, 0, 0, 1)");
            chart.data.datasets[0].backgroundColor.Add("rgba(0, 0, 0, 0)");

            int ii = 0;
            for (int i = wgt.Count-1; i >= 0; i--)
            {
                chart.data.datasets[0].data.Add(new CharData() { 
                y= wgt[i].Weight,
                x= wgt[i].days
            });
                chart.data.labels.Add(new string(wgt[i].days.ToString()));

                ii++;
            }
            JsonSerializerOptions options = new()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented=true// change to false in production
            };
            var chartModel = new WeightChartViewModel()
            {
                Chart = chart,
                  ChartJson = JsonSerializer.Serialize(chart, options)
                  
              };
            System.Diagnostics.Debug.WriteLine(chartModel.ChartJson);
            return View(chartModel);
        }
    }
}
