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
using static RabbitFarmLocal.Controllers.MyFunctions;
using RabbitFarmLocal.Start;
using System.Dynamic;
using RabbitFarmLocal.Controllers;
using static RabbitFarmLocal.BusinessLogic.FinReport;

namespace RabbitFarmLocal.Controllers
{
    public class FinanceController : Controller
    {
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public ActionResult FinSpntView()
        {
            double days = -1;
            if (days == -1) days = 360;
            string date = DateToString(DateTime.Now.AddDays(-days));
            List<FinModel> fin = FinanceSpent.LoadList(date);
            return View(fin);
        }
        public ActionResult FinSpntCeate()
        {
            FinModel fin = new FinModel();
            ViewBag.Date = DateToString(DateTime.Now);
            return PartialView(fin);
        }
        [HttpPost]
        public ActionResult FinSpntCeate(FinModel fin)
        {
            FinanceSpent.Create(fin);
            return RedirectToAction("FinSpntView");
        }
        public ActionResult FinSpntEdite(int id)
        {
            FinModel fin = FinanceSpent.LoadOne(id);
            //ViewBag.Date = DateToString(fin.Date);
            return PartialView(fin);
        }
        [HttpPost]
        public ActionResult FinSpntEdite(FinModel fin)
        {
            FinanceSpent.Edit(fin);
            return RedirectToAction("FinSpntView");
        }
        public ActionResult FinSpntDelete(int id)
        {
            FinanceSpent.Delete(id);
            return RedirectToAction("FinSpntView");
        }

        public ActionResult SelectPeriodForFinRep()
        {
            DateTime _DateFrom = new DateTime(DateTime.Today.Year, DateTime.Today.Month, Settings.FinRepDate());
            if (_DateFrom.Day <= Settings.FinRepDate()) _DateFrom = _DateFrom.AddMonths(-1);

            FinRepSelectDates fin = new FinRepSelectDates()
            {
                
                DateFrom = _DateFrom,
                DateUntil = DateTime.Now
             };

            var today = DateTime.Today;
            var thisMonth = new DateTime(today.Year, today.Month, 1);
            var lastMonthBeg = thisMonth.AddMonths(-1);
            var lastMonthFin = thisMonth.AddDays(-1);
            ViewBag.PrewMonthBeg = DateToString(lastMonthBeg);
            ViewBag.PrewMonthFin = DateToString(lastMonthFin);
            var last6MonthsBeg = thisMonth.AddMonths(-7);
            var During6MonthsBeg = today.AddMonths(-6);
            ViewBag.During6MonthsBeg = DateToString(During6MonthsBeg);//During12MonthsBeg During6MonthsBeg 
            var During1MonthsBeg = today.AddMonths(-1);
            ViewBag.During1MonthsBeg = DateToString(During1MonthsBeg);
            ViewBag.Last6MonthsBeg = DateToString(last6MonthsBeg);
            ViewBag.Today = DateToString(today);
            var last12MonthsBeg = thisMonth.AddMonths(-12);
            var During12MonthsBeg = today.AddMonths(-12);
            ViewBag.Last12MonthsBeg = DateToString(last12MonthsBeg);
            ViewBag.During12MonthsBeg = DateToString(During12MonthsBeg);
            
            
            
            return View(fin);
        }
        public ActionResult FinanceRepYearly(DateTime? from=null,DateTime? until=null)
        {
            //if(DateTime.Now.Year > initialYear)
            //{
            //   // List<FinRepHistory> history=new List<FinRepHistory>();
            //    int yearsForReport= DateTime.Now.Year - initialYear;

            //    for (int i = 0; i < yearsForReport; i++)
            //    {
            //        List<FinRepModel> finRepHist = ReportForYear(initialYear + i);
            //        FinRepHistory finRepHistYear = new FinRepHistory();
            //        finRepHistYear.Year = initialYear + i;
            //        foreach (FinRepModel finRep in finRepHist)
            //        {
            //            finRepHistYear.BenefitWithOurConsum += finRep.BenefitWithOurConsum;
            //            finRepHistYear.BenefitTotal += finRep.BenefitTotal;
            //            finRepHistYear.SpentTotal += finRep.SpentTotal;
            //        }
            //        //history.Add(finRepHistYear);
            //    }
            //   //ViewBag.FinRepHistory = history;
            //}
            List<FinRepModel> FinList;
            if (from!=null && until != null)
            {
                FinList = ReportForYear((DateTime)from, (DateTime)until);
                ViewBag.StDat = DateToString(from);
                ViewBag.FinDat = DateToString(until);
            } else
            {
                int year;
                DateTime thresholdDate = new DateTime(DateTime.Now.Year, 2, Settings.FinRepDate());
                if (DateTime.Now > thresholdDate) year = DateTime.Now.Year;
                else year = DateTime.Now.AddYears(-1).Year;
                ViewBag.finDates = Settings.FinRepDate();
                ViewBag.year = year;
                FinList = ReportForYear(year);
            }
           
          
            return View(FinList);
        }

        [HttpPost]
        public ActionResult FinanceReport (FinRepSelectDates dts)
        {
            TimeSpan ts = dts.DateUntil- dts.DateFrom;
            if (ts.TotalDays > 30) return RedirectToAction("FinanceRepYearly", new {from= (DateTime?)dts.DateFrom, until= (DateTime?)dts.DateUntil } );
            FinRepModel rep = Report(dts.DateFromStringForEdit, dts.DateUntilStringForEdit);
            ViewBag.StDat = dts.DateFromStringForEdit;
            ViewBag.FinDat = dts.DateUntilStringForEdit;
            return View(rep);
        }
    }
}
