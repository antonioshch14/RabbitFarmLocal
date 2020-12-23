using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RabbitFarmLocal.Models
{
    public class DLRabbitModel
    {
        public int Id { get; set; }
        [DisplayName("Номер кролика")]
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

        
        public string BornString { get; set; }
        
        [DisplayName("Д.р. г-м-д")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Born { get; set; }
        [DisplayName("Мать")]
        public int Mother { get; set; }
        [DisplayName("Отец")]
        public int Father { get; set; }
        [DisplayName("Живой")]
        public bool IsAlive { get; set; }
        [DisplayName("Статус")]
        public Status RabbitStatus { get; set; }



    }
    public class RabbitModelDelete
    {
        public int RabbitId { get; set; }


    }
    public enum Status
    {
        [Description("рабочий самец")]
        workMale, //0
        [Description("растущий самец")]
        growMale,//1
        [Description("растущая самка")]
        growFemale,//2
        [Description("беременная самка")]
        pregFemale,//3
        [Description("кормящая самка")]
        feedFEmale,//4
        [Description("отдыхающая самка")]
        restFemale,//5
        [Description("готовая самка")]
        readyFemale,//6
        [Description("история")]
        history//7

    }
}