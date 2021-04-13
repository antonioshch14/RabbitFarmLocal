using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;


namespace DataLibrary
{
    //public class DataAccess : IDataAccess
    //{
    //    public List<T> LoadData<T, U>(string sql, U parameters, string connectionString)
    //    {
    //        using (IDbConnection connection = new MySqlConnection(connectionString))
    //        {
    //            List<T> rows = connection.Query<T>(sql, parameters).ToList();

    //            return rows;
    //        }
    //    }
    //    public void SaveData<T>(string sql, T parameters, string connectionString)
    //    {
    //        using (IDbConnection connection = new MySqlConnection(connectionString))
    //        {
    //            connection.Execute(sql, parameters);

    //        }
    //    }
    //}
    public class DataAccess
    {
       
        public static List<T> LoadDataRabbit<T>(string sql)
        {
            using (IDbConnection cnn = new MySqlConnection("Server=127.0.0.1;Port=3306;database=rabbitfarm;user id=root; password=Kukuruza144;"))
            {
                return cnn.Query<T>(sql).AsList();
            }

        }
        public static T LoadDataOneLine <T>(string sql)
        {
            using (IDbConnection cnn = new MySqlConnection("Server=127.0.0.1;Port=3306;database=rabbitfarm;user id=root; password=Kukuruza144;"))
            {
                return cnn.Query<T>(sql).SingleOrDefault();
            }

        }
        public static int SaveDataRabbit<T>(string sql, T data)
        {
            using (IDbConnection cnn = new MySqlConnection("Server=127.0.0.1;Port=3306;database=rabbitfarm;user id=root; password=Kukuruza144;Charset=utf8;"))
            {
                return cnn.Execute(sql, data);
            }
        }
        public static int SaveDataRabbit<T>(string sql, List<T> data)
        {
            using (IDbConnection cnn = new MySqlConnection("Server=127.0.0.1;Port=3306;database=rabbitfarm;user id=root; password=Kukuruza144;Charset=utf8;"))
            {
                foreach(var d in data)
                {
                    cnn.Execute(sql, d);
                }
                return 1;
            }
        }
        public static int SaveData (string sql)
        {
            using (IDbConnection cnn = new MySqlConnection("Server=127.0.0.1;Port=3306;database=rabbitfarm;user id=root; password=Kukuruza144;Charset=utf8;"))
            {
                return cnn.Execute(sql);
            }
        }


    }

}
