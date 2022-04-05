using Microsoft.AspNetCore.Mvc;
using RabbitFarmLocal.BusinessLogic;
using RabbitFarmLocal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static RabbitFarmLocal.BusinessLogic.RabbitProcessor;
using static RabbitFarmLocal.Controllers.MyExtensions;

namespace RabbitFarmLocal.Controllers
{
    public class DevelopmentController : Controller
    {

        public IActionResult Index()
        {
            
            return View();
        }
        public ActionResult CalculateWeightPredictionDeviation()
        {
            List<FattWeightModel> weights = FattWeight.LoadAll().OrderBy(a => a.PartId).ThenBy(a => a.RabId).ThenBy(a => a.Date).ToList();
            Weight[] breedCurve = RabbitFarmLocal.BusinessLogic.WeightLogic.CreateGrowCurve(out List<RabWeightCurve> BreedRabCurves, weights);
            List<FatteningModel> fatts = LoadAllFattening();
            List<float> relativeDifference = new List<float>();
            
            foreach (var (RW, index) in weights.WithIndex())
            {
                
                FatteningModel rabbit = fatts.Find(f => f.PartId == RW.PartId && f.RabPartId == RW.RabId);
                if (rabbit == null) continue;
                float calculatedWeight = RabbitFarmLocal.BusinessLogic.WeightLogic.GetProjectedWeight(RW.Date, rabbit.Born, rabbit.WeightDate, rabbit.LastWeight, rabbit.Breed);
                //float calculatedWeight = RabbitFarmLocal.BusinessLogic.WeightLogic.GetProjectedWeight(RW.Date, rabbit.Born, rabbit.WeightDate, rabbit.LastWeight, "");
                relativeDifference.Add(Math.Abs(calculatedWeight - RW.Weight) / RW.Weight);
            }
            float averDiff = relativeDifference.Average();
            ViewBag.Message = $"Average Difference is: {averDiff*100} %";
            return View("Index");
        }
        public ActionResult FillParturationStatus()
        {
            List<ParturationModel> parts = LoadParturation(new DateTime(2000, 1, 1));
            List<DLRabbitModel> rabs = Rabbit.LoadList();
            foreach(var p in parts)
            {
                int? cage = rabs.Find(x => x.RabbitId == p.MotherId).Cage;
                if (cage != null)p.Cage = (int)cage;
                if (p.SeparationDate != null) p.Status = parturStatus.separated;
                else if ((DateTime.Now - p.Date).TotalDays > 200) p.Status = parturStatus.separated;
                else if (p.Children == p.DiedChild && p.Children != 0) p.Status = parturStatus.allDead;
                else p.Status = parturStatus.feeded;
                EditParturation(p);
            }
            return View("Index");

        }
        public ActionResult ParturationUpdateAll()
        {
            int updated=ParturationUpdate.UpdateAll();
            ViewBag.Message = $"Updated {updated}";
            return View("Index");
        }
    }
}
