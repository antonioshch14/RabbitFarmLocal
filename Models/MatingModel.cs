using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static RabbitFarmLocal.Controllers.MyFunctions;
using RabbitFarmLocal.Start;

namespace RabbitFarmLocal.Models
{
    public class MatingModel : Controllers._Caller
    {
        public int Id { get; set; }
        [DisplayName("Дата покрытия д-м-г")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        //[Required(ErrorMessage = "Введите дату покрытия")]
        public DateTime Date { get; set; }
        [DisplayName("Покрытие")]
        public string DateString { get { return DateToStringRU(Date); } }
        [DisplayName("Мать")]
        public int MotherId { get; set; }
        [DisplayName("Клетка")]
        public int Cage { get; set; }
        [DisplayName("Отец")]
        public int FatherId { get; set; }
        public int? ParturationId { get; set; }
        [DisplayName("Статус")]
        public string Status { get {
                if (ParturationId == null) {
                    TimeSpan ts = (DateTime.Today - Date);
                    if (ts.TotalDays > Settings.PregnantDays()) return "просрочен";
                    else return "беременная";
                } 
                else if (ParturationId == -1) return "прохолост";
                else return "удачный окрол";
            
            } }
        
        [DisplayName("Установка гнезда")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        //[Required(ErrorMessage = "Введите дату покрытия")]
        public DateTime? PutNest { get; set; }
        [DisplayName("Усвтановка гнезда")]
        public string PutNestString { get { return DateToStringRU(PutNest); } }
        [DisplayName("Гнездо поставлено")]
        public YesNo NestPut { get { return (PutNest != null) ? YesNo.Yes : YesNo.No; } }

        [DisplayName("Гнездо поставлено")]
        public YesNo NestPutView { get; set; }
        public List<LevelOfRelations> Relations { get; set; }
        [DisplayName("Статус")]
        public bool FailMate { get; set; }
    }
    
}