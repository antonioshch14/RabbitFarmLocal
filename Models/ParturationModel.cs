using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using static RabbitFarmLocal.Controllers.MyFunctions;

namespace RabbitFarmLocal.Models
{
    public class ParturationModel: Controllers._Caller
    {
        public int Id { get; set; }
        [DisplayName("Дата родов г-м-д")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        [DisplayName("Дата родов")]
        public string DateString { get { return DateToStringRU(Date); } }
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
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? SeparationDate { get; set; } = null;
        [DisplayName("Дата рассадки")]
        public string SeparationDateString { get { return DateToStringRU(SeparationDate); } }
        [DisplayName("Крольчата рассажины")]
        public YesNo Separated { get { return  (SeparationDate!=null) ?  YesNo.Yes :  YesNo.No; } }
        [DisplayName("Крольчата рассажины")]
        public YesNo SeparatedView { get; set; }
        [DisplayName("Клетка окрола")]
        public int Cage { get; set; }
        
        [DisplayName("Cтатус окрола")]
        public parturStatus Status { get; set; }
        [DisplayName("Дата уборки гнезда г-м-д")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DateNestRemoval { get; set; } = null;
        [DisplayName("Дата уборки гнезда ")]
        public string DateNestRemovalString { get { return DateToStringRU(DateNestRemoval); } }
        [DisplayName("Гнездо убрано")]
        public YesNo NestRemoved { get { return (DateNestRemoval != null) ? YesNo.Yes: YesNo.No; } }
        [DisplayName("Гнездо убрано")]
        public YesNo NestRemovedView { get; set; }
        public int MateId { get; set; }
        [DisplayName("Коментарий")]
        public string Comment { get; set; }
        [DisplayName("Отец")]
        public int FatherId { get; set; }
        [DisplayName("Покрытие г-м-д")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime MateDate { get; set; }
        [DisplayName("Покрытие")]
        public string MateDateString { get { return DateToStringRU(MateDate); } }
    }
    public enum parturStatus
    {
        [Display(Name ="Кормит мать")]
        feeded,
        [Display(Name = "Рассажены")]
        separated,
        [Display(Name = "Все умерли")]
        allDead,
        [Display(Name = "Мать умерла")]
        leftAlone,
        [Display(Name = "Ожидается уборка гнезда")] 
        nestRemovalAwaited, //4
        [Display(Name = "Ожидается рассадка")]
        separationAwaited, //5
        [Display(Name = "Мать и окрол в разных клетках")]
        inDifferentCages, //6
    }

}