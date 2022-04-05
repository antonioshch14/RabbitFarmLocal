using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;


namespace RabbitFarmLocal.Models
{
    public class CollorModel
    {
        public int Id { get; set; }
        [DisplayName("Окрас")]
        public string? Name { get; set; }
        public string? ImageName { get; set; }
    }
}
