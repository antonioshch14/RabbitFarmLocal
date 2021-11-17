using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

using System.Web;
using static RabbitFarmLocal.Controllers.MyFunctions;
using System.ComponentModel;

namespace RabbitFarmLocal.Models
{
    public class CageModel
    {
        [DisplayName("Номер")]
        public int Id { get; set; }
        [DisplayName("Ряд")]
        public int Row { get; set; }
        [DisplayName("Уровень")]
        public int Level { get; set; }
        [DisplayName("Номер в ряду")]
        public int PositionInRow { get; set; }
        [DisplayName("Ширина")]
        public int Width { get; set; }
        [DisplayName("Глубина")]
        public int Depth { get; set; }
        [DisplayName("Высота")]
        public int Height { get; set; }
        public string LocationString { get
            {
                string r = Row switch
                {
                    1 => "Расположена в сарае слева, секция номер " + PositionInRow + " от входной двери, уровень от пола " + Level,
                    2 => "Расположена в сарае справа, секция номер " + PositionInRow + " от двери на поле, уровень от пола " + Level,
                    _ => "Расположена на улице, секция номер " + PositionInRow + " от кроличьего сарая, уровень от пола " + Level,
                };
                return r;
            } }

        [DisplayName("Сделана")]
        public string MadeString
        {
            get => DateToStringRU(Made);

        }

        [DisplayName("Сделана")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]

        public DateTime Made { get; set; }
        public string MadeStringForEdit { get { return DateToString(Made); } }
        [DisplayName("Тип")]
        public CageType Type { get; set; }
        public string Location { get => "row: " + Row + " position: " + PositionInRow + " level: " + Level;
            set {
                try
                {
                    string[] st = value.Split(' ');
                    this.Row = Int32.Parse(st[Array.IndexOf(st, "row:") + 1]);
                    this.PositionInRow = Int32.Parse(st[Array.IndexOf(st, "position:") + 1]);
                    this.Level = Int32.Parse(st[Array.IndexOf(st, "level:") + 1]);

                }
                catch (FormatException)
                {

                }

            } }
        public int Area()
        {
            return this.Width * this.Depth;
        }



    }
    public class CageAssumptions
    {
        public List<CageRow> Rows { get; set; }
        //public int[] FaremesInRow { get; set; }
        //public int[,] CagesInFrame { get; set; }

    }
    public class CageRow
    {
        public int Nr { get; set; }
        public List<CageFrame> Frames { get; set; }

    }
    public class CageFrame
    {
        public int Nr { get; set; }
        public List<CageLevel> Levels { get; set; }
    }
    public class CageLevel
    {
        public int Nr { get; set; }
        public int Id { get; set; }
        public occupancy Oc { get; set; }//occupied
        public int Rbs { get; set; }//rabbits lives there
    }
    
    public enum CageType
    {

        [Display(Name = "деревянная со сплошным полом")]
        woodenWithFlatFloor,
        [Display(Name = "деревянная с реечным полом")]
        woodenRackAndPinoinFlatFloor,
        [Display(Name = "металлическая с полом из сетки")]
        metalWithMeshFloor
    }
    public class ListOfCages
    {
        public int Id { get; set; }
        public occupancy Occupancy { get; set; }
        public string OccupancyString { get {
                DisplayAttribute Oc = GetDisplayAttributesFrom(Occupancy, typeof(occupancy));
                return Id + " " + Oc.Name; } }
        public int Livers { get; set; }
    }
    public enum occupancy
    {
        [Display(Name ="Свободна")]
        empty,
        [Display(Name = "Занята самкой")]
        occupiedFemail,
        [Display(Name = "Занята самцом")]
        occupiedMale,
        [Display(Name = "Занята откормом самками")]
        occupiedFatFemale,
        [Display(Name = "Занята откормом самцами")]
        occupiedFatMale
    }
}
