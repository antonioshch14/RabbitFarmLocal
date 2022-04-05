using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using static RabbitFarmLocal.Controllers.MyFunctions;
using RabbitFarmLocal.Start;

namespace RabbitFarmLocal.Models
{
    public class ReportModel //TotalRabits Females GrowFemales Males GrowMales PregnantFemales FeedFemales RestFemales PutNest RemoveNest Separate Mate
    {
        [DisplayName("Всего откорм")]
        public int Fattening { get; set; }

        [DisplayName("Всего кроликов")]
        public int TotalRabits { get; set; }
        [DisplayName("Всего самок")]
        public int Females { get; set; }

        [DisplayName("Ремонтантных самок")]
        public int GrowFemales { get; set; }
        [DisplayName("Всего самцов")]
        public int Males { get; set; }

        [DisplayName("Ремонтантных самцов")]
        public int GrowMales { get; set; }
        [DisplayName("Покрытых самок")]
        public int PregnantFemales { get; set; }
        [DisplayName("Кормящих самок")]
        public int FeedFemales { get; set; }
        [DisplayName("Отдыхающих самок")]
        public int RestFemales { get; set; }
        [DisplayName("Готовых самок")]
        public int ReadyFemales { get; set; }
        [DisplayName("Поставить гнездо")]
        public List<PutNest> PutNest { get; set; } = new List<PutNest>();
        [DisplayName("Убрать гнездо")]
        public List<RemoveNest> RemoveNest { get; set; } = new List<RemoveNest>();
        [DisplayName("Покрыть")]
        public List<Mate> Mate { get; set; } = new List<Mate>();
        [DisplayName("Рассадить")]
        public List<Separate> Separate { get; set; } = new List<Separate>();
        [DisplayName("Проверить окрол")]
        public List<CheckPart> CheckPart { get; set; } = new List<CheckPart>();

    }
    public class PutNest : StatusCehck { };
    public class RemoveNest : StatusCehck { };

    public class Mate : StatusCehck { };
    public class Separate : StatusCehck { };
    public class CheckPart : StatusCehck {
        [DisplayName("Мать")]
        public int MotherId { get; set; }
        [DisplayName("Отец")]
        public int FatherId { get; set; }
        public int MateId { get; set; }
        //public DateTime MateDate
        //{
        //    get { Date.AddDays(-Settings.PregnantDays());

        //    }
        //}
    }

    public class StatusCehck
    {
        public int Id { get; set; }
        [DisplayName("Кролик")]
        public int RabbitId { get; set; }
        [DisplayName("Клетка")]
        public int Cage { get; set; }
        public DateTime Date {get;set;}
        public bool Alert { get
            {
                if (Date <= DateTime.Now)
                {
                    TimeSpan ts = DateTime.Now - Date;
                    DaysOverdue = ts.Days;
                    return true;
                }
                else return false;
            } }
        [DisplayName("просрочено дней: ")]
        public int DaysOverdue { get; set; }
        [DisplayName("Дата")]
        public string DateString
        {
            get
            {
                return DateToStringRU(Date);
            }
        }
    }
    
    

}
