using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;
using RabbitFarmLocal.BusinessLogic;

namespace RabbitFarmLocal.ViewComponents
{
    [ViewComponent(Name = "pedigree")]
    public class PedigreeComponent : ViewComponent
    {
        public IViewComponentResult Invoke(int rabid)
        {
            JsonSerializerOptions options = new()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = false// change to false in production
            };
            string DecsentJson = System.Text.Json.JsonSerializer.Serialize(DescentLogic.GetRabDecent(rabid), options);
            return View("Default",model:DecsentJson);
        }
    }
}
