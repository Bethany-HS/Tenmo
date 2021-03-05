using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public class TransferSqlDAO
    {
        private readonly string ConnectionString;
        private readonly AccountDAO accountDAO;
        public TransferSqlDAO(string connectionString)
        {
            accountDAO = new AccountDAO(connectionString);
            ConnectionString = connectionString;
        }
        public Transfer SendMoney(Transfer newTransfer)
        {
            try
            {
                //SqlTransaction transaction;

                using (SqlConnection _conn = new SqlConnection(ConnectionString))
                {
                    _conn.Open();
                    //transaction = _conn.BeginTransaction();

                    SqlCommand command = new SqlCommand("insert into transfers" +
                        "(transfer_type_id, transfer_status_id, account_from, account_to, amount) " +
                        "values(@transfer_type_id, @transfer_status_id, @account_from, @account_to, @amount)", _conn);
                    command.Parameters.AddWithValue("@transfer_type_id", newTransfer.Transfer_type_id);
                    command.Parameters.AddWithValue("@transfer_status_id", newTransfer.Transfer_status_id);
                    command.Parameters.AddWithValue("@account_from", newTransfer.Account_from);
                    command.Parameters.AddWithValue("@account_to", newTransfer.Account_to);
                    command.Parameters.AddWithValue("@amount", newTransfer.Amount);
                    command.ExecuteNonQuery();
                    //command.Transaction = transaction;

                    command = new SqlCommand("select @@identity", _conn);
                    newTransfer.SetTransferID(Convert.ToInt32(command.ExecuteScalar()));
                    //transaction.Commit();
                    accountDAO.UpdateBalance(newTransfer.Amount, newTransfer.Account_to);
                    accountDAO.UpdateBalance(0 - newTransfer.Amount, newTransfer.Account_from);

                    return newTransfer;

                }
            }
            catch (Exception)
            {
                //transaction.Rollback?
                throw;
            }
        }
        //write another 2 more query and command from the account to/from 
        // join transfers from account
        public List<ReturnTransfer> GetTransfers(int userId)//#5 in Read me
        {
            List<ReturnTransfer> listOfTransfers = new List<ReturnTransfer>();
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    SqlCommand command = new SqlCommand("select * from transfers " +
                        "where account_from = @userId or account_to = @userId", conn);
                    command.Parameters.AddWithValue("@userId", userId);
                    SqlDataReader reader = command.ExecuteReader();
                    while(reader.Read())
                    {
                        ReturnTransfer returnTransfer = GetTransferFromReader(reader);
                        listOfTransfers.Add(returnTransfer);
                    }
                    conn.Close();
                    conn.Open();
                    foreach (ReturnTransfer transfer in listOfTransfers)
                    { SqlCommand comm2 = new SqlCommand("select username from users " +
                        "join accounts on accounts.user_id = users.user_id " +
                        "where account_id = @account_id", conn);
                        comm2.Parameters.AddWithValue("@account_id", transfer.Account_from);
                        transfer.FromName = Convert.ToString(comm2.ExecuteScalar());

                        SqlCommand comm3 = new SqlCommand("select username from users " +
                        "join accounts on accounts.user_id = users.user_id " +
                        "where account_id = @account_id", conn);
                        comm3.Parameters.AddWithValue("@account_id", transfer.Account_to);
                        transfer.ToName = Convert.ToString(comm3.ExecuteScalar());
                    }
                    return listOfTransfers;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        private ReturnTransfer GetTransferFromReader(SqlDataReader reader)
        {
            int transferId = Convert.ToInt32(reader["transfer_id"]);
            int transferTypeId = Convert.ToInt32(reader["transfer_type_id"]);
            int transferStatusId = Convert.ToInt32(reader["transfer_status_id"]);
            int accountFrom = Convert.ToInt32(reader["account_from"]);
            int accountTo = Convert.ToInt32(reader["account_to"]);
            decimal amount = Convert.ToDecimal(reader["amount"]);

            ReturnTransfer transfer = new ReturnTransfer(transferId, transferTypeId, transferStatusId, accountFrom, accountTo, amount);
            
            return transfer;
        }


    }
}
