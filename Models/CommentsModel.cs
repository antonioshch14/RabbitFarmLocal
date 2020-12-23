using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace RabbitFarmLocal.Models
{
    public class CommentsModel
    {
        public int Id { get; set; }
        [Display(Name = "Номер кролика")]
        public int RabbitId { get; set; }
        [Display(Name = "Коментарий")]
        public string Comment { get; set; }
        [Display(Name = "Дата записи")]
        
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; } = DateTime.Now;

    }
}