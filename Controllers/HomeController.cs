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
using static RabbitFarmLocal.BusinessLogic.DataUpdates;
using static RabbitFarmLocal.BusinessLogic.CreateReport;

using RabbitFarmLocal.Start;
using RabbitFarmLocal.messaging;
using RabbitFarmLocal.BusinessLogic;

namespace RabbitFarmLocal.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        //[Authorize(Roles = "admin,owner")]
        public ActionResult ViewRabbits()
        {
            ViewBag.Message = "Список кроликов";
            List<DLRabbitModel> rabbits = Rabbit.LoadList();
            return View(rabbits);
        }
        //[Authorize(Roles = "admin,owner")]
        public ActionResult FUllRabbitView(int id)
        {
            DLRabbitModel item = Rabbit.LoadOne(id);
            ViewBag.Message = "Кролик " + item.RabbitId + "  " +item.StoredRabStatus;
            FullRabbitModel rabbit = new FullRabbitModel
            {
                RabbitId = item.RabbitId,
                Gender = item.Gender,
                Cage = item.Cage,
                Collor = item.Collor,
                Breed = item.Breed
            };
            if (item.IsAlive) rabbit.IsAlive = "живой";
            else rabbit.IsAlive = "история";
            Age age = new Age(item.Born);
            rabbit.Age = "Рожден " + item.Born.ToShortDateString() + ", сейчас " + age.years + " лет " + age.months + " месяцев " + age.days + " дней";
            rabbit.Descent = "мать, " + item.Mother + " отец " + item.Father;
            var comments = LoadComments(item.RabbitId);
            rabbit.Comments = comments;
            rabbit.Matting = LoadMating(item.RabbitId);
            rabbit.Parturation = LoadParturations(item.RabbitId);
            var desc = LoadDescent(item.RabbitId);
            rabbit.DescentData = desc;
            rabbit.Weights = RabWeight.Load(item.RabbitId);
            return View(rabbit);
        }


        //[Authorize(Roles = "admin")]
        public ActionResult Delete(RabbitFarmLocal.Models.DLRabbitModel rabbit)
        {
            deteRabbitFromBD(rabbit.Id);
            return RedirectToAction("ViewRabbits");
        }


        //[Authorize(Roles = "admin,owner")]
        public ActionResult AddRabbit()
        {
            return PartialView(new InpRAbbitModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddRabbit(InpRAbbitModel model)
        {
            ViewBag.Message = "Add rabbit" + model.RabbitId;

            if (ModelState.IsValid)
            {
                int recordCreated = CreateRabbit(model.RabbitId, model.Cage, model.Breed, model.Collor, model.Born,
                    model.Mother, model.Father, model.IsAlive, model.RabbitGender);
                UpdateRabbitsStatus();
                //int rabbitId, int cage, bool isMale, string breed, string collor, DateTime born, int mother, int father, bool isAlive
                return RedirectToAction("ViewRabbits");
            }

            return View();
        }

        /*public ActionResult Edit(RabbitFarmWeb.Models.DLRabbitModel rabbitToEdit)
        {
            //deteRabbitFromBD(rabbitId.Id);
            return RedirectToAction("EdditRabbit");
        }*/
        //[Authorize(Roles = "admin,owner")]
        public ActionResult RabEdit(int id) // RabbitId  Cage  RabbitGender  Breed  BreedType Collor  Born  Mother Father IsAlive status termDate price killWeight
        {
            DLRabbitModel rab = Rabbit.LoadOne(id);
            ViewBag.Message = "Rabbit Edit";
            ViewBag.Name = "Редактировать кролика " + rab.RabbitId;
            if (rab.IsMale) rab.RabbitGender = RabGender.male;
            else rab.RabbitGender = RabGender.female;

            ViewBag.Born = DateToString(rab.Born);

            return View(rab);

            // return View(new InpRAbbitModel()); ;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RabEdit(DLRabbitModel rab)
        {
            
            if (ModelState.IsValid)
            {
                if (rab.RabbitGender == RabGender.male) rab.IsMale = true;
                else rab.IsMale=false;
                int recordCreated = Rabbit.EditGeneral(rab);
                if (!rab.IsAlive) Rabbit.EditKill(rab);
                UpdateRabbitsStatus();
                return RedirectToAction("ViewRabbits");
            }
            
            return View();
        }


        // [HttpPost]
        //[Authorize(Roles = "admin,owner")]
        public ActionResult CommentsView(int Id)
        {
            ViewBag.Message = "Коментарии кролика " + Id;
            ViewBag.LinkableId = Id;
            var data = LoadComments(Id);
            List<CommentsModel> com = new List<CommentsModel>();
            foreach (var row in data)
            {
                com.Add(new CommentsModel
                {
                    Date = row.Date,
                    Comment = row.Comment,
                    Id = row.Id,
                    RabbitId = Id

                });
            }
            return View(com);
        }
        //[Authorize(Roles = "admin,owner")]
        public ActionResult CommentCreate(int Id)
        {
            ViewBag.Message = "Коментарий";
            CommentsModel com = new CommentsModel();
            com.RabbitId = Id;
            com.Date = DateTime.Now;
            return View(com);
        }
        [HttpPost]
        //[Authorize(Roles = "admin,owner")]
        public ActionResult CommentCreate(CommentsModel com)
        {
            ViewBag.Message = "Коментарий";
            if (ModelState.IsValid) //SaveComment(int Id, string comment, DateTime date)
            {
                int recordCreated = SaveComment(com.RabbitId, com.Comment, com.Date);
                return RedirectToAction("CommentsView", "Home", new { id = com.RabbitId });
            }

            return View();
        }
        //[Authorize(Roles = "admin,owner")]
        public ActionResult CommentEdit()
        {

            return View();
        }

        [HttpPost]
        //[Authorize(Roles = "admin,owner")]
        public ActionResult CommentetDelete(int rabId, int comId)
        {
            if (ModelState.IsValid)
            {
                int recordCreated = DeleteComment(comId);


            }

            return RedirectToAction("CommentsView", "Home",new { id = rabId });//"Index", "Home", new { id = 2}
        }

        //[Authorize(Roles = "admin,owner")]
        public ActionResult CommentetEdit(int comId, int radId, string com)
        {
            ViewBag.LinkableId = radId;
            ViewBag.Message = "Коментарий";
            CommentsModel comment = new CommentsModel();
            comment.Id = comId;
            comment.RabbitId = radId;
            comment.Date = DateTime.Now;
            comment.Comment = com;

            return View("CommentetEdit", comment);
        }
        [HttpPost]
        //[Authorize(Roles = "admin,owner")]
        [ValidateAntiForgeryToken]
        public ActionResult CommentetEdit(CommentsModel model)
        {
            ViewBag.Message = "Коментарий кролика " + model.RabbitId;
            if (ModelState.IsValid) //SaveComment(int Id, string comment, DateTime date)
            {
                int recordCreated = EditComment(model.Id, model.RabbitId, model.Comment, model.Date);
                return RedirectToAction("CommentsView", "Home", new { id = model.RabbitId });
            }

            return View();
        }

        //[Authorize(Roles = "admin,owner")]

        public ActionResult DeadRabbit(int id)
        {
            DLRabbitModel rab = Rabbit.LoadOne(id);
            //int rabbitDead = PutRabToArchive(id);
            return View(rab);
        }
        [HttpPost]
        public ActionResult DeadRabbit(DLRabbitModel rab)
        {
            if (ModelState.IsValid) //SaveComment(int Id, string comment, DateTime date)
            {
                rab.IsAlive = false;
                int recordCreated = Rabbit.EditKill(rab);
            }
            return RedirectToAction("ViewRabbits", "Home");
        }
       // public List <DLRabbitModel> UpdateRabbitsStatus()
        public ActionResult SettingsEdit()
        {
            SettingsModel set=loadSettings();
            return View("SettingsEdit",set);

        }
        [HttpPost]
        //[Authorize(Roles = "admin,owner")]
        [ValidateAntiForgeryToken]
        public ActionResult SettingsEdit(SettingsModel model)
        {
            
            if (ModelState.IsValid) 
            {
                if (model.FinRepDate > 28 || model.FinRepDate < 1) model.FinRepDate = 1;
                int recordCreated = editSettings(model);
                Settings.ReloadSettings();
                UpdateRabbitsStatus();
                return RedirectToAction("ViewRabbits", "Home");
            }

            return View();
        }
        public ActionResult Report()
        {
            UpdateRabbitsStatus();
            ReportModel rep = FillReport();
            return View(rep);
        }
        public ActionResult UpdateRabbitsStatusView()
        {
            UpdateRabbitsStatus();
            return RedirectToAction("ViewRabbits");

        }
        //CallBot
        public ActionResult CallBot()
        {
            string? report = CreateReport.GetAlertString();
            if (report != null) MyTelegram.SendMessageToBot(report);
            
            return RedirectToAction("Index");

        }
        public ActionResult CallBotFin()
        {
            string report = FinReport.MonthReportString();
            MyTelegram.SendFinMessageToBot(report);

            return RedirectToAction("Index");

        }
        public ActionResult NotesView(string date)
        {
           // DateTime? dateDateTime=null;
           //if (date!=null) dateDateTime = StringToDate(date);
            NoteModel note = Note.Load(date);
            ViewBag.Date = DateToString(note.Date);
            return View(note);
        }
        public ActionResult NoteCreate()
        {
            NoteModel nt = new NoteModel();
            ViewBag.Date = DateToString(DateTime.Now);
            return View(nt);
        }
        [HttpPost]
        //[Authorize(Roles = "admin,owner")]
        [ValidateAntiForgeryToken]
        public ActionResult NoteCreate(NoteModel model)
        {
            if (ModelState.IsValid)
            {
               // model.Date = DateTime.Now;
                int entry = Note.Create(model);
                return RedirectToAction("NotesView", "Home");
            }
            return View();
        }
       
        [HttpPost]
        //[Authorize(Roles = "admin,owner")]
        [ValidateAntiForgeryToken]
        public ActionResult NoteEdit(NoteModel model, string Save, string Edit)
        {
            if (ModelState.IsValid)
            {
                if (Save != null)
                {
                    model.Date = DateTime.Now;
                    int entry = Note.Create(model);
                    
                }
                else{
                    int entry = Note.Edit(model);
                   
                }
                return RedirectToAction("NotesView", "Home");
            }
            return View();
        }
        public ActionResult NoteDelete(string date)
        {
        //    DateTime dateDateTime = StringToDate(date);
            Note.Delete(date);
            return RedirectToAction("NotesView", "Home");
        }
    }
}
