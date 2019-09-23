using NetCoreAPI.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreAPI.Services
{
    public class DBUtility
    {
        public static User GetUserById(string connStr, int id)
        {
            var user = new User();
            using (SqlConnection connection = new SqlConnection(connStr))
            {
                connection.Open();
                try
                {
                    using(SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "GetUserByUserName";
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@Id", id));

                        var sqlReader = command.ExecuteReader();
                        while(sqlReader.Read())
                        {
                            user.Id = Convert.ToInt32(sqlReader["Id"]);
                            user.UserName = sqlReader["UserName"].ToString();
                            user.FirstName = sqlReader["FirstName"].ToString();
                            user.LastName = sqlReader["LastName"].ToString();
                            user.PasswordHash = (byte[])sqlReader["PasswordHash"];
                            user.PasswordSalt = (byte[])sqlReader["PasswordSalt"];
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    connection.Close();
                }
            }
            return user;
        }

        public static User GetUserByUserName(string connStr, string userName)
        {
            User user = null;
            using (SqlConnection connection = new SqlConnection(connStr))
            {
                connection.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "GetUserByUserName";
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@UserName", userName));

                        var sqlReader = command.ExecuteReader();
                        while (sqlReader.Read())
                        {
                            user = new User();
                            user.UserName = sqlReader["UserName"].ToString();
                            user.FirstName = sqlReader["FirstName"].ToString();
                            user.LastName = sqlReader["LastName"].ToString();
                            user.PasswordHash = (byte[])sqlReader["PasswordHash"];
                            user.PasswordSalt = (byte[])sqlReader["PasswordSalt"];
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    connection.Close();
                }
            }
            return user;
        }

        public static void SaveUser(string connStr, User user)
        {
            using (SqlConnection connection = new SqlConnection(connStr))
            {
                connection.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "InsertUpdateUser";
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@Id", user.Id));
                        command.Parameters.Add(new SqlParameter("@UserName", user.UserName));
                        command.Parameters.Add(new SqlParameter("@FirstName", user.FirstName));
                        command.Parameters.Add(new SqlParameter("@LastName", user.LastName));
                        command.Parameters.Add(new SqlParameter("@PasswordHash", user.PasswordHash));
                        command.Parameters.Add(new SqlParameter("@PasswordSalt", user.PasswordSalt));

                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    connection.Close();
                }
            }
        }
    }
}
