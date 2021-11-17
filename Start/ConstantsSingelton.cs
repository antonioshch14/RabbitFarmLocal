using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static RabbitFarmLocal.BusinessLogic.RabbitProcessor;

namespace RabbitFarmLocal.Start
{
    public class ConstantsSingelton
    {
        private static readonly ConstantsSingelton _instance = new ConstantsSingelton();
        private int[] cageNumbers;
        private List<Models.ListOfCages> CageList;
        private List<Models.BreedsModel> BreedList;
        private ConstantsSingelton()
        {
            CageList = new List<Models.ListOfCages>();
            FillListOfCages(ref CageList);
            cageNumbers = new int[CageList.Count];
            cageNumbers = (from c in CageList select c.Id).ToArray();
            BreedList = Breed.LoadAll();
        }
        public static void UpdateCages()
        {
            
            FillListOfCages(ref _instance.CageList);
            _instance.cageNumbers = new int[_instance.CageList.Count];
            _instance.cageNumbers = (from c in _instance.CageList select c.Id).ToArray();
        }
        public static void UpdateBreeds()
        {
            _instance.BreedList = Breed.LoadAll();
        }
        public static ConstantsSingelton GetConstantSingelton()
        {
            return _instance;
        }
        public static int[] GetCageNumbers()
        {
            return _instance.cageNumbers;
        }
        public static List<Models.ListOfCages> GetCageLists()
        {
            return _instance.CageList;
        }
        public static List<Models.BreedsModel> GetListOfBreeds()
        {
            return _instance.BreedList;
        }
        private static void FillListOfCages(ref List<Models.ListOfCages> cages)
        {
            cages.Clear();
            List<Models.CageModel> cagesload = RabbitFarmLocal.BusinessLogic.RabbitProcessor.Cage.LoadAll();
            List<Models.FatteningModel> fatt = LoadFattenigAllAlive();
            List<Models.DLRabbitModel> rab = LoadRabbitsAlive();
            int maleFatOc;
            int femaleFatOc;
            int maleOc;
            int femOc;
            foreach (var c in cagesload)
            {
                cages.Add(new Models.ListOfCages()
                {
                    Id = c.Id
                });
                maleFatOc = fatt.FindAll(f => f.RabbitGender == Models.Gender.самец && f.Cage == c.Id).Count;
                if (maleFatOc > 0)
                {
                    cages[^1].Livers = maleFatOc;
                    cages[^1].Occupancy = Models.occupancy.occupiedFatMale;
                    continue;
                }
                femaleFatOc = fatt.FindAll(f => f.RabbitGender == Models.Gender.самка && f.Cage == c.Id).Count;
                if (femaleFatOc > 0)
                {
                    cages[^1].Livers = femaleFatOc;
                    cages[^1].Occupancy = Models.occupancy.occupiedFatFemale;
                    continue;
                }
                maleOc = rab.FindAll(r => r.IsMale == true && r.Cage == c.Id).Count;
                if (maleOc > 0)
                {
                    cages[^1].Livers = maleOc;
                    cages[^1].Occupancy = Models.occupancy.occupiedMale;
                    continue;
                }
                femOc = rab.FindAll(r => r.IsMale == false && r.Cage == c.Id).Count;
                if (femOc > 0)
                {
                    cages[^1].Livers = femOc;
                    cages[^1].Occupancy = Models.occupancy.occupiedFemail;
                }
            }
            cages.OrderBy(i => i.Id);
        }
    }
    
}
