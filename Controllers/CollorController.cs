using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RabbitFarmLocal.Models;
using static RabbitFarmLocal.BusinessLogic.RabbitProcessor;

namespace RabbitFarmLocal.Controllers
{
    public class CollorController : Controller
    {
        // GET: CollorController
        public ActionResult Index()
        {
            List<CollorModel> collors = Collor.LoadAll();
            return View(collors);
        }

       
        // GET: CollorController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CollorController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CollorModel col)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    int recordcreated = Collor.Create(col);
                }
                RabbitFarmLocal.Start.ConstantsSingelton.UpdateCollors();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CollorController/Edit/5
        public ActionResult Edit(int id)
        {
            CollorModel col = Collor.LoadAll().Find(x => x.Id == id);
            return View(col);
        }

        // POST: CollorController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CollorModel col)
        {
            try
            {
                if (ModelState.IsValid) Collor.Edit(col);
                RabbitFarmLocal.Start.ConstantsSingelton.UpdateCollors();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CollorController/Delete/5
        public ActionResult Delete(int id)
        {
            CollorModel col = Collor.LoadAll().Find(x => x.Id == id);

            int[] rabbits = LoadRabbits().FindAll(x => x.CollorId == id).Select(x => x.RabbitId).ToArray();
            List <FatteningModel> fattening=LoadAllFattening().FindAll(x => x.CollorId == id).ToList();   
            if (rabbits.Length > 0 )
            {
                ViewBag.Rabbits = string.Join(", ", rabbits);
                
                ViewBag.NotallowedR = true;
            }
            else ViewBag.NotallowedR = false;
            if (fattening.Count > 0)
            {
                
                ViewBag.Fattenings = fattening;
                ViewBag.NotallowedF = true;
            }
            else ViewBag.NotallowedF = false;

            return View(col);
        }

        // POST: CollorController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                int recodcreated = Collor.Delete(id);
                RabbitFarmLocal.Start.ConstantsSingelton.UpdateCollors();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        public ActionResult JoinCollors (int id)
        {
            List<CollorModel> collors = Collor.LoadAll();
            CollorModel CollorToJointInto = collors.Find(x => x.Id == id);
            collors.Remove(CollorToJointInto);
            ViewBag.CollorToJointInto = CollorToJointInto;
            var collorsList = RabbitFarmLocal.Start.ConstantsSingelton.GetCollors().FindAll(x => x.Name != "Новый цвет" && x.Name!=CollorToJointInto.Name).ToList();
            ViewBag.CollorList = new SelectList(collorsList, "Id", "Name"); 
            return View(collors);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult JoinCollors(IFormCollection collection, List<CollorModel> collors )
        {
            if (int.TryParse(collection["idToJoinInto"], out int idToJoinInto))
            {
                List<DLRabbitModel> rabbits = LoadRabbits();
                List<FatteningModel> fattening = LoadAllFattening();
                foreach (var c in collors)
                {
                    List<DLRabbitModel> rabToChange = rabbits.FindAll(x => x.CollorId == c.Id);
                    foreach (var rabChange in rabToChange)
                    {
                        rabChange.CollorId = idToJoinInto;
                        EditRabbit(rabChange);
                    }
                    List<FatteningModel> fattToChange = fattening.FindAll(x => x.CollorId == c.Id);
                    foreach (var f in fattToChange)
                    {
                        f.CollorId = idToJoinInto;
                    }
                    EditFattenigPerPart(fattToChange);
                }
                if (collection["deleteAfterJoin"] == "on")
                {
                    foreach(var c in collors)
                    {
                        CollorModel col = Collor.LoadAll().Find(x => x.Id == c.Id);
                        bool canBeDeleted = true;
                        int[] rabbitsCollorDelete = LoadRabbits().FindAll(x => x.CollorId == c.Id).Select(x => x.RabbitId).ToArray();
                        List<FatteningModel> fatteningCollorDelete = LoadAllFattening().FindAll(x => x.CollorId == c.Id).ToList();
                        if (rabbitsCollorDelete.Length > 0) canBeDeleted = false;
                        if (fatteningCollorDelete.Count > 0) canBeDeleted = false;
                        if (canBeDeleted) Collor.Delete(c.Id);
                    }
                }
            }
            RabbitFarmLocal.Start.ConstantsSingelton.UpdateCollors();
            return RedirectToAction(nameof(Index));
        }

        public void populateCollors()
        {
            List<string> collors = LoadRabbits().Select(x => x.Collor).Distinct().ToList();
            collors.AddRange(LoadAllFattening().Select(x => x.Collor).Distinct().ToList());
            CollorModel first = new CollorModel()
            {
                Name =  "не уст"
            };
            Collor.Create(first);
            foreach (string c in collors)
            {
                if (c == null) continue;
                CollorModel CM = new CollorModel()
                {
                    Name = c
                };
                Collor.Create(CM);
            }
            RedirectToAction(nameof(Index));
        }
        public void assignCollorIds()
        {
            List<CollorModel> clollors = Collor.LoadAll();
            List<DLRabbitModel> rabbits = LoadRabbits();
            foreach(var r in rabbits)
            {
                if(r.Collor!=null) r.CollorId = clollors.Find(x => x.Name == r.Collor).Id;
               else  r.CollorId = 1;
                EditRabbit(r);
            }
            List<FatteningModel> fattening = LoadAllFattening();
            
            foreach(var f in fattening)
            {
                if(f.Collor!=null) f.CollorId=clollors.Find(x => x.Name == f.Collor).Id;
                else f.CollorId = 1;
                
            }
            RabbitFarmLocal.Start.ConstantsSingelton.UpdateCollors();
            EditFattenigPerPart(fattening);
        }
    }
}
