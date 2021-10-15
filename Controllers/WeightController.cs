using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RabbitFarmLocal.BusinessLogic;
using RabbitFarmLocal.Models;
using static RabbitFarmLocal.BusinessLogic.RabbitProcessor;
using static RabbitFarmLocal.Controllers.MyFunctions;


namespace RabbitFarmLocal.Controllers
{
    public class WeightController : Controller
    {
        public ActionResult RabWeightCreate(int rabId)
        {
            WeightModel wgt = new WeightModel()
            {
                RabId=rabId
            };
            ViewBag.Message = String.Format("Занести в базу новое взвешивание кролика {0}", rabId);
            ViewBag.Date = DateToString(DateTime.Now);

            return PartialView(wgt);
        }
        [HttpPost]
        //[Authorize(Roles = "admin,owner")]
        [ValidateAntiForgeryToken]
        public ActionResult RabWeightCreate(WeightModel wgt)
        {
            if (ModelState.IsValid)
            {
                int recordCreated = RabWeight.Create(wgt);
                return RedirectToAction("RabWeightView", "Weight", new { rabId = wgt.RabId });
            }
            return View(wgt);
        }
        public ActionResult RabWeightView(int rabId)
        {
            List<WeightModel> wgt = RabWeight.Load(rabId);
            
            ViewBag.Message = String.Format("Взвешивание кролика ",rabId);
            ViewBag.RabId = rabId;
            return View(wgt);
        }
        public ActionResult RabWeightEdit(int rabId, int weightId)
        {
            List<WeightModel>? wgtList = RabWeight.Load(rabId);
            WeightModel? wgt = wgtList.Find(x => x.Id == weightId);
            ViewBag.Date = DateToString(wgt.Date);
            return PartialView(wgt);
        }
        [HttpPost]
        public ActionResult RabWeightEdit(WeightModel wgt)
        {
            RabWeight.Edit(wgt);
            return RedirectToAction("RabWeightView", "Weight",new {rabId=wgt.RabId });
        }
        public ActionResult RabWeightDelete(int rabId, int weightId)
        {
            
            RabWeight.Delete(weightId);
            return RedirectToAction("RabWeightView", "Weight", new { rabId = rabId });
        }
        //=========================
        public ActionResult FatWeightCreate(int rabId, int partId, Caller CalledFrom = Caller.fatWeight)
        {
            FattWeightModel wgt = new FattWeightModel()
            {
                RabId = rabId,
                PartId= partId,
               ECaller= CalledFrom
            }; ;
            ViewBag.Message = String.Format("Занести в базу новое взвешивание кролика {0}", rabId);
            ViewBag.Date = DateToString(DateTime.Now);

            return PartialView(wgt);
        }
        [HttpPost]
        //[Authorize(Roles = "admin,owner")]
        [ValidateAntiForgeryToken]
        public ActionResult FatWeightCreate(FattWeightModel wgt)
        {
            if (ModelState.IsValid)
            {
                List<FattWeightModel> wgtToFindIfLatestExist = FattWeight.Load(wgt.RabId, wgt.PartId);
                if (wgtToFindIfLatestExist.Exists(x => x.Date == wgt.Date))
                {
                    wgt.Id = wgtToFindIfLatestExist.Find(x => x.Date == wgt.Date).Id;
                    FattWeight.Edit(wgt);
                }
                else { int recordCreated = FattWeight.Create(wgt); }
                RabbitFarmLocal.Start.WeighGrow.UpdateWeitghCurve();
                if (wgt.ECaller==Caller.allfatt) return RedirectToAction("AllFatteningView", "Mate");
                return RedirectToAction("FatWeightView", "Weight", new { rabId = wgt.RabId, partId=wgt.PartId });
            }
            
            return View(wgt);
        }
        public ActionResult FatWeightView(int rabId, int partId)
        {
            List<FattWeightModel> wgt = FattWeight.Load(rabId, partId);

            ViewBag.Message = String.Format("Взвешивание кролика ", rabId);
            ViewBag.RabId = rabId;
            ViewBag.PartId = partId;
            return View(wgt);
        }
        public ActionResult FatWeightEdit(int rabId, int partId, int weightId)
        {
            List<FattWeightModel>? wgtList = FattWeight.Load(rabId, partId);
            FattWeightModel? wgt = wgtList.Find(x => x.Id == weightId);
            ViewBag.Date = DateToString(wgt.Date);
            return PartialView(wgt);
        }
        [HttpPost]
        public ActionResult FatWeightEdit(FattWeightModel wgt)
        {
            FattWeight.Edit(wgt);
            return RedirectToAction("FatWeightView", "Weight", new { rabId = wgt.RabId, partId = wgt.PartId });
        }
        public ActionResult FatWeightDelete(int rabId, int partId, int weightId)
        {

            FattWeight.Delete(weightId);
            return RedirectToAction("FatWeightView", "Weight", new { rabId = rabId, partId = partId });
        }
        public ActionResult AllFatteningWeight()
        {
            List<FatteningModel> fatt = LoadFattenigAllAlive();

            //Age partAge = new Age(part.Date);
            ViewBag.Message1 = String.Format("Кроликов на откорм {0} ", fatt.Count());
            return View(fatt);
        }
        public ActionResult DrawWeightCurve()
        {
            //WeightLogic.CreateGrowCurve();

            //return RedirectToAction("AllFatteningView","Mate");
            return View();
        }

    }
}
