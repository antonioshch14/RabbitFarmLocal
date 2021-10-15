using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RabbitFarmLocal.Models;
using static RabbitFarmLocal.BusinessLogic.RabbitProcessor;
using RabbitFarmLocal.Start;

namespace RabbitFarmLocal.BusinessLogic
{
    public class CreateReport
    {
        public static ReportModel FillReport()
        {
            ReportModel rep = new ReportModel(); //TotalRabits Females GrowFemales Males GrowMales PregnantFemales FeedFemales RestFemales PutNest RemoveNest Separate Mate
           int daysBackToUpdateRabbits = 720;
            //DateTime loadFromMate = DateTime.Today.AddDays(-Settings.FeedDays()-Settings.RestDays()-20);
            //DateTime loadFromPartur = DateTime.Today.AddDays(-Settings.PregnantDays() - 20);
            DateTime loadFrom = DateTime.Today.AddDays(-daysBackToUpdateRabbits);
            var parturs = LoadParturation(loadFrom);
            var mattings = LoadMating(loadFrom);
            var rabbits = LoadRabbits();
            var fattening = LoadFattenigAllAlive();
            rep.Fattening = fattening.Count();
            rep.TotalRabits = rep.Fattening;

            MatingModel mating = new MatingModel();
            ParturationModel partur = new ParturationModel();
            DateTime now = DateTime.Now;
            foreach (var rab in rabbits)
            {
                if (!rab.IsAlive) continue;
                rep.TotalRabits++;
                if (!rab.IsMale)
                {
                    rep.Females++;
                    mating = mattings.Find(x => x.MotherId == rab.RabbitId);
                    partur = parturs.Find(x => x.MotherId == rab.RabbitId);

                    switch (rab.StoredRabStatus) // workMale growMale growFemale pregFemale feedFEmale restFemale readyFemale
                    {
                        case Status.growFemale:
                            rep.GrowFemales++;
                            break;

                        case Status.pregFemale:
                            rep.PregnantFemales++;
                            if (mating.PutNest == null)
                            {
                                PutNest put = new PutNest();
                                put.Date = mating.Date.AddDays(Settings.PutNestDays());
                                put.RabbitId = rab.RabbitId;
                                put.Id = mating.Id;
                                put.Cage = rab.Cage;
                                rep.PutNest.Add(put);
                            }
                            CheckPart chrp = new CheckPart();
                            chrp.RabbitId = rab.RabbitId;
                            chrp.Date = mating.Date.AddDays(Settings.PregnantDays());
                            chrp.Cage = rab.Cage;
                            chrp.MateId = mating.Id;
                            chrp.MotherId = mating.MotherId;
                            chrp.FatherId = mating.FatherId;
                            rep.CheckPart.Add(chrp);
                            break;
                        case Status.feedFEmale:
                            rep.FeedFemales++;
                            if (partur.DateNestRemoval == null)
                            {
                                RemoveNest rem = new RemoveNest();
                                rem.Date = partur.Date.AddDays(Settings.NestRemoalDays());
                                rem.RabbitId = rab.RabbitId;
                                rem.Id = partur.Id;
                                rem.Cage = rab.Cage;
                                rep.RemoveNest.Add(rem);
                            }
                            if (DateTime.Now >= partur.Date.AddDays(Settings.NestRemoalDays()))
                            {
                                Separate sep = new Separate();
                                sep.Date = partur.Date.AddDays(Settings.FeedDays());
                                sep.RabbitId = rab.RabbitId;
                                sep.Id = partur.Id;
                                sep.Cage = rab.Cage;
                                rep.Separate.Add(sep);
                            }
                            break;
                        case Status.restFemale:
                            rep.RestFemales++;
                            //mate.Date = DateTime.Now.AddDays(-1);
                            //mate.RabbitId = rab.RabbitId;
                            //rep.Mate.Add(mate);
                            break;
                        case Status.readyFemale:
                            rep.ReadyFemales++;
                            Mate mate = new Mate();
                            if (partur != null) mate.Date = partur.Date.AddDays(Settings.FeedDays() + Settings.RestDays());
                            else mate.Date = rab.Born.AddDays(Settings.FemaleGrowDays());
                            //mate.Date = DateTime.Now.AddDays(-1);
                            mate.Cage = rab.Cage;
                            mate.RabbitId = rab.RabbitId;
                            rep.Mate.Add(mate);
                            break;
                        case Status.checkPart:
                            rep.PregnantFemales++;
                            CheckPart c = new CheckPart();
                            c.RabbitId = rab.RabbitId;
                            c.Date = mating.Date.AddDays(Settings.PregnantDays());
                            c.Cage = rab.Cage;
                            c.MateId = mating.Id;
                            c.MotherId = mating.MotherId;
                            c.FatherId = mating.FatherId;
                            rep.CheckPart.Add(c);
                            break;
                    }
                }
                else { rep.Males++;
                    if (rab.StoredRabStatus == Status.growMale) rep.GrowMales++; 
                   
                }
            }
            
            return rep;
        }
        public static string? GetAlertString()
        {
            int numb = 1;
            string? message = null;
            ReportModel repport = FillReport();
            foreach (var pn in repport.PutNest)
            {
                if (pn.Alert)
                {
                    message += String.Format("{3})поставить гездо крольчихе {0} (клетка {2}) нужно было {1} \n\r", pn.RabbitId, pn.DateString,pn.Cage,numb);
                    numb++;
                    
                }
            }
            foreach (var rn in repport.RemoveNest)
            {
                if (rn.Alert)
                {
                    message += String.Format("{3})убрать гездо у крольчихи {0} (клетка {2}) нужно было {1} \n\r", rn.RabbitId, rn.DateString,rn.Cage,numb);
                    numb++;
                    
                }
            }
            foreach (var m in repport.Mate)
            {
                if (m.Alert)
                {
                    message += String.Format("{3})покрыть крольчиху {0} (клетка {2}) нужно было {1} \n\r", m.RabbitId, m.DateString,m.Cage,numb);
                    numb++;
                    
                }
            }
            foreach (var s in repport.Separate)
            {
                if (s.Alert)
                {
                    message += String.Format("{3})рассадить крольчат от крольчихи {0} (клетка {2}) нужно было {1} \n\r", s.RabbitId, s.DateString,s.Cage,numb);
                    numb++;
                   
                }
            }
            foreach (var c in repport.CheckPart)
            {
                if (c.Alert)
                {
                    message += String.Format("{3})проверить окрол крольчихи {0} (клетка {2}) нужно было {1} \n\r", c.RabbitId, c.DateString, c.Cage,numb);
                    numb++;

                }
            }
            return message;
        }
    }
    
}
