using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RabbitFarmLocal.Controllers
{
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
        public static string DateToStringTan(DateTime date)
        {
            string day = (date.Day < 10 ? "0" : "") + date.Day;
            string month = (date.Month < 10 ? "0" : "") + date.Month;

            return $"{day}/{month}/{date.Year}";

        }
        public static DateTime StringToDateTan(string date)
        {
            int year = Convert.ToInt32(date.Substring(6, 4));
            int month = Convert.ToInt32(date.Substring(3, 2));
            int day = Convert.ToInt32(date.Substring(0, 2));
            DateTime dateTime = new DateTime(year, month, day);


            return dateTime;
        }
    }
}