
using RabbitFarmLocal.Models;
using RabbitFarmLocal.Models.Chart;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using static RabbitFarmLocal.BusinessLogic.RabbitProcessor;
using System;
using System.Linq;
using RabbitFarmLocal.BusinessLogic;

namespace RabbitFarmLocal.ViewComponents
{
    [ViewComponent(Name = "weightcurvejs")]
    public class WeitghCurveComponent : ViewComponent
    {

        private string GetCharJSON(float[] values, string lable, List<float>[] outerValues=null, string lable2=null) {
           
            ChartJs chart = new ChartJs(values.Length, true);
           
            chart.data.datasets.Add(new Dataset());
            chart.data.datasets[0].data = new List<CharData>();
            chart.data.datasets[0].spanGaps = false;
            chart.data.datasets[0].fill = false;
            chart.data.datasets[0].label = lable;
            if (outerValues == null)
            {
                chart.type = "line";
            }
            else
            {
                chart.type = "scatter";
                chart.data.datasets.Add(new Dataset());
                chart.data.datasets[1].data = new List<CharData>();
                chart.data.datasets[1].spanGaps = false;
                chart.data.datasets[1].fill = false;
                chart.data.datasets[1].label = lable2;
                chart.data.datasets[1].borderColor.Add("rgba(74, 50, 168, 1)");
                
                chart.data.datasets[1].borderWidth = 1;




            }
            chart.options.responsive = true;
            
            //chart.responsive = true;
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
           
            chart.data.datasets[0].borderColor.Add("rgba(255, 0, 0, 1)");
            chart.data.datasets[0].backgroundColor.Add("rgba(0, 0, 0, 0)");

            //int ii = 0;
            for (int i = values.Length - 1; i >= 0; i--)
            {
                if (values[i] != 0)
                {
                    chart.data.datasets[0].data.Add(new CharData()
                    {
                        y = values[i],
                        x = i
                    });
                    if (outerValues != null) chart.data.datasets[0].backgroundColor.Add("rgba(255, 0, 0, 1)");

                    chart.data.labels.Add(new string(i.ToString()));
                    // ii++;
                }

                if (outerValues != null)
                {
                    if (outerValues[i] == null) continue;
                    foreach (var v in outerValues[i])
                    {
                        chart.data.datasets[1].data.Add(new CharData()
                        {
                            y = v,
                            x = i
                        });
                        chart.data.datasets[1].backgroundColor.Add("rgba(74, 50, 168, 1)");
                    }
                }


            }
            JsonSerializerOptions options = new()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = true// change to false in production
            };
            string jsonObj = JsonSerializer.Serialize(chart, options);
            System.Diagnostics.Debug.WriteLine(jsonObj);
            return jsonObj;
        }
        public IViewComponentResult Invoke()
        {
            //bool viewWeightCurve = name == "curve";
            List<RabWeightCurve> RabCurves;
            var curve = RabbitFarmLocal.BusinessLogic.WeightLogic.CreateGrowCurve(out RabCurves);
            WeightChartCurveViewModel chart = new WeightChartCurveViewModel();
            float[] weight=new float[curve.Length];
            float[] factor=new float[curve.Length];
            float[] weightWOGaps = new float[curve.Length];
            float[] weightSteam = new float[curve.Length];
            var outSortedValues = Enumerable.Range(0,curve.Length).Select(i=>new List<float>()).ToArray();
            for (int i = 0; i < curve.Length; i++)
            {
                weight[i] = curve[i].MeanWeight;
                factor[i] = curve[i].DayRiseFactor;
                weightWOGaps[i] = curve[i].MeanWeightWOGaps;
                weightSteam[i] = curve[i].WeightSteamlighned;
                outSortedValues[i] = curve[i].SortedOutWeight;
            }
            chart.ChartJsonW = GetCharJSON(weight, "Средниий вес кроликов на откорме", outSortedValues, "Filtered out values");
            chart.ChartJsonF = GetCharJSON(factor, "Средниий коэффициент роста кроликов на откорме");
            chart.ChartJsonWWOG = GetCharJSON(weightWOGaps, "Средниий средний вес без промиежутков");
            chart.ChartJsonWST = GetCharJSON(weightSteam, "Средниий вес спрямленный");
            chart.ChartJsonAllLines = GetCharJSONLines(RabCurves, weightSteam);

            return View(chart);
        }
        private string GetCharJSONLines(List<RabWeightCurve> RabCurves, float[] streamedVAlues)
        {

            ChartJs chart = new ChartJs(RabCurves.Count, true);
            chart.type = "line";
            chart.options.responsive = true;
            for(int ii=0;ii< RabCurves.Count;ii++)
            {
                chart.data.datasets.Add(new Dataset());
                chart.data.datasets[ii].data = new List<CharData>();
                chart.data.datasets[ii].spanGaps = false;
                chart.data.datasets[ii].fill = false;
               // chart.data.datasets[ii].label = RabCurves[ii].PartId.ToString();
                if(RabCurves[ii].Status!= GrowStat.norm) chart.data.datasets[ii].borderColor.Add(String.Format("rgba({0}, 0, 0, 1)", RabCurves[ii].PartId+100));
                else chart.data.datasets[ii].borderColor.Add(String.Format("rgba(0, {0}, 0, 1)", RabCurves[ii].PartId + 100));
                chart.data.datasets[ii].backgroundColor.Add("rgba(0, 0, 0, 0)");
                for (int i=0;i< RabCurves[ii].WeightArray.Length;  i++)
                {
                    if (RabCurves[ii].WeightArray[i]!= null)
                    {
                        chart.data.datasets[ii].data.Add(new CharData()
                        {
                            y = RabCurves[ii].WeightArray[i].MeanWeight,
                            x = i
                        });
                        //chart.data.labels.Add(new string(i.ToString()));
                        // ii++;
                    }
                }
            }
            //----------------------------
            int ind = RabCurves.Count ;
            chart.data.datasets.Add(new Dataset());
            chart.data.datasets[ind].data = new List<CharData>();
            chart.data.datasets[ind].spanGaps = false;
            chart.data.datasets[ind].fill = false;
            chart.data.datasets[ind].label = "Streamlined";
           chart.data.datasets[ind].borderColor.Add("rgba(0, 0, 0, 1)");
            chart.data.datasets[ind].backgroundColor.Add("rgba(0, 0, 0, 0)");
            for (int i = 0; i < streamedVAlues.Length; i++)
            {
               
                    chart.data.datasets[ind].data.Add(new CharData()
                    {
                        y = streamedVAlues[i],
                        x = i
                    });
                    chart.data.labels.Add(new string(i.ToString()));
            }
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
            chart.options.legend = new Display()
            {
                display = false
            };
            //int ii = 0;
            
            JsonSerializerOptions options = new()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = true// change to false in production
            };
            string jsonObj = JsonSerializer.Serialize(chart, options);
            System.Diagnostics.Debug.WriteLine(jsonObj);
            return jsonObj;
        }
    }
}
