using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web;

namespace RabbitFarmLocal.Controllers
{
    public static class MyExtensions
    {
        public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> source)
        {
            return source.Select((item, index) => (item, index));
        }
    }
    public class MyFunctions
    {
       
        public static DateTime StringToDate(string date)
        {
            int year = Convert.ToInt32(date.Substring(0, 4));
            int month = Convert.ToInt32(date.Substring(5, 2));
            int day = Convert.ToInt32(date.Substring(8, 2));
            DateTime dateTime = new DateTime(year, month, day);


            return dateTime;
        }
        public static string DateToString(DateTime date)
        {
            string day = (date.Day < 10 ? "0" : "") + date.Day;
            string month = (date.Month < 10 ? "0" : "") + date.Month;

            return $"{date.Year}-{month}-{day}";

        }
        public static string DateToString(DateTime? _date)
        {
            string day;
            string month;
            DateTime date;
            if (_date == null) {
                date = DateTime.Now;
                day = (date.Day < 10 ? "0" : "") + date.Day;
                month = (date.Month < 10 ? "0" : "") + date.Month;
            }
            else
            {
                date = (DateTime)_date;
                day = (date.Day < 10 ? "0" : "") + date.Day;
                month = (date.Month < 10 ? "0" : "") + date.Month;
            }
            return $"{date.Year}-{month}-{day}";

        }
        public static string DateToStringRU(DateTime? init_date)
        {
            if (init_date == null) return "не уст.";
            else
            {
                DateTime date = (DateTime)init_date;
                string day = (date.Day < 10 ? "0" : "") + date.Day;
                var enumMonth = (Months)date.Month;
                string month = enumMonth.ToString();

                return $"{day}-{month}-{date.Year}";
            }

        }
        public static string DateToStringRU(DateTime date)
        {
                
                string day = (date.Day < 10 ? "0" : "") + date.Day;
                var enumMonth = (Months)date.Month;
                string month = enumMonth.ToString();

                return $"{day}-{month}-{date.Year}";
            

        }
        public static DateTime StringToDateTan(string date)
        {
            int year = Convert.ToInt32(date.Substring(6, 4));
            int month = Convert.ToInt32(date.Substring(3, 2));
            int day = Convert.ToInt32(date.Substring(0, 2));
            DateTime dateTime = new DateTime(year, month, day);


            return dateTime;
        }
        public enum Months
        {
            Янв=1,
            Фев,
            Мар,
            Апр,
            Май,
            Июнь,
            Июль,
            Авг,
            Сент,
            Окт,
            Нояб,
            Дек
        }
        public enum YesNo
        {   
            [Description("Нет")]
            [Display(Name = "Нет")]
            No,
            [Description("Да")]
            [Display(Name = "Да")]
            Yes
        }
           public class Age
        {
            DateTime date;
            private TimeSpan ts ;
            public double daysTot ;
            public double years ;
            public double months ;
            public double days;
            public Age(DateTime date) { //sets the age until now
                this.date = date;
                this.ts = (DateTime.Today - date);
                this.daysTot = ts.TotalDays;
                this.years = Math.Floor(this.daysTot / 365);
                this.months = Math.Floor(daysTot % 365 / 30);
                this.days = daysTot % 365 % 30;

            }
            public Age(DateTime date,DateTime until) //sets the age from  date to until
            {
                this.date = date;
                this.ts = (until-date );
                this.daysTot = ts.TotalDays;
                this.years = Math.Floor(this.daysTot / 365);
                this.months = Math.Floor(daysTot % 365 / 30);
                this.days = daysTot % 365 % 30;

            }

        }
        public static DisplayAttribute GetDisplayAttributesFrom(Enum enumValue, Type enumType)
        {
            return enumType.GetMember(enumValue.ToString())
                           .First()
                           .GetCustomAttribute<DisplayAttribute>();
        }
        public static string GetPropertyDisplayName(PropertyInfo pi)
        {
            var dp = pi.GetCustomAttributes(typeof(DisplayNameAttribute), true).Cast<DisplayNameAttribute>().SingleOrDefault();
            return dp != null ? dp.DisplayName : pi.Name;
        }
    }
    public class _Caller:I_Caller
    {
        public int Caller { get; set; }
        public Caller ECaller { get; set; }

    }
    public interface I_Caller
    {
        int Caller { get; set; }
        Caller ECaller { get; set; }

    }
    public enum Caller
    {
        rabbits,
        allmate,
        allPartur,//2
        report,
        fattening,
        mate,
        allfatt,
        fattPerStat,
        fatWeight

    }
}