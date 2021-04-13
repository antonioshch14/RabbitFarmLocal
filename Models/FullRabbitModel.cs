using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace RabbitFarmLocal.Models
{
    public class FullRabbitModel
    {
        public int Id { get; set; }
        [DisplayName("Номер кролика")]
        public int RabbitId { get; set; }
        [DisplayName("Клетка")]
        public int Cage { get; set; }

       
        [DisplayName("Пол")]
        public string Gender { get; set; }
        [DisplayName("Порода")]

        public string Breed { get; set; }
        [DisplayName("Окрас")]
        public string Collor { get; set; }

       

        [DisplayName("Возраст")]
        public string  Age { get; set; }
        [DisplayName("Родословная")]
        public string Descent { get; set; }
        
        [DisplayName("Живой")]
        public String IsAlive { get; set; }
        [DisplayName("Коментарии")]
        public List<CommentsModel> Comments { get; set; }

        [DisplayName("Покрытия")]
        public List<MatingModel> Matting { get; set; }
        [DisplayName("Случек")]
        public int MattingCount { get { return Matting.Count; } }

        [DisplayName("Окролы")]
        public List<ParturationModel> Parturation { get; set; }
        [DisplayName("Окролов")]
        public int ParturCount { get { return Parturation.Count; } }
        [DisplayName("Всего крольчат")]
        public int ChildrenCount { get {
                int child=0;
                foreach (var part in Parturation)
                {
                    child += part.Children;
                }
                return child; } }
        [DisplayName("Всего крольчат умерло")]
        public int DiedChildrenCount
        {
            get
            {
                int child = 0;
                foreach (var part in Parturation)
                {
                    child += part.DiedChild;
                }
                return child;
            }
        }
        [DisplayName("Среднее число крольчат в окроле")]
        public int AverChildCount
        {
            get
            {
                if (ParturCount != 0)
                {

                    return ChildrenCount / ParturCount;
                }
                else return 0;
            }
        }
        
        public DescentModel DescentData { get; set; }
        public List<WeightModel> Weights { get; set; }

    }

}