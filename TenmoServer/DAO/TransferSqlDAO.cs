using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public class TransferSqlDAO : ITransferDAO
    {
        private readonly string ConnectionString;
        public TransferSqlDAO(string connectionString)
        {
            ConnectionString = connectionString;
        }
        public Transfer CreateTransfer(Transfer newTransfer)
        {
            try
            {

                using (SqlConnection _conn = new SqlConnection(ConnectionString))
                {
                    _conn.Open();

                    using (TransactionScope transaction = new TransactionScope())
                    {
                        SqlCommand command = new SqlCommand("insert into transfers" +
                                    "(transfer_type_id, transfer_status_id, account_from, account_to, amount) " +
                                    "values(@transfer_type_id, @transfer_status_id, @account_from, @account_to, @amount)", _conn);
                        command.Parameters.AddWithValue("@transfer_type_id", newTransfer.Transfer_type_id);
                        command.Parameters.AddWithValue("@transfer_status_id", newTransfer.Transfer_status_id);
                        command.Parameters.AddWithValue("@account_from", newTransfer.Account_from);
                        command.Parameters.AddWithValue("@account_to", newTransfer.Account_to);
                        command.Parameters.AddWithValue("@amount", newTransfer.Amount);
                        command.ExecuteNonQuery();

                        command = new SqlCommand("select @@identity", _conn);
                        newTransfer.SetTransferID(Convert.ToInt32(command.ExecuteScalar()));
                        transaction.Complete();
                    }
                    return newTransfer;
                }
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public List<ReturnTransfer> GetTransfers(int userId)//#5 in Read me
        {
            List<ReturnTransfer> listOfTransfers = new List<ReturnTransfer>();
            try
            {
                using (TransactionScope transaction = new TransactionScope())
                {
                    using (SqlConnection conn = new SqlConnection(ConnectionString))
                    {
                        conn.Open();
                        SqlCommand command = new SqlCommand("select * from transfers " +
                            "where account_from = @userId or account_to = @userId", conn);
                        command.Parameters.AddWithValue("@userId", userId);
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            ReturnTransfer returnTransfer = GetTransferFromReader(reader);
                            listOfTransfers.Add(returnTransfer);
                        }
                        conn.Close();
                        conn.Open();
                        foreach (ReturnTransfer transfer in listOfTransfers)
                        {
                            SqlCommand comm2 = new SqlCommand("select username from users " +
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
                        transaction.Complete();
                        return listOfTransfers;
                    }
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        public ReturnTransfer GetSingleTransfer(int id)
        {
            ReturnTransfer output = new ReturnTransfer();
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("select * from transfers " +
                        "where transfer_id = @transferId", conn);
                    cmd.Parameters.AddWithValue("@transferId", id);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        output = GetTransferFromReader(reader);
                    }
                    return output;
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

        public string GetTransferType(int typeId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("select transfer_type_desc from transfer_types " +
                        "where transfer_type_id = @typeId",conn);
                    cmd.Parameters.AddWithValue("@typeId", typeId);
                    SqlDataReader reader = cmd.ExecuteReader();

                    string output = "";
                    while (reader.Read())
                    {
                        output = Convert.ToString(reader["transfer_type_desc"]);
                    }
                    return output;
                }
            }
            catch
            {
                throw;
            }
        }
        public string GetTransferStatus(int statusId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("select transfer_status_desc from transfer_statuses " +
                        "where transfer_status_id = @typeId",conn);
                    cmd.Parameters.AddWithValue("@typeId", statusId);
                    SqlDataReader reader = cmd.ExecuteReader();

                    string output = "";
                    while (reader.Read())
                    {
                        output = Convert.ToString(reader["transfer_status_desc"]);
                    }
                    return output;
                }
            }
            catch
            {
                throw;
            }
        }

        public bool UpdateTransferStatus(Transfer transfer)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("update transfers set transfer_status_id = @transferType " +
                        "where transfer_id = @transferId", conn);
                    cmd.Parameters.AddWithValue("@transferType", transfer.Transfer_status_id);
                    cmd.Parameters.AddWithValue("@transferId", transfer.Transfer_id);
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

    }
}
