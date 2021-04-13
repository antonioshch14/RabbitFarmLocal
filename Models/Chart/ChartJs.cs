using System.Collections.Generic;

namespace RabbitFarmLocal.Models.Chart
{
    public class ChartJs
    {
       
        public string type { get; set; }
        public static int duration { get; set; }
        public string easing { get; set; }
        public bool responsive { get; set; }
        public Title title { get; set; }= new Title();
        public bool lazy { get; set; }
        public Data data { get; set; }=new Data();
        public Options options { get; set; } = new Options();
    }
    public class Data
    {
        public List<string> labels { get; set; } = new List<string>();
        public List<Dataset> datasets { get; set; } = new List<Dataset>();
    }
   public class Dataset
    {
        public string label { get; set; }
        public List<string> data { get; set; } = new List<string>();
        public List<string> backgroundColor { get; set; } = new List<string>();
        public List<string> borderColor { get; set; } = new List<string>();
        public int borderWidth { get; set; }
        public string yAxisID { get; set; }
        public string xAxisID { get; set; }
    }
    public class Scales
    {
        public List<yAxes> yAxes { get; set; } = new List<yAxes>();
        public List<xAxes> xAxes { get; set; } = new List<xAxes>();
    }
    public class yAxes
    {
        public string id { get; set; }
        public bool display { get; set; }
        public string type { get; set; }
        public Ticks ticks { get; set; } = new Ticks();
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
    }
    public class Ticks
    {
        public bool beginAtZero { get; set; }
    }
    public class Title
    {
        public bool display { get; set; }
        public string text { get; set; }
    }
}