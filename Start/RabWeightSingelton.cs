using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static RabbitFarmLocal.BusinessLogic.RabbitProcessor;
using RabbitFarmLocal.BusinessLogic;

namespace RabbitFarmLocal.Start
{
   
        public class WeighGrow
        {
            private static readonly WeighGrow _instance = new WeighGrow();
            private float[] DayRiseFactor;
            private float[] MeanWeight;
            private int bound;
            
            private WeighGrow()
            {
                List<RabWeightCurve> RabCurves;
                var curve = RabbitFarmLocal.BusinessLogic.WeightLogic.CreateGrowCurve(out RabCurves);
                DayRiseFactor = new float[curve.Length];
                MeanWeight = new float[curve.Length];
                bound = curve.Length;
                for (int i=0;i< bound; i++)
                {
                    DayRiseFactor[i] = curve[i].DayRiseFactor;
                    MeanWeight[i] = curve[i].WeightSteamlighned;
                }
            }
            public static void UpdateWeitghCurve()
            {
                List<RabWeightCurve> RabCurves;
                var curve = RabbitFarmLocal.BusinessLogic.WeightLogic.CreateGrowCurve(out RabCurves);
                
                for (int i = 0; i < curve.Length; i++)
                {
                   _instance.DayRiseFactor[i] = curve[i].DayRiseFactor;
                   _instance.MeanWeight[i] = curve[i].WeightSteamlighned;
                }
            }
            public static WeighGrow GetWeightGrow()
            {
                return _instance;
            }
            public static float GetMeanWeight(int day)//returns a mean weight for this day
            {
                if (day >= _instance.bound) day = _instance.bound-1;
                return _instance.MeanWeight[day];
            }
            public static float GetRiseFactor(int StartDay,int TargDay)//returns an argument which has to be multiplied on weight in start day to get predicted weight on TardDay 
            {
                
                if ( StartDay < TargDay)
            {
                if(TargDay < _instance.bound)  return _instance.MeanWeight[TargDay] / _instance.MeanWeight[StartDay];
                else return _instance.MeanWeight[_instance.bound-1] / _instance.MeanWeight[StartDay];
            }
                    
                else return 1F;
            }
    }
}
