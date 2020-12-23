using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RabbitFarmLocal.Models
{
    public class ParturationModel
    {
        public int Id { get; set; }
        [DisplayName("Дата родов г-м-д")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; } 
        [DisplayName("Мать")]
        public int MotherId { get; set; }
        [DisplayName("Крольчат")]
        public int Children { get; set; }
        
        [DisplayName("Мальчиков")]
        public int Males { get; set; }
        [DisplayName("Девочек")]
        public int Females { get; set; }
        [DisplayName("Умерло крольчат")]
        public int DiedChild { get; set; }
        [DisplayName("Дата рассадки г-м-д")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? SeparationDate { get; set; } = null;
        [DisplayName("Клетка")]
        public int Cage { get; set; }
        [DisplayName("Дата уборки гнезда г-м-д")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DateNestRemoval { get; set; } = null;
        public int MateId { get; set; }
        [DisplayName("Коментарий")]
        public string Comment { get; set; }
        [DisplayName("Отец")]
        public int FatherId { get; set; }
        [DisplayName("Покрытие г-м-д")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime MateDate { get; set; }

    }
}