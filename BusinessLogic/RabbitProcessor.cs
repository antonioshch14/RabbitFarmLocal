using DataLibrary;
using RabbitFarmLocal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RabbitFarmLocal.BusinessLogic
{
    public static class RabbitProcessor
    {
        public static int CreateRabbit(int rabbitId, int cage, string breed, string collor,
            DateTime born, int mother, int father, bool isAlive, Gender gender)
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
                Collor = collor

            };
            if (gender == Gender.самка) data.IsMale = false;
            else data.IsMale = true;

            /*[Id],[rabId],[cage],[breed],[collor],[born],[mother],[father],[isMale],[isAlive] FROM [dbo].[rabbits2]*/
            /* Id, RabbitId, Cage, IsMale, Breed, Collor, Born, Mother, Father, IsAlive, */

            string sql = @"insert into rabbits (rabId,cage,breed,collor,born,mother,father,isMale,isAlive) 
                            values(@RabbitId,@Cage,@Breed,@Collor,@Born,@Mother,@Father,@IsMale,@IsAlive)";
            return DataAccess.SaveDataRabbit(sql, data);
        }
        public static int EditRabbit(int rabbitId, int cage, string breed, string collor,
            DateTime born, int mother, int father, bool isAlive, Gender gender)
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
                Collor = collor

            };
            if (gender == Gender.самка) data.IsMale = false;
            else data.IsMale = true;

            /*[Id],[rabId],[cage],[breed],[collor],[born],[mother],[father],[isMale],[isAlive] FROM [dbo].[rabbits2]*/
            /* Id, RabbitId, Cage, IsMale, Breed, Collor, Born, Mother, Father, IsAlive, */

            string sql = @"UPDATE rabbits SET cage=@Cage ,breed=@Breed,collor=@Collor,born=@Born,mother=@Mother,father=@Father,
                        isMale=@IsMale,isAlive=@IsAlive  WHERE rabId=@RabbitId;";
            return DataAccess.SaveDataRabbit(sql, data);
        }
        public static List<DLRabbitModel> LoadRabbits()
        {
            string sql = @"select Id as Id, rabId as RabbitId, cage as Cage, breed as Breed, collor as Collor,
                         mother as Mother, father as Father, isMale as IsMale, isAlive as IsAlive, born as Born from rabbits ORDER BY isAlive DESC, rabId ASC;";
            return DataAccess.LoadDataRabbit<DLRabbitModel>(sql);
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
        public static int EditComment(int comId, int rabId, string comment, DateTime date)
        {
            CommentsModel data = new CommentsModel
            {Id=comId,
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
                "as Date, parturId as ParturationId from mating WHERE father_id=" + Id + " OR mother_id="+ Id + " ORDER BY dateOfMate DESC;";
            return DataAccess.LoadDataRabbit<MatingModel>(sql);
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
        public static int EditMating(int id, int matherId, int fatherId, DateTime date)
        {
            MatingModel data = new MatingModel
            {
                Id = id,
                MotherId = matherId,
                FatherId = fatherId,
                Date = date
            };

            string sql = @"UPDATE mating SET father_id=@FatherId,dateOfMate=@Date,mother_id=@MotherId  WHERE id=@Id;";
            return DataAccess.SaveDataRabbit(sql, data);
        }
        
        public static int MarkMateAsFail(int id)
        {
            MatingModel data = new MatingModel
            { };

            string sql = @"UPDATE mating SET parturId=-1  WHERE id="+id+";";
            return DataAccess.SaveDataRabbit(sql, data);
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
        public static List<ParturationModel> LoadParturation(int rabbitId)
        {
            string sql = "select p.Id as Id, p.birth_date as Date,  p.mather as MotherId,p.children as Children,p.male as Males,p.female as Females,p.died_children as DiedChild," +
                "p.separation_date as SeparationDate,p.cage as Cage,p.comment,p.nest_removal as DateNestRemoval,p.mateId as MateId, m.father_id as FatherId, m.dateOfMate as MateDate" +
                " from parturation p LEFT JOIN mating m ON p.mateId=m.id WHERE p.mather=" + rabbitId + " OR m.father_id="+rabbitId+" ORDER BY p.birth_date DESC;";
            return DataAccess.LoadDataRabbit<ParturationModel>(sql);
        }
        public static int SaveParturation(DateTime date, int motherId, int  children, int males, int  females, int diedChild, DateTime?  separationDate, int cage, string comment, DateTime? dateNestRemoval, int mateId)
        {
            ParturationModel data = new ParturationModel
            {
                Date = date,
                MotherId =motherId ,
                Children =children ,
                Males =males ,
                Females =females,
                DiedChild  =diedChild,
                SeparationDate  =separationDate,
                Cage =cage ,
                Comment=comment,
                DateNestRemoval = dateNestRemoval,
                MateId=mateId

            };

            //string sql = @"insert into parturation (birth_date,mather,children,male,female,died_children,separation_date,cage,comment,nest_removal,mateId) 
            //                values(@Date, @MotherId,  @Children,  @Males,  @Females, @DiedChild,  @SeparationDate,  @Cage, @Comment, @DateNestRemoval,  @MateId)";

            string sql = @"CALL storeNewParturation (@Date, @MotherId,  @Children,  @Males,  @Females, @DiedChild,  
                @SeparationDate,  @Cage, @Comment, @DateNestRemoval,  @MateId)";


            return DataAccess.SaveDataRabbit(sql, data);
        }
        
        public static int EditParturation(int id, DateTime date, int motherId, int children, int males, int females, int diedChild, DateTime? separationDate, int cage, string comment, DateTime? dateNestRemoval)
        {
            ParturationModel data = new ParturationModel
            {
                Id = id,
                Date = date,
                MotherId = motherId,
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

    }   
}