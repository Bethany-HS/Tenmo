using System;
using System.Collections.Generic;
using System.Text;

namespace TenmoClient.Data
{
    public class Transfer
    {
        public int Transfer_id {get; set;}
        public int Transfer_type_id { get;  set; }
        public int Transfer_status_id { get; set; }
        public int Account_from { get; set;  }
        public int Account_to { get; set; }
        public decimal Amount { get; set;}

        public Transfer()
        {

        }
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

    public class ReturnTransfer : Transfer
    {
        public string ToName { get; set; }
        public string FromName { get; set; }

        public ReturnTransfer()
        {

        }
        public ReturnTransfer(int transferTypeId, int transferStatusId, int accountFrom, int accountTo, decimal amount) 
            : base ( transferTypeId,  transferStatusId,  accountFrom,  accountTo, amount)
        {

        }
    }
}
