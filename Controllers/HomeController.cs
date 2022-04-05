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
using static RabbitFarmLocal.BusinessLogic.CageLogic;

using RabbitFarmLocal.Start;
using RabbitFarmLocal.messaging;
using RabbitFarmLocal.BusinessLogic;
using System.Web.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RabbitFarmLocal.Controllers
{
    [System.Runtime.InteropServices.Guid("015CD6A6-FBDC-46F7-BF44-96CFA8CE1779")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            //ViewBag.Overdues = RabbitFarmLocal.Start.ConstantsSingelton.GetNumberOfOverDues();
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
            foreach(var rab in rabbits)
            {
               // rab.GetBreedDictionary();
                rab.SetBreedStringToDisplay();
                rab.SetCollorString();
            }
            return View(rabbits);
        }
        //[Authorize(Roles = "admin,owner")]
        public ActionResult FUllRabbitView(int id)
        {
            DLRabbitModel item = Rabbit.LoadOne(id);
            item.SetBreedStringToDisplay();
            item.SetCollorString();
            ViewBag.Message = "Кролик " + item.RabbitId + "  " +item.StoredRabStatus;
            FullRabbitModel rabbit = new FullRabbitModel
            {
                RabbitId = item.RabbitId,
                Gender = item.Gender,
                Cage = item.Cage,
                Collor = item.Collor,
                Breed = item.BreedString,

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
            

            //JsonSerializerOptions options = new()
            //{
            //    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            //    WriteIndented = true// change to false in production
            //};
             
            //ViewBag.DecsentTest = JsonSerializer.Serialize(DescentLogic.GerRabDecent(item.RabbitId), options);
            return View(rabbit);
        }


        //[Authorize(Roles = "admin")]
        public ActionResult Delete(RabbitFarmLocal.Models.DLRabbitModel rabbit)
        {
            deteRabbitFromBD(rabbit.Id);
            ConstantsSingelton.UpdateCages();
            return RedirectToAction("ViewRabbits");
        }


        //[Authorize(Roles = "admin,owner")]
        public ActionResult AddRabbit(RabbitFarmLocal.Models.FatteningModel? fatt=null, string? ErrorMesssage=null)
        {
            if (ErrorMesssage != null) ViewBag.Error = ErrorMesssage;
            InpRAbbitModel rab = new InpRAbbitModel();
            if (fatt.RabPartId!=0 && fatt.PartId!=0)
            {
                rab.Mother = fatt.MotherId;
                rab.Father = fatt.FatherId;
                rab.Born = fatt.Born;
                rab.CollorId = fatt.CollorId;
                rab.RabbitGender = fatt.RabbitGender;
                rab.PartId = fatt.PartId;
                rab.PartRabId = fatt.RabPartId;
                ViewBag.Date = DateToString(rab.Born);
                ViewBag.BreedChoseShow = false;
                rab.BreedString = fatt.BreedString;
                rab.Breed = fatt.Breed;
            }
            else
            {
                ViewBag.BreedChoseShow = true;
                ViewBag.BreedList = new SelectList(ConstantsSingelton.GetListOfBreeds(), "Id", "Name", rab.BreedId);
                rab.CollorId = 1;  //set collor as  'not set'
             }
            List<DLRabbitModel> rabbits = Rabbit.LoadList();
            List<int> ids = (from r in rabbits select r.RabbitId).ToList();
            ids.Sort();
            int prev=0, length=0;
            foreach(var i in ids) //look through the list and find the last of more than 10th in a row
            {
                if (i == prev + 1)
                {                    
                    length++;                   
                }
                else
                {
                    length = 0;
                }
                if(length>=10) rab.RabbitId = i+1;
                prev = i;
            }
            //rab.CageListTest = RabbitFarmLocal.Start.ConstantsSingelton.GetCageNumbers();
            rab.CageList = RabbitFarmLocal.Start.ConstantsSingelton.GetCageLists();
            ViewBag.caJson = CageJSON(0, false);
            ViewBag.CollorList = new SelectList(RabbitFarmLocal.Start.ConstantsSingelton.GetCollors(),"Id","Name",rab.CollorId);
            return PartialView(rab);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddRabbit(InpRAbbitModel model)
        {
            List<DLRabbitModel> rabbits = Rabbit.LoadList();
            List<int> ids = (from r in rabbits select r.RabbitId).ToList();
            if (ids.IndexOf(model.RabbitId) != -1)
            {
                String Error = "Такой номер кролика "+ model.RabbitId +" уже существует!!!";
                return RedirectToAction("AddRabbit","Home", new { ErrorMesssage = Error });
            }
            ViewBag.Message = "Add rabbit" + model.RabbitId;
            if (ModelState.IsValid)
            {
                if (model.Breed == null)
                {
                    model.Breed = model.BreedId+", 100";
                }
                int recordCreated = CreateRabbit(model.RabbitId, model.Cage, model.Breed, model.CollorId, model.Born,
                    model.Mother, model.Father, model.IsAlive, model.RabbitGender, model.PartId, model.PartRabId, model.BreedId) ;
                if (recordCreated > 0 && model.PartRabId != null) 
                    EditFattenigStatus(new FatteningModel() { PartId = (int)model.PartId, RabPartId = (int)model.PartRabId, Status = FatStatus.used4Bread });
                UpdateRabbitsStatus();
                //int rabbitId, int cage, bool isMale, string breed, string collor, DateTime born, int mother, int father, bool isAlive
                ConstantsSingelton.UpdateCages();
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
            DescentModel desc = LoadDescent(rab.RabbitId);
            ViewBag.FirstInDescent = (desc.Parents[0].BeginnerOfLine == true && desc.Parents.Count() == 1) ?true:false;
            ViewBag.BreedList = new SelectList(ConstantsSingelton.GetListOfBreeds(), "Id", "Name",rab.BreedId);
            ViewBag.CollorList = new SelectList(RabbitFarmLocal.Start.ConstantsSingelton.GetCollors(), "Id", "Name", rab.CollorId);
            ViewBag.Born = DateToString(rab.Born);
            rab.SetCollorString();
            rab.CageList = RabbitFarmLocal.Start.ConstantsSingelton.GetCageLists();
            ViewBag.caJson = CageJSON(0, false);
            
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
                ParturationUpdate.UpdateOne(rab);
                UpdateRabbitsStatus();
                ConstantsSingelton.UpdateCages();
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
           // ConstantsSingelton.UpdateCages();
            return View(rab);
        }
        [HttpPost]
        public ActionResult DeadRabbit(DLRabbitModel rab)
        {
            if (ModelState.IsValid) //SaveComment(int Id, string comment, DateTime date)
            {
                rab.IsAlive = false;
                int recordCreated = Rabbit.EditKill(rab);
                ParturationUpdate.UpdateOne(rab);
                ConstantsSingelton.UpdateCages();
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
            RabbitFarmLocal.Start.ConstantsSingelton.SetNumberOfOverDues();
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
