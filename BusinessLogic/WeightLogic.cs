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

namespace RabbitFarmLocal.BusinessLogic
{
    
    public class Weight
    {
        public int Date { get; set; }// date of weigth meausre
        public float DayRiseFactor { get; set; } //represents the rise of mass
        
    }
    public class RabWeightCurve
    {
        public int RabId { get; set; }
        public int PartId { get; set; }
        public Weight[] WeightArray { get; set; }
        public RabWeightCurve (int daysToStore)
        {
            WeightArray = new Weight[daysToStore];
        }

    }
    public class FattWeightModelComarable : FattWeightModel, IComparable<FattWeightModel>
    {
        public int CompareTo(FattWeightModel other)
        {
            if (this.RabId > other.RabId || this.PartId > other.PartId)
                return 1;
            else if (this.RabId < other.RabId || this.PartId < other.PartId)
                return -1;
            else 
                return 0;
        }
        //private class sortYearAscendingHelper : IComparer
        //{
        //    int IComparer.Compare(object a, object b)
        //    {
        //        FattWeightModel c1 = (FattWeightModel)a;
        //        FattWeightModel c2 = (FattWeightModel)b;

        //        if (c1.RabId > c2.RabId)
        //            return 1;

        //        if (c1.RabId < c2.RabId)
        //            return -1;

        //        else
        //            return 0;
        //    }
        //}
        //int IComparable.CompareTo(object obj)
        //{
        //    FattWeightModel c = (FattWeightModel)obj;
        //    return String.Compare(this.make, c.make);
        //}
        //public static IComparer sortYearAscending()
        //{
        //    return (IComparer)new sortYearAscendingHelper();
        //}
    }
    public class WeightLogic
    {
        public static void CreateGrowCurve()
        {
            int daysToCreateCurve = 30 * 6;
            List<FattWeightModel> weights = FattWeight.LoadAll();
            List<FattWeightModelComarable> weightsComp=new List<FattWeightModelComarable>(weights.Count);
            foreach(var w in weights)
            {
                weightsComp.Add((FattWeightModelComarable)w);

            }
            // weights.Sort(delegate (FattWeightModel f1, FattWeightModel f2) { return f1.RabId.CompareTo(f2.RabId); });
            weightsComp.Sort();
            Console.WriteLine("");
             List <RabWeightCurve> RWC;



        }
       


    }
}
