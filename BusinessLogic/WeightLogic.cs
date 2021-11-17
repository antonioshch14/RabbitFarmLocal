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

        public float DayRiseFactor { get; set; } //represents the rise of mass

        public float MeanWeight { get; set; } //represents the mean of mass
        public float MeanWeightWOGaps { get; set; }
        public float WeightSteamlighned { get; set; }
        public float WeightSteamlighnedPrev { get; set; }
        public List<float> SortedOutWeight { get; set; }

    }
   
    public class RabWeightCurve
    {
        public RabWeightCurve() { }
        public int RabId { get; set; }
        public int PartId { get; set; }
        public static int daysToStoreCurve = 30 * 7;
        public GrowStat Status { get; set; }
        public Weight[] WeightArray { get; set; }
        public RabWeightCurve(int daysToStore)
        {
            WeightArray = new Weight[daysToStore];
        }

    }
    public enum GrowStat
    {
        norm,
        underGrow,
        overGrow

    }

    public class WeightLogic
    {
        public static Weight[] CreateGrowCurve(out List<RabWeightCurve> RabCurves)
        {
            List<FattWeightModel> weights = FattWeight.LoadAll().OrderBy(a => a.PartId).ThenBy(a => a.RabId).ThenBy(a => a.Date).ToList();

            int rab;// = weights[0].RabId;
            int part;// = weights[0].PartId;
            int index = 0;//of weights array
            int rangeToSelect = 10;
            float percentOfMatch = 0.2F;

            var range = Enumerable.Range(0, RabWeightCurve.daysToStoreCurve).Select((i) => new List<float>()).ToArray();// range to check boundaries

            RabCurves = new List<RabWeightCurve>();

            List<FattWeightModel> rabWeights; //= weights.Where(x => x.RabId == rab && x.PartId == part).ToList();//extract weights of one rabbit

            while (index < weights.Count)
            {
                rab = weights[index].RabId;
                part = weights[index].PartId;
                rabWeights = weights.Where(x => x.RabId == rab && x.PartId == part).ToList();
                RabCurves.Add(new RabWeightCurve(RabWeightCurve.daysToStoreCurve)
                {
                    RabId = rab,
                    PartId = part
                });

                for (int i = 0; i < rabWeights.Count; i++)//fill out RabCurves per rabbit
                {
                    //if (rabWeights[i].days > RabWeightCurve.daysToStoreCurve - 1)
                    //{
                    //    int tt = 1;
                    //}
                        
                    int days = (rabWeights[i].days >= RabWeightCurve.daysToStoreCurve-1) ? RabWeightCurve.daysToStoreCurve-1 : rabWeights[i].days;// boundry check

                    RabCurves[^1].WeightArray[days] = new Weight()
                    {
                        MeanWeight = rabWeights[i].Weight
                    };

                    range[days].Add(rabWeights[i].Weight);
                }

                index += rabWeights.Count;//go to next rabbit in the List
            }
            float[] clearRange = new float[RabWeightCurve.daysToStoreCurve];
            for (int i = 0; i < RabWeightCurve.daysToStoreCurve; i++)//remove all that are out of range
            {
                if (range[i].Count == 0) continue;
                restart:
                foreach (var v in range[i])
                {
                    int dayStart = i > rangeToSelect ? i - rangeToSelect : 0;
                    int dayEnd = i < (RabWeightCurve.daysToStoreCurve - rangeToSelect) ? i + rangeToSelect : RabWeightCurve.daysToStoreCurve;
                    if (checkIfOutOfRange(v, range[dayStart..dayEnd], percentOfMatch) != GrowStat.norm)
                    {
                        range[i].Remove(v);
                        goto restart;
                    }

                }
                if (range[i].Count != 0) clearRange[i] = range[i].Average();
            }
            foreach (var w in weights)//define if weight is in or out of range
            {
                int days = (w.days >= RabWeightCurve.daysToStoreCurve - 1) ? RabWeightCurve.daysToStoreCurve - 1 : w.days;// boundry check
                float deviation = w.Weight * percentOfMatch;
                if (w.Weight > clearRange[days] + deviation)
                {
                    int indexOfRab = RabCurves.FindIndex(x => x.RabId == w.RabId && x.PartId == w.PartId);
                    RabCurves[indexOfRab].Status = GrowStat.overGrow;
                }
                else if (w.Weight < clearRange[days] - deviation)
                {
                    int indexOfRab = RabCurves.FindIndex(x => x.RabId == w.RabId && x.PartId == w.PartId);
                    RabCurves[indexOfRab].Status = GrowStat.underGrow;
                }
            }
            Weight[] sumWeitgh = new Weight[RabWeightCurve.daysToStoreCurve];

            Weight[] sumWeightCount = new Weight[RabWeightCurve.daysToStoreCurve];
            for (int i = 0; i < RabWeightCurve.daysToStoreCurve; i++)
            {
                sumWeitgh[i] = new Weight();
                sumWeightCount[i] = new Weight();
            }
            foreach (var RC in RabCurves)//summ up all values in weitgh and rise factor arrays
            {
                for (int i = 0; i < RC.WeightArray.Length; i++)
                {
                    if (RC.WeightArray[i] != null)
                    {
                        if (RC.Status == GrowStat.norm)
                        {
                            //if (RC.WeightArray[i].DayRiseFactor > 0)
                            //{
                            //    sumWeitgh[i].DayRiseFactor += RC.WeightArray[i].DayRiseFactor;
                            //    sumWeightCount[i].DayRiseFactor++;
                            //}
                            if (RC.WeightArray[i].MeanWeight > 0)
                            {
                                sumWeitgh[i].MeanWeight += RC.WeightArray[i].MeanWeight;
                                sumWeightCount[i].MeanWeight++;
                            }
                        }
                        else
                        {
                            if (sumWeitgh[i].SortedOutWeight == null) sumWeitgh[i].SortedOutWeight = new List<float>();
                            sumWeitgh[i].SortedOutWeight.Add(RC.WeightArray[i].MeanWeight);
                        }
                    }
                }
            }
            foreach (var RC in RabCurves)//summ up all values in weitgh and rise factor arrays
            {
                for (int i = 0; i < RC.WeightArray.Length; i++)
                {
                    if (RC.WeightArray[i] != null)
                    {
                        if (RC.Status == GrowStat.norm)
                        {
                            //if (RC.WeightArray[i].DayRiseFactor > 0)
                            //{
                            //    sumWeitgh[i].DayRiseFactor += RC.WeightArray[i].DayRiseFactor;
                            //    sumWeightCount[i].DayRiseFactor++;
                            //}
                            if (RC.WeightArray[i].MeanWeight > 0)
                            {
                                sumWeitgh[i].MeanWeight += RC.WeightArray[i].MeanWeight;
                                sumWeightCount[i].MeanWeight++;
                            }
                        }
                        else
                        {
                            if (sumWeitgh[i].SortedOutWeight == null) sumWeitgh[i].SortedOutWeight = new List<float>();
                            sumWeitgh[i].SortedOutWeight.Add(RC.WeightArray[i].MeanWeight);
                        }
                    }
                }
            }
            for (int i = 0; i < RabWeightCurve.daysToStoreCurve; i++)// caclulting the mean value of weight and factor
            {
                //if (sumWeightCount[i].DayRiseFactor != 0) sumWeitgh[i].DayRiseFactor = sumWeitgh[i].DayRiseFactor / sumWeightCount[i].DayRiseFactor;
                if (sumWeightCount[i].MeanWeight != 0) sumWeitgh[i].MeanWeight = sumWeitgh[i].MeanWeight / sumWeightCount[i].MeanWeight;
            }
            sumWeitgh[0].MeanWeightWOGaps = 0.062F;//62 gr is a weight of a new born rabbit
            sumWeitgh[0].MeanWeight = sumWeitgh[0].MeanWeightWOGaps;
            int? countFromZero = null;
            for (int i = 1; i < RabWeightCurve.daysToStoreCurve; i++)
            {
                if (i == RabWeightCurve.daysToStoreCurve - 1 && countFromZero != 0)
                {
                    for (int ii = 1; ii <= i - countFromZero; ++ii)
                    {
                        sumWeitgh[(countFromZero??0) + ii].MeanWeightWOGaps = sumWeitgh[countFromZero??0].MeanWeight;
                    }
                }//fill the end if ends with zero
                if (sumWeitgh[i].MeanWeight == 0 && countFromZero == null) continue;
                //else if (sumWeitgh[i].MeanWeight != 0 && countFromZero == null) countFromZero = 0;
                else if (sumWeitgh[i].MeanWeight == 0 && countFromZero == 0) countFromZero = i - 1;
                else if (sumWeitgh[i].MeanWeight != 0 && countFromZero == 0) sumWeitgh[i].MeanWeightWOGaps = sumWeitgh[i].MeanWeight;
                else if (sumWeitgh[i].MeanWeight == 0 && countFromZero != 0) continue;
                else
                {

                    sumWeitgh[i].MeanWeightWOGaps = sumWeitgh[i].MeanWeight;
                    float incr = (sumWeitgh[i].MeanWeight - sumWeitgh[countFromZero ?? 0].MeanWeight) / (i - (countFromZero ?? 0));
                    for (int ii = 1; ii < i - (countFromZero??0); ++ii)
                    {
                        sumWeitgh[(countFromZero ??0) +ii].MeanWeightWOGaps = sumWeitgh[countFromZero ?? 0].MeanWeight + incr * ii;
                    }
                    countFromZero = 0;
                }
            }//fill out weight without gaps
            for (int i = 1; i < RabWeightCurve.daysToStoreCurve - 1; i++)//steamline curves
            {
                sumWeitgh[i].WeightSteamlighnedPrev = sumWeitgh[i - 1].MeanWeightWOGaps + 2 * (-1 * (sumWeitgh[i + 1].MeanWeightWOGaps - sumWeitgh[i].MeanWeightWOGaps) / 2 + sumWeitgh[i + 1].MeanWeightWOGaps - sumWeitgh[i - 1].MeanWeightWOGaps) / 3;
                if (i == RabWeightCurve.daysToStoreCurve - 2) sumWeitgh[i + 1].WeightSteamlighnedPrev = sumWeitgh[i].WeightSteamlighnedPrev;
            }
            int st = 1000;//iteration of streamlining if decline is not eleminated
            bool Declined = true;
            while (st > 0 && Declined)
            {
                Declined = false;
                for (int i = 1; i < RabWeightCurve.daysToStoreCurve - 1; i++)
                {
                    if (sumWeitgh[i + 1].WeightSteamlighnedPrev - sumWeitgh[i].WeightSteamlighnedPrev < 0) Declined = true;
                    sumWeitgh[i].WeightSteamlighned = sumWeitgh[i - 1].WeightSteamlighnedPrev + 2 * (-1 * (sumWeitgh[i + 1].WeightSteamlighnedPrev - sumWeitgh[i].WeightSteamlighnedPrev) / 2 + sumWeitgh[i + 1].WeightSteamlighnedPrev - sumWeitgh[i - 1].WeightSteamlighnedPrev) / 3; ;
                    if (i == RabWeightCurve.daysToStoreCurve - 2) sumWeitgh[i + 1].WeightSteamlighned = sumWeitgh[i].WeightSteamlighned;
                }
                for (int i = 1; i < RabWeightCurve.daysToStoreCurve; i++)
                {
                    sumWeitgh[i].WeightSteamlighnedPrev = sumWeitgh[i].WeightSteamlighned;
                }
                st -= 1;
            }
            for(int i = 1; i < sumWeitgh.Length; i++)//calculate day rise factor from steamlined weights
            {
                sumWeitgh[i].DayRiseFactor = sumWeitgh[i].WeightSteamlighned-sumWeitgh[i - 1].WeightSteamlighned;
            }

            return sumWeitgh;

            static GrowStat checkIfOutOfRange(float weight, List<float>[] valuesToCompare, float range)
            {
                bool rangeExceded = false;
                List<float> CW = new List<float>();
                foreach (var wa in valuesToCompare)
                {
                    foreach (float w in wa)
                    {
                        CW.Add(w);
                    }
                }
                CW.Remove(weight);
                float[] allValues = CW.ToArray();
                float[] sumWieghtOfArrays = new float[allValues.Length];
                float checkingWeighSumDiffereces = 0;
                float maxSum = 0;
                float deviation = weight * range;
                foreach (var w in CW)
                {
                    float valToAdd = Math.Abs(weight - w);
                    if (valToAdd > deviation) rangeExceded = true;
                    checkingWeighSumDiffereces += valToAdd;
                }
                if (rangeExceded)
                {
                    for (int i = 0; i < allValues.Length; i++)
                    {
                        for (int ii = 0; ii < allValues.Length; ii++)
                        {
                            if (ii == i) continue;
                            sumWieghtOfArrays[i] += Math.Abs(allValues[i] - allValues[ii]);
                        }
                    }
                    maxSum = sumWieghtOfArrays.Max();
                }
                if (rangeExceded && checkingWeighSumDiffereces >= maxSum)
                {
                    bool OverValue = CW.Average() < weight ? true : false;
                    if (OverValue) return GrowStat.overGrow;
                    else return GrowStat.underGrow;
                }
                return GrowStat.norm;
            }


        }



    }

}
