using RabbitFarmLocal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static RabbitFarmLocal.BusinessLogic.RabbitProcessor;

namespace RabbitFarmLocal.BusinessLogic
{
    public static class BreedLogic
    {
        public static DLRabbitModel SetBreedOfRabbit(DLRabbitModel rab, List<DLRabbitModel> rabs = null, List<BreedsModel> Breeds = null)
        {
            if (Breeds == null) Breeds = Breed.LoadAll();
            if (rabs == null) rabs = LoadRabbits();
            DescentModel descent = LoadDescent(rab.RabbitId);
            if (descent.Parents[0].Id == 0 && descent.Parents[0].MotherId == 0 && descent.Parents[0].FatherId == 0) descent.Parents.RemoveAt(0);
            Dictionary<int, float> BreedDictFloat = new Dictionary<int, float>();//to avoid unprescise % caused by int loss of fractions
            foreach (var rabParents in descent.Parents)
            {
                
                if (rabParents.BeginnerOfLine)
                {
                    if (rab.BreedDict == null) rab.BreedDict = new Dictionary<int, int>();
                    int breedId = rabs.Find(x => x.RabbitId == rabParents.Id).BreedId;
                    float descPers = 100 / (float)Math.Pow(2, (double)rabParents.Step);
                    if (!BreedDictFloat.ContainsKey(breedId)) BreedDictFloat.Add(breedId, descPers);
                    else BreedDictFloat[breedId] += descPers;
                }
            }
            if (BreedDictFloat.Count() > 0)
            {
                foreach (KeyValuePair<int, float> kvp in BreedDictFloat)
                {
                    rab.BreedDict.Add(kvp.Key, (int)kvp.Value);
                }
            }
            
            if (rab.BreedDict == null)
            {
                rab.BreedDict = new Dictionary<int, int>();
                rab.BreedDict.Add(rab.BreedId, 100);
            }
            rab.SetBreedString();
            return rab;
        }
        public static Dictionary<int,int> GetBreedDictionaryForRabit (int mother, int father, List<DLRabbitModel> rabs = null, List<BreedsModel> Breeds = null)
        {
            if (Breeds == null) Breeds = Breed.LoadAll();
            if (rabs == null) rabs = LoadRabbits();
            Dictionary<int, int> BreedDict = new Dictionary<int, int>();
            DescentModel descentFather = LoadDescent(father);
            DescentModel descentMother = LoadDescent(mother);
            if (descentFather.Parents[0].Id == 0 && descentFather.Parents[0].MotherId == 0 && descentFather.Parents[0].FatherId == 0) descentFather.Parents.RemoveAt(0);
            if (descentMother.Parents[0].Id == 0 && descentMother.Parents[0].MotherId == 0 && descentMother.Parents[0].FatherId == 0) descentMother.Parents.RemoveAt(0);
            if (descentMother.Parents.Count != 0)
            {
                Dictionary<int, float> BreedDictFloat = new Dictionary<int, float>();//to avoid unprecise % caused by int loss of fractions
                foreach (var rabParents in descentMother.Parents)
                {
                    if (rabParents.BeginnerOfLine)
                    {
                        int breedId = rabs.Find(x => x.RabbitId == rabParents.Id).BreedId;
                        float descPers = 100 / (float)Math.Pow(2, (double)rabParents.Step);
                        if (!BreedDictFloat.ContainsKey(breedId)) BreedDictFloat.Add(breedId, descPers);
                        else BreedDictFloat[breedId] += descPers;
                    }
                }
                if (BreedDictFloat.Count() > 0)
                {
                    foreach (KeyValuePair<int, float> kvp in BreedDictFloat)
                    {
                        BreedDict.Add(kvp.Key, (int)kvp.Value);
                    }
                }
            }
            else
            {
                
                BreedDict.Add(rabs.Find(x => x.RabbitId == mother).BreedId, 50);
            }
            //if (descentFather.Parents.Count != 0)
            //{
            //    foreach (var rabParents in descentFather.Parents)
            //                {
            //                    if (rabParents.BeginnerOfLine)
            //                    {
            //                        int breedId = rabs.Find(x => x.RabbitId == rabParents.Id).BreedId;
            //                        int descPers = 50 / (int)Math.Pow(2, (double)rabParents.Step);
            //                        if (!BreedDict.ContainsKey(breedId)) BreedDict.Add(breedId, descPers);
            //                        else BreedDict[breedId] += descPers;
            //                    }
            //                }
            //}
            //else
            //{
            //    int breedId = rabs.Find(x => x.RabbitId == father).BreedId;
            //    if (!BreedDict.ContainsKey(breedId)) BreedDict.Add(breedId, 50);
            //    else BreedDict[breedId] += 50;
            //}
            return BreedDict;
        }
        public static List<BreedMixModel> GetBreedMixForAllRabbits()
        {
            List<BreedMixModel> breeds = new List<BreedMixModel>();
            List<DLRabbitModel> rabs = LoadRabbits();
            List<String> rabBreeds = rabs.Select(r => r.Breed).Distinct().ToList();
            List<FatteningModel> fatts = LoadAllFattening();
            List<String> fatBreeds = fatts.Select(f => f.Breed).Distinct().ToList();
            List<String> comdBreeds = rabBreeds;
            comdBreeds.AddRange(fatBreeds);
            comdBreeds = comdBreeds.Distinct().ToList();
            foreach (var b in comdBreeds)
            {
                Descent rabDesc = new Descent() { Breed = b };
                rabDesc.BreedString = rabDesc.SetBreedStringToDisplay(b);
                rabDesc.BreedDict = rabDesc.GetBreedDictionary(b);
                breeds.Add(new BreedMixModel()
                {
                    BreedOfRab = rabDesc
                });
                breeds[^1].NumberRabbitsOfThisBreed = fatts.Count(f => f.Breed == b) + rabs.Count(r => r.Breed == b);

            }
            return breeds;
        }
        public static int[] GetRabbitsOfBreed(BreedsModel breedToFind)
        {
            //List<BreedMixModel> breeds = new List<BreedMixModel>();
            List<DLRabbitModel> rabs = LoadRabbits();
            foreach(var R in rabs)
            {
                R.GetBreedDictionary();
            }

            int[] rabOfBreed = rabs.FindAll(r => r.BreedDict.ContainsKey(breedToFind.Id)).Select(r =>  r.RabbitId ).ToArray();
            
           
            return rabOfBreed;
        }
    }
}
