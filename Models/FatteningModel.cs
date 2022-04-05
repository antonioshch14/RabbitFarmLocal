using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using static RabbitFarmLocal.Controllers.MyFunctions;
using RabbitFarmLocal.BusinessLogic;

namespace RabbitFarmLocal.Models
{
    public class FatteningModel: IDescent, Controllers.I_Caller
    {
        public int Caller { get; set; }
        public Controllers.Caller ECaller { get; set; }

        private Descent DescentInstance = new Descent();
        public string Breed { get; set; }
        [DisplayName("Порода")]
        public string BreedString { get; set; }
        public Dictionary<int, int> BreedDict { get; set; }
        public void GetBreedDictionary()
        {
            BreedDict=DescentInstance.GetBreedDictionary(Breed);
        }

        public void CreateBreedDictionary()
        {
            BreedDict= BreedLogic.GetBreedDictionaryForRabit(MotherId,FatherId);
        }
        public void SetBreedString()
        {
            Breed=DescentInstance.SetBreedString(BreedDict);
        }
        public void SetBreedStringToDisplay()
        {
            BreedString=DescentInstance.SetBreedStringToDisplay(Breed);
        }
        public FatteningModel() { }
        public DateTime Born { get; set; }
        [DisplayName("Д.р.")]
        public string BornString { get { return DateToStringRU(Born); } }
        [DisplayName("Возраст \n\r м-д")]
        public string Age { get
            {
                Age rabAge = new Age(Born);
                return String.Format("{0}-{1}", rabAge.months, rabAge.days);
            } }
        [DisplayName("Возраст забоя")]
        public string AgeKilled
        {
            get
            {
                if (KillDate == null) return "не уст";
                else {
                    DateTime d = (DateTime)KillDate;
                Age rabAge = new Age(Born, d);
                return String.Format("{0}-{1}", rabAge.months, rabAge.days);
                }
            }
        }
        public int PartId { get; set; }
        [DisplayName("Номер в помете")]
        public int RabPartId { get; set; }
        [DisplayName("Клетка")]
        public int Cage { get; set; }
        [DisplayName("Окрас")]
        public string Collor { get; set; }
        public int CollorId { get; set; }
        public void SetCollorString()
        {
            Collor = Start.ConstantsSingelton.GetCollors().Find(x => x.Id == CollorId).Name;
        }
        [Display(Name = "Статус")]
        public FatStatus Status { get; set; }=FatStatus.alive ;
        [Display(Name ="Пол (статус)")]        
        public string StatusForAllFattView { get {
                DisplayAttribute stat = GetDisplayAttributesFrom(Status, typeof(FatStatus));
                return ((int)Status == 1 || (int)Status == 8) ? String.Format("{0} {1}", stat.Name , RabbitGender.ToString()): RabbitGender.ToString(); //Description
            } }
        [Display(Name = "пол")]
        public Gender RabbitGender { get; set; }
       
        [DisplayName("Дата забоя г-м-д")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? KillDate { get; set; }
        [DisplayName("Дата забоя")]
        public string KillDateString { get { return DateToStringRU(KillDate); } }
        [DisplayName("Выход мяса")]
        [Range(0,15,ErrorMessage ="Выход мяса должен быть от 0 до 15")]
        public float Weight { get; set; }//kill weight
     
        [DisplayName("Вес")]
        public float LastWeight { get; set; }
        [DisplayName("Дата взвешивания")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? WeightDate { get; set; }
        [DisplayName("Расчитанный вес")]
        public double ProjectedWeight
        {
            get
            {
                if (Born.Year != 1)//to avoit calculation if modewl used not for view building
                {
                    double forecas;
                    TimeSpan eageSpan = DateTime.Today - Born;
                    int daysRabNow = (int)eageSpan.TotalDays;
                    if (WeightDate.HasValue)
                    {
                        DateTime WD = (DateTime)WeightDate;
                        TimeSpan TSWeight = (WD - Born);
                        int daysWeghtMesured = (int)TSWeight.TotalDays;
                        float riseFactor = RabbitFarmLocal.Start.WeighGrow.GetRiseFactor(daysWeghtMesured, daysRabNow, Breed);
                        forecas = Math.Round(riseFactor * LastWeight, 1);
                        AverageWeight= Math.Round((double)RabbitFarmLocal.Start.WeighGrow.GetMeanWeight(daysRabNow, Breed), 1);
                        DaysSinceWeightMesurment = daysRabNow - daysWeghtMesured;
                    }
                    else
                    {
                        forecas = Math.Round((double)RabbitFarmLocal.Start.WeighGrow.GetMeanWeight(daysRabNow, Breed), 1);

                    }
                    return forecas;
                }
                return 0;
            }
        }
        [DisplayName("Дней от взвешивания")]
        public int? DaysSinceWeightMesurment { get; set; }
        [DisplayName("Средний вес кролика в этом возрасте")]
        public double? AverageWeight { get; set; }
        [DisplayName("Дата взвешивания")]
        public string WeightDateString { get { return DateToStringRU(WeightDate); } }
        [DisplayName("Цена, руб")]
        [Range(0, 10000, ErrorMessage = "Цена должна быть от 0 до 10000")]
        public int Price { get; set; }

        [DisplayName("Коммент")]
        public string Comment { get; set; }
        [DisplayName("Новый вес")]
        public float NewWeight { get; set; }
        [DisplayName("Мать")]
        public int MotherId { get; set; }
        [DisplayName("Отец")]
        public int FatherId { get; set; }
        public string pedigreeString { get; set; }

    }
    public enum FatStatus //alive diedItself soldAsMeat eatenByUs left4Bread sold4Bread
    {
        [Display(Name = "Живой")]
        alive, //0
        [Display(Name = "Резерв")]
        left4Bread,//1
        [Display(Name = "Умер сам")]
        diedItself,//2
        [Display(Name = "Продан мясом")] //benefited: soldAsMeat, eatenByUs, sold4Bread, sold4Bread, canned
        soldAsMeat,//3
        [Display(Name = "Себе")]
        eatenByUs,//4
        [Display(Name = "Продан на разведение")]
        sold4Bread,//5
        [Display(Name ="Оставлен на потомство")]
        used4Bread,//6
        [Display(Name = "Забит на тушенку")]
        canned,//7
        [Display(Name = "На забой")]
        selectedForKill//8
    }
    public enum FatStatusKilledView //alive diedItself soldAsMeat eatenByUs left4Bread sold4Bread
    {
        
        [Display(Name = "Продан мясом")]
        soldAsMeat=3,//3
        [Display(Name = "Себе")]
        eatenByUs=4,//4
        [Display(Name = "Забит на тушенку")]
        canned=7//7

    }


}
