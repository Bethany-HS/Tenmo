using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.DAO;
using TenmoServer.Models;

namespace TenmoServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TransferController : ControllerBase
    {
        private TransferSqlDAO TransferDAO;
        private AccountSqlDAO AccountDAO;

        public TransferController(TransferSqlDAO _transferDAO, AccountSqlDAO _accountDAO)
        {
            TransferDAO = _transferDAO;
            AccountDAO = _accountDAO;
        }

        [HttpPost]
        public ActionResult<Transfer> SendMoney(Transfer incomingTransfer)
        {
            if(incomingTransfer.Transfer_status_id == 2)
            {
                if (AccountDAO.UpdateBalance(0 - incomingTransfer.Amount, incomingTransfer.Account_from))
                {
                    if (AccountDAO.UpdateBalance(incomingTransfer.Amount, incomingTransfer.Account_to))
                    {
                        Transfer result = TransferDAO.CreateTransfer(incomingTransfer);
                        return Created($"/transfers/{result.Transfer_id}", result);
                    }
                }
            }
            else
            {
                Transfer result = TransferDAO.CreateTransfer(incomingTransfer);
                return Created($"/transfers/{result.Transfer_id}", result);
            }

            return null;
        }

        [HttpGet("{id}")]
        public ActionResult<ReturnTransfer> GetSingleTransfer(int id)
        {
            ReturnTransfer result = TransferDAO.GetSingleTransfer(id);
            result.FromName = AccountDAO.GetName(result.Account_from);
            result.ToName = AccountDAO.GetName(result.Account_to);
            result.TransferStatus = TransferDAO.GetTransferStatus(result.Transfer_status_id);
            result.TransferType = TransferDAO.GetTransferType(result.Transfer_type_id);
            return result;

        }

        [HttpPut]
        public ActionResult UpdateTransferStatus(Transfer transfer)
        {
            if (TransferDAO.UpdateTransferStatus(transfer))
            {
                if(transfer.Transfer_status_id == 2)
                {
                    AccountDAO.UpdateBalance(0 - transfer.Amount, transfer.Account_from);
                    AccountDAO.UpdateBalance(transfer.Amount, transfer.Account_to);
                }
                return Ok();
            }
            else return null;
        }
    }
}
