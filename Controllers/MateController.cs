﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RabbitFarmLocal.Models;
using static RabbitFarmLocal.BusinessLogic.RabbitProcessor;
using static RabbitFarmLocal.Controllers.MyFunctions;
using static RabbitFarmLocal.BusinessLogic.DataUpdates;
using static RabbitFarmLocal.BusinessLogic.CheckRabbitRelations;
using RabbitFarmLocal.Start;
using System.Dynamic;
using RabbitFarmLocal.Controllers;
using System.ComponentModel.DataAnnotations;

namespace RabbitFarmWeb.Controllers
{
    public class MateController : Controller
    {
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        //[Authorize(Roles = "admin,owner")]
        public ActionResult MateView(int Id)
        {
            ViewBag.Message = "Покрытия";
            ViewBag.LinkableId = Id;
            var data = LoadMating(Id);
            List<MatingModel> com = new List<MatingModel>();
            foreach (var row in data)
            {
                com.Add(new MatingModel
                {
                    Date = row.Date,
                    FatherId = row.FatherId,
                    MotherId = row.MotherId,
                    Id=row.Id,
                    ParturationId=row.ParturationId,
                    PutNest=row.PutNest
                    

                });
            }
            return View(com);
        }
        //MaleMate
        public ActionResult MaleMate(int Id)
        {
            ViewBag.Message = "Случки";
            ViewBag.Name = "Случки самца  " + Id;
            // ViewBag.LinkableId = Id;
            var data = LoadMating(Id);
            List<MatingModel> com = new List<MatingModel>();
            foreach (var row in data)
            {
                com.Add(new MatingModel
                {
                    Date = row.Date,
                    
                    MotherId = row.MotherId,
                    Id = row.Id,
                    ParturationId = row.ParturationId


                });
            }
            return View(com);
        }
        //[Authorize(Roles = "admin,owner")]
        public ActionResult MateCreate(int Id,Caller caller)
        {
            ViewBag.Message = "Покрытие";
            ViewBag.Name = "Покрытие крольчихи " + Id;
            MatingModel com = new MatingModel();
            com.Date = DateTime.Now;
            com.MotherId = Id;
            com.Relations = CheckRelations(Id);
            string m="Родители: ";
            foreach (var p in com.Relations[0].Parents)
            {
                m += (p.ParGender==Gender.самка?"+":"^")+p.Id+ " ("+(p.Step)+") ";
            }
            ViewBag.Message = m;
            ViewBag.Date = DateToString(DateTime.Now);
            com.Relations.RemoveRange(0, 1);
            com.ECaller = caller;
            if (caller == Caller.report) return PartialView(com);
            else if (caller == Caller.mate) return View(com);
            return View(com);
        }
        [HttpPost]
        //[Authorize(Roles = "admin,owner")]
        public ActionResult MateCreate(MatingModel com)
        {
            
            if (ModelState.IsValid) //SaveComment(int Id, string comment, DateTime date)
            {
                int recordCreated = SaveMating(com.MotherId, com.FatherId, com.Date);
                UpdateRabbitsStatus();
                if (com.ECaller == Caller.report) return RedirectToAction("Report", "Home");
                else if (com.ECaller == Caller.allmate) return RedirectToAction("AllMateView", "Mate");
                else  return RedirectToAction("MateView", "Mate", new { id = com.MotherId });

            }
            
            return View();
        }
        

      

        //[Authorize(Roles = "admin,owner")]
        public ActionResult MateEdit(int id, Caller caller)
        {
            MatingModel mat = LoadMate(id);
            ViewBag.Message = "Покрытие";
            ViewBag.Name = "Редактирование покрытия крольчихи " + mat.MotherId + ", " + DateToStringRU(mat.Date);
            //MatingModel mat = new MatingModel();
            //mat.Id = id;
            //mat.FatherId = fatherId;
            //mat.Date = date;
            //mat.MotherId = motherId;
            //mat.PutNest = putNest;
            ViewBag.MateDate = DateToString(mat.Date);
            mat.ECaller = caller;
            ViewBag.PutNest = DateToString(mat.PutNest);
            mat.NestPutView = mat.NestPut;
            if (caller == Caller.allmate) return PartialView(mat);
           else return View("MateEdit", mat);
        }
        public ActionResult wrongMate(MatingModel mod)
        {

            ViewBag.Message = "Покрытие";
            ViewBag.Name = "ОШИБКА ДАННЫХ Редактирование покрытия крольчихи " + mod.MotherId + ", " + DateToStringRU(mod.Date);
           
            ViewBag.MateDate = DateToString(mod.Date);
            
            return View("MateEdit", mod);
        }

        [HttpPost]
        //[Authorize(Roles = "admin,owner")]
        [ValidateAntiForgeryToken]
        public ActionResult MateEdit(MatingModel model)
        {
            if (model.NestPutView == YesNo.Yes && model.PutNest == null) model.PutNest = DateTime.Now;
            else if (model.NestPutView == YesNo.No) model.PutNest = null;
            if (model.ParturationId == -2) model.ParturationId = null; //unabole to return null value from View
            
            if (ModelState.IsValid) 
            {
                int recordCreated = EditMating(model);//int id, int matherId, int fatherId, DateTime date
                UpdateRabbitsStatus();
                if (model.ECaller == Caller.allmate) return RedirectToAction("AllMateView", "Mate");
                else return RedirectToAction("MateView", "Mate", new { id = model.MotherId });
            }
           //MatingModel mod = LoadMate(model.Id);
           return wrongMate(model);
            
        }
        //[Authorize(Roles = "admin,owner")]
        public ActionResult FailMate(int id, int rabId, Caller caller)
        {

            int recordCreated = MarkMateAsFail(id);
            if (caller == Caller.allmate) return RedirectToAction("AllMateView");
            else if (caller == Caller.report) return RedirectToAction("Report", "Home");
            else return RedirectToAction("MateView", "Mate", new { id = rabId });
        }

        //******************************************Parturation
        //[Authorize(Roles = "admin,owner")]
        public ActionResult ParturView(int Id)
        {
            ViewBag.Message = "Окрол";
            ViewBag.LinkableId = Id;
            ViewBag.Name = "Окролы крольчихи " + Id;
            var data = LoadParturations(Id);
            List<ParturationModel> com = new List<ParturationModel>();
            foreach (var row in data)
            {
                com.Add(new ParturationModel
                {
                   Id=row.Id,
                    Date = row.Date,
                    MotherId = row.MotherId,
                    Children = row.Children,
                    Males = row.Males,
                    Females = row.Females,
                    DiedChild = row.DiedChild,
                    SeparationDate = row.SeparationDate,
                    Cage = row.Cage,
                    Comment = row.Comment,
                    DateNestRemoval = row.DateNestRemoval,
                    FatherId=row.FatherId,
                    MateDate=row.MateDate,
                    MateId=row.MateId

                });
            }
            return View(com);
        }
        //[Authorize(Roles = "admin,owner")]
        public ActionResult ParturCreate(int mateId, int motherId, int fatherId, DateTime mateDate,Caller caller )
        {
            ViewBag.Message = "Окрол";
            ViewBag.Name = "Новый окрол крольчихи " + motherId + ", покрыта " +DateToStringRU(mateDate)+ ", отец "+ fatherId;
            
            ParturationModel com = new ParturationModel();
            com.MateId = mateId;
            com.MotherId = motherId;
            ViewBag.BornDate = DateToString(mateDate.AddDays(30));
            UpdateRabbitsStatus();
            com.ECaller = caller;
            if (caller == Caller.allmate) return PartialView(com);
            //else if (caller == Caller.report) return PartialView(com);
            else return View(com);
        }
        [HttpPost]
        //[Authorize(Roles = "admin,owner")]
        public ActionResult ParturCreate(ParturationModel com)
        {

            if (ModelState.IsValid) //SaveComment(int Id, string comment, DateTime date)
            {
                int recordCreated = SaveParturation(com.Date, com.MotherId, com.Children, com.Males, com.Females, com.DiedChild, com.SeparationDate,  com.Comment, com.DateNestRemoval, com.MateId);
                if (com.ECaller == Caller.allmate) return RedirectToAction("AllMateView");
                else if (com.ECaller == Caller.report) return RedirectToAction("Report", "Home");
                return RedirectToAction("ParturView", "Mate", new { id = com.MotherId });
           }

           return wrongPartur(com);
        }
        public ActionResult wrongPartur(ParturationModel mod)
        {

            ViewBag.Message = "Окрол";
            ViewBag.Name = "ОШИБКА ДАННЫХ  окрола крольчихи " + mod.MotherId + ", " + DateToStringRU(mod.Date);

            ViewBag.BornDate = DateToString(mod.MateDate.AddDays(30));

            return View("ParturCreate", mod);
        }



        //[Authorize(Roles = "admin,owner")]
        public ActionResult ParturEdit(int id, Caller caller)
        {
            ParturationModel prt = LoadParturation(id);
            ViewBag.Message = "Окрол крольчихи " + prt.MotherId + ", рождены " + prt.DateString;
           
            //ViewBag.LinkableId = id;

            ViewBag.MateDate = DateToString(prt.Date);
            ViewBag.SeparationDate = DateToStringRU(prt.SeparationDate);
            ViewBag.DateNestRemoval =DateToStringRU(prt.DateNestRemoval);
            prt.NestRemovedView = prt.NestRemovedView;
            prt.SeparatedView = prt.Separated;
            prt.ECaller = caller;

            return View("ParturEdit", prt);
        }
       
        [HttpPost]
        //[Authorize(Roles = "admin,owner")]
        [ValidateAntiForgeryToken]
        public ActionResult ParturEdit(ParturationModel model)
        {
            //if (model.SeparatedView == YesNo.Yes && model.SeparationDate == null) model.SeparationDate = DateTime.Now;
            //else if (model.SeparatedView == YesNo.No) model.SeparationDate = null;
            //if (model.NestRemovedView == YesNo.Yes && model.DateNestRemoval == null) model.DateNestRemoval = DateTime.Now;
            //else if (model.NestRemovedView == YesNo.No) model.DateNestRemoval = null;
            if (model.SeparatedView == YesNo.Yes) model.SeparationDate = DateTime.Now;
            else if (model.SeparatedView == YesNo.No) model.SeparationDate = null;
            if (model.NestRemovedView == YesNo.Yes ) model.DateNestRemoval = DateTime.Now;
            else if (model.NestRemovedView == YesNo.No) model.DateNestRemoval = null;

            if (ModelState.IsValid)
            {
                int recordCreated = EditParturation(model.Id, model.Date, model.Children, model.Males, model.Females, model.DiedChild, model.SeparationDate, model.Cage, model.Comment, model.DateNestRemoval);
                UpdateRabbitsStatus();
                if (model.ECaller == Caller.allPartur) return RedirectToAction("AllParturView");
                return RedirectToAction("ParturView", "Mate", new { id = model.MotherId });
            }

            return View();
        }
        //DeleteParturation
        public ActionResult DeleteParturation(int id, int rabId, int mateId, Caller caller)

        {
            if (mateId != 0) DeleteMateFromSql(mateId);
            DeleteParturationFromSql(id);
            if (caller == Caller.allPartur) return RedirectToAction("AllParturView");
            return RedirectToAction("ParturView", "Mate", new { id = rabId });
        }
        public ActionResult DeleteMate(int id, int rabId,Caller caller)

        {
            DeleteMateFromSql(id);
            if (caller == Caller.allmate) return RedirectToAction("AllMateView", "Mate");
            else return RedirectToAction("MateView", "Mate", new { id = rabId });
        }

        public ActionResult AllParturView()
        {
            ViewBag.Message = "Все окролы";
            int period = Settings.AllParturViewPeriod();
            
            DateTime showFrom = DateTime.Today.AddDays(-period);

            ViewBag.Name = "Все окролы начиная с " + DateToStringRU(showFrom) + " за период "+ period + " дней";
            List < ParturationModel > part = LoadParturation(showFrom);
            
            
            return View(part);
        }
        public ActionResult AllMateView() // tbd what period to show
        {
            ViewBag.Message = "Все покрытия";
            int period = Settings.AllMateViewPeriod();

            DateTime showFrom = DateTime.Today.AddDays(-period);

            ViewBag.Name = "Все покрытия начиная с " + DateToStringRU( showFrom) + " за период " + period + " дней";
            List < MatingModel > allMate = LoadMating(showFrom);
            //List<MatingModel> com = new List<MatingModel>();
            //foreach (var row in data)
            //{
            //    com.Add(new MatingModel
            //    {
            //        Date = row.Date,
                    
            //        FatherId = row.FatherId,
            //        MotherId = row.MotherId,
            //        Id = row.Id,
            //        ParturationId = row.ParturationId,
            //        PutNest=row.PutNest


            //    });
            //}
            return View(allMate);
        }

        public ActionResult PreFattenigCreate(int partId, Caller caller)
        {
            ParturationModel fatt = LoadParturation(partId);
            fatt.ECaller = caller;
            return PartialView(fatt);
        }
        [HttpPost]
        public ActionResult FattenigCreate (ParturationModel parturation)
        {
            EditParturationChild(parturation);
            //ParturationModel parturation = new ParturationModel();
            //parturation = LoadParturation(partId);
            IList<FatteningModel> fatt = new List<FatteningModel>();
            for (int i = 1; i <= parturation.Males; i++)
            {
                fatt.Add(new FatteningModel
                {
                    PartId = parturation.Id,
                    RabbitGender=Gender.самец,
                    RabPartId=i
                });
            }
            for (int i = parturation.Males+1; i <= parturation.Females+ parturation.Males; i++)
            {
                fatt.Add(new FatteningModel
                {
                    PartId = parturation.Id,
                    RabbitGender = Gender.самка,
                    RabPartId = i
                });
            }

            Age partAge = new Age(parturation.Date);
            ViewBag.Message1 =String.Format( "Рассадить крольчат крольчихи {0}, рожденных {1}, возрастом месяцев {2}, дней {3}", parturation.MotherId, parturation.DateString, partAge.months, partAge.days);
            ViewBag.Message2 = String.Format("Крольчат в помете {0}, из них мальчиков {1}, девочек {2}", parturation.Children,parturation.Males,parturation.Females);
            if (parturation.Males + parturation.Females+parturation.DiedChild != parturation.Children) ViewBag.Message2 += ". !!!! Похоже, что вы не правльно посчитали крольчат";
            if (fatt.Count == 0)
            {
                fatt.Add(new FatteningModel
                {
                    PartId = parturation.Id,
                    RabbitGender = Gender.самец,
                    RabPartId = 1
                });
            }
            fatt[0].ECaller = parturation.ECaller;

            return View(fatt);
        }
        [HttpPost]
        //[Authorize(Roles = "admin,owner")]
        [ValidateAntiForgeryToken]
        public ActionResult FattenigCreatePost(List<FatteningModel> fatt)
        {
            if (ModelState.IsValid)
            {
                int recordCreated = CreateFatting(fatt);
                if (fatt[0].ECaller == Caller.report) return RedirectToAction("Report", "Home");
                else if (fatt[0].ECaller == Caller.allPartur) return RedirectToAction("AllParturView", "Mate");
            else return RedirectToAction("FatteningView", "Mate", new { partId = fatt[0].PartId });
            }

            return View(fatt);

        }
        public ActionResult FatteningView (int partId)
        {
            List<FatteningModel> fatt = LoadFattenigPerPart(partId);
            ParturationModel part = LoadParturation(partId);
            Age partAge = new Age(part.Date);
            ViewBag.Message1 = String.Format("Кролики на откорм крольчихи {0}, рожденны {1}, возрастом месяцев {2}, дней {3}", part.MotherId, part.DateString, partAge.months, partAge.days);
            return View(fatt);
        }
        public ActionResult FattKilled (int PartId, int rabId, int CalledFrom)
        {
            List<FatteningModel> fatt = LoadFattenigPerPart(PartId);
            FatteningModel rab = fatt.Find(f => f.RabPartId == rabId);
            rab.Caller = CalledFrom;
            rab.KillDate = DateTime.Now;
            ViewBag.Date = DateToString(rab.KillDate);
            //rab.Weight = ; 

            return PartialView(rab);
           
        }
        [HttpPost]
        public ActionResult FattKilled(FatteningModel fatt)
        {
            
            List<FatteningModel> fattList = new List<FatteningModel>
            {
                fatt
            };

            EditFattenigPerPart(fattList);

            if (fatt.ECaller == Caller.fattening) return RedirectToAction("FatteningView", "Mate", new { partId = fatt.PartId });
            else return RedirectToAction("AllFatteningView", "Mate");
        }
        public ActionResult FattEdit(int PartId, int rabId, Caller CalledFrom)
        {
            List<FatteningModel> fatt = LoadFattenigPerPart(PartId);
            FatteningModel rab = fatt.Find(f => f.RabPartId == rabId);
            rab.ECaller = CalledFrom;
            ViewBag.Date = (rab.KillDate!=null)?DateToString(rab.KillDate):null;
            //rab.Weight = ; 
            return PartialView(rab);
         }
        [HttpPost]
        public ActionResult FattEdit(FatteningModel fatt)
        {
            List<FatteningModel> fattList = new List<FatteningModel>
            {
                fatt
            };
            if (fatt.ECaller != Caller.fattPerStat) { 
                if (fattList[0].Status == FatStatus.canned && fattList[0].KillDate == null) fattList[0].KillDate = DateTime.Now;
                else if (fattList[0].Status == FatStatus.diedItself && fattList[0].KillDate == null) fattList[0].KillDate = DateTime.Now;
                else if (fattList[0].Status == FatStatus.sold4Bread && fattList[0].KillDate == null) fattList[0].KillDate = DateTime.Now;
                else if (fattList[0].Status == FatStatus.eatenByUs && fattList[0].KillDate == null) fattList[0].KillDate = DateTime.Now;
            } 
            EditFattenigPerPart(fattList);
            if (fatt.ECaller ==Caller.fattening) return RedirectToAction("FatteningView", "Mate", new { partId = fatt.PartId });
            else if (fatt.ECaller==Caller.fattPerStat) return RedirectToAction("FattStatusView", "Mate", new { Dates = 360, Status=fatt.Status  });
            else return RedirectToAction("AllFatteningView", "Mate");
        }
      
        public ActionResult FattDiedItself(int PartId, int rabId, Caller CalledFrom)
        {
            List<FatteningModel> fatt = LoadFattenigPerPart(PartId);
            FatteningModel rab = fatt.Find(f => f.RabPartId == rabId);
            rab.Status = FatStatus.diedItself;
            rab.KillDate = DateTime.Now;
            List<FatteningModel> fattList = new List<FatteningModel>
            {
                rab
            };
            EditFattenigPerPart(fattList);
            rab.ECaller = CalledFrom;
            if (CalledFrom == Caller.fattening) return RedirectToAction("FatteningView", "Mate", new { partId = PartId });
            else return RedirectToAction("AllFatteningView", "Mate");
        }
        //DeleteFattRab
        public ActionResult FattDelete(int PartId, int rabId, Caller CalledFrom)
        {
            DeleteFattRab(PartId, rabId);
            if (CalledFrom == Caller.fattening) return RedirectToAction("FatteningView", "Mate", new { partId = PartId });
            else return RedirectToAction("AllFatteningView", "Mate");
        }
        public ActionResult AllFatteningView()
        {
            List<FatteningModel> fatt = LoadFattenigAllAlive();

            
            //Age partAge = new Age(part.Date);
            ViewBag.Message1 = String.Format("Кроликов на откорм {0} ",fatt.Count());
            return View(fatt);
        }
        [HttpPost]
        public ActionResult AllFatteningView(List<FatteningModel> fatt)
        {
            foreach(var ft in fatt)
            {
                if (ft.NewWeight != 0)
                {
                    FattWeightModel wgt = new FattWeightModel()
                    {
                        RabId = ft.RabPartId,
                        PartId = ft.PartId,
                        Date = DateTime.Now.Date,
                        Weight = ft.NewWeight
                    };
                    if (ft.WeightDate == DateTime.Now.Date)//update if weight for today already exists
                    {
                        List<FattWeightModel> wgtToFindIfLatestExist = FattWeight.Load(wgt.RabId, wgt.PartId);
                        wgt.Id = wgtToFindIfLatestExist.Find(x => x.Date == wgt.Date).Id;
                        FattWeight.Edit(wgt);
                    }
                    else { int recordCreated = FattWeight.Create(wgt); }
                }
            }
            
            return RedirectToAction("AllFatteningView");
        }
        public ActionResult FattStatusView(int Dates, FatStatus Status)
        {
            string from = DateToString(DateTime.Now.AddDays(-Dates));
            string until = DateToString(DateTime.Now);
            DisplayAttribute St = GetDisplayAttributesFrom(Status, typeof(FatStatus));
            ViewBag.Message = "Показаны все кролики статус которых <" + St.Name + "> за период " + Dates;
            ViewBag.Status = Status;
            List<FatteningModel> fatt = LoadFattenigPerStatus(from,until,(int)Status);
            return View(fatt);
        }
        [HttpPost]
        public ActionResult FattStatusView(int Dates, int Status)
        {
            string from = DateToString(DateTime.Now.AddDays(-Dates));
            string until = DateToString(DateTime.Now);
            FatStatus statusEn = (FatStatus)Status;
            DisplayAttribute St = GetDisplayAttributesFrom(statusEn, typeof(FatStatus));
            ViewBag.Message = "Показаны все кролики статус которых <" + St.Name + "> за период " + Dates;
            ViewBag.Status = statusEn;
            List < FatteningModel > fatt= LoadFattenigPerStatus(from, until, (int)Status);
            return View(fatt);
        }
        //"PutNest", "Mate", new { mateId=item.Id, calledFrom=0 
        public ActionResult PutNest(int mateId,int rabId, Caller caller)
        {
            MatingModel mat = new MatingModel() {
                Id = mateId,
                ECaller = caller
                
            };
            ViewBag.Date = DateToString(DateTime.Now);
            ViewBag.Rabbit=rabId;
            return PartialView(mat);
        }
        [HttpPost]
        public ActionResult PutNest(MatingModel mat)
        {
            
            PutNestSaveToDB(mat);
            if (mat.ECaller == Caller.report) return RedirectToAction("Report", "Home");
            else return RedirectToAction("AllMateView", "Mate");
        }
        public ActionResult RemoveNest(int partId, int rabId, Caller caller)
        {
            ParturationModel prt = new ParturationModel()
            {
                Id = partId,
                ECaller = caller

            };
            ViewBag.Date = DateToString(DateTime.Now);
            ViewBag.Rabbit = rabId;
            return PartialView(prt);
        }
        [HttpPost]
        public ActionResult RemoveNest(ParturationModel prt)
        {

            RemoveNestSaveToDB(prt);
            if (prt.ECaller == Caller.report) return RedirectToAction("Report", "Home");
            else return RedirectToAction("AllMateView", "Mate");
        }
    }
}