using DataLibrary;
using RabbitFarmLocal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static RabbitFarmLocal.Controllers.MyFunctions;


namespace RabbitFarmLocal.BusinessLogic
{
    public static class RabbitProcessor
    {
        public static int CreateRabbit(int rabbitId, int cage, string breed, string collor,
            DateTime born, int mother, int father, bool isAlive, Gender gender, int? partId,int? rabPartId)
        {
            DLRabbitModel data = new DLRabbitModel
            {
                RabbitId = rabbitId,
                Mother = mother,
                Father = father,
                IsAlive = isAlive,
                Cage = cage,
                Born = born,
                Breed = breed,
                Collor = collor,
                PartId=partId,
                PartRabId=rabPartId

            };
            if (gender == Gender.самка) data.IsMale = false;
            else data.IsMale = true;

            /*[Id],[rabId],[cage],[breed],[collor],[born],[mother],[father],[isMale],[isAlive] FROM [dbo].[rabbits2]*/
            /* Id, RabbitId, Cage, IsMale, Breed, Collor, Born, Mother, Father, IsAlive, */

            string sql = @"insert into rabbits (rabId,cage,breed,collor,born,mother,father,isMale,isAlive,part_id,part_rab_id) 
                            values(@RabbitId,@Cage,@Breed,@Collor,@Born,@Mother,@Father,@IsMale,@IsAlive,@PartId,@PartRabId)";
            return DataAccess.SaveDataRabbit(sql, data);
        }
        public static int EditRabbit(DLRabbitModel rab)
        {
           

            string sql = @"UPDATE rabbits SET cage=@Cage,breed=@Breed,collor=@Collor,born=@Born,mother=@Mother,father=@Father,
                        isMale=@IsMale,isAlive=@IsAlive,breed_id=@BreedId  WHERE rabId=@RabbitId;";
            return DataAccess.SaveDataRabbit(sql, rab);
        }
        public static List<DLRabbitModel> LoadRabbits()
        {
            string sql = @"select Id as Id, rabId as RabbitId, cage as Cage, breed as Breed, collor as Collor,
                         mother as Mother, father as Father, isMale as IsMale, isAlive as IsAlive, born as Born, 
                        status as StoredRabStatus, part_id as PartId, part_rab_id as PartRabId, breed_id as BreedId from rabbits ORDER BY isAlive DESC, 
                        rabId ASC;";
            return DataAccess.LoadDataRabbit<DLRabbitModel>(sql);
        }
        public static List<DLRabbitModel> LoadRabbitsAlive()
        {
            string sql = @"select Id as Id, rabId as RabbitId, cage as Cage, breed as Breed, collor as Collor,
                         mother as Mother, father as Father, isMale as IsMale, isAlive as IsAlive, born as Born, 
                        status as StoredRabStatus, part_id as PartId, part_rab_id as PartRabId from rabbits WHERE isAlive=1 ORDER BY rabId ASC;";
            return DataAccess.LoadDataRabbit<DLRabbitModel>(sql);
        }
        public static class Rabbit
        {
            static readonly string tbl = "rabbits";
            static readonly string[] t = new string[] { "Id","rabId", "cage", "breed", "collor", "born", "mother", "father", "isMale", "isAlive", "status","breed_id"};//0-11
            static readonly string[] t2 = new string[] { "termDate", "price", "killWeight" };
            static readonly string[] m = new string[] { "Id", "RabbitId", "Cage",  "Breed", "Collor",  "Born", "Mother", "Father", "IsMale", "IsAlive", "StoredRabStatus", "BreedId" };//0-11
            static readonly string[] m2 = new string[] { "TermDate", "Price", "Weight" };
            //public static int Create(DLRabbitModel wgt)
            //{
            //    string sql = string.Format(@"insert into {0} ({1}, {2}, {3},{7}) values(@{4}, @{5}, @{6},@{8})",
            //        tbl, t[0], t[1], t[2], m[0], m[1], m[2], t[3], m[3]);
            //    return DataAccess.SaveDataRabbit<DLRabbitModel>(sql, wgt);
            //}
            public static int EditGeneral(DLRabbitModel rab)
            {
                string sql = string.Format(@"UPDATE {0} SET {1}=@{2}, {3}=@{4}, {5}=@{6}, {7}=@{8}, {9}=@{10}, {11}=@{12}, {13}=@{14}, {15}=@{16}, {17}=@{18}, {21}=@{22} WHERE {19} =@{20};",
                    tbl, t[2], m[2], t[3], m[3], t[4], m[4],t[5],m[5],t[6],m[6],t[7],m[7],t[8],m[8],t[9],m[9],t[10],m[10], t[0], m[0],t[11],m[11]);
                return DataAccess.SaveDataRabbit<DLRabbitModel>(sql, rab);
            }
            public static int EditKill(DLRabbitModel rab)
            {
                string sql = string.Format(@"UPDATE {0} SET {1}=@{2}, {3}=@{4}, {5}=@{6}, {7}=@{8}, {9}=@{10} WHERE {11} = @{12};",
                    tbl, t2[0], m2[0], t2[1], m2[1], t2[2], m2[2], t[10], m[10], t[9], m[9], t[0], m[0]);
                return DataAccess.SaveDataRabbit<DLRabbitModel>(sql, rab);
            }
            //public static int Delete(int wgtId)
            //{

            //    string sql = String.Format("DELETE FROM {0} WHERE {1}={2};", tbl, t[4], wgtId);
            //    return DataAccess.SaveData(sql);
            //}
            public static List<DLRabbitModel> LoadRabPerStatus(string from, string until, params int[] stat)
            {
                System.Text.StringBuilder stSQL = new System.Text.StringBuilder();
                for (int i = 0; i < stat.Length; i++)
                {
                    if (i > 0) { stSQL.Append(","); }
                    stSQL.Append(stat[i]);
                }
                string sql = string.Format(@"select {1} as {2}, {3} as {4}, {5} as {6}, {7} as {8}, {9} as {10}, {11} as {12}, {13} as {14}, {15} as {16}, {17} as {18}, {19} as {20}, {21} as {22}, {23} as {24}, {25} as {26}, {27} as {28}, {29} as {30}" +
                                   " FROM {0} WHERE {21} in (" + stSQL + ") AND {23} >= '" + from + "' AND {23} <= '" + until + "';", tbl, t[0], m[0], t[1], m[1], t[2], m[2], t[3], m[3], t[4], m[4], t[5], m[5], t[6], m[6], t[7], m[7], t[8], m[8], t[9], m[9], t[10], m[10], t2[0], m2[0], t2[1], m2[1], t2[2], m2[2], t[0],m[0]);//30
                return DataAccess.LoadDataRabbit<DLRabbitModel>(sql);
            }
            public static DLRabbitModel LoadOne(int id)
            {
                string sql = string.Format(@"select {1} as {2}, {3} as {4}, {5} as {6}, {7} as {8}, {9} as {10}, {11} as {12}, {13} as {14}, {15} as {16}, {17} as {18}, {19} as {20}, {21} as {22}, {23} as {24}, {25} as {26}, {27} as {28}, {29} as {30}, {32} as {33}" +
                                   " FROM {0} WHERE {1} ={31};", tbl, t[0], m[0], t[1], m[1], t[2], m[2], t[3], m[3], t[4], m[4], t[5], m[5], t[6], m[6], t[7], m[7], t[8], m[8], t[9], m[9], t[10], m[10], t2[0], m2[0], t2[1], m2[1], t2[2], m2[2],t[0],m[0],id, t[11], m[11]);//33
                return DataAccess.LoadDataOneLine<DLRabbitModel>(sql);
            }
            public static List<DLRabbitModel> LoadList()
            {
                string sql = string.Format(@"select {1} as {2}, {3} as {4}, {5} as {6}, {7} as {8}, {9} as {10}, {11} as {12}, {13} as {14}, {15} as {16}, {17} as {18}, {19} as {20}, {21} as {22}, {23} as {24}, {25} as {26}, {27} as {28}, {29} as {30}" +
                                   " FROM {0} ORDER BY isAlive DESC, rabId ASC;", tbl, t[0], m[0], t[1], m[1], t[2], m[2], t[3], m[3], t[4], m[4], t[5], m[5], t[6], m[6], t[7], m[7], t[8], m[8], t[9], m[9], t[10], m[10], t2[0], m2[0], t2[1], m2[1], t2[2], m2[2], t[0], m[0]);//31
                return DataAccess.LoadDataRabbit<DLRabbitModel>(sql);
            }
            public static List<RabbitModelDelete> LoadRabIds (int id)
            {
                string sql = string.Format(@"select {1} as {2} FROM {0} ORDER BY {1} ASC", tbl, t[1], m[1]);
                return DataAccess.LoadDataRabbit<RabbitModelDelete>(sql);
            }

        }
        public static int deteRabbitFromBD(int rabbitId)
        {
            RabbitModelDelete data = new RabbitModelDelete
            {
                RabbitId = rabbitId
                //DELETE FROM table_name WHERE condition
            };
            string sql = @"CALL erase_rabbit (@RabbitId);"; //erase_rabbit
            return DataAccess.SaveDataRabbit(sql, data);
        }
        public static List<CommentsModel> LoadComments(int Id)
        {
            string sql = "select Id as Id, date as Date, comment as Comment from rabbit_comment WHERE rabbid_id=" + Id + " ORDER BY date;";
            return DataAccess.LoadDataRabbit<CommentsModel>(sql);
        }
        public static int SaveComment(int Id, string comment, DateTime date)
        {
            CommentsModel data = new CommentsModel
            {
                RabbitId = Id,
                Comment = comment,
                Date = date

            };

            string sql = @"insert into rabbit_comment (rabbid_id,date,comment) 
                            values(@RabbitId,@Date,@Comment)";
            return DataAccess.SaveDataRabbit(sql, data);
        }
        public static int DeleteComment(int id)
        {
            CommentsModel data = new CommentsModel
            {
                Id = id

            };
            string sql = @"DELETE FROM rabbit_comment WHERE Id=@Id;";
            return DataAccess.SaveDataRabbit(sql, data);
        }

        public static int DeleteMateFromSql(int id)
        {

            string sql = @"DELETE FROM mating WHERE id=" + id + ";";
            return DataAccess.SaveData(sql);
        }
    
        public static int DeleteParturationFromSql(int id)
        {

            string sql = @"DELETE FROM parturation WHERE Id=" + id + ";";
            return DataAccess.SaveData(sql);
        }
        public static int EditComment(int comId, int rabId, string comment, DateTime date)
        {
            CommentsModel data = new CommentsModel
            {
                Id = comId,
                RabbitId = rabId,
                Comment = comment,
                Date = date
            };

            string sql = @"UPDATE rabbit_comment SET rabbid_id=@RabbitId,date=@Date,comment=@Comment  WHERE Id=@Id;";
            return DataAccess.SaveDataRabbit(sql, data);
        }
        public static List<MatingModel> LoadMating(int Id)
        {
            string sql = "select Id as Id, father_id as FatherId,mother_id as MotherId, dateOfMate " +
                "as Date, parturId as ParturationId, putNestDate as PutNest from mating WHERE father_id=" + Id + " OR mother_id=" + Id + " ORDER BY dateOfMate DESC;";
            return DataAccess.LoadDataRabbit<MatingModel>(sql);
        }
        public static MatingModel LoadMate(int Id)
        {
            string sql = "select Id as Id, father_id as FatherId,mother_id as MotherId, dateOfMate " +
                "as Date, parturId as ParturationId, putNestDate as PutNest from mating WHERE Id=" + Id + ";";
            return DataAccess.LoadDataOneLine<MatingModel>(sql);
        }
        public static int SaveMating(int matherId, int fatherId, DateTime date)
        {
            MatingModel data = new MatingModel
            {
                MotherId = matherId,
                FatherId = fatherId,
                Date = date

            };

            string sql = @"insert into mating (father_id,mother_id,dateOfMate) 
                            values(@FatherId,@MotherId,@Date)";
            return DataAccess.SaveDataRabbit(sql, data);
        }
        //public static int DeleteMating(int id)
        //{
        //    CommentsModel data = new CommentsModel
        //    {
        //        Id = id

        //    };
        //    string sql = @"DELETE FROM rabbit_comment WHERE Id=@Id;";
        //    return DataAccess.SaveDataRabbit(sql, data);
        //}
        public static int EditMating(MatingModel mat)
        {
            

            string sql = @"UPDATE mating SET father_id=@FatherId,dateOfMate=@Date,mother_id=@MotherId, putNestDate=@PutNest, parturId=@ParturationId  WHERE id=@Id;";
            return DataAccess.SaveDataRabbit(sql, mat);
        }

        public static int MarkMateAsFail(int id)
        {
            MatingModel data = new MatingModel
            { };

            string sql = @"UPDATE mating SET parturId=-1  WHERE id=" + id + ";";
            return DataAccess.SaveDataRabbit(sql, data);
        }
       
        public static int PutNestSaveToDB(MatingModel mat)
        {
            string sql = @"UPDATE mating SET putNestDate=@PutNest  WHERE id=" + mat.Id + ";";
            return DataAccess.SaveDataRabbit(sql, mat);
        }
        public static int PutRabToArchive(int id)
        {
            DLRabbitModel data = new DLRabbitModel
            {
                RabbitId = id,
                IsAlive = false
            };

            string sql = @"UPDATE rabbits SET isAlive=@IsAlive  WHERE rabId=@RabbitId;";
            return DataAccess.SaveDataRabbit(sql, data);

        }
        //SQL Id,birth_date,mather,children,male,female,died_children,separation_date,cage,comment,nest_removal,mateId  FROM[dbo].[parturation]
        // MODEL Id Date  MotherId  Children  Males  Females DiedChild  SeparationDate  Cage Comment DateNestRemoval  MateId 
        public static List<ParturationModel> LoadParturations(int rabbitId)
        {
            string sql = "select p.Id as Id, p.birth_date as Date,  p.mather as MotherId,p.children as Children,p.male as Males,p.female as Females,p.died_children as DiedChild," +
                "p.separation_date as SeparationDate,r.cage as Cage,p.comment,p.nest_removal as DateNestRemoval,p.mateId as MateId, m.father_id as FatherId, m.dateOfMate as MateDate" +
                " from parturation p LEFT JOIN mating m ON p.mateId=m.id LEFT JOIN rabbits r ON p.mather=r.rabId WHERE p.mather=" + rabbitId + " OR m.father_id=" + rabbitId + " ORDER BY p.birth_date DESC;";
            return DataAccess.LoadDataRabbit<ParturationModel>(sql);
        }
        public static int SaveParturation(DateTime date, int motherId, int children, int males, int females, int diedChild, DateTime? separationDate, string comment, DateTime? dateNestRemoval, int mateId)
        {
            ParturationModel data = new ParturationModel
            {
                Date = date,
                MotherId = motherId,
                Children = children,
                Males = males,
                Females = females,
                DiedChild = diedChild,
                SeparationDate = separationDate,
                
                Comment = comment,
                DateNestRemoval = dateNestRemoval,
                MateId = mateId

            };

            //string sql = @"insert into parturation (birth_date,mather,children,male,female,died_children,separation_date,cage,comment,nest_removal,mateId) 
            //                values(@Date, @MotherId,  @Children,  @Males,  @Females, @DiedChild,  @SeparationDate,  @Cage, @Comment, @DateNestRemoval,  @MateId)";

            string sql = @"CALL storeNewParturation (@Date, @MotherId,  @Children,  @Males,  @Females, @DiedChild,  
                @SeparationDate,  @Cage, @Comment, @DateNestRemoval,  @MateId)";


            return DataAccess.SaveDataRabbit(sql, data);
        }
       
        public static int RemoveNestSaveToDB(ParturationModel prt)
        {
            string sql = @"UPDATE parturation SET nest_removal=@DateNestRemoval  WHERE Id=" + prt.Id + ";";
            return DataAccess.SaveDataRabbit(sql, prt);
        }
        public static int EditParturation(int id, DateTime date, int children, int males, int females, int diedChild, DateTime? separationDate, int cage, string comment, DateTime? dateNestRemoval)
        {
            ParturationModel data = new ParturationModel
            {
                Id = id,
                Date = date,
               
                Children = children,
                Males = males,
                Females = females,
                DiedChild = diedChild,
                SeparationDate = separationDate,
                Cage = cage,
                Comment = comment,
                DateNestRemoval = dateNestRemoval
            };

            string sql = @"UPDATE parturation SET birth_date= @Date,children=@Children,male=@Males,female=@Females,died_children=@DiedChild,separation_date=@SeparationDate,
            cage=@Cage,comment=@Comment,nest_removal=@DateNestRemoval WHERE Id=@Id;";
            return DataAccess.SaveDataRabbit(sql, data);
        }
        public static int EditParturationChild (ParturationModel prt)
        {
            string sql = @"UPDATE parturation SET children=@Children,male=@Males,female=@Females,died_children=@DiedChild WHERE Id=@Id;";
            return DataAccess.SaveDataRabbit(sql, prt);
        }
        public static List<MatingModel> LoadMating(DateTime from)
        {
            string sql = "select m.Id as Id, m.father_id as FatherId,m.mother_id as MotherId, m.dateOfMate " +
                "as Date, m.parturId as ParturationId, m.putNestDate as PutNest, r.cage as Cage from mating m LEFT JOIN rabbits r ON m.mother_id=r.rabId WHERE m.dateOfMate > '" + DateToString(from) + "' ORDER BY m.dateOfMate DESC;";
            return DataAccess.LoadDataRabbit<MatingModel>(sql);
        }
        public static List<ParturationModel> LoadParturation(DateTime from)
        {
            string sql = "select p.Id as Id, p.birth_date as Date,  p.mather as MotherId,p.children as Children,p.male as Males,p.female as Females,p.died_children as DiedChild," +
                "p.separation_date as SeparationDate,r.cage as Cage,p.comment,p.nest_removal as DateNestRemoval,p.mateId as MateId, m.father_id as FatherId, m.dateOfMate as MateDate" +
                " from parturation p LEFT JOIN mating m ON p.mateId=m.id LEFT JOIN rabbits r ON p.mather=r.rabId WHERE p.birth_date >'" + DateToString(from) + "' ORDER BY p.birth_date DESC;";
            return DataAccess.LoadDataRabbit<ParturationModel>(sql);
        }
        public static List<MatingModel> LoadLatestMatings(DateTime from)
        {
            string sql = "select Id as Id, father_id as FatherId,mother_id as MotherId, max(dateOfMate) " +
                "as Date, parturId as ParturationId, putNestDate as PutNest from mating WHERE dateOfMate > '" + DateToString(from) + "' GROUP BY mother_id;";
            return DataAccess.LoadDataRabbit<MatingModel>(sql);
        }
        public static List<ParturationModel> LoadLatestParturation(DateTime from)
        {
            string sql = "select p.Id as Id, max(p.birth_date) as Date,  p.mather as MotherId,p.children as Children,p.male as Males,p.female as Females,p.died_children as DiedChild," +
                "p.separation_date as SeparationDate,p.cage as Cage,p.comment,p.nest_removal as DateNestRemoval,p.mateId as MateId, m.father_id as FatherId, m.dateOfMate as MateDate" +
                " from parturation p LEFT JOIN mating m ON p.mateId=m.id WHERE p.birth_date >'" + DateToString(from) + "' GROUP BY p.birth_date;";
            return DataAccess.LoadDataRabbit<ParturationModel>(sql);
        }
        //UpdateSqlRabbitStatus
        public static int UpdateSqlRabbitStatus(string data) //UPDATE rabbits SET cage=@Cage ,breed=@Breed,collor=@Collor,born=@Born,mother=@Mother,father=@Father,
                                                             //isMale=@IsMale,isAlive=@IsAlive WHERE rabId=@RabbitId;"
        {
            //string sql = $"INSERT INTO rabbits (Id,StoredRabStatus) VALUES {data} ON DUPLICATE KEY UPDATE status=VALUES(status);";
            return DataAccess.SaveData(data);
        }
        public static SettingsModel loadSettings()
        {

            string sql = "select def_price as DefaultPrice, maleGrowDays as MaleGrowDays, femaleGrowDays as FemaleGrowDays, pregnantDays as PregnantDays, nestRemoalDays as NestRemoalDays, putNestDays as PutNestDays, " +
                            "feedDays as FeedDays, restDays as RestDays, allParturViewPeriod as AllParturViewPeriod, allMateViewPeriod as AllMateViewPeriod, checkPart as CheckPart, finrep_date as FinRepDate from settings ORDER BY Id DESC LIMIT 1"; //WHERE Id=0
            return DataAccess.LoadDataOneLine<SettingsModel>(sql);
        }
        public static int editSettings(SettingsModel set)
        {
            string sql = "INSERT INTO  settings (def_price, maleGrowDays, femaleGrowDays, pregnantDays, nestRemoalDays, putNestDays, feedDays, restDays, allParturViewPeriod, allMateViewPeriod, saveDate, checkPart, finrep_date) " +
                            " VALUES (@DefaultPrice,  @MaleGrowDays, @FemaleGrowDays, @PregnantDays, @NestRemoalDays, @PutNestDays, @FeedDays, @RestDays, @AllParturViewPeriod, @AllMateViewPeriod, @SaveDate, @CheckPart, @FinRepDate)";
            return DataAccess.SaveDataRabbit<SettingsModel>(sql, set);
        }
        public static DescentModel LoadDescent(int id)
        {
            string sql = @"select rabId as Id, mother as MotherId, father as FatherId from rabbits;";
            List<Parents> rabbits = DataAccess.LoadDataRabbit<Parents>(sql);

            List<Parents> par = new List<Parents>();
            DescentModel desc = new DescentModel
            {
                Id = id,
                MotherId = rabbits.FirstOrDefault(x => x.Id == id).MotherId,
                FatherId = rabbits.FirstOrDefault(x => x.Id == id).FatherId
            };
            void findParent(int _id, int _step, Parents parents)
            {
                int step = _step + 1;
                int foundMother = rabbits.Find(x => x.Id == _id).MotherId;
                int foundFather = rabbits.Find(x => x.Id == _id).FatherId;
                if (foundMother != 0)
                {
                    par.Add(new Parents
                    {
                        MotherId = rabbits.Find(x => x.Id == foundMother).MotherId,
                        FatherId = rabbits.Find(x => x.Id == foundMother).FatherId,
                        Step = step,
                        Id = foundMother
                    });
                    findParent(foundMother, step,par[par.Count()-1]);
                }
                else
                {
                    parents.BeginnerOfLine = true;
                }
                if (foundFather != 0)
                {
                    par.Add(new Parents
                    {
                        MotherId = rabbits.Find(x => x.Id == foundFather).MotherId,
                        FatherId = rabbits.Find(x => x.Id == foundFather).FatherId,
                        Step = step,
                        Id = foundFather
                    });
                    findParent(foundFather, step, par[par.Count() - 1]);
                }
                else
                {
                    parents.BeginnerOfLine = true;
                }
            };
            par.Add(new Parents());//add one element for defining as a begginer of a line if no parents are found
            findParent(id, 0, par[par.Count() - 1]);
            if (par.Count() > 1) par.RemoveAt(0);//remove empty parrent element if it is not a begginer of a line
            desc.Parents = par.OrderBy(x => x.Step).ToList();
            return desc;
        }
        public static List<ParentsFull> LoadParents()
        {
            string sql = @"select Id as IdinDB, rabId as Id, mother as MotherId, father as FatherId, isAlive as IsAlive, isMale as IsMale, cage as Cage from rabbits;";
            List<ParentsFull> rabbits = DataAccess.LoadDataRabbit<ParentsFull>(sql);
            return rabbits;
        }
        public static ParturationModel LoadParturation(int partId)
        {
            string sql = "select p.Id as Id, p.birth_date as Date,  p.mather as MotherId,p.children as Children,p.male as Males,p.female as Females,p.died_children as DiedChild," +
                "p.separation_date as SeparationDate,p.cage as Cage,p.comment,p.nest_removal as DateNestRemoval,p.mateId as MateId, m.father_id as FatherId, m.dateOfMate as MateDate" +
                " from parturation p LEFT JOIN mating m ON p.mateId=m.id WHERE p.Id=" + partId + ";";
            return DataAccess.LoadDataOneLine<ParturationModel>(sql);
        }
        //PartId, RabPartId, Cage, Collor, DeadItself, RabbitGender, KillDate, Weight
        public static int CreateFatting (List<FatteningModel> fatt)
        {
            string sqlPart = String.Format("UPDATE parturation SET separation_date='{0}' WHERE Id={1}", DateToString(DateTime.Now), fatt[0].PartId);
            DataAccess.SaveData(sqlPart);
            string sql= @"insert into fattening (partId, rabPartId, cage, collor, rabbitGender, status, breed) 
                            values(@PartId, @RabPartId, @Cage, @Collor, @RabbitGender, @Status, @Breed)";
            return DataAccess.SaveDataRabbit<FatteningModel>(sql, fatt);
        }
        public static List<FatteningModel> LoadFattenigPerPart(int part_Id)
        {
            string sql = @"SELECT f.partId as PartId, f.rabPartId as RabPartId, f.cage as Cage, f.collor as Collor, f.status as Status, f.rabbitGender as RabbitGender, " +
                "f.weight as Weight, f.comment as Comment, f.breed as Breed,  p.birth_date as Born, m.mother_id as MotherId, m.Father_id as FatherId, " +
                "f.killDate as KillDate, f.price as Price" +
                " FROM fattening f LEFT JOIN parturation p ON f.partId=p.Id LEFT JOIN mating m ON p.mateId=m.id WHERE f.partId = " + part_Id + ";";

            //@"SELECT partId as PartId, rabPartId as RabPartId, cage as Cage, collor as Collor, status as Status, rabbitGender as RabbitGender, killDate as KillDate," +
            //    "weight as Weight, price as Price, comment as Comment, breed as Breed FROM fattening WHERE partId = " + part_Id + ";";                             
            return DataAccess.LoadDataRabbit<FatteningModel>(sql);
        }

        public static List<FatteningModel> LoadFattenigPerStatus(string from, string until, params int[] stat)
        {
            System.Text.StringBuilder stSQL = new System.Text.StringBuilder();
            for (int i = 0; i < stat.Length; i++)
            {
                if (i > 0) { stSQL.Append(","); }
                stSQL.Append(stat[i]);
            }
            //string dateFrom = DateToString(until.AddDays(-period));
            string sql = @"SELECT f.partId as PartId, f.rabPartId as RabPartId, f.cage as Cage, f.collor as Collor, f.status as Status, f.rabbitGender as RabbitGender, " +
                "f.weight as Weight, f.price as Price, f.comment as Comment, f.breed as Breed, p.birth_date as Born, f.killDate as KillDate, b.weight as LastWeight, m.mother_id as MotherId, m.Father_id as FatherId  " +
                "FROM fattening f LEFT JOIN parturation p ON f.partId=p.Id " +
                "LEFT JOIN (SELECT b1.*  FROM fat_weight b1 JOIN (SELECT rabId, partId, MAX(date) As maxDate FROM fat_weight GROUP BY rabId, partId ) b2 " +
                "ON b1.rabId = b2.rabId AND b1.partId= b2.partId AND b1.date = b2.maxDate) b ON f.partId = b.partId AND f.rabPartId = b.rabId " +
                "LEFT JOIN mating m ON p.mateId=m.id " +
                " WHERE f.status in (" + stSQL + ") AND p.birth_date >= '" + from + "' AND p.birth_date <= '" + until + "';";
            return DataAccess.LoadDataRabbit<FatteningModel>(sql);
        }
        public static List<FatteningModel> LoadFattenigPerStatusKilled(string from, string until, params int[] stat)
        {
            System.Text.StringBuilder stSQL = new System.Text.StringBuilder();
            for (int i = 0; i < stat.Length; i++)
            {
                if (i > 0) { stSQL.Append(","); }
                stSQL.Append(stat[i]);

            }
            //string dateFrom = DateToString(until.AddDays(-period));
            string sql = @"SELECT f.partId as PartId, f.rabPartId as RabPartId, f.cage as Cage, f.collor as Collor, f.status as Status, f.rabbitGender as RabbitGender, " +
                "f.weight as Weight, f.price as Price, f.comment as Comment, p.birth_date as Born, f.killDate as KillDate, f.breed as Breed FROM fattening f LEFT JOIN parturation p ON f.partId=p.Id WHERE f.status in (" + stSQL + ") AND f.killDate >= '" + from + "' AND f.killDate <= '" + until + "';";
            return DataAccess.LoadDataRabbit<FatteningModel>(sql);
        }
        public static int EditFattenigPerPart (List<FatteningModel> fatt)
        {
            string sql = @"UPDATE fattening SET cage=@Cage, collor=@Collor, status=@Status, rabbitGender=@RabbitGender, killDate=@KillDate," +
                "weight=@Weight, price=@Price, comment=@Comment WHERE partId = @PartId AND rabPartId=@RabPartId;"; 
            return DataAccess.SaveDataRabbit<FatteningModel>(sql,fatt);
        }
        public static int EditFattenigStatus(FatteningModel fatt)
        {
            string sql = @"UPDATE fattening SET  status=@Status WHERE partId = @PartId AND rabPartId=@RabPartId;";
            return DataAccess.SaveDataRabbit<FatteningModel>(sql, fatt);
        }
        public static int EditFattenigBreed (FatteningModel fatt)
        {
            string sql = @"UPDATE fattening SET  breed=@Breed WHERE partId = @PartId AND rabPartId=@RabPartId;";
            return DataAccess.SaveDataRabbit<FatteningModel>(sql, fatt);
        }
        public static List<FatteningModel> LoadAllFattening()
        {
            string sql = @"SELECT f.partId as PartId, f.rabPartId as RabPartId, f.cage as Cage, f.collor as Collor, f.status as Status, f.rabbitGender as RabbitGender, " +
                "f.weight as Weight, f.comment as Comment, f.breed as Breed,  p.birth_date as Born, m.mother_id as MotherId, m.Father_id as FatherId " +
                " FROM fattening f LEFT JOIN parturation p ON f.partId=p.Id LEFT JOIN mating m ON p.mateId=m.id";
            return DataAccess.LoadDataRabbit<FatteningModel>(sql);
        }
        public static List<FatteningModel> LoadFattenigAllAlive()
        {
            string sql = @"SELECT f.partId as PartId, f.rabPartId as RabPartId, f.cage as Cage, f.collor as Collor, f.status as Status, f.rabbitGender as RabbitGender, " +
                "f.weight as Weight, f.comment as Comment, f.breed as Breed,  p.birth_date as Born, b.weight as LastWeight, b.date as WeightDate, m.mother_id as MotherId, m.Father_id as FatherId " +
                " FROM fattening f LEFT JOIN parturation p ON f.partId=p.Id " +
                "LEFT JOIN (SELECT b1.*  FROM fat_weight b1 JOIN (SELECT rabId, partId, MAX(date) As maxDate FROM fat_weight GROUP BY rabId, partId ) b2 "+
                "ON b1.rabId = b2.rabId AND b1.partId= b2.partId AND b1.date = b2.maxDate) b ON f.partId = b.partId AND f.rabPartId = b.rabId "+
                "LEFT JOIN mating m ON p.mateId=m.id " +
                "WHERE f.status < 2 OR f.status=8  ORDER BY f.Cage";
            return DataAccess.LoadDataRabbit<FatteningModel>(sql);
        }
        public static int DeleteFattRab(int PartId, int rabId)
        {
            
            string sql = String.Format("DELETE FROM fattening WHERE partId={0} AND rabPartId={1};", PartId, rabId);
            return DataAccess.SaveData(sql);
        }
        //Date Weight RabId PartId
        public static class RabWeight
        {
            static readonly string tbl = "rab_weight";
            static readonly string[] t = new string[] { "date", "weight", "rabId","id" };
            static readonly string[] m= new string[] { "Date", "Weight", "RabId","Id" };
            public static int Create(WeightModel wgt)
            {
                string sql = string.Format(@"insert into {0} ({1}, {2}, {3}) values(@{4}, @{5}, @{6})",tbl,t[0],t[1],t[2],m[0],m[1],m[2]);
                return DataAccess.SaveDataRabbit<WeightModel>(sql,wgt);
            }
            public static int Edit(WeightModel wgt)
            {
                string sql = string.Format(@"UPDATE {0} SET {1}=@{1}, {5}=@{6} WHERE {3} = {4};",tbl,t[1],m[1],t[3],wgt.Id,t[0],m[0]);
                return DataAccess.SaveDataRabbit<WeightModel>(sql, wgt);
            }
            public static int Delete(int wgtId)
            {

                string sql = String.Format("DELETE FROM {0} WHERE {1}={2};",tbl,t[3], wgtId);
                return DataAccess.SaveData(sql);
            }
            public static List<WeightModel> Load(int rabId)
            {
                string sql = string.Format(@"SELECT {0} as {1}, {2} as {3}, {4} as {5},{9} as {10} FROM {6} WHERE {7}={8};",t[0],m[0],t[1],m[1],t[2],m[2],tbl,t[2],rabId,t[3],m[3]);
                return DataAccess.LoadDataRabbit<WeightModel>(sql);
            }
        }
        public static class FattWeight
        {
            static readonly string tbl = "fat_weight";
            static readonly string[] t = new string[] { "date", "weight", "rabId","partId","id" };
            static readonly string[] m = new string[] { "Date", "Weight", "RabId","PartId","Id","Born" };
            public static int Create(FattWeightModel wgt)
            {
                string sql = string.Format(@"insert into {0} ({1}, {2}, {3},{7}) values(@{4}, @{5}, @{6},@{8})", 
                    tbl, t[0], t[1], t[2], m[0], m[1], m[2],t[3],m[3]);
                return DataAccess.SaveDataRabbit<FattWeightModel>(sql, wgt);
            }
            public static int Edit(FattWeightModel wgt)
            {
                string sql = string.Format(@"UPDATE {0} SET {1}=@{2}, {5}=@{6} WHERE {3} = {4};",
                    tbl, t[1], m[1], t[4], wgt.Id, t[0], m[0]);
                return DataAccess.SaveDataRabbit<FattWeightModel>(sql, wgt);
            }
            public static int Delete(int wgtId)
            {

                string sql = String.Format("DELETE FROM {0} WHERE {1}={2};", tbl, t[4], wgtId);
                return DataAccess.SaveData(sql);
            }
            public static List<FattWeightModel> Load(int rabId, int partId)
            {
                string sql = string.Format(@"SELECT w.{0} as {1}, w.{2} as {3}, w.{4} as {5}, w.{9} as {10}, w.{11} as {12}, p.birth_date as {13} FROM {6} w LEFT JOIN parturation p ON w.{14}= p.Id  WHERE w.{7}={8} AND w.{15}={16} ORDER BY w.{17} DESC;",
                    t[0], m[0], t[1], m[1], t[2], m[2], tbl, t[2], rabId, t[3], m[3], t[4], m[4], m[5], t[3], t[3], partId, t[0]);
                return DataAccess.LoadDataRabbit<FattWeightModel>(sql);
            }
            public static List<FattWeightModel> LoadAll()
            {
                string sql = string.Format(@"SELECT w.{0} as {1}, w.{2} as {3}, w.{4} as {5}, w.{6} as {7}, w.{8} as {9}, p.birth_date as {10} FROM {11} w LEFT JOIN parturation p ON w.{6}= p.Id;",
                    t[0], m[0], t[1], m[1], t[2], m[2], t[3], m[3], t[4], m[4], m[5], tbl);
                return DataAccess.LoadDataRabbit<FattWeightModel>(sql);
            }

        }
        public static class Note
        {
            static readonly string[] t = new string[] { "notes", "date", "note" };
            static readonly string[] m = new string[] { "Date", "Note", "DateList" };
            public static int Create(NoteModel note)
            {
                string sql= string.Format(@"INSERT INTO {0} ({1}, {2}) VALUES(@{3}, @{4}) ON DUPLICATE KEY UPDATE {2}=@{4};",
                    t[0],t[1],t[2],m[0],m[1]);
//                INSERT INTO eventcounter(userID, eventID, activityID) VALUES(1, 1, 1)
//ON DUPLICATE KEY UPDATE
//  activityID = VALUES(activityID)
                return DataAccess.SaveDataRabbit<NoteModel>(sql, note);
            }
            public static int Edit(NoteModel note)
            {
                string sql = string.Format(@"UPDATE {0} SET {1}=@{2} WHERE {3} = '{4}';",
                    t[0], t[2], m[1], t[1],DateToString(note.Date));
                return DataAccess.SaveDataRabbit<NoteModel>(sql, note);
            }
            public static int Delete(string date)
            {

                string sql = String.Format("DELETE FROM {0} WHERE {1}='{2}';", t[0],t[1],date);
                return DataAccess.SaveData(sql);
            }
            public static NoteModel Load(string? date)
            {
                NoteModel? nt;
                string sql = "";
                if (date != null)
                {
                    sql = string.Format(@"SELECT {0} as {1}, {2} as {3} FROM {5} WHERE {1}='{4}';",
                                        t[1], m[0], t[2], m[1],date,t[0]);
                }
                else sql = string.Format(@"SELECT {0} as {1}, {2} as {3} FROM {4} WHERE {1}=(SELECT MAX({1}) FROM {4});",
                    t[1],m[0],t[2],m[1],t[0]);
                nt = DataAccess.LoadDataOneLine<NoteModel>(sql);
                if (nt != null) {
                    sql = string.Format(@"SELECT {0} as {2} FROM (SELECT {0} FROM {1} ORDER BY {0} DESC LIMIT 50) sub ORDER BY {0} ASC", t[1], t[0], m[0]);
                    List<NoteModelDate> dates = DataAccess.LoadDataRabbit<NoteModelDate>(sql);
                    nt.DateList = dates;
                }
                else
                {
                    nt = new NoteModel()
                    {
                        Date = DateTime.Now
                    };
                }
                return nt;
            }
        }
        public static class FinanceSpent
        {
            static readonly string tbl = "finance";
            static readonly string[] t = new string[] { "id", "date", "price", "weight","type", "comment" };//Id,Date,Price, Weight, Type, Comment
            static readonly string[] m = new string[] { "Id", "Date", "Price", "Weight", "Type", "Comment" };
            public static int Create(FinModel mod)
            {
                string sql = string.Format(@"INSERT INTO {0} ({1}, {3}, {5}, {7}, {9}) VALUES(@{2}, @{4}, @{6}, @{8}, @{10});",
                tbl, t[1],m[1],t[2],m[2],t[3],m[3],t[4],m[4],t[5],m[5]);
                return DataAccess.SaveDataRabbit<FinModel>(sql, mod);
            }
            public static int Edit(FinModel mod)
            {
                string sql = string.Format(@"UPDATE {0} SET {1}=@{2}, {3}=@{4}, {5}= @{6}, {7}=@{8}, {9}=@{10} WHERE {11} = @{12};",
                    tbl, t[1], m[1], t[2], m[2], t[3], m[3], t[4], m[4], t[5], m[5],t[0],m[0]);
                return DataAccess.SaveDataRabbit<FinModel>(sql, mod);
            }
            public static int Delete(int id)
            {
                string sql = String.Format(@"DELETE FROM {0} WHERE {1}={2};", tbl, t[0], id);
                return DataAccess.SaveData(sql);
            }
            public static List<FinModel> LoadList(string dateFrom, string until=null)
            {
                if (until == null) until = DateToString(DateTime.Now);
                string sql =string.Format(@"select {1} as {2}, {3} as {4}, {5} as {6}, {7} as {8}, {9} as {10}, {11} as {12}" +
                    " FROM {0} WHERE {1} >='{13}' AND {1} <= '{14}' ORDER BY {1} DESC;", tbl, t[1], m[1], t[2], m[2], t[3], m[3], t[4], m[4], t[5], m[5], t[0], m[0],dateFrom, until);
                return DataAccess.LoadDataRabbit<FinModel>(sql);
            }
            public static FinModel LoadOne(int id)
            {
                string sql = string.Format(@"select {1} as {2}, {3} as {4}, {5} as {6}, {7} as {8}, {9} as {10}" +
                    " FROM {0} WHERE {11} ={12};", tbl, t[1], m[1], t[2], m[2], t[3], m[3], t[4], m[4], t[5], m[5],t[0],id);
                return DataAccess.LoadDataOneLine<FinModel>(sql);
            }
        }
        public static class Cage
        {
            static readonly string tbl = "cage";
            static readonly string[] t = new string[] { "Id", "location", "built", "width", "depth","height","type" };
            static readonly string[] m = new string[] { "Id", "Location", "Made", "Width", "Depth", "Height", "Type" };
            public static int Create(CageModel cg)
            {
                string sql = string.Format(@"insert into {0} ({1}, {2}, {3}, {4}, {5}, {6}, {7}) values(@{8}, @{9}, @{10}, @{11}, @{12}, @{13}, @{14});",
                    tbl, t[0], t[1], t[2], t[3], t[4], t[5], t[6], m[0], m[1], m[2], m[3], m[4], m[5],m[6]);
                return DataAccess.SaveDataRabbit<CageModel>(sql, cg);
            }
            public static int Edit(CageModel cg)
            {
                string sql = string.Format(@"UPDATE {0} SET {2}=@{9}, {3}=@{10}, {4}=@{11}, {5}=@{12}, {6}=@{13}, {7}=@{14} WHERE {1} = @{8};",
                    tbl, t[0], t[1], t[2], t[3], t[4], t[5],t[6], m[0], m[1], m[2], m[3], m[4], m[5], m[6]);
                return DataAccess.SaveDataRabbit<CageModel>(sql, cg);
            }
            public static int Delete(int cg)
            {

                string sql = String.Format("DELETE FROM {0} WHERE {1}={2};", tbl, t[0], cg);
                return DataAccess.SaveData(sql);
            }
            public static List<CageModel> LoadAll()
            {
                string sql = string.Format(@"SELECT {1} as {8}, {2} as {9}, {3} as {10}, {4} as {11}, {5} as {12}, {6} as {13}, {7} as {14} FROM {0} ORDER BY {1} ASC;",
                    tbl, t[0], t[1], t[2], t[3], t[4], t[5],t[6], m[0], m[1], m[2], m[3], m[4], m[5], m[6]);
                return DataAccess.LoadDataRabbit<CageModel>(sql);
            }
            public static CageModel LoadOne(int Id)
            {
                string sql = string.Format(@"SELECT {1} as {8}, {2} as {9}, {3} as {10}, {4} as {11}, {5} as {12}, {6} as {13}, {7} as {14} FROM {0} WHERE {1}={15};",
                    tbl, t[0], t[1], t[2], t[3], t[4], t[5], t[6], m[0], m[1], m[2], m[3], m[4], m[5], m[6],Id);
                return DataAccess.LoadDataOneLine<CageModel>(sql);
            }

        }
        public static class Breed
        {
            static readonly string tbl = "breeds";
            static readonly string[] t = new string[] { "id", "name" };
            static readonly string[] m = new string[] { "Id", "Name"};
            public static int Create(BreedsModel br)
            {
                string sql = string.Format(@"insert into {0} ({1},{2}) values(@{3},@{4});", tbl, t[0], t[1], m[0], m[1]);
                return DataAccess.SaveDataRabbit<BreedsModel>(sql, br);
            }
            public static int Edit(BreedsModel br)
            {
                string sql = string.Format(@"UPDATE {0} SET {2}=@{4} WHERE {1} = @{3};",
                    tbl, t[0], t[1], m[0], m[1]);
                return DataAccess.SaveDataRabbit<BreedsModel>(sql, br);
            }
            public static int Delete(int br)
            {

                string sql = String.Format("DELETE FROM {0} WHERE {1}={2};", tbl, t[0], br);
                return DataAccess.SaveData(sql);
            }
            public static List<BreedsModel> LoadAll()
            {
                string sql = string.Format(@"SELECT {1} as {3}, {2} as {4} FROM {0} ORDER BY {1} ASC;",
                    tbl, t[0], t[1], m[0], m[1]);
                return DataAccess.LoadDataRabbit<BreedsModel>(sql);
            }
            //public static CageModel LoadOne(int Id)
            //{
            //    string sql = string.Format(@"SELECT {1} as {8}, {2} as {9}, {3} as {10}, {4} as {11}, {5} as {12}, {6} as {13}, {7} as {14} FROM {0} WHERE {1}={15};",
            //        tbl, t[0], t[1], t[2], t[3], t[4], t[5], t[6], m[0], m[1], m[2], m[3], m[4], m[5], m[6], Id);
            //    return DataAccess.LoadDataOneLine<CageModel>(sql);
            //}
        }
    }

}