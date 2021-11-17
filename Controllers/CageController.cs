using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RabbitFarmLocal.Models;
using static RabbitFarmLocal.BusinessLogic.RabbitProcessor;
using static RabbitFarmLocal.BusinessLogic.CageLogic;
using static RabbitFarmLocal.Controllers.MyFunctions;

namespace RabbitFarmLocal.Controllers
{
    public class CageController : Controller
    {
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public ActionResult CagesView()
        {
            List<CageModel> cages = Cage.LoadAll();
            ViewBag.caJson = CageJSON(0, false);
            return View(cages);
        }
        public ActionResult Create()
        {
            ViewBag.MadeDate = DateToString(DateTime.Now);
            return PartialView(new CageModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CageModel model)
        {
            if (ModelState.IsValid)
            {
                int recordCreated = Cage.Create(model);
                RabbitFarmLocal.Start.ConstantsSingelton.UpdateCages();
            }
            
            return RedirectToAction("CagesView");
        }
        public ActionResult Edit(int Id)
        {
            CageModel cage = Cage.LoadOne(Id);
            ViewBag.MadeDate = DateToString(cage.Made);
            return PartialView(cage);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CageModel model)
        {
            if (ModelState.IsValid)
            {
                int recordCreated = Cage.Edit(model);
                RabbitFarmLocal.Start.ConstantsSingelton.UpdateCages();
            }
            return RedirectToAction("CagesView");
        }
       
        public ActionResult CageDetail(string _id)
        {
            int id = Convert.ToInt32(_id);
            var cage = Cage.LoadOne(id);
            ViewBag.caJson = CageJSON(id, true);
            return PartialView(cage);
        }
        [HttpPost]
        public ActionResult _allCagesmage(string _id)
        {
            int id = Convert.ToInt32(_id);
            //var cage = Cage.LoadOne(id);
            ViewBag.caJson = CageJSON(id,false);
            ViewBag.Id = _id;
            return PartialView();
        }
        public ActionResult Delete(int id)
        {
            int recordDeleted = Cage.Delete(id);
            return RedirectToAction("CagesView");
        }

    }
}
