using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using static RabbitFarmLocal.Controllers.MyFunctions;

namespace RabbitFarmLocal.Models
{
    public class WeightModel : Controllers._Caller//Date Weight RabId PartId
    {
        [DisplayName("Дата взвешивания")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        [DisplayName("Дата взвешивания")]
        public string DateString { get { return DateToStringRU(Date); } }
        [DisplayName("Масса кг")]
        public float Weight { get; set; }
        [DisplayName("Кроик")]
        public int RabId { get; set; }
        public int Id { get; set; }
        
        

    }
    public class FattWeightModel: WeightModel
    {
      
        public int PartId { get; set; }
        public DateTime Born { get; set; }
        [DisplayName("Возраст мес-дн")]
        public string Age
        {
            get
            {
                Age rabAge = new Age(Born,Date);
                return String.Format("{0}-{1}", rabAge.months, rabAge.days);
            }
        }
        public int days { get {
                Age rabAge = new Age(Born, Date);
                return (int)rabAge.daysTot;

            } 
        }
    }
}
