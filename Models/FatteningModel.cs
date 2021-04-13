using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using static RabbitFarmLocal.Controllers.MyFunctions;

namespace RabbitFarmLocal.Models
{
    public class FatteningModel: Controllers._Caller //PartId, RabPartId, Cage, Collor, DeadItself, RabbitGender, KillDate, Weight
    {
        
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
        [Display(Name = "Статус")]
        public FatStatus Status { get; set; }=FatStatus.alive ;
        [Display(Name ="Пол (статус)")]        
        public string StatusForAllFattView { get {
                DisplayAttribute stat = GetDisplayAttributesFrom(Status, typeof(FatStatus));

                return ((int)Status == 1) ? String.Format("{0} {1}", stat.Name , RabbitGender.ToString()): RabbitGender.ToString(); //Description
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
        public float Weight { get; set; }
        [DisplayName("Вес")]
        public float LastWeight { get; set; }
        [DisplayName("Дата взвешивания")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? WeightDate { get; set; }
        [DisplayName("Дата взвешивания")]
        public string WeightDateString { get { return DateToStringRU(WeightDate); } }
        [DisplayName("Цена, руб")]
        public int Price { get; set; }

        [DisplayName("Коммент")]
        public string Comment { get; set; }
        [DisplayName("Новый вес")]
        public float NewWeight { get; set; }


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
        canned//7
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
//public enum ModesOfTransport
//{
//    [Display(Name = "Driving", Description = "Driving a car")] Land,
//    [Display(Name = "Flying", Description = "Flying on a plane")] Air,
//    [Display(Name = "Sea cruise", Description = "Cruising on a dinghy")] Sea
//}

//void Main()
//{
//    ModesOfTransport TransportMode = ModesOfTransport.Sea;
//    DisplayAttribute metadata = TransportMode.GetDisplayAttributesFrom(typeof(ModesOfTransport));
//    Console.WriteLine("Name: {0} \nDescription: {1}", metadata.Name, metadata.Description);
//}