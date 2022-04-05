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
    [ViewComponent(Name = "weightperbreed")]
    public class WeightPerBreedComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string breeds)
        {
            List<string> breedList = breeds.Split("|").ToList();
            List<FattWeightModel>  weights = FattWeight.LoadAll();
            List<FatteningModel> fatts = LoadAllFattening();
            List<WeightPerBreed> ListOfWeightsPerBreed = new List<WeightPerBreed>();
            List<string> chart = new List<string>();
            foreach (var F in fatts)
            {
                F.GetBreedDictionary();
            }

            List<FattWeightModel> weightsOfBreed = new List<FattWeightModel>();//temp storage for weights of a breed
            List<FattWeightModel> weightsOfOtherBreed = new List<FattWeightModel>();//temp storage for weights of breed that compared against
            
            foreach (string B in breedList)//go throught all breed variations
            {
                weightsOfBreed.Clear();
                weightsOfOtherBreed.Clear();
                foreach (var w in weights)
                {
                    weightsOfOtherBreed.Add(w.ShallowCopy()); 
                }
                var rabOfBreed = fatts.FindAll(r => r.Breed==B).Select(Rab => new { Rab.PartId}).Distinct();
                foreach(var RB in rabOfBreed)//find and store weights of the breed
                {
                    weightsOfBreed.AddRange(weights.FindAll(w => w.PartId == RB.PartId).ToList());
                    weightsOfOtherBreed.RemoveAll(w => w.PartId == RB.PartId);
                }
                
                if (weightsOfBreed.Count < 5) continue;//scip if there are less than 5 weight of that breed
                weightsOfBreed = weightsOfBreed.OrderBy(a => a.PartId).ThenBy(a => a.RabId).ThenBy(a => a.Date).ToList();
                weightsOfOtherBreed = weightsOfOtherBreed.OrderBy(a => a.PartId).ThenBy(a => a.RabId).ThenBy(a => a.Date).ToList();
                Weight[] breedCurve = RabbitFarmLocal.BusinessLogic.WeightLogic.CreateGrowCurve(out List<RabWeightCurve> BreedRabCurves, weightsOfBreed);
                Weight[] breedCurveOfOthers = RabbitFarmLocal.BusinessLogic.WeightLogic.CreateGrowCurve(out List<RabWeightCurve> BreedRabCurves2, weightsOfOtherBreed);
                
                ListOfWeightsPerBreed.Add(new WeightPerBreed()
                {
                    Breed = B,
                    RabbitsOverWeight = BreedRabCurves.Count(r => r.Status == GrowStat.overGrow),
                    RabbitsUnderWeight = BreedRabCurves.Count(r => r.Status == GrowStat.underGrow),
                    RabbitsWithNormWeight = BreedRabCurves.Count(r => r.Status == GrowStat.norm)
                }) ;
                for (int i = 0; i < breedCurve.Length; i++)
                {
                    ListOfWeightsPerBreed[^1].Weights[i] = breedCurve[i].WeightSteamlighned;
                }
                chart.Add(GetCharJSONLines(ListOfWeightsPerBreed[^1], breedCurveOfOthers));
            }
            
            return View(chart);
        }
        private string GetCharJSONLines(WeightPerBreed RabCurves, Weight[] RabCurvesOthers)
        {

            ChartJs chart = new ChartJs(RabCurves.Weights.Length, true);
            chart.type = "line";
            chart.options.responsive = true;
            
            chart.data.datasets.Add(new Dataset());
            chart.data.datasets[0].data = new List<CharData>();
            chart.data.datasets[0].spanGaps = false;
            chart.data.datasets[0].fill = false;
            RabbitFarmLocal.Models.Descent descent = new Descent();
            descent.BreedString = descent.SetBreedStringToDisplay(RabCurves.Breed);
            chart.options.title = new Title();

            chart.options.title.display = true;
            chart.options.title.text = string.Format("Веса {3} кроликов породы [{0}] не включены в отценку: {1} из-за превышение веса, {2} из-за недостаток веса"
                , descent.BreedString
                , RabCurves.RabbitsOverWeight
                , RabCurves.RabbitsUnderWeight
                , RabCurves.RabbitsWithNormWeight);
            chart.data.datasets[0].label = descent.BreedString;

           chart.data.datasets[0].borderColor.Add(String.Format("rgba({0}, 0, 0, 1)", 255));
            

            chart.data.datasets[0].backgroundColor.Add("rgba(0, 0, 0, 0)");

            for (int i = 0; i < RabCurves.Weights.Length; i++)
            {
                chart.data.datasets[0].data.Add(new CharData()
                {
                    y = RabCurves.Weights[i],
                    x = i
                });
            }

            chart.data.datasets.Add(new Dataset());
            chart.data.datasets[1].data = new List<CharData>();
            chart.data.datasets[1].spanGaps = false;
            chart.data.datasets[1].fill = false;
            chart.data.datasets[1].label = "Остальные кролики";
            chart.data.datasets[1].borderColor.Add("rgba(0, 0, 0, 1)");
            chart.data.datasets[1].backgroundColor.Add("rgba(0, 0, 0, 0)");

            for (int i = 0; i < RabCurves.Weights.Length; i++)
            {
                chart.data.datasets[1].data.Add(new CharData()
                {
                    y = RabCurvesOthers[i].WeightSteamlighned,
                    x = i
                });
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
            //chart.options.legend = new Display()
            //{
            //    display = true
            //};
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
