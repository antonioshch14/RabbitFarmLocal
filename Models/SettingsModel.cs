using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using static RabbitFarmLocal.Controllers.MyFunctions;

namespace RabbitFarmLocal.Models
{
    public class SettingsModel //maleGrowDays, femaleGrowDays, pregnantDays, nestRemoalDays, putNestDays, feedDays, restDays, allParturViewPeriod, allMateViewPeriod
    {

        [DisplayName("Возраст до которого самец не допускается к случке (дней)")]
        public int MaleGrowDays { get; set; }
        [DisplayName("Возраст до которого самка не допускается к случке (дней)")]
        public int FemaleGrowDays { get; set; }
        [DisplayName("Срок в течении которого самка считается беремнной (дней)")]
        public int PregnantDays { get; set; }
        [DisplayName("Срок  по истечении которого должно быть убрано гнездо (дней)")]
        public int NestRemoalDays { get; set; }
        [DisplayName("Срок  по истечении которого после покрытия должно быть поставлено гнездо (дней)")]
        public int PutNestDays { get; set; }
        [DisplayName("Срок в течении которого самка кормит крольчат (дней)")]
        public int FeedDays { get; set; }
        [DisplayName("Срок  который дается самке для отдыха после рассадки крольчат (дней)")]
        public int RestDays { get; set; }
        [DisplayName("Период вермени за который показывать окролы на вкладке (Все) Окролы (дней)")]
        public int AllParturViewPeriod { get; set; }

        [DisplayName("Период вермени за который показывать покрытия на вкладке (Все) Покрытия (дней)")]
        public int AllMateViewPeriod { get; set; }
        [DisplayName("Период вермени в течении которого проверять окрол после истечения срока беременности")]
        public int CheckPart { get; set; }
        [DisplayName("Цена килограма мяся по умолчанию, используется в расчете, где не поставлена цена, к примеру если съели сами")]
        public int DefaultPrice { get; set; }
        [DisplayName("Дата месяца для финансового отчета (1-28)")]
        public int FinRepDate { get; set; }
        public int Id { get; set; }
        public string SaveDate {get{ return DateToString(DateTime.Now); } }

    }
}
