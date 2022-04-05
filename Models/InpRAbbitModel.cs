using System;
using System.Collections.Generic;

using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace RabbitFarmLocal.Models
{
    public class InpRAbbitModel
    {
           
            [Display(Name = "Номер кролика")]
            [Required(ErrorMessage = "Введите номер кролика")]
            public int RabbitId { get; set; }
            [Display(Name = "Номер клетки")]
            [Required(ErrorMessage = "Выберите номер клетки")]
            public int Cage { get; set; }
            public List<ListOfCages> CageList { get; set; }
            public int[] CageListTest { get; set; }

             [Display(Name = "пол")]
             public Gender RabbitGender { get; set; } = Gender.самка;

            [Display(Name = "Порода")]
            public string Breed { get; set; }
            [DisplayName("Порода")]
            public string BreedString { get; set; }
            public int BreedId { get; set; }
            [Display(Name = "Окрас")]
            public string Collor { get; set; }
        public int CollorId { get; set; }

            [Display(Name = "Дата рождения")]
           [Required(ErrorMessage = "Введите дату рождения")]
            [DataType(DataType.Date)]
           [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
            public DateTime Born { get; set; } = DateTime.Now;
            [Display(Name = "Номер матери")]
            public int Mother { get; set; } = 0;
            [Display(Name = "Номер отца")]
            public int Father { get; set; } = 0;
            [Display(Name = "отметить если живой")]
            public bool IsAlive { get; set; } = true;
            public int? PartId { get; set; }
        public int? PartRabId { get; set; }


       
    }
    public enum Gender
    {
        самец,
        самка
        
    }
    
}