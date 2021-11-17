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
            foreach (var rabParents in descent.Parents)
            {
                if (rabParents.BeginnerOfLine)
                {
                    if (rab.BreedDict == null) rab.BreedDict = new Dictionary<int, int>();
                    int breedId = rabs.Find(x => x.RabbitId == rabParents.Id).BreedId;
                    int descPers = 100 / (int)Math.Pow(2, (double)rabParents.Step);
                    if (!rab.BreedDict.ContainsKey(breedId)) rab.BreedDict.Add(breedId, descPers);
                    else rab.BreedDict[breedId] += descPers;
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
                foreach (var rabParents in descentMother.Parents)
                            {
                                if (rabParents.BeginnerOfLine)
                                {
                                    int breedId = rabs.Find(x => x.RabbitId == rabParents.Id).BreedId;
                                    int descPers = 50 / (int)Math.Pow(2, (double)rabParents.Step);
                                    if (!BreedDict.ContainsKey(breedId)) BreedDict.Add(breedId, descPers);
                                    else BreedDict[breedId] += descPers;
                                }
                            }
            }
            else
            {
                
                BreedDict.Add(rabs.Find(x => x.RabbitId == mother).BreedId, 50);
            }
            if (descentFather.Parents.Count != 0)
            {
                foreach (var rabParents in descentFather.Parents)
                            {
                                if (rabParents.BeginnerOfLine)
                                {
                                    int breedId = rabs.Find(x => x.RabbitId == rabParents.Id).BreedId;
                                    int descPers = 50 / (int)Math.Pow(2, (double)rabParents.Step);
                                    if (!BreedDict.ContainsKey(breedId)) BreedDict.Add(breedId, descPers);
                                    else BreedDict[breedId] += descPers;
                                }
                            }
            }
            else
            {
                int breedId = rabs.Find(x => x.RabbitId == father).BreedId;
                if (!BreedDict.ContainsKey(breedId)) BreedDict.Add(breedId, 50);
                else BreedDict[breedId] += 50;
            }
            return BreedDict;
        }
    }
}
