using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

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
    }
}
