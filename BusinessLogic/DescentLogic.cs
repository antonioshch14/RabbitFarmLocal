using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static RabbitFarmLocal.BusinessLogic.RabbitProcessor;
using  RabbitFarmLocal.Models;

namespace RabbitFarmLocal.BusinessLogic
{
    public class RabDescent
    {
        public int RabId { get; set; }
        public RabDescent Mother { get; set; }
        public RabDescent Father { get; set; }
        public int Step { get; set; }
        public string Breed { get; set; }
    }
    public static class DescentLogic
    {
       public static RabDescent GetRabDecent(int rabId)
        {
            List<DLRabbitModel> rabbits = LoadRabbits();

            RabDescent rabDescent = new RabDescent()
            {
                RabId = rabId,
                Step=0
            };
            findParent(rabId, 0, rabDescent);
            void findParent(int _id, int _step, RabDescent parent)
            {
                int step = _step + 1;
                int foundMother = rabbits.Find(x => x.RabbitId == _id).Mother;
                int foundFather = rabbits.Find(x => x.RabbitId == _id).Father;
                DLRabbitModel mother = rabbits.Find(x => x.RabbitId == foundMother);
                DLRabbitModel father = rabbits.Find(x => x.RabbitId == foundFather);
                if (mother!= null)
                {
                    if (foundMother != 0)
                    {
                        parent.Mother = new RabDescent()
                        {
                            RabId = mother.RabbitId,
                            Step = step
                        };
                        findParent(foundMother, step, parent.Mother);
                    }
                }
                if(father!= null) {
                    if (foundFather != 0)
                    {
                        parent.Father = new RabDescent()
                        {
                            RabId = father.RabbitId,
                            Step = step
                        };
                        findParent(foundFather, step, parent.Father);
                    }
                    if (foundFather == 0 && foundMother == 0)
                    {
                        DLRabbitModel rab = rabbits.Find(x => x.RabbitId == _id);
                        rab.SetBreedStringToDisplay();
                        parent.Breed = rab.BreedString;
                    }
                }
            };
            
            return rabDescent;
        }
        public static RabDescent GetFattDecent(int motherId, int fatherId)
        {
            List<DLRabbitModel> rabbits = LoadRabbits();

            RabDescent rabDescent = new RabDescent()
            {
                RabId = 0,//replace rabId where it is called
                Step = 0,
                Father=new RabDescent() { RabId=fatherId,Step=1},
                Mother=new RabDescent() { RabId=motherId,Step=1}
            };
            findParent(motherId, 1, rabDescent.Mother);
            findParent(fatherId, 1, rabDescent.Father);
            void findParent(int _id, int _step, RabDescent parent)
            {
                int step = _step + 1;
                int foundMother = rabbits.Find(x => x.RabbitId == _id).Mother;
                int foundFather = rabbits.Find(x => x.RabbitId == _id).Father;
                DLRabbitModel mother = rabbits.Find(x => x.RabbitId == foundMother);
                DLRabbitModel father = rabbits.Find(x => x.RabbitId == foundFather);
                if (mother != null)
                {
                    if (foundMother != 0)
                    {
                        parent.Mother = new RabDescent()
                        {
                            RabId = mother.RabbitId,
                            Step = step
                        };
                        findParent(foundMother, step, parent.Mother);
                    }
                }
                if (father != null)
                {
                    if (foundFather != 0)
                    {
                        parent.Father = new RabDescent()
                        {
                            RabId = father.RabbitId,
                            Step = step
                        };
                        findParent(foundFather, step, parent.Father);
                    }
                    if (foundFather == 0 && foundMother == 0)
                    {
                        DLRabbitModel rab = rabbits.Find(x => x.RabbitId == _id);
                        rab.SetBreedStringToDisplay();
                        parent.Breed = rab.BreedString;
                    }
                }
            };
            //void findParent(int _id, int _step, RabDescent parent)
            //{
            //    int step = _step + 1;
            //    int foundMother = rabbits.Find(x => x.RabbitId == _id).Mother;
            //    int foundFather = rabbits.Find(x => x.RabbitId == _id).Father;
            //    if (foundMother != 0)
            //    {
            //        parent.Mother = new RabDescent()
            //        {
            //            RabId = rabbits.Find(x => x.RabbitId == foundMother).RabbitId,
            //            Step = step
            //        };
            //        findParent(foundMother, step, parent.Mother);
            //    }

            //    if (foundFather != 0)
            //    {
            //        parent.Father = new RabDescent()
            //        {
            //            RabId = rabbits.Find(x => x.RabbitId == foundFather).RabbitId,
            //            Step = step
            //        };
            //        findParent(foundFather, step, parent.Father);
            //    }
            //    if (foundFather == 0 && foundMother == 0)
            //    {
            //        DLRabbitModel rab = rabbits.Find(x => x.RabbitId == _id);
            //        rab.SetBreedStringToDisplay();
            //        parent.Breed = rab.BreedString;
            //    }

            //};

            return rabDescent;
        }
    }
}
