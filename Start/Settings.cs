using RabbitFarmLocal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static RabbitFarmLocal.BusinessLogic.RabbitProcessor;


namespace RabbitFarmLocal.Start
{
    public class Settings
    {
        private static readonly Settings _instance = new Settings();
        private int maleGrowDays;
        private int femaleGrowDays ;
        private int pregnantDays;
        private int nestRemoalDays;
        private int putNestDays;
        private int feedDays;
        private int restDays;
        private int allParturViewPeriod;
        private int allMateViewPeriod;
        private int checkPart;
        public int defaultPrice;
        public int finRepDate;

        private Settings() {
            SettingsModel set = loadSettings();
            maleGrowDays = set.MaleGrowDays;
            femaleGrowDays = set.FemaleGrowDays;
            pregnantDays = set.PregnantDays;
             putNestDays=set.PutNestDays;
              feedDays=set.FeedDays;
            restDays=set.RestDays;
            allParturViewPeriod =set.AllParturViewPeriod;
            allMateViewPeriod = set.AllMateViewPeriod;
            nestRemoalDays = set.NestRemoalDays;
            checkPart = set.CheckPart;
            defaultPrice = set.DefaultPrice;
            finRepDate = set.FinRepDate;
        }
        public static void ReloadSettings()
        {
            SettingsModel set = loadSettings();
            _instance.maleGrowDays = set.MaleGrowDays;
            _instance.femaleGrowDays = set.FemaleGrowDays;
            _instance.pregnantDays = set.PregnantDays;
            _instance.putNestDays = set.PutNestDays;
            _instance.feedDays = set.FeedDays;
            _instance.restDays = set.RestDays;
            _instance.allParturViewPeriod = set.AllParturViewPeriod;
            _instance.allMateViewPeriod = set.AllMateViewPeriod;
            _instance.nestRemoalDays = set.NestRemoalDays;
            _instance.checkPart = set.CheckPart;
            _instance.defaultPrice = set.DefaultPrice;
            _instance.finRepDate = set.FinRepDate;
        }
        public static Settings GetSettings()
        {
            return _instance;
        }
        public static int MaleGrowDays()
        {
            return _instance.maleGrowDays;
        }
        public static int FemaleGrowDays()
        {
            return _instance.femaleGrowDays;
        }
        public static int PregnantDays()
        {
            return _instance.pregnantDays;
        }
        public static int NestRemoalDays()
        {
            return _instance.nestRemoalDays;
        }
        public static int PutNestDays()
        {
            return _instance.putNestDays;
        }
        public static int FeedDays()
        {
            return _instance.feedDays;
        }
        public static int RestDays()
        {
            return _instance.restDays;
        } 
            public static int AllParturViewPeriod()
        {
            return _instance.allParturViewPeriod;
        }
        public static int AllMateViewPeriod()
        {
            return _instance.allMateViewPeriod;
        }
        public static int CheckPart()
        {
            return _instance.checkPart;
        }
        public static int DefaultPrice()
        {
            return _instance.defaultPrice;
        }
        public static int FinRepDate()
        {
            return _instance.finRepDate;
        }
    }


}
