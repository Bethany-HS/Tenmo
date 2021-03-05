using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public class AccountDAO
    {
        private string ConnectionString;

        public AccountDAO(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public decimal GetBalance(int id)
        {
            decimal output = -1;
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    SqlCommand command = new SqlCommand("select balance from accounts where user_id = @userID", conn);
                    command.Parameters.AddWithValue("@userID", id);
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        output = Convert.ToDecimal(reader["balance"]);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return output;
        }
        //TRANSFERS TABLE account_from -- Foreign key to the accounts table; 
        //identifies the account that the funds are being taken from
        // TRANSFERS TABLE account_to -- Foreign key to the accounts table
        //identifies the account that the funds are going to
        public bool UpdateBalance(decimal updatedBalance, int accountId)
        {
            try
            {
                using(SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    decimal originalBalance = -1;
                    SqlCommand cmd = new SqlCommand("select balance from accounts " +
                        "where account_id = @accountId", conn);
                    cmd.Parameters.AddWithValue("@accountId", accountId);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while(reader.Read())
                    {
                        originalBalance = Convert.ToDecimal(reader["balance"]);
                    }
                    conn.Close();
                    if ((originalBalance + updatedBalance) < 0)
                    {
                        return false;
                    }

                    conn.Open();
                    SqlCommand cmd2 = new SqlCommand("update accounts set balance = @balance " +
                        "where account_id = @accountId", conn);
                    cmd2.Parameters.AddWithValue("@balance", updatedBalance + originalBalance);
                    cmd2.Parameters.AddWithValue("@accountId", accountId);
                    cmd2.ExecuteNonQuery();
                }
             }
            catch (Exception)
            {

                throw;
            }
            return true;
        }

        public string GetName(int accountId)
        {
            try
            {
                using(SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("select username from users " +
                        "join accounts on accounts.user_id = users.user_id " +
                        "where account_id = @accountId", conn);
                    cmd.Parameters.AddWithValue("@accountId", accountId);
                    SqlDataReader reader = cmd.ExecuteReader();
                    string output = "";
                    while (reader.Read())
                    {
                        output = Convert.ToString(reader["username"]);
                    }
                    return output;
                }
            }
            catch(Exception e)
            {
                throw;
            }
        }

    }
}
