using RabbitFarmLocal.Models;
using RabbitFarmLocal.Models.Chart;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using static RabbitFarmLocal.BusinessLogic.FinReport;

namespace RabbitFarmLocal.ViewComponents
{
    [ViewComponent(Name = "chartjs")]
    public class ChartJsViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            // Ref: https://www.chartjs.org/docs/latest/
            var chartData = @"
            {
                type: 'bar',
                responsive: true,
                data:
                {
                    labels: ['Red', 'Blue', 'Yellow', 'Green', 'Purple', 'Orange'],
                    datasets: [{
                        label: '# of Votes',
                        data: [12, 19, 3, 5, 2, 3],
                        backgroundColor: [
                        'rgba(255, 99, 132, 0.2)',
                        'rgba(54, 162, 235, 0.2)',
                        'rgba(255, 206, 86, 0.2)',
                        'rgba(75, 192, 192, 0.2)',
                        'rgba(153, 102, 255, 0.2)',
                        'rgba(255, 159, 64, 0.2)'
                            ],
                        borderColor: [
                        'rgba(255, 99, 132, 1)',
                        'rgba(54, 162, 235, 1)',
                        'rgba(255, 206, 86, 1)',
                        'rgba(75, 192, 192, 1)',
                        'rgba(153, 102, 255, 1)',
                        'rgba(255, 159, 64, 1)'
                            ],
                        borderWidth: 1
                    }]
                },
                options:
                {
                    scales:
                    {
                        yAxes: [{
                            ticks:
                            {
                                beginAtZero: true
                            }
                        }]
                    }
                }
            }";
           

            List<FinRepModel> FinList = ReportForYear(2021);
            ChartJs chart = new ChartJs(FinList.Count,false);
            chart.type = "bar";
            chart.responsive = true;
            chart.options.scales.yAxes.Add(new yAxes());
            chart.options.scales.yAxes[0].ticks.beginAtZero=true;
            chart.data.datasets.Add(new Dataset());
            chart.data.datasets[0].data = new List<string>();
            chart.options.scales.yAxes.Add(new yAxes());
            chart.options.scales.yAxes[1].ticks.beginAtZero = true;
            chart.data.datasets.Add(new Dataset());
            chart.data.datasets[1].data = new List<string>();
            chart.options.responsive = true;
          
            for (int i = 0; i < FinList.Count; i++)
            {
                chart.data.datasets[0].data.Add(new string(""));
                chart.data.datasets[0].data[i] = FinList[i].EarnedTotalWIthOurConsum.ToString();
                chart.data.datasets[0].label = "Прибыль общая";
                chart.data.datasets[0].backgroundColor.Add("rgba(54, 162, 235, 0.2)");
                chart.data.datasets[1].data.Add(new string(""));
                chart.data.datasets[1].data[i] = FinList[i].SpentTotal.ToString();
                chart.data.datasets[1].label = "Затраты";
                chart.data.datasets[1].backgroundColor.Add("rgba(255, 159, 64, 0.2)");
                chart.data.labels.Add(FinList[i].Month);
                
            }
            //var chart = JsonConvert.DeserializeObject<ChartJs>(chartData);
            var chartModel = new ChartJsViewModel
            {
                Chart = chart,
                ChartJson = JsonConvert.SerializeObject(chart, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                })
            };
            //System.Diagnostics.Debug.WriteLine(chartModel.ChartJson);
            return View(chartModel);
        }
    }
}
