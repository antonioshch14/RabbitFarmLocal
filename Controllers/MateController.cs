using System;
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

namespace RabbitFarmWeb.Controllers
{
    public class MateController : Controller
    {
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
                    ParturationId=row.ParturationId
                    

                });
            }
            return View(com);
        }
        //[Authorize(Roles = "admin,owner")]
        public ActionResult MateCreate(int Id)
        {
            ViewBag.Message = "Покрытие";
            ViewBag.Name = "Покрытие крольчихи " + Id;
            MatingModel com = new MatingModel();
            com.MotherId = Id;
            com.Date = DateTime.Now;
            com.FatherId = 0;
            return View(com);
        }
        [HttpPost]
        //[Authorize(Roles = "admin,owner")]
        public ActionResult MateCreate(MatingModel com)
        {
            
            if (ModelState.IsValid) //SaveComment(int Id, string comment, DateTime date)
            {
                int recordCreated = SaveMating(com.MotherId, com.FatherId, com.Date);
                return RedirectToAction("MateView", "Mate", new { id = com.MotherId });
            }

            return View();
        }
        

      

        //[Authorize(Roles = "admin,owner")]
        public ActionResult MateEdit(int id, int motherId, int fatherId, DateTime date)
        {
            
            ViewBag.Message = "Покрытие";
            MatingModel mat = new MatingModel();
            mat.Id = id;
            mat.FatherId = fatherId;
            mat.Date = date;
            mat.MotherId = motherId;
            ViewBag.MateDate = DateToString(mat.Date);

            return View("MateEdit", mat);
        }
        
       
        [HttpPost]
        //[Authorize(Roles = "admin,owner")]
        [ValidateAntiForgeryToken]
        public ActionResult MateEdit(MatingModel model)
        {
            ViewBag.Message = "Покрытие кролика " + model.MotherId;
            if (ModelState.IsValid) 
            {
                int recordCreated = EditMating(model.Id, model.MotherId, model.FatherId, model.Date);//int id, int matherId, int fatherId, DateTime date
                return RedirectToAction("MateView", "Mate", new { id = model.MotherId });
            }

            return View();
        }
        //[Authorize(Roles = "admin,owner")]
        public ActionResult FailMate(int id, int rabId)
        {

            int recordCreated = MarkMateAsFail(id);

            return RedirectToAction("MateView", "Mate", new { id = rabId });
        }

        //******************************************Parturation
        //[Authorize(Roles = "admin,owner")]
        public ActionResult ParturView(int Id)
        {
            ViewBag.Message = "Окрол";
            ViewBag.LinkableId = Id;
            ViewBag.Name = "Окролы крольчихи " + Id;
            var data = LoadParturation(Id);
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
                    

                });
            }
            return View(com);
        }
        //[Authorize(Roles = "admin,owner")]
        public ActionResult ParturCreate(int mateId, int motherId, int fatherId, DateTime mateDate )
        {
            ViewBag.Message = "Окрол";
            ViewBag.Name = "Новый окрол крольчихи " + motherId + ", покрыта " + mateDate.ToShortDateString()+ ", отец "+ fatherId;
            ParturationModel com = new ParturationModel();
            com.MateId = mateId;
            com.MotherId = motherId;
            com.Date = mateDate.AddDays(30);
            
            return View(com);
        }
        [HttpPost]
        //[Authorize(Roles = "admin,owner")]
        public ActionResult ParturCreate(ParturationModel com)
        {

            //if (ModelState.IsValid) //SaveComment(int Id, string comment, DateTime date)
            //{
                int recordCreated = SaveParturation(com.Date, com.MotherId, com.Children, com.Males, com.Females, com.DiedChild, com.SeparationDate, com.Cage, com.Comment, com.DateNestRemoval, com.MateId);
                return RedirectToAction("ParturView", "Mate", new { id = com.MotherId });
            //}

           // return View();
        }




        //[Authorize(Roles = "admin,owner")]
        public ActionResult ParturEdit(int id, DateTime date, int motherId, int children, int males,
            int females, int diedChild, DateTime? separationDate, int cage, string comment, DateTime? dateNestRemoval)
        {

            ViewBag.Message = "Окрол крольчихи " + motherId + ", рождены " + DateToString(date);
            ParturationModel mat = new ParturationModel();
            ViewBag.LinkableId = id;

            mat.Id = id;
            mat.Date = date;
            mat.MotherId = motherId;
            mat.Children = children;
            mat.Males = males;
            mat.Females = females;
            mat.DiedChild = diedChild;
            mat.SeparationDate = separationDate;
            mat.Cage = cage;
            mat.Comment = comment;
            mat.DateNestRemoval = dateNestRemoval;
            ViewBag.MateDate = DateToString(mat.Date);
            ViewBag.SeparationDate = mat.SeparationDate != null ?  mat.SeparationDate.Value.ToString("yyyy-MM-dd"):"";
            ViewBag.DateNestRemoval = mat.DateNestRemoval != null ? mat.DateNestRemoval.Value.ToString("yyyy-MM-dd") : ""; 

            return View("ParturEdit", mat);
        }
       
        [HttpPost]
        //[Authorize(Roles = "admin,owner")]
        [ValidateAntiForgeryToken]
        public ActionResult ParturEdit(ParturationModel model)
        {
            ViewBag.Message = "Окрол";
            if (ModelState.IsValid)
            {
                int recordCreated = EditParturation(model.Id, model.Date, model.MotherId, model.Children, model.Males, model.Females, model.DiedChild, model.SeparationDate, model.Cage, model.Comment, model.DateNestRemoval);
                return RedirectToAction("ParturView", "Mate", new { id = model.MotherId });
            }

            return View();
        }
    }
}