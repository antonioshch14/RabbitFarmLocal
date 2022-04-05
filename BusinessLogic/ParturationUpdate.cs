using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static RabbitFarmLocal.BusinessLogic.RabbitProcessor;
using RabbitFarmLocal.Models;
using RabbitFarmLocal.Controllers;
using RabbitFarmLocal.Start;

namespace RabbitFarmLocal.BusinessLogic
{
    public static class ParturationUpdate
    {
        public static int UpdateAll()
        {
            int daysBackToUpdateParturation = 320;
            List<DLRabbitModel> rabbits = LoadRabbits();
            DateTime loadFrom = DateTime.Today.AddDays(-daysBackToUpdateParturation);
            List<ParturationModel> parturs = LoadParturation(loadFrom).GroupBy(x => x.MotherId).Select(x => x.First()).ToList();
            foreach (var P in parturs)
            {
                var mother = rabbits.Find(x => x.RabbitId == P.MotherId);
                MyFunctions.Age age = new MyFunctions.Age(P.Date);
                if (P.SeparationDate != null) P.Status = parturStatus.separated;
                else if (P.Children > 0 && P.Children == P.DiedChild) P.Status = parturStatus.allDead;
                else if (age.daysTot > Settings.FeedDays()) P.Status = parturStatus.separationAwaited;
                else if (age.daysTot > Settings.NestRemoalDays() && P.DateNestRemoval==null) P.Status = parturStatus.nestRemovalAwaited;
                else if (mother.IsAlive == false) P.Status = parturStatus.leftAlone;
                else if (P.Cage != mother.Cage) P.Status = parturStatus.inDifferentCages;
                else P.Status = parturStatus.feeded;
                EditParturation(P);
            }
            return parturs.Count();
        }
        public static void UpdateOne(DLRabbitModel rab)
        {
            int daysBackToUpdateParturation = 320;
            DateTime loadFrom = DateTime.Today.AddDays(-daysBackToUpdateParturation);
            ParturationModel P = LoadParturation(loadFrom).FirstOrDefault(x => x.MotherId == rab.RabbitId);

            if (P != null)
            {
                var mother = Rabbit.LoadOneRabbId(rab.RabbitId);
                MyFunctions.Age age = new MyFunctions.Age(P.Date);
                if (P.SeparationDate != null) P.Status = parturStatus.separated;
                else if (P.Children > 0 && P.Children == P.DiedChild) P.Status = parturStatus.allDead;
                else if (age.daysTot > Settings.FeedDays()) P.Status = parturStatus.separationAwaited;
                else if (age.daysTot > Settings.NestRemoalDays() && P.DateNestRemoval == null) P.Status = parturStatus.nestRemovalAwaited;
                else if (mother.IsAlive == false) P.Status = parturStatus.leftAlone;
               
                else if (P.Cage != mother.Cage) P.Status = parturStatus.inDifferentCages;
                else P.Status = parturStatus.feeded;
                EditParturation(P);
            }
        
        }
    }
}
