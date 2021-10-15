using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RabbitFarmLocal.Models;
using static RabbitFarmLocal.BusinessLogic.RabbitProcessor;
using RabbitFarmLocal.Start;

namespace RabbitFarmLocal.BusinessLogic
{
    public static class CheckRabbitRelations
    {
        //public static void CheckForMails(int Id)
        //{
        //    List<ParentsFull> rabbits = LoadParents();
        //    List<ParentsFull> malesAndFem = new List<ParentsFull>();
        //    malesAndFem.Add(rabbits.Find(x => x.Id == Id));
        //    malesAndFem.AddRange(rabbits.FindAll(x => x.IsAlive == true && x.IsMale == true));
        //}
        public static List<LevelOfRelations> CheckRelations(int id)
        {
            List<ParentsFull> rabbits = LoadParents();
            List<ParentsFull> malesAndFem = new List<ParentsFull>();//all males and a females whose relations have to be checked
            malesAndFem.Add(rabbits.Find(x => x.Id == id)); //put female with the zero index
            malesAndFem.AddRange(rabbits.FindAll(x => x.IsAlive == true && x.IsMale == true));//put all alive males in

            List<LevelOfRelations> relations = new List<LevelOfRelations>();
            foreach (var male in malesAndFem)
            {
                List<Parents> par = new List<Parents>();
                LevelOfRelations desc = new LevelOfRelations
                {
                    ChildId = male.Id,
                    NameAndCage = male.Id.ToString() + "K" + male.Cage.ToString(),
                    Id=male.IdinDB
                };
                void findParent(int _id, int _step, string _path)
                {
                    int step = _step + 1;
                    
                    int foundMother = rabbits.Find(x => x.Id == _id).MotherId;
                    int foundFather = rabbits.Find(x => x.Id == _id).FatherId;
                    if (foundMother != 0)
                    {
                        string path = _path + " +" + foundMother.ToString();
                        par.Add(new Parents
                        {
                            MotherId = rabbits.Find(x => x.Id == foundMother).MotherId,
                            FatherId = rabbits.Find(x => x.Id == foundMother).FatherId,
                            Step = step,
                            Id = foundMother,
                            ParGender = Gender.самка,
                            Path=path
                        }); 
                        findParent(foundMother, step, path);
                    }
                    if (foundFather != 0)
                    {
                        string path = _path + " ^" + foundFather.ToString();
                        par.Add(new Parents
                        {
                            MotherId = rabbits.Find(x => x.Id == foundFather).MotherId,
                            FatherId = rabbits.Find(x => x.Id == foundFather).FatherId,
                            Step = step,
                            Id = foundFather,
                            ParGender=Gender.самец,
                            Path = path
                        });
                        findParent(foundFather, step, path);
                    }
                };
                findParent(male.Id, 0, male.Id.ToString() + "=");
                desc.Parents = par.OrderBy(x => x.Step).ToList();
                relations.Add(desc);
            }
            foreach(var fp in relations[0].Parents)// parents of female whose relations are checked
            {
                foreach (var m in relations)//all males and 0 element is the female
                {
                    if (m.ChildId == id) continue;// scip female
                    if (fp.Id == m.ChildId)
                    {   
                        m.MatchMatherStep = fp.Step;
                        m.MotherMatchPath = fp.Path + ";  \n\r"; ;
                        m.MatchFatherStep = 1;
                        m.FatherMatchPath = m.ChildId.ToString() + ";  \n\r"; 
                        continue;
                    }
                    foreach (var p in m.Parents)
                    {
                        if (fp.Id == p.Id)
                        {
                            m.MatchFatherStep = p.Step;
                            m.FatherMatchPath += p.Path + ";  \n\r";
                            m.MatchMatherStep = fp.Step;
                            m.MotherMatchPath += fp.Path + ";  \n\r";
                        }
                    }
                }
            }
            
            return relations;
        }
    }
}
