﻿using RabbitFarmLocal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static RabbitFarmLocal.BusinessLogic.RabbitProcessor;
using RabbitFarmLocal.Start;

namespace RabbitFarmLocal.BusinessLogic
{
    public class DataUpdates
    {
        public static void UpdateRabbitsStatus()
        {
            int daysBackToUpdateRabbits = 720;
            string sql = "";
           // DateTime dt = DateTime.Now.AddDays(-Settings.PregnantDays());
            var rabbits = LoadRabbits();
            DateTime loadFrom = DateTime.Today.AddDays(-daysBackToUpdateRabbits);
            var parturs = LoadParturation(loadFrom);
            var mattings = LoadMating(loadFrom);
            foreach (var rab in rabbits)
            {
                TimeSpan age = (DateTime.Today - rab.Born);
                //if (!rab.IsAlive) rab.StoredRabStatus = Status.history;
                //else
                if (rab.IsAlive) 
                {
                    if (rab.IsMale) //if male
                    {
                        if (age.TotalDays < Settings.MaleGrowDays()) rab.StoredRabStatus = Status.growMale;
                        else rab.StoredRabStatus = Status.workMale;
                    }
                    else // if female
                    {
                        MatingModel mating = mattings.Find(x => x.MotherId == rab.RabbitId);
                        if (mating != null)
                        {
                            ParturationModel partur = parturs.Find(x => x.MotherId == rab.RabbitId);
                            if (mating.Date > DateTime.Now.AddDays(-Settings.PregnantDays()) && mating.ParturationId==null)
                            {
                                rab.StoredRabStatus = Status.pregFemale; //preg
                                sql += $"UPDATE rabbits SET status={ (int)rab.StoredRabStatus} WHERE Id={ rab.Id};";
                                continue;
                            }
                            else  if (mating.Date > DateTime.Now.AddDays(-Settings.PregnantDays() - Settings.CheckPart()) && mating.ParturationId == null)
                            {
                                rab.StoredRabStatus = Status.checkPart; //check if has delivered
                                sql += $"UPDATE rabbits SET status={ (int)rab.StoredRabStatus} WHERE Id={ rab.Id};";
                                continue;
                            }
                            else if (partur != null)
                            {
                                if (partur.Status == parturStatus.feeded || partur.Status==parturStatus.nestRemovalAwaited || partur.Status==parturStatus.separationAwaited)
                                // if (partur.Date > DateTime.Now.AddDays(-Settings.FeedDays()) || partur.Status == parturStatus.feeded)
                                {
                                    //if (partur.Children > partur.DiedChild || partur.Children == 0)
                                    //{
                                    //if (partur.SeparationDate == null)
                                    //{
                  
                                     if(partur.Date.AddDays(Settings.FeedDays()+Settings.RestDays()) < DateTime.Now)
                                    {
                                        rab.StoredRabStatus = Status.readyFemale; //feed
                                        sql += $"UPDATE rabbits SET status={ (int)rab.StoredRabStatus} WHERE Id={ rab.Id};";
                                        continue;
                                    }
                                    else if (partur.Date.AddDays(Settings.FeedDays()) < DateTime.Now)
                                    {
                                        rab.StoredRabStatus = Status.restFemale; //feed
                                        sql += $"UPDATE rabbits SET status={ (int)rab.StoredRabStatus} WHERE Id={ rab.Id};";
                                        continue;
                                    }
                                    else 
                                    {
                                        rab.StoredRabStatus = Status.feedFEmale; //feed
                                        sql += $"UPDATE rabbits SET status={ (int)rab.StoredRabStatus} WHERE Id={ rab.Id};";
                                        continue;
                                    }

                                    //}
                                    //}
                                }
                                else
                                {
                                    if (partur.SeparationDate > DateTime.Now.AddDays(-Settings.RestDays()) &&
                                        partur.Date.AddDays(Settings.FeedDays() + Settings.RestDays()) > DateTime.Now)
                                    {
                                        rab.StoredRabStatus = Status.restFemale; //rest
                                        sql += $"UPDATE rabbits SET status={ (int)rab.StoredRabStatus} WHERE Id={ rab.Id};";
                                        continue;
                                    }
                                }
                            }
                        }
                        if (age.TotalDays < Settings.FemaleGrowDays()) rab.StoredRabStatus = Status.growFemale; //grow fem
                        else rab.StoredRabStatus = Status.readyFemale;
                    }
                }
                sql += $"UPDATE rabbits SET status={ (int)rab.StoredRabStatus} WHERE Id={ rab.Id};";
            }
            UpdateSqlRabbitStatus(sql);
            // return rabbits;


        }
    }  
}
