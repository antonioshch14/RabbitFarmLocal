using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static RabbitFarmLocal.Controllers.MyFunctions;

namespace RabbitFarmLocal.Models
{
    public class DescentModel
    {
        [Display(Name = "Кролик")]
        public int Id { get; set; }
        [Display(Name = "Мать")]
        public int MotherId { get; set; }
        [Display(Name = "Отец")]
        public int FatherId { get; set; }
        [Display(Name = "Предки")]
        public List<Parents> Parents { get; set; }
    }
    public class Parents
    {
        public Parents() { }
        [Display(Name = "Степень")]
        public int Step { get; set; }
        [Display(Name = "Кролик")]
        public int Id { get; set; }
        [Display(Name = "Пол")]
        public Gender ParGender { get; set; }
        [Display(Name = "Мать")]
        public int MotherId { get; set; }
        [Display(Name = "Отец")]
        public int FatherId { get; set; }
        [Display(Name = "Линия")]
        public string Path { get; set; }

    }
    public class ParentsFull
    {
        
        [Display(Name = "Степень")]
        public int Step { get; set; }
        [Display(Name = "Кролик")]
        public int Id { get; set; }
        [Display(Name = "Мать")]
        public int MotherId { get; set; }
        [Display(Name = "Отец")]
        public int FatherId { get; set; }
        public bool IsMale { get; set; }
        public bool IsAlive { get; set; }
    }
    public class LevelOfRelations
    {

        
        [Display(Name = "Кролик")]
        public int ChildId { get; set; }
        [Display(Name = "Совпадение в поколении самки")]
        public int MatchMatherStep { get; set; }
        [Display(Name = "Линия совпадения по самке")]
        public string MotherMatchPath { get; set; }
        [Display(Name = "Совпадение в поколении самца")]
        public int MatchFatherStep { get; set; }
        [Display(Name = "Линия совпадения по самцу")]
        public string FatherMatchPath { get; set; }
        public List<Parents> Parents { get; set; }
       
    }
}
