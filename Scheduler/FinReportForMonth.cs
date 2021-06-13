using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using RabbitFarmLocal.messaging;
using  RabbitFarmLocal.BusinessLogic;
using RabbitFarmLocal.Models;
using RabbitFarmLocal.Start;

namespace RabbitFarmLocal.Scheduler
{
    public class FinReportForMonth : ScheduledProcessor
    {

        public FinReportForMonth(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
        {
        }
        string time = "2 10 " + Settings.FinRepDate() + " * *";
       
        protected override string Schedule => time;

        public override Task ProcessInScope(IServiceProvider serviceProvider)
        {
            string report = FinReport.MonthReportString();
            MyTelegram.SendFinMessageToBot(report);

            return Task.CompletedTask;
        }
    }
}