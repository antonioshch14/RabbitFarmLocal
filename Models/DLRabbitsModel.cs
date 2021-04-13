using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static RabbitFarmLocal.Controllers.MyFunctions;

namespace RabbitFarmLocal.Models
{
    public class DLRabbitModel: RabbitKillModel
    {
        public int Id { get; set; } //RabbitId Cage  RabbitGender Breed  BreedType Collor  Born Mother Father IsAlive Status 
        [DisplayName("Номер кролика")] // Id rabId cage born mother father isMale status
        public int RabbitId { get; set; }
        [DisplayName("Клетка")]
        public int Cage { get; set; }
        
        public bool IsMale { get; set; }
        [DisplayName("Пол")]
        public string Gender { get; set; }
        [DisplayName("Порода")]

        public string Breed { get; set; }
        [DisplayName("Окрас")]
        public string Collor { get; set; }

        [DisplayName("Д.р.")]
        public string BornString { get {
                return DateToStringRU(Born);
            } }

        [DisplayName("Д.р.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Born { get; set; }
        public string BornStringForEdit { get { return DateToString(Born); } }

        [DisplayName("Мать")]
        public int Mother { get; set; }
        [DisplayName("Отец")]
        public int Father { get; set; }
        [DisplayName("Живой")]
        public bool IsAlive { get; set; }
        //[DisplayName("Статус")]
        //public Status? RabbitStatus {
        //    get  {
        //        if (StoredRabStatus != null) return StoredRabStatus.Value;
        //        else {
        //            if (!IsAlive) return Status.history;
        //            TimeSpan age = (DateTime.Today - Born);

        //            if (IsMale)
        //            {
        //                if (age.TotalDays < 30 * 9) return Status.growMale;
        //                return Status.workMale;
        //            }
        //            else
        //            {
        //                if (age.TotalDays < 30 * 9) return Status.growFemale;
        //                return Status.readyFemale;
        //            }
        //        }

        //    }
        //        }
        [DisplayName("Статус")]
        public Status? StoredRabStatus { get; set; }

        [DisplayName("Возраст")]
        public string Age
        {
            get
            { String age = "";
                if (IsAlive)
                {
                    Age rabAge = new Age(Born, DateTime.Today);
                    age= String.Format(" лет:{0} мес:{1} дн:{2}", rabAge.years, rabAge.months, rabAge.days);
                } else
                {
                    DateTime TermDateSub=DateTime.Today;
                    if(TermDate != null) TermDateSub =  (DateTime)TermDate;
                    
                    Age rabAge = new Age(Born, TermDateSub);
                    age = String.Format(" лет:{0} мес:{1} дн:{2}", rabAge.years, rabAge.months, rabAge.days);
                }
                return age;
            }
        }
        [Display(Name = "пол")]
        public RabGender RabbitGender { get; set; } 
    }
    public class RabbitKillModel //  termDate price killWeight
    {
        [DisplayName("Дата перехода в историю г-м-д")] // TermDate Weight Price
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? TermDate { get; set; } = null;
        [DisplayName("Дата смерти")]
        public string TermDateString { get { return DateToStringRU(TermDate); } }
        public string TermDateStringForEdit { get { return DateToString(TermDate); } }
        [DisplayName("Выход мяса")]
        public float Weight { get; set; }
        [DisplayName("Цена")]
        public int Price { get; set; }
        
    }
    public class RabbitModelDelete
    {
        public int RabbitId { get; set; }


    }
    public enum Status //workMale growMale growFemale pregFemale feedFEmale restFemale readyFemale
    {
        [Display(Name = "самец")]
        workMale, //0
        [Display(Name = "ремонтный самец")]
        growMale,//1
        [Display(Name = "ремонтная самка")]
        growFemale,//2
        [Display(Name = "беременная самка")]
        pregFemale,//3
        [Display(Name = "кормящая самка")]
        feedFEmale,//4
        [Display(Name = "отдыхающая самка")]
        restFemale,//5
        [Display(Name = "самка")]
        readyFemale,//6
        [Display(Name = "история")] 
        history,//7
        [Display(Name = "проверить окрол")]
        checkPart,//8
        [Display(Name = "Умер сам")] //update
        diedItself,//9
        [Display(Name = "Себе")]
        eatenByUs,//10
        [Display(Name = "Продан на разведение")]
        sold4Bread,//11
        [Display(Name = "Забит на тушенку")]
        canned,//12
        [Display(Name = "Продан мясом")]
        soldAsMeat //13
    }
    public enum RabKillStatus
    {
        [Display(Name = "Умер сам")]
        diedItself=9,//9
        [Display(Name = "Себе")]
        eatenByUs,//10
        [Display(Name = "Продан на разведение")]
        sold4Bread,//11
        [Display(Name = "Забит на тушенку")]
        canned,//12
        [Display(Name = "Продан мясом")]
        soldAsMeat //13
    }
    public enum RabGender
    {
        [Display(Name = "самка")]
        female,
        [Display(Name = "самец")]
        male
    }
}