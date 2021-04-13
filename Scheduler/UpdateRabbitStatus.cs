using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using static RabbitFarmLocal.BusinessLogic.DataUpdates;
using RabbitFarmLocal.messaging;

namespace RabbitFarmLocal.Scheduler
{
    public class UpdateRabbitStatus : ScheduledProcessor
    {

        public UpdateRabbitStatus(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
        {
        }

        protected override string Schedule => "1 0 * * *";

        public override Task ProcessInScope(IServiceProvider serviceProvider)
        {
            Console.WriteLine("Processing starts here");
            UpdateRabbitsStatus();
            return Task.CompletedTask;
        }
    }
   
}
