using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitFarmLocal.ViewComponents
{
    [ViewComponent(Name = "overdues")]
    public class TaskOverdueComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            int overdues = RabbitFarmLocal.Start.ConstantsSingelton.GetNumberOfOverDues();
            return View(overdues);
        }
    }
}
