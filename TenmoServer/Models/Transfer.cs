using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenmoServer.Models
{
    public class Transfer
    {
        public int? Transfer_id { get; set; }
        public int Transfer_type_id { get; set; }
        public int Transfer_status_id { get; set;  }
        public int Account_from { get; set; }
        public int Account_to { get; set; }
        public decimal Amount { get; set; }
        
        public Transfer()
        {

        }
        public Transfer(int transfer_type_id, int transfer_status_id, int account_from, int account_to, decimal amount)
        {
            Transfer_type_id = transfer_type_id;
            Transfer_status_id = transfer_status_id;
            Account_from = account_from;
            Account_to = account_to;
            Amount = amount;
        }
        public void SetTransferID(int transferId)
        {
            Transfer_id = transferId;
        }
    }

    //send transfer back to client with username
    public class ReturnTransfer : Transfer
    {
        public ReturnTransfer()
        {

        }
        public ReturnTransfer(int transfer_id, int transfer_type_id, int transfer_status_id, int account_from, int account_to, decimal amount) 
            : base(transfer_type_id, transfer_status_id, account_from, account_to, amount)
        {
            SetTransferID(transfer_id);
        }
        public string ToName { get; set; }
        public string FromName { get; set; }
        public string TransferType { get; set; }
        public string TransferStatus { get; set; }
    }

}
