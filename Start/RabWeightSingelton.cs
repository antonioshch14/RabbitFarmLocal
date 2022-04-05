using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static RabbitFarmLocal.BusinessLogic.RabbitProcessor;
using RabbitFarmLocal.BusinessLogic;
using RabbitFarmLocal.Models;

namespace RabbitFarmLocal.Start
{
    public class BreedsMixWeights 
    {
        public float[] DayRiseFactor;
        public float[] MeanWeight;
        public string BreedOfRab;
        public BreedsMixWeights(int length)
        {
            DayRiseFactor = new float[length];
            MeanWeight = new float[length];
        }
        
    }
    public class WeighGrow
    {
        private static readonly WeighGrow _instance = new WeighGrow();
        //private float[] DayRiseFactor;
        //private float[] MeanWeight;
        private int bound;
        private List<BreedsMixWeights> BreedWeightsPerBreed; 
        private WeighGrow()
        {
            //List<RabWeightCurve> RabCurves;
            //var curve = RabbitFarmLocal.BusinessLogic.WeightLogic.CreateGrowCurve(out RabCurves);
            bound = RabWeightCurve.daysToStoreCurve;
            BreedWeightsPerBreed = _GetWeightsMixBreed();

            //DayRiseFactor = new float[curve.Length];
            //MeanWeight = new float[curve.Length];
            //bound = curve.Length;
            //for (int i = 0; i < bound; i++)
            //{
            //    DayRiseFactor[i] = curve[i].DayRiseFactor;
            //    MeanWeight[i] = curve[i].WeightSteamlighned;
            //}




        }
        private static List<BreedsMixWeights> _GetWeightsMixBreed()
        {
            List<BreedsMixWeights> BreedWeightsPerBreed=new List<BreedsMixWeights>();
            List<FattWeightModel> weights = FattWeight.LoadAll().OrderBy(a => a.PartId).ThenBy(a => a.RabId).ThenBy(a => a.Date).ToList(); 
            List<FatteningModel> fatts = LoadAllFattening();
            List<BreedMixModel> breedList = BreedLogic.GetBreedMixForAllRabbits();
            List<FattWeightModel> weightsOfBreed = new List<FattWeightModel>();
            //get weight grow curve for all rabbits

            Weight[] AllbreedCurve = RabbitFarmLocal.BusinessLogic.WeightLogic.CreateGrowCurve(out List<RabWeightCurve> BreedRabCurves, weights);
            BreedWeightsPerBreed.Add(new BreedsMixWeights(AllbreedCurve.Length));
            
            for (int i = 0; i < AllbreedCurve.Length; i++)
            {
                BreedWeightsPerBreed[^1].DayRiseFactor[i] = AllbreedCurve[i].DayRiseFactor;
                BreedWeightsPerBreed[^1].MeanWeight[i] = AllbreedCurve[i].WeightSteamlighned;
            }
            //get weight grow curve for all breed combinations
            foreach (BreedMixModel B in breedList)//go throught all breed variations
            {
                weightsOfBreed.Clear();
                var rabOfBreed = fatts.FindAll(r => r.Breed == B.BreedOfRab.Breed).Select(Rab => new { Rab.PartId }).Distinct();
                foreach (var RB in rabOfBreed)//find and store weights of the breed
                {
                    weightsOfBreed.AddRange(weights.FindAll(w => w.PartId == RB.PartId).ToList());
                }

                if (weightsOfBreed.Count < 5) continue;//skip if there are less than 5 weights of that breed
                weightsOfBreed = weightsOfBreed.OrderBy(a => a.PartId).ThenBy(a => a.RabId).ThenBy(a => a.Date).ToList();
                Weight[] breedCurve = RabbitFarmLocal.BusinessLogic.WeightLogic.CreateGrowCurve(out List<RabWeightCurve> BreedRabCurves1, weightsOfBreed);
                BreedWeightsPerBreed.Add(new BreedsMixWeights(breedCurve.Length));
                BreedWeightsPerBreed[^1].BreedOfRab = B.BreedOfRab.Breed;
                for (int i = 0; i < breedCurve.Length; i++)
                {
                    BreedWeightsPerBreed[^1].DayRiseFactor[i] = breedCurve[i].DayRiseFactor;
                    BreedWeightsPerBreed[^1].MeanWeight[i] = breedCurve[i].WeightSteamlighned;
                }
            }
            return BreedWeightsPerBreed;
        }
        public static void UpdateWeitghCurve()
        {
            _instance.BreedWeightsPerBreed.Clear();
            _instance.BreedWeightsPerBreed = _GetWeightsMixBreed();
        }
        public static WeighGrow GetWeightGrow()
        {
            return _instance;
        }
        public static float GetMeanWeight(int day, string breed="")//returns a mean weight for this day
        {
            if (day >= _instance.bound) day = _instance.bound - 1;
            int index=_instance.BreedWeightsPerBreed.FindIndex(x => x.BreedOfRab == breed);
            if (index == -1) index=0;
            //index = 0;
            return _instance.BreedWeightsPerBreed[index].MeanWeight[day]; 
        }
        public static float[] GetWeightArray(string breed)
        {
            int index = _instance.BreedWeightsPerBreed.FindIndex(x => x.BreedOfRab == breed);
            if (index == -1) index = 0;
            //index = 0;
            return _instance.BreedWeightsPerBreed[index].MeanWeight;
        }
        public static float GetRiseFactor(int StartDay, int TargDay, string breed = "")//returns an argument which has to be multiplied on weight in start day to get predicted weight on TardDay 
        {
            if (StartDay < TargDay && StartDay < _instance.bound - 1)
            {
                int index = _instance.BreedWeightsPerBreed.FindIndex(x => x.BreedOfRab == breed);
                if (index == -1) index = 0;
                //index = 0;
                if (TargDay < _instance.bound - 1) return _instance.BreedWeightsPerBreed[index].MeanWeight[TargDay] / _instance.BreedWeightsPerBreed[index].MeanWeight[StartDay];
                else return _instance.BreedWeightsPerBreed[index].MeanWeight[_instance.bound - 1] / _instance.BreedWeightsPerBreed[index].MeanWeight[StartDay];
            }
            return 1F;
        }
    }
}
