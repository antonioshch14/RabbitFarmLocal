using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RabbitFarmLocal.Models
{
    public class MatingModel
    {
        public int Id { get; set; }
        [DisplayName("Дата покрытия г-м-д")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        [DisplayName("Мать")]
        public int MotherId { get; set; }
        [DisplayName("Отец")]
        public int FatherId { get; set; }
        public int? ParturationId { get; set; }
        [DisplayName("Статус")]
        public string Status { get {
                if (ParturationId == null) {
                    TimeSpan ts = (DateTime.Today - Date);
                    if (ts.TotalDays > 31) return "просрочен";
                    else return "беременная";
                } 
                else if (ParturationId == -1) return "прохолост";
                else return "смотри окрол";
            
            } }
        
        
    }
}