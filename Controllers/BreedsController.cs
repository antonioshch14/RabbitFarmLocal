using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RabbitFarmLocal.Models;
using RabbitFarmLocal.Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static RabbitFarmLocal.BusinessLogic.RabbitProcessor;

namespace RabbitFarmLocal.Controllers
{
    public class BreedsController : Controller
    {
        // GET: BreedsController
        public ActionResult Index()
        {
            List<BreedsModel> Brs = Breed.LoadAll();
            return View(Brs);
        }

        // GET: BreedsController/Details/5
        //public ActionResult Details(int id)
        //{
        //    return View();
        //}

        // GET: BreedsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BreedsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                int MaxId = Breed.LoadAll().Max(m => m.Id);
                BreedsModel Br = new BreedsModel()
                {
                    Id = MaxId+1,
                    Name = collection["Name"].ToString()
                };
                Breed.Create(Br);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: BreedsController/Edit/5
        public ActionResult Edit(int id)
        {
            BreedsModel Br = Breed.LoadAll().Find(x => x.Id == id);
            return View(Br);
        }

        // POST: BreedsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                BreedsModel Br = new BreedsModel()
                {
                    Id = Convert.ToInt32(collection["Id"].ToString()),
                    Name = collection["Name"].ToString()
                };
                Breed.Edit(Br);
                ConstantsSingelton.UpdateBreeds();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: BreedsController/Delete/5
        public ActionResult Delete(int id)
        {
            BreedsModel Br = Breed.LoadAll().Find(x => x.Id == id);
            return View(Br);
        }

        // POST: BreedsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                //check if this Id belong to any rabbit
                //List<DLRabbitModel> Rbs = LoadRabbits();
                //List<int> existingRabsWithId=Rbs.FindAll(x=>x.Breed.)
                Breed.Delete(id);
                ConstantsSingelton.UpdateBreeds();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
     
        //public void PopulateBreeds()
        //{
        //    List<DLRabbitModel> rabs = LoadRabbits();
        //    List<String> breeds = rabs.Select(r => r.Breed).Distinct().ToList();
        //    //List<BreedsModel> BM = new List<BreedsModel>();

        //    for (int i = 0; i < breeds.Count; i++)
        //    {
        //        BreedsModel newBR = new BreedsModel()
        //        {
        //            Id = i,
        //            Name = string.IsNullOrEmpty(breeds[i]) ? "not named" : breeds[i]
        //        };
        //        Breed.Create(newBR);
        //    }
        //}
  
        //public void AssignBreedId()
        //{
        //    List<BreedsModel> Breeds = Breed.LoadAll();
        //    List<DLRabbitModel> rabs = LoadRabbits();
        //    foreach (var r in rabs)
        //    {
        //        r.BreedId = (Breeds.Exists(x => x.Name == r.Breed)) ? Breeds.Find(x => x.Name == r.Breed).Id : 6;
        //        EditRabbit(r);
        //    }
        //}
     
        public void FillBreeds()
        {
            List<BreedsModel> Breeds = Breed.LoadAll();
            List<DLRabbitModel> rabs = LoadRabbits();
            foreach(var rab in rabs)
            {
                BusinessLogic.BreedLogic.SetBreedOfRabbit(rab, rabs, Breeds);
                EditRabbit(rab);
            }        
        }
        
        public void FillBreedForFattening()
        {
            List<FatteningModel> fatList = LoadAllFattening();
            foreach(var F in fatList)
            {
                F.CreateBreedDictionary();
                F.SetBreedString();
                EditFattenigBreed(F);
            }
        }
       
    }
}
