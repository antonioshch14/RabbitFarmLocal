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
            var data = LoadRabbits();
            List<DLRabbitModel> rabbit = new List<DLRabbitModel>();
            string genderName;
            foreach (var row in data)
            {
                if (row.IsMale) genderName = "самец";
                else genderName = "самка";
                rabbit.Add(new DLRabbitModel
                {
                    RabbitId = row.RabbitId,
                    Mother = row.Mother,
                    Father = row.Father,
                    Gender = genderName,
                    IsAlive = row.IsAlive,
                    Cage = row.Cage,
                    //BornString = row.Born.ToShortDateString(),
                    //BornString = $"{row.Born.Year}-{month}-{day}",
                    Id = row.Id,
                    Breed = row.Breed,
                    Collor = row.Collor,
                    Born = row.Born

                });
            }

            return View(rabbit);
        }
        //[Authorize(Roles = "admin,owner")]
        public ActionResult FUllRabbitView(DLRabbitModel item)
        {
            ViewBag.Message = "Кролик " + item.RabbitId;
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
            TimeSpan ts = (DateTime.Today - item.Born);
            double daysTot = ts.TotalDays;
            double years = Math.Floor(daysTot / 365);
            double months = Math.Floor(daysTot % 365 / 30);
            double days = daysTot % 365 % 30;
            rabbit.Age = "Рожден " + item.Born.ToShortDateString() + ", сейчас " + years + " лет " + months + " месяцев " + days + " дней";
            rabbit.Descent = "мать, " + item.Mother + " отец " + item.Father;
            var comments = LoadComments(item.RabbitId);
            rabbit.Comments = comments;
            rabbit.Matting = LoadMating(item.RabbitId);

            rabbit.Parturation = LoadParturation(item.RabbitId);
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
            ViewBag.Message = "Добавить кролика";

            return View(new InpRAbbitModel());
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
        public ActionResult Edit(RabbitFarmLocal.Models.DLRabbitModel sourcceRabbit)
        {
            ViewBag.Message = "Rabbit Edit";
            ViewBag.Name = "Редактировать кролика " + sourcceRabbit.RabbitId;


            InpRAbbitModel outpRabbit = new InpRAbbitModel
            {
                // RabbitId  Cage  RabbitGender  Breed  BreedType Collor  Born  Mother Father IsAlive
                RabbitId = sourcceRabbit.Id,
                Cage = sourcceRabbit.Cage,
                //RabbitGender=sourcceRabbit.Gender,
                Breed = sourcceRabbit.Breed,
                Collor = sourcceRabbit.Collor,
                //Born=StringToDate(sourcceRabbit.BornString),
                Mother = sourcceRabbit.Mother,
                Father = sourcceRabbit.Father,
                IsAlive = sourcceRabbit.IsAlive
            };
            if (sourcceRabbit.Gender == "самка") outpRabbit.RabbitGender = Gender.самка;
            else outpRabbit.RabbitGender = Gender.самец;
            //ViewBag.Born = outpRabbit.Born;
            //ViewBag.Born = outpRabbit.Born.ToString("yyyy-MM-dd");
            ViewBag.Born = sourcceRabbit.Born.ToShortDateString();

            return View(outpRabbit);

            // return View(new InpRAbbitModel()); ;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(InpRAbbitModel model)
        {
            


            if (ModelState.IsValid)
            {
                int recordCreated = EditRabbit(model.RabbitId, model.Cage, model.Breed, model.Collor, model.Born,
                    model.Mother, model.Father, model.IsAlive, model.RabbitGender);
                //int rabbitId, int cage, bool isMale, string breed, string collor, DateTime born, int mother, int father, bool isAlive
                return RedirectToAction("ViewRabbits");
            }
            //int rabbitId, int cage, bool isMale, string breed, string collor, DateTime born, int mother, int father, bool isAlive



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

            int rabbitDead = PutRabToArchive(id);
            return RedirectToAction("ViewRabbits");


        }
        public ActionResult UpdateStatus()
        {

            return View();
        }

    }
}
