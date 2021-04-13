using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using RabbitFarmLocal.messaging;
using  RabbitFarmLocal.BusinessLogic;
using RabbitFarmLocal.Models;

namespace RabbitFarmLocal.Scheduler
{
    public class SendMessage : ScheduledProcessor
    {
        
        public SendMessage(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
        {
        }

        protected override string Schedule => "1 10 * * *";

        public override Task ProcessInScope(IServiceProvider serviceProvider)
        {
            string? report = CreateReport.GetAlertString();
            if(report!=null) MyTelegram.SendMessageToBot(report);
            
            return Task.CompletedTask;
        }
    }
}
