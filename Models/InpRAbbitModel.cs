using System;
using System.Collections.Generic;

using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace RabbitFarmLocal.Models
{
    public class InpRAbbitModel
    {
           
            [Display(Name = "Номер кролика")]
            [Required(ErrorMessage = "Введите номер кролика")]
            public int RabbitId { get; set; }
            [Display(Name = "Номер клетки")]
            [Required(ErrorMessage = "Введите номер клетки")]
            public int Cage { get; set; }

             [Display(Name = "пол")]
             public Gender RabbitGender { get; set; } = Gender.самка;

            [Display(Name = "Порода")]
            public string Breed { get; set; }
       
        

            [Display(Name = "Окрас")]
            public string Collor { get; set; }

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
            


       
    }
    public enum Gender
    {
        самец,
        самка
        
    }
    
}