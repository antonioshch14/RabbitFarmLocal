using System.Collections.Generic;

namespace RabbitFarmLocal.Models.Chart
{
    public class ChartJs
    {
        //public ChartJs()
        //{
        //    data.datasets.Add(new Dataset());
        //    data.datasets[0].data.Add(new List<string>());
        //}
        public ChartJs(int Charts, bool isNew)
        {
            //data.datasets.Add(new Dataset());
            //if (isNew)
            //{
            //    data.datasets[0].data = new CharData[Charts];
            //} else
            //{

            //    data.datasets[0].data = new string[Charts];
            //}
        }
        public string type { get; set; }
        public static int duration { get; set; }
        public string easing { get; set; }
       
        public Data data { get; set; } = new Data();
        public Options options { get; set; } = new Options();
    }
    public class Data
    {
        public List<string> labels { get; set; } 
        public List<Dataset> datasets { get; set; } = new List<Dataset>();
    }
    public class Dataset
    {
        public string label { get; set; }
       
        public dynamic data { get; set; }
        public List<string> backgroundColor { get; set; } = new List<string>();
        public List<string> borderColor { get; set; } = new List<string>();
        public int borderWidth { get; set; }
        public string yAxisID { get; set; }
        public string xAxisID { get; set; }
        public bool spanGaps { get; set; }
        public bool fill { get; set; }
    }

    public class CharData
    {
        public float x { get; set; }
        public float y { get; set; }
    }
    public class Scales
    {
        public List<yAxes> yAxes { get; set; } = new List<yAxes>();
        public List<xAxes> xAxes { get; set; } = new List<xAxes>();
        public ScaleXY y { get; set; }
        public string grace { get; set; }
        public string type { get; set; }

    }
    public class ScaleXY
    {
        public float min { get; set; }
        public float max { get; set; }
    }
    public class yAxes
    {
        public string id { get; set; }
        public bool display { get; set; }
        public ScaleLabel scaleLabel { get; set; }
        public string type { get; set; }
        public Ticks ticks { get; set; } = new Ticks();
    }
    public class ScaleLabel
    {
        public bool display { get; set; }
    }
    public class xAxes
    {
        public string id { get; set; }
        public bool display { get; set; }
        public string type { get; set; }
        public Ticks ticks { get; set; } = new Ticks();
    }
    public class Options
    {
        public Scales scales { get; set; } = new Scales();
        public bool responsive { get; set; }
        public Display legend { get; set; }
        public Title title { get; set; }

    }
   
    public class Display
    {
        public bool display { get; set; }
    }
    public class Ticks
    {
        public bool beginAtZero { get; set; }
        public bool autoSkip { get; set; }
        public int maxTicksLimit { get; set; }
    }
    public class Title
    {
        public bool display { get; set; }
        public string text { get; set; }
       
        public string position { get; set; } = "bottom";
    }
   
}