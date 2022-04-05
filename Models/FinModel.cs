using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static RabbitFarmLocal.Controllers.MyFunctions;

namespace RabbitFarmLocal.Models
{
    public class FinModel //Id,Date,Price, Weight, Type, Comment
    {
        public int Id { get; set; }
        [DisplayName("Дата")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        [DisplayName("Дата")]
        public string DateString { get { return DateToStringRU(Date); } }
        public string DateStringForEdit { get { return DateToString(Date); } }
        [DisplayName("Цена, руб")]
        public float Price { get; set; }
        [DisplayName("Масса, кг.")]
        public float Weight { get; set; }
        [DisplayName("Тип")]
        public FodderName Type { get; set; }
        [DisplayName("Комментрий")]
        public string Comment { get; set; }
    }
    public enum FodderName
    {
        [Display(Name="Гранулы")]
        grains,
        [Display(Name = "Добавки")]
        supliments,
        [Display(Name = "Сено")]
        hay,
        [Display(Name = "Лекарства")]
        Medicine,
        [Display(Name = "Строительство")]
        Building,
        [Display(Name = "Ремонт")]
        Repair,
        [Display(Name = "Продано тушенкой")]
        SoldASCannedMeat

    }
    public class FinRepSelectDates
    {
        [DisplayName("отчет c")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateFrom { get; set; }
        [DisplayName("отчет c")]
        public string DateFromString { get { return DateToStringRU(DateFrom); } }
        public string DateFromStringForEdit { get { return DateToString(DateFrom); } }
        [DisplayName("отчет до")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateUntil { get; set; }
        [DisplayName("отчет до")]
        public string DateUntilString { get { return DateToStringRU(DateUntil); } }
        public string DateUntilStringForEdit { get { return DateToString(DateUntil); } }
        [DisplayName("За месяц")]
        public string Month { get 
            {
                var enumMonth = (Months)DateUntil.Month;
                string month = enumMonth.ToString();
                return month;
            }
        }
    }
   
    public class FinRepModel: FinRepSelectDates //Id,Date,Price, Weight, Type, Comment
    {
        [DisplayName("Денег получено, руб")]
        public double EarnedTotal { get { return Math.Ceiling(SoldAsMeat + SoldForBread + SoldCanned); } }
        [DisplayName("Было бы получено, если бы продали, то что съели сами ")]
        public double EarnedTotalWIthOurConsum { get { return Math.Ceiling(EarnedTotal + EatenByUs); } }
        [DisplayName("Продано мяса на ")]
        public double SoldAsMeat { get; set; } //BenefitWithOurConsum, BenefitTotal, SpentTotal, EarnedTotalWIthOurConsum, EarnedTotal, SoldAsMeat, SoldForBread, SoldCanned, EatenByUs, SpentOnGrain, SpentOnSuplim, SpentOnHay, SpentOnMed, SpentOnMed, SpentOnBuild, SpentOnRepair
        [DisplayName("Продано на разведение")]
        public double SoldForBread { get; set; }
        [DisplayName("Продано тушенкой")]
        public double SoldCanned { get; set; }
        [DisplayName("Съедино нами на")]
        public double EatenByUs { get; set; }
        [DisplayName("Затраты всего")]
        public double SpentTotal { get { return SpentOnGrain + SpentOnSuplim + SpentOnHay + SpentOnMed + SpentOnBuild + SpentOnRepair; } }
        [DisplayName("Затраты на гранулы")]
        public double SpentOnGrain { get; set; }
        [DisplayName("Затраты на добавки")]
        public double SpentOnSuplim { get; set; }
        [DisplayName("Затраты на сено")]
        public double SpentOnHay { get; set; }
        [DisplayName("Затраты на лекарства")]
        public double SpentOnMed { get; set; }
        [DisplayName("Затраты на постройки")]
        public double SpentOnBuild { get; set; }
        [DisplayName("Затраты на ремонт")]
        public double SpentOnRepair { get; set; }
        [DisplayName("Заработано всего, руб")]
        public double BenefitTotal { get { return Math.Ceiling(EarnedTotal - SpentTotal); } }
        [DisplayName("Было бы заработано, если бы сами не съели")]
        public double BenefitWithOurConsum { get { return Math.Ceiling(EarnedTotalWIthOurConsum - SpentTotal); } }
    }
    public class FinRepHistory
    {
        [DisplayName("Затраты всего")]
        public double SpentTotal { get; set; }
        [DisplayName("Год")]
        public double Year { get; set; }
        [DisplayName("Заработано всего, руб")]
        public double BenefitTotal { get; set; }
        [DisplayName("Было бы заработано, если бы сами не съели")]
        public double BenefitWithOurConsum { get; set; }
    }
}
