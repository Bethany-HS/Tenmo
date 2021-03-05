using System;
using System.Collections.Generic;
using System.Text;

namespace TenmoClient.Data
{
    public class Transfer
    {
        public int Transfer_id {get; private set;}
        public int Transfer_type_id { get; }
        public int Transfer_status_id { get; }
        public int Account_from { get; }
        public int Account_to { get; }
        public decimal Amount { get; }

        public Transfer(int transferTypeId, int transferStatusId, int accountFrom, int accountTo, decimal amount)
        {
            Transfer_type_id = transferTypeId;
            Transfer_status_id = transferStatusId;
            Account_from = accountFrom;
            Account_to = accountTo;
            Amount = amount;
        }

        public void SetTransferId(int transferId)
        {
            Transfer_id = transferId;
        }
    }
}
