using RabbitFarmLocal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static RabbitFarmLocal.BusinessLogic.RabbitProcessor;
using RabbitFarmLocal.Start;
using static RabbitFarmLocal.Controllers.MyFunctions;

namespace RabbitFarmLocal.BusinessLogic
{
    public class FinReport
    {
        public static FinRepModel Report (string from, string until)
        {
            FinRepModel rep = new FinRepModel() {
            DateFrom=StringToDate(from),
            DateUntil=StringToDate(until)
            };
            List<FatteningModel> ft = LoadFattenigPerStatusKilled(from, until,(int)FatStatus.canned, (int)FatStatus.eatenByUs,(int)FatStatus.sold4Bread,(int)FatStatus.soldAsMeat);//benefited: soldAsMeat, eatenByUs, sold4Bread, sold4Bread, canned
            List<DLRabbitModel> mr = Rabbit.LoadRabPerStatus(from, until, (int)RabKillStatus.canned, (int)RabKillStatus.eatenByUs, (int)RabKillStatus.sold4Bread, (int)RabKillStatus.soldAsMeat);
            List<FinModel> sp = FinanceSpent.LoadList(from, until);
            //SoldAsMeat, SoldForBread, SoldCanned, EatenByUs, SpentOnGrain, SpentOnSuplim, SpentOnHay, SpentOnMed, SpentOnBuild, SpentOnRepair
            rep.SpentOnBuild = sp.Where(x => x.Type == FodderName.Building).Sum(x=> x.Price);
            rep.SpentOnGrain = sp.Where(x => x.Type == FodderName.grains).Sum(x => x.Price);
            rep.SpentOnHay= sp.Where(x => x.Type == FodderName.hay).Sum(x => x.Price);
            rep.SpentOnMed = sp.Where(x => x.Type == FodderName.Medicine).Sum(x => x.Price);
            rep.SpentOnRepair = sp.Where(x => x.Type == FodderName.Repair).Sum(x => x.Price);
            rep.SpentOnSuplim = sp.Where(x => x.Type == FodderName.supliments).Sum(x => x.Price);
            foreach (var r in ft.Where(x => x.Status == FatStatus.soldAsMeat))//sold as meat
            {
                if (r.Price == 0)
                {
                    rep.SoldAsMeat += Math.Ceiling((double)r.Weight * (double)Settings.DefaultPrice());
                }
                else rep.SoldAsMeat += r.Price;
            }
            foreach (var r in mr.Where(x => x.StoredRabStatus == Status.soldAsMeat))//sold as meat main rabits
            {
                if (r.Price == 0)
                {
                    rep.SoldAsMeat += Math.Ceiling((double)r.Weight * (double)Settings.DefaultPrice());
                }
                else rep.SoldAsMeat += r.Price;
            }
            rep.SoldForBread = ft.Where(x => x.Status == FatStatus.sold4Bread).Sum(x => x.Price);
            rep.SoldForBread += mr.Where(x => x.StoredRabStatus == Status.sold4Bread).Sum(x => x.Price);//main rabbits
            foreach (var r in ft.Where(x => x.Status == FatStatus.canned))
            {
                if (r.Price == 0) rep.EatenByUs += Math.Ceiling((double)r.Weight * (double)Settings.DefaultPrice());
                rep.SoldCanned += r.Price;
            }
            foreach (var r in mr.Where(x => x.StoredRabStatus == Status.canned))// main rabbits
            {
                if (r.Price == 0) rep.EatenByUs += Math.Ceiling((double)r.Weight * (double)Settings.DefaultPrice());
                rep.SoldCanned += r.Price;
            }
            foreach (var r in ft.Where(x => x.Status == FatStatus.eatenByUs))
            {
                rep.EatenByUs += Math.Ceiling((double)r.Weight * (double)Settings.DefaultPrice());
            }
            foreach (var r in mr.Where(x => x.StoredRabStatus == Status.eatenByUs))//main rabbits
            {
                rep.EatenByUs += Math.Ceiling((double)r.Weight * (double)Settings.DefaultPrice());
            }
            return rep;
        }
        public static List<FinRepModel> ReportForYear(int year)
        {
            List<FinRepModel> rep = new List<FinRepModel>();
            
            DateTime repBegin = new DateTime(year,1, Settings.FinRepDate() + 1);
            DateTime repEnd;
            int mon;
            var today = DateTime.Today;
            if (today.Year == year) mon = today.Month;
            else mon = 12;
            for (int i = 1; i <= mon; i++)
            {
                repEnd = repBegin.AddMonths(1).AddDays(-1);
                rep.Add(Report(DateToString(repBegin),DateToString(repEnd)));
                repBegin = repBegin.AddMonths(1);
                
            }
            return rep;
        }
        public string GerFinReportString (FinRepModel rep)
        {
            return "";
        }
    }
}
